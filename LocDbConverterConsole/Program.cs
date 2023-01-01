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
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocDbConverterConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool quitProgram = false;
            int returnValue;
            string exportPath;
            string userEntry;
            string[] userEntrySplit;
            ConfigurationCS2 _cs2 = new ConfigurationCS2();
            ConfigurationZ21 _z21 = new ConfigurationZ21();
            


            // Initialize mapping for functions -> TODO: Make this mapping editable via xml-file
            /*-------------------------------------------------------------------------------------------------------*/
            FunctionTypeMappingList.Set(new FunctionTypeMapping() { Key = 0,    ShortName = "",             Duration = 0,   FunctionTypeIndexCS2 = (int)FunctionTypeCS2.None,           FunctionTypeZ21 = FunctionTypeZ21.none });
            FunctionTypeMappingList.Set(new FunctionTypeMapping() { Key = 1,    ShortName = "Lichtwechs",   Duration = 0,   FunctionTypeIndexCS2 = (int)FunctionTypeCS2.Lichtwechsel,   FunctionTypeZ21 = FunctionTypeZ21.light });

            FunctionTypeMappingList.Set(new FunctionTypeMapping() { Key = 5,    ShortName = "Nachfassen",   Duration = 0,   FunctionTypeIndexCS2 = 5,   FunctionTypeZ21 = FunctionTypeZ21.rotate_right });

            FunctionTypeMappingList.Set(new FunctionTypeMapping() { Key = 7,    ShortName = "Rauch",        Duration = 0,   FunctionTypeIndexCS2 = 7,   FunctionTypeZ21 = FunctionTypeZ21.steam });
            FunctionTypeMappingList.Set(new FunctionTypeMapping() { Key = 8,    ShortName = "RangrGang",    Duration = 0,   FunctionTypeIndexCS2 = 8,   FunctionTypeZ21 = FunctionTypeZ21.hump_gear });

            FunctionTypeMappingList.Set(new FunctionTypeMapping() { Key = 12,   ShortName = "PfeifeLng",    Duration = 0,   FunctionTypeIndexCS2 = 12,  FunctionTypeZ21 = FunctionTypeZ21.whistle_long });
            FunctionTypeMappingList.Set(new FunctionTypeMapping() { Key = 13,   ShortName = "Glocke",       Duration = 0,   FunctionTypeIndexCS2 = 13,  FunctionTypeZ21 = FunctionTypeZ21.bell });

            FunctionTypeMappingList.Set(new FunctionTypeMapping() { Key = 18,   ShortName = "ABV aus",      Duration = 0,   FunctionTypeIndexCS2 = 18,  FunctionTypeZ21 = FunctionTypeZ21.acc_delay });

            FunctionTypeMappingList.Set(new FunctionTypeMapping() { Key = 20,   ShortName = "BrmsQutAUS",   Duration = 0,   FunctionTypeIndexCS2 = 20,  FunctionTypeZ21 = FunctionTypeZ21.sound_brake });

            FunctionTypeMappingList.Set(new FunctionTypeMapping() { Key = 22,   ShortName = "Lichtmasch",   Duration = 0,   FunctionTypeIndexCS2 = 22,  FunctionTypeZ21 = FunctionTypeZ21.alternator });
            FunctionTypeMappingList.Set(new FunctionTypeMapping() { Key = 23,   ShortName = "Geräusch",     Duration = 0,   FunctionTypeIndexCS2 = 23,  FunctionTypeZ21 = FunctionTypeZ21.sound2 });

            FunctionTypeMappingList.Set(new FunctionTypeMapping() { Key = 26,   ShortName = "KohleShauf",   Duration = 0,   FunctionTypeIndexCS2 = 26,  FunctionTypeZ21 = FunctionTypeZ21.scoop_coal });

            FunctionTypeMappingList.Set(new FunctionTypeMapping() { Key = 36,   ShortName = "Kipprost",     Duration = 0,   FunctionTypeIndexCS2 = 36,  FunctionTypeZ21 = FunctionTypeZ21.shaking_grates });
            FunctionTypeMappingList.Set(new FunctionTypeMapping() { Key = 37,   ShortName = "SchienStos",   Duration = 0,   FunctionTypeIndexCS2 = 37,  FunctionTypeZ21 = FunctionTypeZ21.rail_kick });

            FunctionTypeMappingList.Set(new FunctionTypeMapping() { Key = 43,   ShortName = "Kuppeln",      Duration = 0,   FunctionTypeIndexCS2 = 43,  FunctionTypeZ21 = FunctionTypeZ21.couple });

            FunctionTypeMappingList.Set(new FunctionTypeMapping() { Key = 48,   ShortName = "FührStBel",    Duration = 0,   FunctionTypeIndexCS2 = 48,  FunctionTypeZ21 = FunctionTypeZ21.cabin_light });
            FunctionTypeMappingList.Set(new FunctionTypeMapping() { Key = 49,   ShortName = "Injektor",     Duration = 0,   FunctionTypeIndexCS2 = 49,  FunctionTypeZ21 = FunctionTypeZ21.injector });

            FunctionTypeMappingList.Set(new FunctionTypeMapping() { Key = 106,  ShortName = "LuftPumpe",    Duration = 0,   FunctionTypeIndexCS2 = 106, FunctionTypeZ21 = FunctionTypeZ21.air_pump });

            FunctionTypeMappingList.Set(new FunctionTypeMapping() { Key = 111,  ShortName = "WassPumpe",    Duration = 0,   FunctionTypeIndexCS2 = 111, FunctionTypeZ21 = FunctionTypeZ21.feed_pump });
            FunctionTypeMappingList.Set(new FunctionTypeMapping() { Key = 112,  ShortName = "BrmsQutAN",    Duration = 0,   FunctionTypeIndexCS2 = 112, FunctionTypeZ21 = FunctionTypeZ21.sound_brake });

            FunctionTypeMappingList.Set(new FunctionTypeMapping() { Key = 118,  ShortName = "RangrLicht",   Duration = 0,   FunctionTypeIndexCS2 = 118, FunctionTypeZ21 = FunctionTypeZ21.all_round_light });

            FunctionTypeMappingList.Set(new FunctionTypeMapping() { Key = 140,  ShortName = "RangrPfiff",   Duration = -1,  FunctionTypeIndexCS2 = 140, FunctionTypeZ21 = FunctionTypeZ21.sound5 });

            FunctionTypeMappingList.Set(new FunctionTypeMapping() { Key = 219,  ShortName = "Dampfablss",   Duration = 0,   FunctionTypeIndexCS2 = 219, FunctionTypeZ21 = FunctionTypeZ21.dump_steam });

            FunctionTypeMappingList.Set(new FunctionTypeMapping() { Key = 236,  ShortName = "Sanden",       Duration = 0,   FunctionTypeIndexCS2 = 236, FunctionTypeZ21 = FunctionTypeZ21.sanden });
            
            

            // Run the program            
            /*-------------------------------------------------------------------------------------------------------*/

            Console.WriteLine("Locomotive database converter v1.0. Use  /help  to list commands available.");

            while (!quitProgram)
            {
                userEntry = Console.ReadLine();
                userEntrySplit = userEntry.Split(' ');
                switch (userEntrySplit[0].ToLowerInvariant())
                {
                    case "/help":
                        Console.WriteLine("The following commands are available");
                        Console.WriteLine("\thelp                 \t- Help menu");
                        Console.WriteLine("\tconvert <file>       \t- Converts a given locomotive config file from CS2/CS3-format to Z21-format");
                        Console.WriteLine("\timport <file>/<path> \t- Imports a given locomotive config file in CS2/CS3-format to internal memory or checks path for these files");
                        Console.WriteLine("\tlist                 \t- Lists all imported locomotive configurations");
                        Console.WriteLine("\texport <No.> <path>  \t- Export a locomotive config file in Z21-format from internal memory by list entry (Defult path is applications folder)");
                        Console.WriteLine("\texit                 \t- Exits the program");
                        break;

                    case "/convert":
                        if (File.Exists(userEntrySplit[1]) && userEntrySplit[1].Substring(userEntrySplit[1].Length - 4).Contains(".cs2"))
                        {
                            exportPath = Path.GetDirectoryName(userEntrySplit[1]);
                            returnValue = _cs2.ImportConfiguration(userEntrySplit[1]);
                            if (returnValue > 0) returnValue = _z21.ExportConfiguration(LocomotiveList.SizeOf() - 1, exportPath);
                            if (returnValue > 0) Console.WriteLine("File exported to " + exportPath);
                        }
                        else
                        {
                            Console.WriteLine("Error. Could not find *.cs2 locomotive configuration file.");
                        }
                        break;

                    case "/import":
                        returnValue = _cs2.ImportConfiguration(userEntrySplit[1]);
                        if (returnValue > 0) Console.WriteLine("Job done.");
                        else Console.WriteLine("Error. Could not find *.cs2 locomotive configuration file.");
                        break;

                    case "/list":
                        for (int index = 0; index < LocomotiveList.SizeOf(); index++)
                        {
                            Console.WriteLine("\t[" + index + "] - " + LocomotiveList.Get(index).Name.ToString());
                        }
                        break;

                    case "/export":
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
                        break;

                    case "/quit":
                        quitProgram = true;
                        break;

                    default:
                        break;
                }
            }

        }
    }
}



