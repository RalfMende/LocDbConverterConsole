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

using Microsoft.Data.Sqlite;
using System.IO.Compression;

namespace LocDbConverterConsole
{
    public class ConfigurationZ21
    {
        /// <summary>
        /// Exports a z21 description file with the given locomotive list entry
        /// </summary>
        /// <param name="listIndex">Index of the locomotives list (0-based)</param>
        /// <param name="exportPath">Directory to where the config file needs to be exported</param>
        /// <returns>0: Code not executed, Negative: Error, Positive: Ok</returns>
        public int ExportConfiguration(int listIndex, string exportPath)
        {
            int returnValue = 0;

            string uuid = Guid.NewGuid().ToString(); // uuid would only be needed in case images should be stored
            string dirTempFiles = Path.Combine(exportPath, "temp");
            string dirTempSqliteFile = Path.Combine(dirTempFiles, "export", uuid);
            string tempSqliteFile = Path.Combine(dirTempSqliteFile, "Loco.sqlite");

            try
            {
                if (Directory.Exists(dirTempFiles))
                {
                    Directory.Delete(dirTempFiles, true);
                }
                Directory.CreateDirectory(dirTempSqliteFile);

                string pictureFile = LocomotiveList.Get(listIndex).Icon;
                string newPictureFilename = string.Empty;

                if (File.Exists(pictureFile))
                {
                    newPictureFilename = $"{uuid}.png";
                    File.Copy(pictureFile, Path.Combine(dirTempSqliteFile, newPictureFilename), true);
                }

                returnValue = ExportDbFile(tempSqliteFile, listIndex, newPictureFilename);

                string zipFileName = Path.Combine(exportPath, LocomotiveList.Get(listIndex).Name.Replace("/", "") + ".z21loco");
                if (File.Exists(zipFileName))
                {
                    File.Delete(zipFileName);
                }
                ZipFile.CreateFromDirectory(dirTempFiles, zipFileName);

                return returnValue;
            }
            catch(Exception ex)
            {
                // Console.Write(ex.ToString());
                return -1;
            }
            finally
            {
                // Ensure the temporary directory is always cleaned up
                if (Directory.Exists(dirTempFiles))
                {
                    Directory.Delete(dirTempFiles, true);
                }
            }
        }

