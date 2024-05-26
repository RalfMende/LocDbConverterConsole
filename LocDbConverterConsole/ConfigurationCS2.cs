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

using System.Diagnostics;

namespace LocDbConverterConsole
{
    public class ConfigurationCS2
    {
        /// <summary>
        /// Imports all locomotive desciption files from a Maerklin CS2/CS3 backup 
        /// </summary>
        /// <param name="file">Path to get locomotive description files from</param>
        /// <param name="overwriteAllExistingConfigs">true: All configs in internal list are overwritten, false: configs are just added to internal list</param>
        /// <returns>0: Code not executed, Negative: Error, Positive: Number of config files found</returns>
        public int ImportConfiguration(string file, bool overwriteAllExistingConfigs)
        {
            int returnValue = 0;

            if (file.EndsWith(".cs2", StringComparison.OrdinalIgnoreCase))
            {
                returnValue = ParseLocomotiveFile(file, overwriteAllExistingConfigs);
            }
            else
            {
                returnValue = -1;
            }
            return returnValue;
        }

        /// <summary>
        /// Imports an *.cs2 locomotive desciption file from CS3/CS2 backup and parses it to internal list
        /// </summary>
        /// <param name="file">*.cs2 locomotive desciption file</param>
        /// <param name="overwriteAllExistingConfigs">true: All configs in internal list are overwritten, false: configs are just added to internal list</param>
        /// <returns>0: Code not executed, Negative: Error, Positive: Number of Locomotives added to internal list</returns>
        private int ParseLocomotiveFile(string file, bool overwriteAllExistingConfigs)
        {
            int returnValue = 0;
            string trimmedLine = string.Empty;
            string key = string.Empty;
            string subKey = string.Empty;
            string subSubKey = string.Empty;
            string value = string.Empty;
            int currentFunctionNumber = -1;
            int numberOfLocomotiveConfigs = 0;
            int numberOfLocomotivesAdded = 0;
            FunctionTypeMapping functionMapping;

            Locomotive locomotive = new Locomotive();

            foreach (string line in System.IO.File.ReadLines(file))
            {
                trimmedLine = line.Trim();

                //if (trimmedLine.StartsWith("["))
                // section currently not needed //TODO check [lokomotive] to proove file to be right

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
                            //case ".uid":
                            //    string tmp = value.Substring(2); break; // 

                            case ".name":
                                string name = value;
                                if (name.Contains("#20")) name = value.Replace("#20", " ");
                                locomotive.Name = name; break;

                            //case ".vorname": name before the last change -> not needed

                            case ".typ":
                            case ".dectyp": //accorting to documentation this should be .typ, but all demo-files show different sub key
                                if (value == "mm2_prg") locomotive.Decodertype = DecoderType.MM;
                                else if (value == "mm2_dil8") locomotive.Decodertype = DecoderType.MM;
                                else if (value == "dcc") locomotive.Decodertype = DecoderType.DCC;
                                else if (value == "mfx") locomotive.Decodertype = DecoderType.MFX;
                                else if (value == "sx1") locomotive.Decodertype = DecoderType.MFX;
                                else locomotive.Decodertype = DecoderType.DCC;
                                break;

                            case ".adresse":
                                locomotive.Address = Convert.ToInt32(value, 16);
                                break;

                            //case ".mfxuid":
                            //    locomotive.mfxuid = value; break;

                            case ".icon":
                                locomotive.Icon = value;
                                break;

                            //case ".symbol": // 0=Electro, 1=Diesel, 2=Steam, 3=No Icon
                            //    locomotive.symbol = value; break;

                            //case ".av":
                            //    locomotive.accelerationDelay = value; break;

                            //case ".bv":
                            //    locomotive.decelerationDelay = value; break;

                            //case ".vmin":
                            //    locomotive.vMin = value; break;

                            case ".vmax":
                                locomotive.VMax = Convert.ToInt32(value);
                                break;

                            case ".tachomax":
                                locomotive.SpeedometerMax = Convert.ToInt32(value);
                                break;

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
                        if (trimmedLine == "lokomotive")
                        {
                            if (numberOfLocomotiveConfigs > 0)
                            {
                                if (!LocomotiveList.Contains(locomotive))
                                {
                                    LocomotiveList.Set(locomotive);
                                    locomotive = new Locomotive();
                                    numberOfLocomotivesAdded++;
                                }
                                else if (overwriteAllExistingConfigs)
                                {
                                    LocomotiveList.Replace(locomotive);
                                    numberOfLocomotivesAdded++;
                                }
                            }
                            numberOfLocomotiveConfigs++;
                        }
                        key = trimmedLine;
                    }
                    else
                    {
                        Debug.Print("Unknown key: " + trimmedLine);
                    }
                }

            } // foreach line

            if (!LocomotiveList.Contains(locomotive))
            {
                LocomotiveList.Set(locomotive);
                locomotive = new Locomotive();
                numberOfLocomotivesAdded++;
            }
            else if (overwriteAllExistingConfigs)
            {
                LocomotiveList.Replace(locomotive);
                numberOfLocomotivesAdded++;
            }
            returnValue = numberOfLocomotivesAdded;
            return returnValue;
        }


    }
}
