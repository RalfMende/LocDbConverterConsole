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

using System;
using System.Configuration;
using System.Diagnostics.Eventing.Reader;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using System.Xml;

namespace LocDbConverterConsole
{
    internal class Program
    {
        static string LocomotiveConfigFilePath = string.Empty;
        static string ExportFilesPath = string.Empty;
        static string IpAddressHostnameCS2 = string.Empty;
        private static string LocomotiveConfigFileName = "lokomotive.cs2";
        static bool remoteFileServer = false;
        
        static void Main()
        {
            bool quitProgram = false;
            int returnValue;
            string userEntry;
            int status = 0;
            bool setupOk = true;
            string osNameAndVersion = RuntimeInformation.OSDescription;
            FileSystemWatcher watcher = new FileSystemWatcher();

            Console.WriteLine($"Welcome to LocDbConverter Console");

            // Initialize application
            setupOk = InitializeApplication();

            // Run actual program
            if (!setupOk)
            {
                Console.WriteLine("ERROR Initialization was not successful. Now exiting Application.");
                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine("Program running. Please enter command or ? for help:");
                while (!quitProgram)
                {
                    userEntry = Console.ReadLine().Trim().ToLowerInvariant();
                    switch (userEntry)
                    {
                        case "?":
                        case "h":
                        case "help":
                            DisplayHelp();
                            break;

                        case "c":
                        case "convert":
                            returnValue = ConvertLocomotiveConfigFile(Path.Combine(LocomotiveConfigFilePath, LocomotiveConfigFileName), ExportFilesPath, false);
                            if (returnValue > 0)
                            {
                                Console.WriteLine($"{DateTime.Now} Z21-config files updated according to Lokomotive.cs2 file.");
                            }
                            break;

                        case "f":
                        case "force":
                            returnValue = ConvertLocomotiveConfigFile(Path.Combine(LocomotiveConfigFilePath, LocomotiveConfigFileName), ExportFilesPath, true);
                            if (returnValue > 0)
                            {
                                Console.WriteLine($"{DateTime.Now} Z21-config files updated according to Lokomotive.cs2 file.");
                            }
                            break;

                        case "a":
                        case "auto":
                            if (!remoteFileServer)
                            {
                                SetupAutoConversion(watcher);
                            }
                            else
                            {
                                // Todo: Setup AutoConvertion via Timer
                                Console.WriteLine("Automatical convertion for Z21 files not possible when using remote location.");
                            }
                            break;

                        case "x":
                        case "exit":
                            quitProgram = true;
                            break;

                        default:
                            Console.WriteLine("ERROR Unknown command. Use help / ? to get a list of available commands.");
                            break;

                    }
                }
            }

            Environment.Exit(0);
        }


        //static bool InitializeApplication(out string locomotiveConfigFilePath, out string exportFilesPath)
        static bool InitializeApplication()
        {
            bool setupOk = true;

            // Get mapping for functions from mapping.xml file
            string mappingFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mapping.xml");
            if (File.Exists(mappingFile))
            {
                Console.Write("Importing mapping.xml file ... ");
                int statusMappingFile = ImportMappingConfigFile(mappingFile);
                if (statusMappingFile < 1)
                {
                    Console.WriteLine("ERROR File could not be imported.");
                    setupOk = false;
                }
                else
                {
                    Console.WriteLine("OK");
                }
            }
            else
            {
                Console.WriteLine($"ERROR File mapping.xml not found at {mappingFile}");
                setupOk = false;
            }

            // Check lokomotive.cs file
            Console.WriteLine("Checking access to lokomotive.cs file ... ");
            int statusConfigPath = GetLocomotiveConfigFilePath();
            if (statusConfigPath < 1)
            {
                Console.WriteLine("ERROR Lokomotive.cs file could not be located.");
                setupOk = false;
            }
            else
            {
                Console.WriteLine("OK");
            }

            // Check export path 
            Console.WriteLine("Checking path for file export ... ");
            int statusExportPath = GetExportFilesPath();
            if (statusExportPath < 1)
            {
                Console.WriteLine("ERROR Path not found.");
                setupOk = false;
            }
            else
            {
                Console.WriteLine("OK");
            }

            // Setup Remote Connection
            if (remoteFileServer)
            {
                LocomotiveConfigFilePath = ExportFilesPath; // Because Lokomotive.cs and icon files are copied to export path to be converted
            }

            return setupOk;
        }

