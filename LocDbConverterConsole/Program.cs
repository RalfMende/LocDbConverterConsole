/*
 * LocDbConverterConsole - Converter for model train descrption files of digital control stations
 * 
 * Copyright (c) 2023 Ralf Mende
 * 
 * 
 * This file is part of LocDbConverterConsole.
 * 
 * LocDbConverterConsole is free software: you can redistribute it and/or modify it 
 * under the terms of the GNU General Public License as published by the 
 * Free Software Foundation, either version 3 of the License, or (at your 
 * option) any later version.
 * 
 * LocDbConverterConsole is distributed in the hope that it will be useful, but 
 * WITHOUT ANY WARRANTY; without even the implied warranty of 
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
 * See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License 
 * along with Foobar. If not, see 
 * <https://www.gnu.org/licenses/>.
*/

using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LocDbConverterConsole
{
    internal class Program
    {
        static string locomotiveListPath = "";

        static void Main(string[] args)
        {
            bool quitProgram = false;
            int returnValue;
            string exportPath;
            string userEntry;
            string[] userEntrySplit;
            int number;
            string osNameAndVersion = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
            if (osNameAndVersion.Contains("Windows"))
            {
                locomotiveListPath = System.Configuration.ConfigurationManager.AppSettings["LocomotiveListPathWin"];
            }
            else
            {
                locomotiveListPath = System.Configuration.ConfigurationManager.AppSettings["LocomotiveListPathLinux"];
            }
            using var watcher = new FileSystemWatcher(Path.GetDirectoryName(locomotiveListPath));
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Changed += OnChanged;
            watcher.Filter = "lokomotive.cs2";


            // Initialize mapping for functions
            /*-------------------------------------------------------------------------------------------------------*/

            XmlTextReader reader = null;
            try
            {
                // Load the reader with the data file and ignore all white space nodes.
                reader = new XmlTextReader("Mapping.xml");
                reader.WhitespaceHandling = WhitespaceHandling.None;
                while (reader.Read())
                {
                    if (reader.Name == "mapping")
                    {
                        //Enum.TryParse(reader.GetAttribute("FunctionTypeCS2"), out FunctionTypeCS2 _functionTypeCS2); -> Working with the CS2-Index from mapping.xml is more reliable
                        Enum.TryParse(reader.GetAttribute("FunctionTypeZ21"), out FunctionTypeZ21 _functionTypeZ21);

                        FunctionTypeMappingList.Set(new FunctionTypeMapping() {
                            Key = Convert.ToInt32(reader.GetAttribute("Id")), 
                            Shortname = reader.GetAttribute("Shortname"), 
                            Duration = Convert.ToInt32(reader.GetAttribute("Duration")),
                            //FunctionTypeIndexCS2 = (int)_functionTypeCS2, -> Working with the CS2-Index from mapping.xml is more reliable
                            FunctionTypeIndexCS2 = Convert.ToInt32(reader.GetAttribute("FunctionTypeIndexCS2")),
                            FunctionTypeZ21 = _functionTypeZ21
                        });
                        
                    }
                }
            }
            finally
            {
                if (reader != null) reader.Close();
            }


            // Run the program            
            /*-------------------------------------------------------------------------------------------------------*/

            if (args.Length != 0)
            {
                if (File.Exists(args[0]) && args[0].Substring(args[0].Length - 4).Contains(".cs2"))
                {
                    ConvertLocomotiveConfigurationFile(args[0], true);
                }
            }
            else
            {
                Console.WriteLine("Locomotive database converter v1.0. Use  /help  to list commands available.");

                while (!quitProgram)
                {
                    userEntry = Console.ReadLine();
                    userEntrySplit = userEntry.Split(' ');
                    switch (userEntrySplit[0].ToLowerInvariant())
                    {
                        case "/help":
                        case "/?":
                            Console.WriteLine("The following commands are available");
                            Console.WriteLine("\t/help                 \t- Help menu");
                            Console.WriteLine("\t/convert <file>       \t- Converts a given locomotive config file from CS2/CS3-format to Z21-format");
                            Console.WriteLine("\t/set <file>           \t- Sets the path of the Locomotive List File of your CS2");
                            Console.WriteLine("\t/update               \t- Generates config files in Z21-format for each locomotive in Locomotive List File of your CS2");
                            Console.WriteLine("\t/autoupdate           \t- Generates config files in Z21-format for each locomotive in Locomotive List File of your CS2, whenever the list changes");
                            //Console.WriteLine("\t/import <file>      \t- Imports a given locomotive config file in CS2/CS3-format to internal memory");
                            Console.WriteLine("\t/list                 \t- Lists all imported locomotive configurations");
                            //Console.WriteLine("\t/export <No.> <path>\t- Export a locomotive config file in Z21-format from internal memory by list entry");
                            Console.WriteLine("\t/exit                 \t- Exits the program");
                            //Console.WriteLine("Or just simply drag'n'drop the *.cs2 locomotive config file onto the LocDbConverterConsole.exe");
                            break;

                        case "/convert":
                            if (userEntrySplit[1].StartsWith("\""))
                            {
                                userEntrySplit[1] = userEntrySplit[1].Replace("\"", "");
                            }
                            returnValue = ConvertLocomotiveConfigurationFile(userEntrySplit[1], true);
                            if (returnValue > 0)
                            {
                                Console.WriteLine("File exported to " + Path.GetDirectoryName(userEntrySplit[1]));
                            }
                            else
                            {
                                Console.WriteLine("Error. Could not find *.cs2 locomotive configuration file.");
                            }
                            break;

                        case "/set":
                            if (File.Exists(userEntrySplit[1]) && userEntrySplit[1].Substring(userEntrySplit[1].Length - 4).Contains(".cs2"))
                            {
                                locomotiveListPath = userEntrySplit[1];
                                Console.WriteLine("Locomotive List is now monitored from: " + locomotiveListPath);
                            }
                            else
                            {
                                Console.WriteLine("Error. Could not find Lokomotive.cs2 file.");
                            }
                            break;

                        case "/update":
                            LocomotiveList.DeleteAll(); //TODO should internal list rather be syncronized?
                            returnValue = ConvertLocomotiveConfigurationFile(locomotiveListPath, true);
                            if (returnValue > 0) Console.WriteLine("Z21-config files updated according to Lokomotive.cs2 file.");
                            break;

                        case "/autoupdate":
                            watcher.EnableRaisingEvents = true;
                            Console.WriteLine("Auomatical convertion for Z21 Files activated for Lokomotive.cs2 file: " + locomotiveListPath);
                            break;

                        /*case "/import":
                            returnValue = _cs2.ImportConfiguration(userEntrySplit[1]);
                            if (returnValue > 0) Console.WriteLine("Job done.");
                            else Console.WriteLine("Error. Could not find *.cs2 locomotive configuration file.");
                            break;*/

                        case "/list":
                            number = LocomotiveList.SizeOf();
                            if (number == 0)
                            {
                                Console.WriteLine("Internal list of locomotive configurations is empty.");
                            }
                            else
                            {
                                for (int index = 0; index < number; index++)
                                {
                                    Console.WriteLine("\t[" + index + "] - " + LocomotiveList.Get(index).Name.ToString());
                                }
                            }
                            break;

                        /*case "/list2":
                            number = FunctionTypeMappingList.SizeOf();
                            if (number == 0)
                            {
                                Console.WriteLine("Internal list of FunctionMappings configurations is empty.");
                            }
                            else
                            {
                                Console.WriteLine("\tKEY \tSHORTNAME \tDURATION \tFUNCTIONTYPEINDEXCS2 \tFUNCTIONTYPEZ21");
                                for (int index = 0; index < number; index++)
                                {
                                    Console.WriteLine(
                                        "\t" + FunctionTypeMappingList.Get(index).Key.ToString()
                                        + "\t" + FunctionTypeMappingList.Get(index).Shortname.ToString()
                                        + "\t" + FunctionTypeMappingList.Get(index).Duration.ToString()
                                        + "\t" + FunctionTypeMappingList.Get(index).FunctionTypeIndexCS2.ToString()
                                        + "\t" + FunctionTypeMappingList.Get(index).FunctionTypeZ21.ToString()
                                        );
                                }
                            }
                            break;*/

                        /*case "/export":
                            number = LocomotiveList.SizeOf();
                            if (number == 0)
                            {
                                Console.WriteLine("Internal list of locomotive configurations is empty.");
                            }
                            else
                            {
                                int listIndex = Convert.ToInt32(userEntrySplit[1]);
                                if (listIndex > -1 && listIndex < LocomotiveList.SizeOf())
                                {
                                    if (userEntrySplit.Length > 1 && Directory.Exists(userEntrySplit[2]))
                                    {
                                        exportPath = userEntrySplit[2];
                                    }
                                    else
                                    {
                                        exportPath = AppContext.BaseDirectory;
                                    }
                                    returnValue = _z21.ExportConfiguration(listIndex, exportPath);
                                    if (returnValue > 0) Console.WriteLine("File exported to " + exportPath);
                                }
                                else
                                {
                                    Console.WriteLine("Error. Index not accepted.");
                                }
                            }
                            break;*/

                        case "/exit":
                            if (osNameAndVersion.Contains("Windows"))
                            {
                                System.Configuration.ConfigurationManager.AppSettings["LocomotiveListPathWin"] = locomotiveListPath;
                            }
                            else
                            {
                                System.Configuration.ConfigurationManager.AppSettings["LocomotiveListPathLinux"] = locomotiveListPath;
                            }
                            quitProgram = true;
                            break;

                        default:
                            Console.WriteLine("Unknown command. Use /help to get a list of available commands.");
                            break;
                    }
                }
            }


        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            LocomotiveList.DeleteAll(); //TODO should internal list rather be syncronized?
            int returnValue = ConvertLocomotiveConfigurationFile(locomotiveListPath, true);
            if (returnValue > 0) Console.WriteLine(DateTime.Now + " Info: Z21-config files updated according to Lokomotive.cs2 file.");
        }

        /// <summary>
        /// Converts a given *.cs2 config file into *.z21loco config files
        /// </summary>
        /// <param name="fileToConvert">*.cs2 config file for convertion</param>
        /// <returns></returns>
        private static int ConvertLocomotiveConfigurationFile(string fileToConvert, bool overwriteInternalConfigs)
        {
            int returnValue = 0;
            int number = 0;
            string exportPath = "";
            ConfigurationCS2 _cs2 = new ConfigurationCS2();
            ConfigurationZ21 _z21 = new ConfigurationZ21();

            returnValue = _cs2.ImportConfiguration(fileToConvert, overwriteInternalConfigs);
            if (returnValue > 0)
            {
                number = LocomotiveList.SizeOf();
                for (int index = 0; index < number; index++)
                {
                    returnValue += _z21.ExportConfiguration(index, Path.GetDirectoryName(fileToConvert));
                }
            }
            else
            {
                returnValue = -1;
            }

            _cs2 = null;
            _z21 = null;
            return returnValue;
        }
    }
}



