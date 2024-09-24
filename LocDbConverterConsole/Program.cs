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
using System.Timers;
using System.Xml;
using System.Collections.Generic;
using System.Diagnostics;

namespace LocDbConverterConsole
{
    internal class Program
    {
        public static string WorkingDirectory = string.Empty;
        private static string locomotiveConfigFilePath = string.Empty;
        private static string exportFilesPath = string.Empty;
        private static string ipAddressHostnameCS2 = string.Empty;
        private static string importPathArg = string.Empty;
        private static string exportPathArg = string.Empty;    
        private const string locomotiveConfigFileName = "lokomotive.cs2";
        private static bool remoteFileServer = false;
        private static string osNameAndVersion = string.Empty;
        private static System.Timers.Timer AutoConvertionTimer;

        static void Main(string[] args)
        {
            bool quitProgram = false;
            int returnValue;
            int status = 0;
            bool setupOk = true;
            string osNameAndVersion = RuntimeInformation.OSDescription;
            FileSystemWatcher watcher = new FileSystemWatcher();
            AutoConvertionTimer = new System.Timers.Timer();

            Console.WriteLine($"Welcome to LocDbConverter Console");

            // Initialize application
            ParseArguments(args);
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
                    string userEntryLine = Console.ReadLine().Trim().ToLowerInvariant();
                    string[] userEntry = userEntryLine.Split(' ');
                    string command = userEntry[0];

                    switch (command)
                    {
                        case "?":
                        case "h":
                        case "help":
                            DisplayHelp();
                            break;

                        case "c":
                        case "convert":
                            returnValue = ConvertLocomotiveConfigFile(Path.Combine(locomotiveConfigFilePath, locomotiveConfigFileName), exportFilesPath, false);
                            if (returnValue > 0)
                            {
                                Console.WriteLine($"{DateTime.Now} {returnValue} Z21-config files updated according to Lokomotive.cs2 file.");
                            }
                            break;

                        case "f":
                        case "force":
                            returnValue = ConvertLocomotiveConfigFile(Path.Combine(locomotiveConfigFilePath, locomotiveConfigFileName), exportFilesPath, true);
                            if (returnValue > 0)
                            {
                                Console.WriteLine($"{DateTime.Now} {returnValue} Z21-config files updated according to Lokomotive.cs2 file.");
                            }
                            break;

                        case "a":
                        case "auto":
                            if (!remoteFileServer)
                            {
                                SetupAutoConversionWatcher(watcher);
                            }
                            else
                            {
                                double timing = GetTiming(userEntry);
                                SetupAutoConversionTimer(timing);
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

        /// <summary>
        /// Parses arguments given to main program for import and export path information
        /// </summary>
        /// <param name="args"></param>
        /// <returns>counts of arguments found</returns>
        static int ParseArguments(string[] args)
        {
            int result = 0;

            foreach (string arg in args)
            {
                string[] argValue = arg.Trim().Split('=');

                if (argValue.Length == 2)
                {
                    string key = argValue[0];
                    string value = argValue[1];

                    switch (key)
                    {
                        case "--importpath":
                            importPathArg = value;
                            result++;
                            break;

                        case "--exportpath":
                            exportPathArg = value;
                            result++;
                            break;
                    }
                }
                else if (argValue.Length == 1 && (argValue[0] == "--help" || argValue[0] == "-h" || argValue[0] == "-?"))
                {
                    DisplayHelp();
                    result++;
                }
                else
                {
                    Console.WriteLine($"ERROR Argument unknown or malformed: {arg}");
                }
            }
            return result;
        }

        /// <summary>
        /// Initializes all Paths, Connections and parameter files that are used by the app 
        /// </summary>
        /// <returns>true = initialization Ok / false = initialization failed / </returns>
        static bool InitializeApplication()
        {
            // Get mapping for functions from mapping.xml file
            string mappingFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mapping.xml");
            if (!File.Exists(mappingFile))
            {
                Console.WriteLine($"ERROR File mapping.xml not found at {mappingFile}");
                return false;
            }
            else
            {
                Console.Write("Importing mapping.xml file ... ");
                int statusMappingFile = ImportMappingConfigFile(mappingFile);
                if (statusMappingFile < 1)
                {
                    Console.WriteLine("ERROR File could not be located beside exe. Application is closed.");
                    return false;
                }
                else
                {
                    Console.WriteLine("OK");
                }
            }

            // Check lokomotive.cs file
            Console.WriteLine("Checking access to lokomotive.cs file ... ");
            int statusConfigPath = GetLocomotiveConfigFilePath();
            if (statusConfigPath < 1)
            {
                Console.WriteLine("ERROR Lokomotive.cs file could not be located. Application is closed.");
                return false;
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
                Console.WriteLine("ERROR Path not found. Application is closed.");
                return false;
            }
            else
            {
                Console.WriteLine("OK");
            }

            // Set temporary working directory
            osNameAndVersion = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
            if (osNameAndVersion.Contains("Windows"))
            {
                WorkingDirectory = Path.Combine(System.IO.Path.GetTempPath(), "LocDbConverter");
                //Console.WriteLine($"Temp directory set to {WorkingDirectory}");
                if (remoteFileServer) locomotiveConfigFilePath = WorkingDirectory;
            }
            else // Todo: Workaround because file copy to/from temporary directory does not work on Linux, maybe due to user rights management
            {
                WorkingDirectory = exportFilesPath;
            }

            return true;
        }

        /// <summary>
        /// Display help menu
        /// </summary>
        /// <param name="programVersion">string with program version</param>
        static void DisplayHelp()
        {
            Console.WriteLine("Arguments:");
            Console.WriteLine("\t--help / -h    \tHelp menu");
            Console.WriteLine("\t--importpath=  \tSets local path or IP address, where Lokomotive.cs2 file and images can be found.");
            Console.WriteLine("\t--exportpath=  \tSets path to where converted files are stored.");
            //Console.WriteLine("\t--auto / -a    \tStart automatic convertion of the Lokomotove.cs2 file."); // TODO not yet implemented
            Console.WriteLine("\nCommands:");
            Console.WriteLine("\th / ?  \tHelp menu");
            Console.WriteLine("\tc      \tConvert Lokomotive.cs2 file");
            Console.WriteLine("\ta 5    \tAuto convert Lokomotive.cs2 file according to settings in App.config file. Opt. Timing in [min].");
            Console.WriteLine("\tf      \tForce convert of all entries in Lokomotive.cs2 file");
            Console.WriteLine("\tx      \tExit the program");
            Console.WriteLine("\n" + $"This is version {Assembly.GetExecutingAssembly().GetName().Version.ToString()} (beta). Code is available under GNU General Public License at Github https://github.com/RalfMende/LocDbConverterConsole.");
        }
        
        /// <summary>
        /// Setup the locomotive config file watcher for auto convertion
        /// </summary>
        /// <param name="watcher">FileSystemWatcher for auto convertion</param>
        static void SetupAutoConversionWatcher(FileSystemWatcher watcher)
        {
            try
            {
                watcher.Path = locomotiveConfigFilePath;
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
        /// Setup the locomotive config file watcher for auto convertion
        /// </summary>
        /// <param name="watcher">FileSystemWatcher for auto convertion</param>
        static void SetupAutoConversionTimer(double timing)
        {
            try
            {
                if (!AutoConvertionTimer.Enabled)
                {
                    AutoConvertionTimer.Elapsed += OnTimedEvent;
                    AutoConvertionTimer.AutoReset = true;
                }
                else
                {
                    AutoConvertionTimer.Stop();
                }
                AutoConvertionTimer.Interval = 1000 * 60 * timing;
                AutoConvertionTimer.Start();
                Console.WriteLine($"Automatical convertion for Z21 files activated every {timing} minutes.");
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
            ConsoleKey userInput;

            if (importPathArg != string.Empty)
            {
                returnValue = CheckLocomotiveConfigFileAccess(importPathArg);
                return returnValue;
            }
            if (returnValue < 1)
            {
                string tmpConfigFilePath = ConfigurationManager.AppSettings["LocomotiveConfigFilePath"].Trim();
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
                        locomotiveConfigFilePath = tmpConfigFilePath;
                        return returnValue;
                    }
                }
                else if (userInput == ConsoleKey.N)
                {
                    returnValue = -1;
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
                            locomotiveConfigFilePath = tmpConfigFilePath;
                            AddUpdateAppSettings("LocomotiveConfigFilePath", tmpConfigFilePath);
                            return returnValue;
                        }
                        else
                        {
                            attemptsLeft--;
                        }
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
            ConsoleKey userInput;

            if (exportPathArg != string.Empty)
            {
                returnValue = CheckExportPathAccess(exportPathArg);
                return returnValue;
            }
            if (returnValue < 1)
            {
                string tmpExportFilesPath = ConfigurationManager.AppSettings["ExportFilesPath"].Trim();
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
                        exportFilesPath = tmpExportFilesPath;
                        return returnValue;
                    }
                }
                else if (userInput == ConsoleKey.N)
                {
                    returnValue = -1;
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
                            exportFilesPath = tmpExportFilesPath;
                            AddUpdateAppSettings("ExportFilesPath", tmpExportFilesPath);
                            return returnValue;
                        }
                        else
                        {
                            attemptsLeft--;
                        }
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
                if (!path.EndsWith(Path.DirectorySeparatorChar)) path += Path.DirectorySeparatorChar;
                returnValue = CheckLocomotiveConfigFileAccessLocal(path);
            }
            else
            {
                returnValue = CheckLocomotiveConfigFileAccessRemote(path);
                if (returnValue > 0)
                {
                    Program.remoteFileServer = true;
                    ipAddressHostnameCS2 = path;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Checks if locomotive config file is available at path
        /// </summary>
        /// <param name="IpAddressHostname">IP address or Hostname defined in config file</param>
        /// <returns>1 = success / -1 = error occured</returns>
        private static int CheckLocomotiveConfigFileAccessRemote(string ipAddressHostname)
        {
            using (WebClient remoteFileServer = new WebClient())
            {
                try
                {
                    string url = $"http://{ipAddressHostname}/config/{locomotiveConfigFileName}"; //Todo: Double check, that config was not entered by user already
                    remoteFileServer.DownloadData(url);
                }
                catch (Exception ex)
                {
                    // Console.WriteLine(ex.ToString()); 
                    return -1;
                }

            }
            return 1;
        }

        /// <summary>
        /// Checks if locomotive config file is available at local path, definded in config-file
        /// </summary>
        /// <param name="locomotiveListPath">Path and file defined in config file</param>
        /// <returns>positive = success / negative = errors</returns>
        private static int CheckLocomotiveConfigFileAccessLocal(string locomotiveListPath)
        {
            string fullPath = Path.Combine(locomotiveListPath, locomotiveConfigFileName);
            return File.Exists(fullPath) ? 1 : -1;
        }

        /// <summary>
        /// Checks if path for file export, defined in config-file is available
        /// </summary>
        /// <param name="exportPath">Path defined in config file</param>
        /// <returns>positive = success / negative = errors</returns>
        private static int CheckExportPathAccess(string exportPath)
        {
            return Directory.Exists(exportPath) ? 1 : -1;
        }

        /// <summary>
        /// Get the timing value from user entry
        /// </summary>
        /// <param name="userEntry">user entry as array</param>
        /// <returns>timing value</returns>
        private static double GetTiming(string[] userEntry)
        {
            double timing = 1; // by default 1 Minute
            if (userEntry.Length > 1 && double.TryParse(userEntry[1], out double tmpValue))
            {
                if (tmpValue >= 0.5 && tmpValue <= 30)
                {
                    timing = tmpValue;
                }
                else
                {
                    Console.WriteLine("ERROR Value must be between 0.5 and 30 minutes. -> Set to every minute (default).");
                }
            }
            return timing;
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
            int returnValue = ConvertLocomotiveConfigFile(Path.Combine(locomotiveConfigFilePath, locomotiveConfigFileName), exportFilesPath, false);
            if (returnValue > 0)
            {
                Console.WriteLine($"{DateTime.Now} {returnValue} Z21-config files updated according to Lokomotive.cs2 file.");
            }
        }

        /// <summary>
        /// OnTimedEventHandler for the auto convertion of the lokomotive.cs2 file
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            int returnValue = ConvertLocomotiveConfigFile(Path.Combine(locomotiveConfigFilePath, locomotiveConfigFileName), exportFilesPath, true);
            if (returnValue > 0)
            {
                Console.WriteLine($"{DateTime.Now} {returnValue} Z21-config files updated according to Lokomotive.cs2 file.");
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

            if (Program.remoteFileServer) // get Lokomotive.cs2 from remote to temporary folder
            {
                DownloadLocomotiveConfigFile();
            }
            
            returnValue = configCS2.ImportConfiguration(fileToConvert, overwriteInternalConfigs);

            if (Program.remoteFileServer) // get only required locomotive icons from remote to temporary folder
            {
                DownloadLocomotiveIcons();
            }

            if (returnValue > 0)
            {
                for (int index = 0; index < LocomotiveList.SizeOf(); index++)
                {
                    if (LocomotiveList.Get(index).RecentlyAdded && LocomotiveList.Get(index).Name != "Lokliste") // SRSEII
                    {
                        if (configZ21.ExportConfiguration(index) > 0)
                        {
                            string newConfigFileName = LocomotiveList.Get(index).Name + ".z21loco";
                            if (osNameAndVersion.Contains("Windows")) //Todo: Does not work in Linux, maybe due to user rights? -> Workaround by WorkingDirectory = exportFilesPath
                            {
                                File.Copy(Path.Combine(WorkingDirectory, newConfigFileName), Path.Combine(pathToPlaceConvertedFile, newConfigFileName), true);
                            }
                            LocomotiveList.Get(index).RecentlyAdded = false;
                            returnValue = 2;
                        }
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
        /// Downloads Lokomotive.cs file from CS2 to temporary folder
        /// </summary>
        /// <returns>1 = success / -1 = Error occured</returns>
        private static int DownloadLocomotiveConfigFile()
        {
            using (WebClient remoteFileServer = new WebClient())
            {
                string url = $"http://{ipAddressHostnameCS2}/config/{locomotiveConfigFileName}";
                string destinationFilename = Path.Combine(WorkingDirectory, locomotiveConfigFileName);
                try
                {
                    remoteFileServer.DownloadFile(url, destinationFilename);
                }
                catch (Exception ex)
                {
                    // Console.Write(ex.ToString()); 
                    return -1;
                }
            }
            return 1;
        }

        /// <summary>
        /// Downloads all referenced icon files from CS2 to temporary folder
        /// </summary>
        /// <returns>1 = success / -1 = error occured</returns>
        private static int DownloadLocomotiveIcons()
        {
            using (WebClient remoteFileServer = new WebClient())
            {
                for (int index = 0; index < LocomotiveList.SizeOf(); index++)
                {
                    string currentIconFilename = $"{LocomotiveList.Get(index).Icon}.png";
                    string destinationPath = Path.Combine(WorkingDirectory, "icons");
                    string destinationFilename = Path.Combine(destinationPath, currentIconFilename);

                    if (!Directory.Exists(destinationPath))
                    {
                        Directory.CreateDirectory(destinationPath);
                    }

                    try
                    {
                        remoteFileServer.DownloadFile($"http://{ipAddressHostnameCS2}/icons/{currentIconFilename}", destinationFilename);
                        LocomotiveList.Get(index).Icon = destinationFilename;
                    }
                    catch (Exception ex)
                    {
                        // Console.Write(ex.ToString()); 
                        return -1;
                    }
                }
            }
            return 1;
        }

        /// <summary>
        /// Adds or Updates key values in App.config
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns>1 = success / -1 = error occured</returns>
        private static int AddUpdateAppSettings(string key, string value)
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
                return -1;
            }
            return 1;
        }



    }
}