        /// <summary>
        /// Display help menu
        /// </summary>
        /// <param name="programVersion">string with program version</param>
        static void DisplayHelp()
        {
            Console.WriteLine("Commands:");
            Console.WriteLine("\th / ?  \tHelp menu");
            Console.WriteLine("\tc      \tConvert Lokomotive.cs2 file");
            Console.WriteLine("\ta      \tAuto convert Lokomotive.cs2 file according to settings in App.config file.");
            Console.WriteLine("\tf      \tForce convert of all entries in Lokomotive.cs2 file");
            Console.WriteLine("\tx      \tExit the program");
            Console.WriteLine($"This is version {Assembly.GetExecutingAssembly().GetName().Version.ToString()} (beta). Code is available under GNU General Public License at Github https://github.com/RalfMende/LocDbConverterConsole.");
        }
        
        /// <summary>
        /// Setup the locomotive config file watcher for auto convertion
        /// </summary>
        /// <param name="watcher">FileSystemWatcher for auto convertion</param>
        static void SetupAutoConversion(FileSystemWatcher watcher)
        {
            try
            {
                watcher.Path = LocomotiveConfigFilePath;
                watcher.NotifyFilter = NotifyFilters.LastWrite;
                watcher.Changed += OnChanged;
                watcher.Filter = "Lokomotive.cs2";
                watcher.EnableRaisingEvents = true;
                Console.WriteLine("Automatical conversion for Z21 files activated whenever Lokomotive.cs file changes.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Imports the mapping.xml file used for convertion of the locomotives functions
        /// </summary>
        /// <param name="mappingConfigFile">Path and filename</param>
        /// <returns>positive = success / 0 = nothing done / negative = errors</returns>
        private static int ImportMappingConfigFile(string mappingConfigFile)
        {
            int returnValue = 0;
            try
            {
                using (var reader = new XmlTextReader(mappingConfigFile))
                {
                    reader.WhitespaceHandling = WhitespaceHandling.None;
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "mapping")
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
                returnValue = 1;
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.ToString());
                returnValue = -1;
            }
            return returnValue;
        }

        /// <summary>
        /// Gets path or webaddress where lokomotive.cs is located
        /// </summary>
        /// <returns>positive = success / 0 = nothing done / negative = errors</returns>
        private static int GetLocomotiveConfigFilePath()
        {
            int returnValue = 0;
            int attemptsLeft = 3;
            string tmpConfigFilePath = ConfigurationManager.AppSettings["LocomotiveConfigFilePath"].Trim();
            ConsoleKey userInput;

            Console.Write($"Do you like to use {tmpConfigFilePath} ? [y/n]");
            do
            {
                userInput = Console.ReadKey(false).Key;
            } while (userInput != ConsoleKey.Y && userInput != ConsoleKey.N);
            Console.WriteLine("");// just for cosmetics ;)
            
            if (userInput == ConsoleKey.Y)
            {
                returnValue = CheckLocomotiveConfigFileAccess(tmpConfigFilePath);
                if (returnValue > 0)
                {
                    LocomotiveConfigFilePath = tmpConfigFilePath;
                    return returnValue;
                }
            }
            else if (userInput == ConsoleKey.N)
            {
                returnValue = -1;
            }
            else
            {
                return returnValue;
            }
            
            while (returnValue < 0 && attemptsLeft > 0)
            {
                Console.WriteLine("ERROR Connection could not be established.");
                Console.WriteLine("Please enter IP address / Hostname of CS2/CS3/CS3+/SRSEII or simply Path to Lokomotive.cs file:");
                tmpConfigFilePath = Console.ReadLine()?.Trim();

                if (!string.IsNullOrEmpty(tmpConfigFilePath))
                {
                    returnValue = CheckLocomotiveConfigFileAccess(tmpConfigFilePath);
                    if (returnValue > 0)
                    {
                        LocomotiveConfigFilePath = tmpConfigFilePath;
                        AddUpdateAppSettings("LocomotiveConfigFilePath", tmpConfigFilePath);
                        return returnValue;
                    }
                    else
                    {
                        attemptsLeft--;
                    }
                }
            }

            return returnValue; 
        }

        /// <summary>
        /// Gets path to save converted files to
        /// </summary>
        /// <returns>positive = success / 0 = nothing done / negative = errors</returns>
        private static int GetExportFilesPath()
        {
            int returnValue = 0;
            int attemptsLeft = 3;
            string tmpExportFilesPath = ConfigurationManager.AppSettings["ExportFilesPath"].Trim();
            ConsoleKey userInput;

            Console.Write($"Do you like to use {tmpExportFilesPath} ? [y/n]");
            do
            {
                userInput = Console.ReadKey(false).Key;
            } while (userInput != ConsoleKey.Y && userInput != ConsoleKey.N);
            Console.WriteLine("");// just for cosmetics ;)
            
            if (userInput == ConsoleKey.Y)
            {
                returnValue = CheckExportPathAccess(tmpExportFilesPath);
                if (returnValue > 0)
                {
                    ExportFilesPath = tmpExportFilesPath;
                    return returnValue;
                }
            }
            else if (userInput == ConsoleKey.N)
            {
                returnValue = -1;
            }
            else
            {
                return returnValue;
            }
            
            while (returnValue < 0 && attemptsLeft > 0)
            {
                Console.WriteLine("ERROR Connection could not be established.");
                Console.WriteLine("Please enter a valid Path:");
                tmpExportFilesPath = Console.ReadLine()?.Trim();

                if (!string.IsNullOrEmpty(tmpExportFilesPath))
                {
                    returnValue = CheckExportPathAccess(tmpExportFilesPath);
                    if (returnValue > 0)
                    {
                        ExportFilesPath = tmpExportFilesPath;
                        AddUpdateAppSettings("ExportFilesPath", tmpExportFilesPath);
                        return returnValue;
                    }
                    else
                    {
                        attemptsLeft--;
                    }
                }
            }
            
            return returnValue;
        }

        /// <summary>
        /// Checks if locomotive config file is available at IP address / hostname
        /// </summary>
        /// <param name="path">Path defined in config file</param>
        /// <returns>positive = success / 0 = nothing done / negative = errors</returns>
        private static int CheckLocomotiveConfigFileAccess(string path)
        {
            int returnValue = 0;

            if (string.IsNullOrWhiteSpace(path))
            {
                return -2;
            }

            path = path.Trim();

            if (path.StartsWith("http://"))
            {
                path = path.Substring(7);
                int index = path.IndexOf('/');
                if(index > 0)
                {
                    path = path.Substring(0, index);
                }
            }

            if (path.Contains("\\") ^ path.Contains("/"))
            {
                if (!path.EndsWith(Path.DirectorySeparatorChar)) path += Path.DirectorySeparatorChar; //Todo: Is this really needded here?
                returnValue = CheckLocomotiveConfigFileAccessLocal(path);
            }
            else
            {
                returnValue = CheckLocomotiveConfigFileAccessRemote(path);
                if (returnValue > 0)
                {
                    Program.remoteFileServer = true;
                    IpAddressHostnameCS2 = path;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Checks if locomotive config file is available at path
        /// </summary>
        /// <param name="IpAddressHostname">IP address or Hostname defined in config file</param>
        /// <returns>positive = success / 0 = nothing done / negative = errors</returns>
        private static int CheckLocomotiveConfigFileAccessRemote(string ipAddressHostname)
        {
            int returnValue = 0;

            using (WebClient remoteFileServer = new WebClient())
            {
                try
                {
                    string url = $"http://{ipAddressHostname}/config/{LocomotiveConfigFileName}";
                    remoteFileServer.DownloadData(url);
                    returnValue = 1;
                }
                catch (Exception ex)
                {
                    // Console.WriteLine(ex.ToString()); 
                    returnValue = -1;
                }

            }
            return returnValue;
        }

        /// <summary>
        /// Checks if locomotive config file is available at local path, definded in config-file
        /// </summary>
        /// <param name="locomotiveListPath">Path and file defined in config file</param>
        /// <returns>positive = success / 0 = nothing done / negative = errors</returns>
        private static int CheckLocomotiveConfigFileAccessLocal(string locomotiveListPath)
        {
            string fullPath = Path.Combine(locomotiveListPath, LocomotiveConfigFileName);
            return File.Exists(fullPath) ? 1 : -1;
        }

        /// <summary>
        /// Checks if path for file export, defined in config-file is available
        /// </summary>
        /// <param name="exportPath">Path defined in config file</param>
        /// <returns>positive = success / 0 = nothing done / negative = errors</returns>
        private static int CheckExportPathAccess(string exportPath)
        {
            return Directory.Exists(exportPath) ? 1 : -1;
        }

        /// <summary>
        /// OnChangedEventHandler for supervision of the lokomotive.cs2 file in auto mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            int returnValue = ConvertLocomotiveConfigFile(Path.Combine(LocomotiveConfigFilePath, LocomotiveConfigFileName), ExportFilesPath, false);
            if (returnValue > 0)
            {
                Console.WriteLine(DateTime.Now + " Info: Z21-config files updated according to Lokomotive.cs2 file.");
            }
        }

        /// <summary>
        /// Converts a given *.cs2 config file into *.z21loco config files
        /// </summary>
        /// <param name="fileToConvert">*.cs2 config file for convertion</param>
        /// <param name="pathToPlaceConvertedFile">Path to put z21 config file</param>
        /// <param name="overwriteInternalConfigs">0 = attach file content to internal locomotive list / 1 = delete all entries in locomotive list before plaving new file content</param>
        /// <returns>0 = success / negative = errors</returns>
        private static int ConvertLocomotiveConfigFile(string fileToConvert, string pathToPlaceConvertedFile, bool overwriteInternalConfigs)
        {
            int returnValue = 0;

            ConfigurationCS2 configCS2 = new ConfigurationCS2();
            ConfigurationZ21 configZ21 = new ConfigurationZ21();

            if (overwriteInternalConfigs)
            {
                LocomotiveList.DeleteAll();
            }

                if (Program.remoteFileServer) // get Lokomotive.cs2 from remote to tmp directory
            {
                DownloadLocomotiveConfigFile();
            }
            
            returnValue = configCS2.ImportConfiguration(fileToConvert, overwriteInternalConfigs);

            if (Program.remoteFileServer) // get locomotives icons from remote to tmp directory
            {
                DownloadLocomotiveIcons();
            }

            if (returnValue > 0)
            {
                for (int index = 0; index < LocomotiveList.SizeOf(); index++)
                {
                    if (LocomotiveList.Get(index).RecentlyAdded)
                    {
                        returnValue += configZ21.ExportConfiguration(index, pathToPlaceConvertedFile);
                        LocomotiveList.Get(index).RecentlyAdded = false;
                    }
                }
            }
            else
            {
                returnValue = -1;
            }

            return returnValue;
        }

        /// <summary>
        /// Downloads LoKomotive.cs file to temporary folder
        /// </summary>
        private static void DownloadLocomotiveConfigFile()
        {
            using (WebClient remoteFileServer = new WebClient())
            {
                string url = $"http://{IpAddressHostnameCS2}/config/{LocomotiveConfigFileName}";
                string destinationFilename = Path.Combine(ExportFilesPath, LocomotiveConfigFileName);
                try
                {
                    remoteFileServer.DownloadFile(url, destinationFilename);
                }
                catch (Exception ex)
                {
                    // Console.Write(ex.ToString()); 
                }
            }
        }

        /// <summary>
        /// Downloads all referenced Icon files to temporary folder
        /// </summary>
        private static void DownloadLocomotiveIcons()
        {
            using (WebClient remoteFileServer = new WebClient())
            {
                for (int index = 0; index < LocomotiveList.SizeOf(); index++)
                {
                    string currentIconFilename = $"{LocomotiveList.Get(index).Icon}.png";
                    string destinationPath = Path.Combine(LocomotiveConfigFilePath, "icons");
                    string destinationFilename = Path.Combine(destinationPath, currentIconFilename);

                    if (!Directory.Exists(destinationPath))
                    {
                        Directory.CreateDirectory(destinationPath);
                    }

                    try
                    {
                        remoteFileServer.DownloadFile($"http://{IpAddressHostnameCS2}/icons/{currentIconFilename}", destinationFilename);
                        LocomotiveList.Get(index).Icon = destinationFilename;
                    }
                    catch (Exception ex)
                    {
                        // Console.Write(ex.ToString()); 
                    }
                }
            }
        }






        // Out of MSDN (https://learn.microsoft.com/en-us/dotnet/api/system.configuration.configurationmanager.appsettings?view=net-8.0&redirectedfrom=MSDN#System_Configuration_ConfigurationManager_AppSettings)
        // Todo: This is not storing the parameter. Why?
        private static void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }



    }
}



