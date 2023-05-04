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

using System.Configuration;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.InteropServices;
using System.Xml;

namespace LocDbConverterConsole
{
    internal class Program
    {
        static string LocomotiveListFile = "";
        static string ExportFilesPath = "";

        static void Main(string[] args)
        {
            bool quitProgram = false;
            int returnValue;
            string userEntry;
            string[] userEntrySplit;
            int number;
            bool autoconverterActive = false;
            string osNameAndVersion = RuntimeInformation.OSDescription;
            FileSystemWatcher watcher = new FileSystemWatcher();

            /*--------------------------------- Initialize mapping for functions -----------------------------------*/
            string mappingConfigFile = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + @"\mapping.xml";
            if (File.Exists(mappingConfigFile))
            {
                XmlTextReader reader = null;
                try
                {
                    // Load the reader with the data file and ignore all white space nodes.
                    reader = new XmlTextReader(mappingConfigFile);
                    reader.WhitespaceHandling = WhitespaceHandling.None;
                    while (reader.Read())
                    {
                        if (reader.Name == "mapping")
                        {
                            //Enum.TryParse(reader.GetAttribute("FunctionTypeCS2"), out FunctionTypeCS2 _functionTypeCS2); -> Working with the CS2-Index from mapping.xml is more reliable
                            Enum.TryParse(reader.GetAttribute("FunctionTypeZ21"), out FunctionTypeZ21 _functionTypeZ21);

                            FunctionTypeMappingList.Set(new FunctionTypeMapping()
                            {
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
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    if (reader != null) reader.Close();
                }
            }
            else
            {
                Console.WriteLine("Error. File mapping.xml not found at " + mappingConfigFile);
            }

            /*------------------------------------------- Run the program -------------------------------------------*/
            if (args.Length != 0)
            {
                switch (args[0])
                {
                    case "-h":
                    case "-?":
                        Console.WriteLine("The following commands are available");
                        Console.WriteLine("\t-h / -?    \tHelp menu");
                        Console.WriteLine("\t-c <file>  \tConverts a given locomotive config file from CS2/CS3-format to Z21-format");
                        Console.WriteLine("\t-a         \tAutoconvert locomotive.cs2 file according to settings in App.config file.");
                        Console.WriteLine("You can simply drop a locomotive.cs2 file on the .exe to be converted.");
                        Console.WriteLine("This is version 1.0 beta. Code is available under GNU General Public License at Github https://github.com/RalfMende/LocDbConverterConsole.");
                        break;

                    case "-c":
                        if (File.Exists(args[1]) & args[1].Substring(args[1].Length - 4).Contains(".cs2"))
                        {
                            if (args[1].StartsWith("\""))
                            {
                                args[1] = args[1].Replace("\"", "");
                            }
                            returnValue = ConvertLocomotiveConfigurationFile(args[1], Path.GetDirectoryName(args[1]), true);
                            if (returnValue > 0)
                            {
                                Console.WriteLine("File(s) exported to " + Path.GetDirectoryName(args[1]));
                            }
                            else
                            {
                                Console.WriteLine("Error. Could not convert file " + args[1]);
                            }
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Error. No locomotive.cs2 file given in argument.");
                        }
                        break;

                    case "-a":
                        if (osNameAndVersion.Contains("Windows"))
                        {
                            LocomotiveListFile = ConfigurationManager.AppSettings["LocomotiveListFileWin"];
                            ExportFilesPath = ConfigurationManager.AppSettings["ExportPathWin"];
                        }
                        else
                        {
                            LocomotiveListFile = ConfigurationManager.AppSettings["LocomotiveListFileLinux"];
                            ExportFilesPath = ConfigurationManager.AppSettings["ExportPathLinux"];
                        }

                        if (File.Exists(LocomotiveListFile) & LocomotiveListFile.Substring(LocomotiveListFile.Length - 4).Contains(".cs2") & Path.Exists(ExportFilesPath))
                        {
                            try
                            {
                                watcher.Path = Path.GetDirectoryName(LocomotiveListFile);
                                watcher.NotifyFilter = NotifyFilters.LastWrite;
                                watcher.Changed += OnChanged;
                                watcher.Filter = "lokomotive.cs2";
                                watcher.EnableRaisingEvents = true;
                                autoconverterActive = true;
                                Console.WriteLine("Automatical convertion for Z21 files activated for file: " + LocomotiveListFile);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error. File/Path could not be found. Please check LocDbConverterConsole.dll.config file."); //TODO: Split error description to be more precise 
                        }
                        break;

                    default:
                        if(File.Exists(args[0]) & args[0].Substring(args[0].Length - 4).Contains(".cs2")) //if locomotive file is dropped on *.exe
                        {
                            if (args[0].StartsWith("\""))
                            {
                                    args[0] = args[1].Replace("\"", "");
                            }
                            returnValue = ConvertLocomotiveConfigurationFile(args[0], Path.GetDirectoryName(args[0]), true);
                            if (returnValue > 0)
                            {
                                Console.WriteLine("File(s) exported to " + Path.GetDirectoryName(args[0]));
                            }
                            else
                            {
                                Console.WriteLine("Error. Could not convert file " + args[0]);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Unknown command. Use /help to get a list of available commands.");
                        }
                        break;
                }

                /*-------------------------------------------------------------------------------------------------------*/
                while (!quitProgram & autoconverterActive)
                {
                    userEntry = Console.ReadLine();
                    switch (userEntry.ToLowerInvariant())
                    {
                        case "-h":
                        case "-?":
                            Console.WriteLine("The following commands are available");
                            Console.WriteLine("\t-h / -?    \tHelp menu");
                            Console.WriteLine("\texit       \tExit the program");
                            break;

                        case "exit":
                            if (osNameAndVersion.Contains("Windows"))
                            {
                                System.Configuration.ConfigurationManager.AppSettings["LocomotiveListPathWin"] = LocomotiveListFile;
                            }
                            else
                            {
                                System.Configuration.ConfigurationManager.AppSettings["LocomotiveListPathLinux"] = LocomotiveListFile;
                            }
                            quitProgram = true;
                            break;

                        default:
                            Console.WriteLine("Unknown command. Use /help to get a list of available commands.");
                            break;

                    }
                }
                Environment.Exit(0);

            }
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            LocomotiveList.DeleteAll(); //TODO should internal list rather be syncronized?
            int returnValue = ConvertLocomotiveConfigurationFile(LocomotiveListFile, ExportFilesPath, true);
            if (returnValue > 0) Console.WriteLine(DateTime.Now + " Info: Z21-config files updated according to Lokomotive.cs2 file.");
        }

        /// <summary>
        /// Converts a given *.cs2 config file into *.z21loco config files
        /// </summary>
        /// <param name="fileToConvert">*.cs2 config file for convertion</param>
        /// <returns></returns>
        private static int ConvertLocomotiveConfigurationFile(string fileToConvert, string pathToPlaceConvertedFile, bool overwriteInternalConfigs)
        {
            int returnValue = 0;
            int number = 0;
            ConfigurationCS2 _cs2 = new ConfigurationCS2();
            ConfigurationZ21 _z21 = new ConfigurationZ21();

            returnValue = _cs2.ImportConfiguration(fileToConvert, overwriteInternalConfigs);
            if (returnValue > 0)
            {
                number = LocomotiveList.SizeOf();
                for (int index = 0; index < number; index++)
                {
                    returnValue += _z21.ExportConfiguration(index, pathToPlaceConvertedFile);
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



