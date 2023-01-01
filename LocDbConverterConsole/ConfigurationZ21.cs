﻿/*
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
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace LocDbConverterConsole
{
    public class ConfigurationZ21
    {
        /// <summary>
        /// Imports a z21 description file to locomotive list entry
        /// </summary>
        /// <param name="fileOrDirectory">Path to get locomotive description files from</param>
        /// <returns>0: Code not executed, Negative: Error, Positive: Number of config files found</returns>
        public int ImportConfiguration(string fileOrDirectory)
        {
            int returnValue = 0;

            //TODO

            return returnValue;
        }

        /// <summary>
        /// Exports a z21 description file with the given locomotive list entry
        /// </summary>
        /// <param name="listIndex"></param>
        /// <param name="exportPath"></param>
        /// <returns></returns>
        public int ExportConfiguration(int listIndex, string exportPath)
        {
            int returnValue = 0;
            string templateFile = @"C:\Temp\z21\z21_template\export\New Folder\Loco.sqlite";

            //Guid myuuid = Guid.NewGuid();
            //string myuuidAsString = myuuid.ToString();
            //_z21.ExportLocomotiveFile(@"\\Mac\Home\Downloads\LokDbConverter_Data\z21\LocomotiveOnly\export\" + myuuidAsString + "\\Loco.sqlite", 0);
            returnValue = ExportLocomotiveFile(templateFile, 0);
            
            string startPath = @"C:\Temp\z21\z21_template";
            //string zipPath = @"C:\Temp\z21\" + LocomotiveList.Get(listIndex).Name + ".z21loco";
            string zipFileName = exportPath + LocomotiveList.Get(listIndex).Name + ".z21loco";
            if(File.Exists(zipFileName))
            {
                File.Delete(zipFileName); 
            }
            ZipFile.CreateFromDirectory(startPath, zipFileName);

            return returnValue;
        }

        /// <summary>
        /// Imports a *.sqlite locomotive desciption file from Roco Z21 and parses it to internal list
        /// </summary>
        /// <param name="file">*.sqlite locomotive desciption file</param>
        /// <returns>0: Code not executed, Negative: Error, Positive: Index of list where element was added</returns>
        /*private int ImportLocomotiveFile(string file)
        {
            int returnValue = 0;
            int tmpNumber = 0;  
            Locomotive locomotive = new Locomotive();

            var connection = new SqliteConnection("Data Source=" + file);
            connection.Open();
            var command = connection.CreateCommand();

            // ---------- read table vehicles ----------
            command.CommandText = @"SELECT * FROM vehicles";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    locomotive.Address = Convert.ToInt32(reader["address"]);
                    locomotive.Name = reader["name"].ToString();
                    locomotive.SpeedometerMax = Convert.ToInt32(reader["max_speed"]);
                    // ... = reader["decoder_type"]
                }
            }

            // ---------- read table functions ----------
            command.CommandText = @"SELECT * FROM functions";
            using (var reader = command.ExecuteReader())
            {
                // Read the first result set
                while (reader.Read())
                {
                    tmpNumber = Convert.ToInt32(reader["function"]);
                    locomotive.functions[tmpNumber].Number = tmpNumber;




                }
                //reader.NextResult();
                //while (reader.Read()) ...
            }
            returnValue = LocomotiveList.Set(locomotive);
            return returnValue;
        }*/

        /// <summary>
        /// Exports a *.sqlite locomotive desciption file for Rodo Z21 from internal list
        /// </summary>
        /// <param name="file"></param>
        /// <param name="listIndex">Index of the locomotives list (0-based)</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private int ExportLocomotiveFile(string databaseFile, int listIndex)
        {
            int returnValue = 0;
            var connection = new SqliteConnection("Data Source=" + databaseFile);
            connection.Open();
            var command = connection.CreateCommand();
            FunctionTypeMapping functionMapping;

            try
            {
                // ---------- create tables ----------

                command.CommandText = @"CREATE TABLE IF NOT EXISTS 'categories' ('id' INTEGER PRIMARY KEY NOT NULL, 'name' TEXT)";
                command.ExecuteNonQuery();
                command.CommandText = @"CREATE TABLE IF NOT EXISTS 'control_station_control_states' ('id' INTEGER PRIMARY KEY, 'control_id' INTEGER, 'state' INTEGER, 'address1_value' INTEGER, 'address2_value' INTEGER, 'address3_value' INTEGER)";
                command.ExecuteNonQuery();
                command.CommandText = @"CREATE TABLE IF NOT EXISTS 'control_station_controls' ('id' INTEGER PRIMARY KEY, 'page_id' INTEGER, 'x' INTEGER, 'y' INTEGER, 'angle' REAL, 'type' INTEGER, 'address1' INTEGER, 'address2' INTEGER, 'address3' INTEGER, button_type INTEGER DEFAULT 0, time REAL DEFAULT 0)";
                command.ExecuteNonQuery();
                command.CommandText = @"CREATE TABLE IF NOT EXISTS 'control_station_images' ('id' INTEGER PRIMARY KEY, 'page_id' INTEGER, 'image_name' TEXT)";
                command.ExecuteNonQuery();
                command.CommandText = @"CREATE TABLE IF NOT EXISTS 'control_station_notes' ('id' INTEGER PRIMARY KEY, 'page_id' INTEGER, 'x' INTEGER, 'y' INTEGER, 'text' TEXT, 'font_size' INTEGER, angle INTEGER DEFAULT 0, type INTEGER DEFAULT 1)";
                command.ExecuteNonQuery();
                command.CommandText = @"CREATE TABLE IF NOT EXISTS 'control_station_pages' ('id' INTEGER PRIMARY KEY, 'position' INTEGER, 'name' TEXT,'thumb' TEXT)";
                command.ExecuteNonQuery();
                command.CommandText = @"CREATE TABLE IF NOT EXISTS 'control_station_rails' ('id' INTEGER PRIMARY KEY, 'page_id' INTEGER, 'left_control_id' INTEGER, 'right_control_id' INTEGER, 'left_outlet' INTEGER, 'right_outlet' INTEGER, 'value' REAL, left_response_module_id INTEGER DEFAULT 0, right_response_module_id INTEGER DEFAULT 0)";
                command.ExecuteNonQuery();
                command.CommandText = @"CREATE TABLE IF NOT EXISTS 'control_station_response_modules' ('id' INTEGER PRIMARY KEY, 'page_id' INTEGER, 'type' INTEGER, 'address' INTEGER, 'report_address' INTEGER, 'afterglow' INTEGER,  'x' INTEGER, 'y' INTEGER, 'angle' INTEGER)";
                command.ExecuteNonQuery();
                command.CommandText = @"CREATE TABLE IF NOT EXISTS 'control_station_route_list' ('id' INTEGER PRIMARY KEY, 'route_id' INTEGER, 'control_id' TEXT, 'state_id' INTEGER, 'position' INTEGER, 'wait_time' INTEGER, type INTEGER DEFAULT 0, signal_id INTEGER DEFAULT 0, signal_aspect INTEGER DEFAULT 0)";
                command.ExecuteNonQuery();
                command.CommandText = @"CREATE TABLE IF NOT EXISTS 'control_station_routes' ('id' INTEGER PRIMARY KEY, 'page_id' INTEGER, 'name' TEXT, 'x' INTEGER, 'y' INTEGER, 'angle' INTEGER)";
                command.ExecuteNonQuery();
                command.CommandText = @"CREATE TABLE IF NOT EXISTS 'control_station_signals' ('id' INTEGER PRIMARY KEY NOT NULL, 'page_id' INTEGER, 'x' INTEGER, 'y' INTEGER, 'angle' REAL, 'signal_id' INTEGER, 'signal_graph' INTEGER, 'address' INTEGER, 'active_aspects' TEXT, 'communication' INTEGER, 'z21_pro_link_id' INTEGER)";
                command.ExecuteNonQuery();
                command.CommandText = @"CREATE TABLE IF NOT EXISTS 'dc_functions' ('id' INTEGER PRIMARY KEY, 'vehicle_id' INTEGER,'position' INTEGER,'time' TEXT,'image_name' TEXT, 'function' INTEGER, 'cab_function_description' TEXT, 'drivers_cab' TEXT, 'shortcut' TEXT NOT NULL Default '', button_type INT NOT NULL Default 0, 'show_function_number' INTEGER NOT NULL Default 1, 'is_configured' INTEGER NOT NULL Default 0)";
                command.ExecuteNonQuery();
                command.CommandText = @"CREATE TABLE IF NOT EXISTS 'functions' ('id' INTEGER PRIMARY KEY  NOT NULL, 'vehicle_id' INTEGER, 'button_type' INTEGER NOT NULL Default 0, 'shortcut' TEXT NOT NULL Default '', 'time' TEXT, 'position' INTEGER, 'image_name' TEXT, 'function' INTEGER, 'show_function_number' INTEGER NOT NULL Default 1, 'is_configured' INTEGER NOT NULL Default 0)";
                command.ExecuteNonQuery();
                command.CommandText = @"CREATE TABLE IF NOT EXISTS 'layout_data' ('id' INTEGER PRIMARY KEY  NOT NULL, 'name' TEXT, control_station_type TEXT DEFAULT 'free', control_station_theme TEXT DEFAULT 'free')";
                command.ExecuteNonQuery();
                command.CommandText = @"CREATE TABLE IF NOT EXISTS 'paired_z21_pro_links' ('id' INTEGER PRIMARY KEY NOT NULL, 'name' TEXT, 'mac' TEXT, 'last_seen_date' DATE, 'ip' TEXT, 'last_connected_device' INTEGER)";
                command.ExecuteNonQuery();
                command.CommandText = @"CREATE TABLE IF NOT EXISTS 'traction_list' ('id' INTEGER PRIMARY KEY  NOT NULL, 'loco_id' INTEGER, 'regulation_step' INTEGER, 'time' REAL)";
                command.ExecuteNonQuery();
                command.CommandText = @"CREATE TABLE IF NOT EXISTS 'train_list' ('id' INTEGER PRIMARY KEY  NOT NULL, 'train_id' INTEGER, 'vehicle_id' INTEGER, 'position' INTEGER)";
                command.ExecuteNonQuery();
                command.CommandText = @"CREATE TABLE IF NOT EXISTS 'update_history' ('id' INTEGER PRIMARY KEY, 'os' TEXT, 'update_date' TEXT, 'build_version' TEXT, 'build_number' INTEGER, 'to_database_version' INTEGER)";
                command.ExecuteNonQuery();
                command.CommandText = @"CREATE TABLE IF NOT EXISTS 'vehicles' ('id' INTEGER PRIMARY KEY  NOT NULL, 'name' TEXT, 'image_name' TEXT, 'type' INTEGER, 'max_speed' INTEGER,'address' INTEGER, 'active' INTEGER, 'position' INTEGER, 'drivers_cab' TEXT, 'full_name' TEXT, 'speed_display' INTEGER, 'railway' TEXT, 'buffer_lenght' TEXT, 'model_buffer_lenght' TEXT, 'service_weight' TEXT, 'model_weight' TEXT, 'rmin' TEXT, 'article_number' TEXT, 'decoder_type' TEXT, 'owner' TEXT, 'build_year' TEXT, 'owning_since' TEXT, 'traction_direction' INTEGER, 'description' TEXT, 'dummy' INTEGER, 'ip' TEXT, 'video' INTEGER, 'video_x' INTEGER, 'video_y' INTEGER, 'video_width' INTEGER, 'panorama_x' INTEGER, 'panorama_y' INTEGER, 'panorama_width' INTEGER, 'panoramaImage' TEXT, 'direct_steering' INTEGER, crane INTEGER DEFAULT 0)";
                command.ExecuteNonQuery();
                command.CommandText = @"CREATE TABLE IF NOT EXISTS 'vehicles_to_categories' ('id' INTEGER PRIMARY KEY NOT NULL, 'vehicle_id' INTEGER NOT NULL, 'category_id' INTEGER NOT NULL)";
                command.ExecuteNonQuery();
                command.Dispose();

                
                // ----------fill table vehicles ----------
                
                command = connection.CreateCommand();
                command.CommandText = "DELETE FROM vehicles";
                command.ExecuteNonQuery();
                command.Dispose();

                command = connection.CreateCommand();
                command.CommandText =
                    "INSERT INTO vehicles ([id], [name], [max_speed], [address], [active]) " +
                    "VALUES(@id, @name, @max_speed, @address, @active); ";
                command.Parameters.AddWithValue("@id", 1);
                command.Parameters.AddWithValue("@name", LocomotiveList.Get(listIndex).Name);
                command.Parameters.AddWithValue("@max_speed", LocomotiveList.Get(listIndex).SpeedometerMax);
                command.Parameters.AddWithValue("@address", LocomotiveList.Get(listIndex).Address);
                command.Parameters.AddWithValue("@active", 1);
                command.ExecuteNonQuery();
                command.Dispose();


                // ---------- fill table functions ----------

                command = connection.CreateCommand();
                command.CommandText = "DELETE FROM functions";
                command.ExecuteNonQuery();
                command.Dispose();

                //int index = 0;
                //while (!(Locomotives.Get(listIndex).functions[index].Type == FunctionTypeCS3.None)) //TODO: What if there is an empty function within between?
                for (int index = 0; index < LocomotiveList.Get(listIndex).Functions.Length; index++)
                {
                    functionMapping = new FunctionTypeMapping() { Key = 0, ShortName = "Fkt " + index, Duration = 0, FunctionTypeIndexCS2 = 0, FunctionTypeZ21 = FunctionTypeZ21.none };
                    functionMapping = FunctionTypeMappingList.Find(x => x.Key == LocomotiveList.Get(listIndex).Functions[index]);
 
                    command = connection.CreateCommand();

                    command.CommandText =
                        "INSERT INTO functions ([id], [vehicle_id], [button_type], [shortcut], [time], [position], [image_name], [function], [show_function_number], [is_configured]) " +
                        "VALUES(@id, @vehicle_id, @button_type, @shortcut, @time, @position, @image_name, @function, @show_function_number, @is_configured); ";
                    command.Parameters.AddWithValue("@id", index + 1);
                    command.Parameters.AddWithValue("@vehicle_id", 1);
                    int buttonType = functionMapping.Duration;
                    command.Parameters.AddWithValue("@button_type", buttonType);
                    string shortcut = functionMapping.ShortName;
                    if (shortcut.Length > 10) shortcut = shortcut.Substring(0, 10);
                    command.Parameters.AddWithValue("@shortcut", shortcut);
                    command.Parameters.AddWithValue("@time", 0);
                    command.Parameters.AddWithValue("@position", index);
                    string imageName = functionMapping.FunctionTypeZ21.ToString();
                    command.Parameters.AddWithValue("@image_name", imageName);
                    command.Parameters.AddWithValue("@function", index);
                    command.Parameters.AddWithValue("@show_function_number", 1);
                    command.Parameters.AddWithValue("@is_configured", 0);
                    command.ExecuteNonQuery();
                    command.Dispose();
                    //index++;
                }


                // ---------- fill table layout_data ----------

                //    command = connection.CreateCommand();
                //    command.CommandText = "DELETE FROM layout_data";
                //    command.ExecuteNonQuery();

                //    command = connection.CreateCommand();
                //    command.CommandText =
                //        "INSERT INTO layout_data ([id], [name], [control_station_type], [control_station_theme]) " +
                //        "VALUES(@id, @name, @control_station_type, @control_station_theme); ";
                //    command.Parameters.AddWithValue("@id", 1);
                //    command.Parameters.AddWithValue("@name", "Neue Anlage");
                //    command.Parameters.AddWithValue("@control_station_type", "schematic");
                //    command.Parameters.AddWithValue("@control_station_theme", "default");
                //    command.ExecuteNonQuery();
                //    command.Dispose();


                // ---------- fill table update_history ----------

                //    command = connection.CreateCommand();
                //    command.CommandText = "DELETE FROM update_history";
                //    command.ExecuteNonQuery();
                //
                //    command = connection.CreateCommand();
                //    command.CommandText =
                //        "INSERT INTO update_history ([id], [os], [update_date], [build_version], [build_number], [to_database_version]) " +
                //        "VALUES(@id, @os, @update_date, @build_version, @build_number, @to_database_version); ";
                //    command.Parameters.AddWithValue("@id", 1);
                //    command.Parameters.AddWithValue("@ios", 13); //Todo
                //    command.Parameters.AddWithValue("@update_date", "23.12.22, 21:08:39 Mitteleuropäische Normalzeit"); //Todo
                //    command.Parameters.AddWithValue("@build_version", "1.4.6");
                //    command.Parameters.AddWithValue("@build_number", 6076);
                //    command.Parameters.AddWithValue("@to_database_version", 1);
                //    command.ExecuteNonQuery();
                //    command.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                returnValue = -1;
            }

            connection.Close();
            connection.Dispose();

            //SqliteConnection.ClearAllPools();
            SqliteConnection.ClearPool(connection);
            //GC.Collect();
            //GC.WaitForPendingFinalizers();

            return returnValue;
        }

    }
}
