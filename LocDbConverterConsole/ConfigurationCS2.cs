/*
 * LocDbConverterConsole - Converter for model train descrption files of digital control stations
 * 
 * Copyright (c) 2023 Ralf Mende
 * 
 * 
 * This file is part of CS6021.
 * 
 * CS6021 is free software: you can redistribute it and/or modify it 
 * under the terms of the GNU General Public License as published by the 
 * Free Software Foundation, either version 3 of the License, or (at your 
 * option) any later version.
 * 
 * CS6021 is distributed in the hope that it will be useful, but 
 * WITHOUT ANY WARRANTY; without even the implied warranty of 
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
 * See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License 
 * along with Foobar. If not, see 
 * <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LocDbConverterConsole
{
    public class ConfigurationCS2
    {
        private int numberOfFilesImported;

        /// <summary>
        /// Imports all locomotive desciption files from a Maerklin CS2/CS3 backup 
        /// </summary>
        /// <param name="fileOrDirectory">Path to get locomotive description files from</param>
        /// <returns>0: Code not executed, Negative: Error, Positive: Number of config files found</returns>
        public int ImportConfiguration(string fileOrDirectory)
        {
            int returnValue = 0;
            numberOfFilesImported = 0;

            FileAttributes attr = File.GetAttributes(fileOrDirectory);

            if (fileOrDirectory.Substring(fileOrDirectory.Length - 4).Contains(".zip"))
            {
                //  TODO: How to unzip https://docs.microsoft.com/en-us/dotnet/api/system.io.compression.ziparchive.getentry?view=netframework-4.5
                //using (ZipArchive archive = ZipFile.Open(zipPath, ZipArchiveMode.Update))
                //  {
                //      ZipArchiveEntry entry = archive.GetEntry("ExistingFile.txt");
                //      using (StreamWriter writer = new StreamWriter(entry.Open()))
                //      {
                //          writer.BaseStream.Seek(0, SeekOrigin.End);
                //          writer.WriteLine("append line to file");
                //      }
                //      entry.LastWriteTime = DateTimeOffset.UtcNow.LocalDateTime;
                //  }
                //  returnValue = 3;
            }
            else if (fileOrDirectory.Substring(fileOrDirectory.Length - 4).Contains(".cs2"))
            {
                ImportLocomotiveFile(fileOrDirectory);
                returnValue = numberOfFilesImported;
            }
            else if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                fileOrDirectory = fileOrDirectory.EndsWith(@"\") ? fileOrDirectory : fileOrDirectory + @"\";
                GetAllLocomotiveFilesInConfiguration(fileOrDirectory);
                returnValue = numberOfFilesImported;
            }
            else
            {
                returnValue = -1;
            }
            return returnValue;
        }

        /// <summary>
        /// Helper function to search for all locomotive description files (recursively) in given directory
        /// </summary>
        /// <param name="directory">Path to get locomotive description files from</param>
        private void GetAllLocomotiveFilesInConfiguration(string directory)
        {
            foreach (string subDirectory in Directory.GetDirectories(directory))
            {
                foreach (string file in Directory.GetFiles(subDirectory))
                {
                    if (file.Substring(file.Length - 4).Contains(".cs2") && subDirectory.Substring(subDirectory.Length - 6).Contains("loks"))
                    {
                        ImportLocomotiveFile(file);
                    }
                }
                GetAllLocomotiveFilesInConfiguration(subDirectory);
            }
        }

        /// <summary>
        /// Imports an *.cs2 locomotive desciption file from CS3/CS2 backup and parses it to internal list
        /// </summary>
        /// <param name="file">*.cs2 locomotive desciption file</param>
        /// <returns>0: Code not executed, Negative: Error, Positive: Index of list where element was added</returns>
        public int ImportLocomotiveFile(string file)
        {
            int returnValue = 0;
            string trimmedLine = null;
            string key = null;
            string subKey = null;
            string subSubKey = null;
            string value = null;
            int currentFunctionNumber = -1;
            FunctionTypeMapping functionMapping;

            Locomotive locomotive = new Locomotive();

            string[] lines = System.IO.File.ReadAllLines(file);

            if (lines[0].Contains("[lokomotive]")) //first check for section to ensure this is a locomotives backup file
            {
                foreach (string line in lines)
                {
                    trimmedLine = line.Trim();

                    //if (trimmedLine.StartsWith("["))
                    // section currently not needed

                    if (trimmedLine.StartsWith(".."))
                    {
                        if (subKey == ".funktionen")
                        {
                            subSubKey = trimmedLine.Substring(0, trimmedLine.IndexOf("="));
                            value = trimmedLine.Substring(trimmedLine.IndexOf("=") + 1);

                            switch (subSubKey)
                            {
                                case "..nr": // 0 -> F0   15 -> F15
                                    currentFunctionNumber = Convert.ToInt32(value);
                                    break;

                                case "..typ":
                                    functionMapping = FunctionTypeMappingList.Find(x => x.FunctionTypeIndexCS2 == Convert.ToInt32(value));
                                    locomotive.Functions[currentFunctionNumber] = functionMapping.Key;
                                    break;

                                case "..typ2":
                                    //TODO: Sometimes this occurs in config-files, but I don't know what it does...
                                    break;

                                case "..dauer": // duration   0: constantly, -1 momentary, else time
                                    //locomotive.functions[currentFunctionNumber].Duration = Convert.ToInt32(value); //TODO: thisis currently not needed, as covered by my mapping
                                    break;

                                default:
                                    Debug.Print("Unknown .funktionen subKey: " + trimmedLine);
                                    break;
                            }
                        }
                        else
                        {
                            Debug.Print("Unknown subSubKey: " + trimmedLine);
                        }
                    }
                    else if (trimmedLine.StartsWith("."))
                    {
                        if (key == "lokomotive")
                        {
                            if (trimmedLine.Contains("="))
                            {
                                value = trimmedLine.Substring(trimmedLine.IndexOf("=") + 1);
                                subKey = trimmedLine.Substring(0, trimmedLine.Length - value.Length - 1);
                            }
                            else
                            {
                                subKey = trimmedLine;
                            }

                            switch (subKey)
                            {
                                case ".uid":
                                    //locomotive.Uid = Convert.ToInt32(value.Substring(2, value.Length - 2)); //TODO: How to best store this information?
                                    locomotive.Address = Convert.ToInt32(value.Substring(value.Length - 2), 16); // get address here, as seperate field is sometimes not available
                                    break;

                                case ".name":
                                    string name = value;
                                    if (name.Contains("#20")) name = value.Replace("#20", " ");
                                    locomotive.Name = name; break;

                                //case ".vorname": name before the last change -> not needed

                                case ".dectyp": //accorting to documentation this should be .typ, but all demo-files show different sub key
                                    if (value == "mm2_prg") locomotive.Decodertype = DecoderType.DCC;
                                    else if (value == "mm2_dil8") locomotive.Decodertype = DecoderType.DCC;
                                    else if (value == "dcc") locomotive.Decodertype = DecoderType.DCC;
                                    else if (value == "mfx") locomotive.Decodertype = DecoderType.DCC;
                                    else if (value == "sx1") locomotive.Decodertype = DecoderType.DCC;
                                    break;

                                case ".adresse":
                                    locomotive.Address = Convert.ToInt32(value); break;

                                //case ".mfxuid":
                                //    locomotive.mfxuid = value; break;

                                //case ".icon":
                                //   locomotive.icon = value; break;

                                //case ".symbol": // 0=Electro, 1=Diesel, 2=Steam, 3=No Icon
                                //    locomotive.symbol = value; break;

                                //case ".av":
                                //    locomotive.accelerationDelay = value; break;

                                //case ".bv":
                                //    locomotive.decelerationDelay = value; break;

                                //case ".vmin":
                                //    locomotive.vMin = value; break;

                                case ".vmax":
                                    locomotive.VMax = Convert.ToInt32(value); break;

                                case ".tachomax":
                                    locomotive.SpeedometerMax = Convert.ToInt32(value); break;

                                //case ".volume":
                                //    locomotive.volume = value; break;

                                //case ".artikelnr":
                                //    locomotive.articleNumber = value; break;

                                //case ".spa":
                                //    locomotive.spa = value; break;

                                //case ".ft":
                                //    locomotive.ft = value; break;

                                case ".funktionen":
                                    currentFunctionNumber++;
                                    //locomotive.SetLocomotiveFunction(new LocomotiveFunction());
                                    break;

                                default:
                                    Debug.Print("Unknown lokomotive subKey: " + trimmedLine);
                                    break;
                            }
                        }
                        else 
                        {
                            Debug.Print("Unknown subKey: " + trimmedLine);
                        }
                    }
                    else
                    {
                        if (!trimmedLine.Contains("="))
                        {
                            key = trimmedLine;
                        }
                        else
                        {
                            Debug.Print("Unknown key: " + trimmedLine);
                        }
                    }

                }//for each line
            }
            else // not a locomotive config file
            {
                returnValue = -1;
            }
            returnValue = LocomotiveList.Set(locomotive);
            numberOfFilesImported++;
            return returnValue;
        }

        /// <summary>
        /// Exports an *.cs2 locomotive desciption file for Maerklin CS3/CS2 from internal list
        /// </summary>
        /// <param name="listIndex">Index of the locomotives list (0-based)</param>
        /// <returns></returns>
        public int ExportLocomotiveFile(int listIndex)
        {
            int returnValue = 0;

            //TODO

            return returnValue;
        }



    }
}