        /// <summary>
        /// Exports a *.sqlite locomotive desciption file for Rodo Z21 from internal list
        /// </summary>
        /// <param name="databaseFile">Sqlite database file that will be used to add data</param>
        /// <param name="listIndex">Index of the locomotives list (0-based)</param>
        /// <returns>0: Code not executed, Negative: Error, Positive: Ok</returns>
        /// <exception cref="Exception"></exception>
        private int ExportDbFile(string databaseFile, int listIndex, string pictureName)
        {
            int returnValue = 0;
            FunctionTypeMapping functionMapping;

            using (var connection = new SqliteConnection("Data Source=" + databaseFile))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    try
                    {
                        // ---------- set missing PRAGMAs ----------

                        command.CommandText = "PRAGMA user_version = 15";
                        command.ExecuteNonQuery();


                        // ---------- create tables ----------
 
                        command.CommandText = @"CREATE TABLE 'categories' ('id' INTEGER PRIMARY KEY NOT NULL, 'name' TEXT)";
                        command.ExecuteNonQuery();
                        command.CommandText = @"CREATE TABLE 'control_station_control_states' ('id' INTEGER PRIMARY KEY, 'control_id' INTEGER, 'state' INTEGER, 'address1_value' INTEGER, 'address2_value' INTEGER, 'address3_value' INTEGER)";
                        command.ExecuteNonQuery();
                        command.CommandText = @"CREATE TABLE 'control_station_controls' ('id' INTEGER PRIMARY KEY, 'page_id' INTEGER, 'x' INTEGER, 'y' INTEGER, 'angle' REAL, 'type' INTEGER, 'address1' INTEGER, 'address2' INTEGER, 'address3' INTEGER, button_type INTEGER DEFAULT 0, time REAL DEFAULT 0)";
                        command.ExecuteNonQuery();
                        command.CommandText = @"CREATE TABLE 'control_station_images' ('id' INTEGER PRIMARY KEY, 'page_id' INTEGER, 'image_name' TEXT)";
                        command.ExecuteNonQuery();
                        command.CommandText = @"CREATE TABLE 'control_station_notes' ('id' INTEGER PRIMARY KEY, 'page_id' INTEGER, 'x' INTEGER, 'y' INTEGER, 'text' TEXT, 'font_size' INTEGER, angle INTEGER DEFAULT 0, type INTEGER DEFAULT 1)";
                        command.ExecuteNonQuery();
                        command.CommandText = @"CREATE TABLE 'control_station_pages' ('id' INTEGER PRIMARY KEY, 'position' INTEGER, 'name' TEXT,'thumb' TEXT)";
                        command.ExecuteNonQuery();
                        command.CommandText = @"CREATE TABLE 'control_station_rails' ('id' INTEGER PRIMARY KEY, 'page_id' INTEGER, 'left_control_id' INTEGER, 'right_control_id' INTEGER, 'left_outlet' INTEGER, 'right_outlet' INTEGER, 'value' REAL, left_response_module_id INTEGER DEFAULT 0, right_response_module_id INTEGER DEFAULT 0)";
                        command.ExecuteNonQuery();
                        command.CommandText = @"CREATE TABLE 'control_station_response_modules' ('id' INTEGER PRIMARY KEY, 'page_id' INTEGER, 'type' INTEGER, 'address' INTEGER, 'report_address' INTEGER, 'afterglow' INTEGER,  'x' INTEGER, 'y' INTEGER, 'angle' INTEGER)";
                        command.ExecuteNonQuery();
                        command.CommandText = @"CREATE TABLE 'control_station_route_list' ('id' INTEGER PRIMARY KEY, 'route_id' INTEGER, 'control_id' TEXT, 'state_id' INTEGER, 'position' INTEGER, 'wait_time' INTEGER, type INTEGER DEFAULT 0, signal_id INTEGER DEFAULT 0, signal_aspect INTEGER DEFAULT 0)";
                        command.ExecuteNonQuery();
                        command.CommandText = @"CREATE TABLE 'control_station_routes' ('id' INTEGER PRIMARY KEY, 'page_id' INTEGER, 'name' TEXT, 'x' INTEGER, 'y' INTEGER, 'angle' INTEGER)";
                        command.ExecuteNonQuery();
                        command.CommandText = @"CREATE TABLE 'control_station_signals' ('id' INTEGER PRIMARY KEY NOT NULL, 'page_id' INTEGER, 'x' INTEGER, 'y' INTEGER, 'angle' REAL, 'signal_id' INTEGER, 'signal_graph' INTEGER, 'address' INTEGER, 'active_aspects' TEXT, 'communication' INTEGER, 'z21_pro_link_id' INTEGER)";
                        command.ExecuteNonQuery();
                        command.CommandText = @"CREATE TABLE 'dc_functions' ('id' INTEGER PRIMARY KEY, 'vehicle_id' INTEGER,'position' INTEGER,'time' TEXT,'image_name' TEXT, 'function' INTEGER, 'cab_function_description' TEXT, 'drivers_cab' TEXT, 'shortcut' TEXT NOT NULL Default '', button_type INT NOT NULL Default 0, 'show_function_number' INTEGER NOT NULL Default 1, 'is_configured' INTEGER NOT NULL Default 0)";
                        command.ExecuteNonQuery();
                        command.CommandText = @"CREATE TABLE 'functions' ('id' INTEGER PRIMARY KEY NOT NULL, 'vehicle_id' INTEGER, 'button_type' INTEGER NOT NULL Default 0, 'shortcut' TEXT NOT NULL Default '', 'time' TEXT, 'position' INTEGER, 'image_name' TEXT, 'function' INTEGER, 'show_function_number' INTEGER NOT NULL Default 1, 'is_configured' INTEGER NOT NULL Default 0)";
                        command.ExecuteNonQuery();
                        command.CommandText = @"CREATE TABLE 'layout_data' ('id' INTEGER PRIMARY KEY NOT NULL, 'name' TEXT, control_station_type TEXT DEFAULT 'free', control_station_theme TEXT DEFAULT 'free')";
                        command.ExecuteNonQuery();
                        command.CommandText = @"CREATE TABLE 'paired_z21_pro_links' ('id' INTEGER PRIMARY KEY NOT NULL, 'name' TEXT, 'mac' TEXT, 'last_seen_date' DATE, 'ip' TEXT, 'last_connected_device' INTEGER)";
                        command.ExecuteNonQuery();
                        command.CommandText = @"CREATE TABLE 'traction_list' ('id' INTEGER PRIMARY KEY NOT NULL, 'loco_id' INTEGER, 'regulation_step' INTEGER, 'time' REAL)";
                        command.ExecuteNonQuery();
                        command.CommandText = @"CREATE TABLE 'train_list' ('id' INTEGER PRIMARY KEY NOT NULL, 'train_id' INTEGER, 'vehicle_id' INTEGER, 'position' INTEGER)";
                        command.ExecuteNonQuery();
                        command.CommandText = @"CREATE TABLE 'update_history' ('id' INTEGER PRIMARY KEY, 'os' TEXT, 'update_date' TEXT, 'build_version' TEXT, 'build_number' INTEGER, 'to_database_version' INTEGER)";
                        command.ExecuteNonQuery();
                        command.CommandText = @"CREATE TABLE 'vehicles' ('id' INTEGER PRIMARY KEY NOT NULL, 'name' TEXT, 'image_name' TEXT, 'type' INTEGER, 'max_speed' INTEGER,'address' INTEGER, 'active' INTEGER, 'position' INTEGER, 'drivers_cab' TEXT, 'full_name' TEXT, 'speed_display' INTEGER, 'railway' TEXT, 'buffer_lenght' TEXT, 'model_buffer_lenght' TEXT, 'service_weight' TEXT, 'model_weight' TEXT, 'rmin' TEXT, 'article_number' TEXT, 'decoder_type' TEXT, 'owner' TEXT, 'build_year' TEXT, 'owning_since' TEXT, 'traction_direction' INTEGER, 'description' TEXT, 'dummy' INTEGER, 'ip' TEXT, 'video' INTEGER, 'video_x' INTEGER, 'video_y' INTEGER, 'video_width' INTEGER, 'panorama_x' INTEGER, 'panorama_y' INTEGER, 'panorama_width' INTEGER, 'panoramaImage' TEXT, 'direct_steering' INTEGER, crane INTEGER DEFAULT 0)";
                        command.ExecuteNonQuery();
                        command.CommandText = @"CREATE TABLE 'vehicles_to_categories' ('id' INTEGER PRIMARY KEY NOT NULL, 'vehicle_id' INTEGER NOT NULL, 'category_id' INTEGER NOT NULL)";
                        command.ExecuteNonQuery();


                        // ----------fill table vehicles ----------

                        //command.CommandText =
                        //    "INSERT INTO vehicles ([id], [name], [type], [max_speed], [address], [active], [position], [speed_display], [traction_direction], [dummy], [ip], [video], [video_x], [video_y], [video_width], [panorama_x], [panorama_y], [panorama_width], [direct_steering], [crane]) " +
                        //    "VALUES(@id, @name, @type, @max_speed, @address, @active, @position, @speed_display, @traction_direction, @dummy, @ip, @video, @video_x, @video_y, @video_width, @panorama_x, @panorama_y, @panorama_width, @direct_steering, @crane); ";
                        command.CommandText =
                            "INSERT INTO vehicles ([id], [name], [image_name], [type], [max_speed], [address], [active], [speed_display], [traction_direction], [crane]) " +
                            "VALUES(@id, @name, @image_name, @type, @max_speed, @address, @active, @speed_display, @traction_direction, @crane); ";
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@id", 1);
                        command.Parameters.AddWithValue("@name", LocomotiveList.Get(listIndex).Name);
                        command.Parameters.AddWithValue("@image_name", pictureName);
                        command.Parameters.AddWithValue("@type", 0); //0=Locomotive
                        command.Parameters.AddWithValue("@max_speed", LocomotiveList.Get(listIndex).SpeedometerMax);
                        command.Parameters.AddWithValue("@address", ConvertAddressSrseii(LocomotiveList.Get(listIndex).Address, LocomotiveList.Get(listIndex).Decodertype));
                        command.Parameters.AddWithValue("@active", 1);
                        //command.Parameters.AddWithValue("@position", 0); //listposition in Z21-App
                        command.Parameters.AddWithValue("@speed_display", 0); //0=km/h
                        command.Parameters.AddWithValue("@traction_direction", 0);
                        //command.Parameters.AddWithValue("@dummy", 0); //don't know if this does something
                        //command.Parameters.AddWithValue("@ip", "192.168.15.2");
                        //command.Parameters.AddWithValue("@video", 0);
                        //command.Parameters.AddWithValue("@video_x", 0);
                        //command.Parameters.AddWithValue("@video_y", 0);
                        //command.Parameters.AddWithValue("@video_width", 768);
                        //command.Parameters.AddWithValue("@panorama_x", 0);
                        //command.Parameters.AddWithValue("@panorama_y", 0);
                        //command.Parameters.AddWithValue("@panorama_width", 1024);
                        //command.Parameters.AddWithValue("@direct_steering", 0); //not clear what this does
                        command.Parameters.AddWithValue("@crane", 0);
                        command.ExecuteNonQuery();


                        // ---------- fill table functions ----------

                        for (int index = 0; index < LocomotiveList.Get(listIndex).Functions.Length; index++)
                        {
                            functionMapping = FunctionTypeMappingList.Find(x => x.Key == LocomotiveList.Get(listIndex).Functions[index]);

                            command.CommandText =
                                "INSERT INTO functions ([id], [vehicle_id], [button_type], [shortcut], [time], [position], [image_name], [function], [show_function_number], [is_configured]) " +
                                "VALUES(@id, @vehicle_id, @button_type, @shortcut, @time, @position, @image_name, @function, @show_function_number, @is_configured); ";
                            command.Parameters.Clear();
                            command.Parameters.AddWithValue("@id", index + 1);
                            command.Parameters.AddWithValue("@vehicle_id", 1);
                            int buttonType = functionMapping.Duration;
                            command.Parameters.AddWithValue("@button_type", buttonType);
                            string shortcut = functionMapping.Shortname;
                            if (shortcut.Length > 10) shortcut = shortcut.Substring(0, 10);
                            command.Parameters.AddWithValue("@shortcut", shortcut);
                            command.Parameters.AddWithValue("@time", "0.000000");
                            command.Parameters.AddWithValue("@position", index);
                            string imageName = functionMapping.FunctionTypeZ21.ToString();
                            command.Parameters.AddWithValue("@image_name", imageName);
                            command.Parameters.AddWithValue("@function", index);
                            command.Parameters.AddWithValue("@show_function_number", 1);
                            command.Parameters.AddWithValue("@is_configured", 0);
                            command.ExecuteNonQuery();
                        }


                        // ---------- fill table update_history ----------

                        command.CommandText =
                            "INSERT INTO update_history ([id], [os], [update_date], [build_version], [build_number], [to_database_version]) " +
                            "VALUES(@id, @os, @update_date, @build_version, @build_number, @to_database_version); ";
                        command.Parameters.Clear(); 
                        command.Parameters.AddWithValue("@id", 1);
                        command.Parameters.AddWithValue("@os", "LokDbConverter");
                        command.Parameters.AddWithValue("@update_date", DateTime.Now.ToString("dd/MM/yy, hh:mm:ss") + " Mitteleuropäische Normalzeit");
                        command.Parameters.AddWithValue("@build_version", "1.4.7");
                        command.Parameters.AddWithValue("@build_number", 6142);
                        command.Parameters.AddWithValue("@to_database_version", 15);
                        command.ExecuteNonQuery();

                        returnValue = 1;
                    }
                    catch (Exception ex)
                    {
                        returnValue = -1;
                        throw new Exception(ex.Message);
                    }


                }
            }

            SqliteConnection.ClearAllPools();

            return returnValue;
        }

        /// <summary>
        /// Converter for locomotive address to make it suitable for SRSEII-Gleisbox by Gerd
        /// </summary>
        /// <param name="addressSrseii">Locomotives address</param>
        /// <param name="decoderType">Type of protocol used</param>
        /// <returns>Locomotives address according to SRSEII</returns>
        private int ConvertAddressSrseii(int addressSrseii, DecoderType decoderType)
        {
            return decoderType switch
            {
                DecoderType.MFX => addressSrseii + 1000,
                DecoderType.DCC => addressSrseii + 5000,
                _ => addressSrseii
            };
        }

    }
}
