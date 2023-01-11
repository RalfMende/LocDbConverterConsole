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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocDbConverterConsole
{
    /// <summary>
    /// This represents the symbols for functions available on the z21 app 
    /// </summary>
    public enum FunctionTypeZ21
    {
        None,
        Light,
        Light2,
        Main,
        Horn_high,
        Horn_low,
        Bugle, //Signalhorn
        Horn_two_sound,
        Whistle_short,
        Whistle_long,
        Hump_gear,
        Hump_funk,
        Main_beam2,
        Main_beam,
        Back_light,
        All_round_light,
        Cycle_light, //Umlaufbeleuchtung
        Coach_side_light_off, //Wagenseite Licht aus
        Coach_side_light_off_2, //Wagenseite Licht aus 2
        Sidelights, //Standlicht
        Cabin_light,
        Cockpit_light_left,
        Cockpit_light_right,
        Interior_light,
        Couple,
        Decouple,
        Puffer_kick,
        Rail_kick,
        Mute,
        Quieter,
        Louder,
        Curve_sound,
        Compressor,
        Air_pump,
        Door_close,
        Door_open,
        Forward_take_power,
        Backward_take_power,
        Sanden,
        Drainage,
        Drain_valve,
        Dump_steam,
        Steam,
        Drain_mud,
        Firebox,
        Generator,
        Diesel_generator,
        Alternator,
        Injector,
        Fan,
        Feed_pump,
        Scoop_coal,
        Scoop_coal_sound,
        Diesel_regulation_step_down,
        Diesel_regulation_step_up,
        Crane_arm_down,
        Crane_arm_up,
        Crane_arm_out,
        Crane_arm_in,
        Pick_down,
        Pick_up,
        Hood_close,
        Hood_open,
        Licence_plate_light,
        Destination_plate_light,
        Rotate_upper_assembly,
        Rotate_left,
        Rotate_right,
        Snow_blower_down,
        Snow_blower_up,
        Snow_blower_up_down,
        Snow_blower_rotate,
        Tish_lamp,
        Stair_light,
        Weight,
        Bell,
        Clef,
        Neutral,
        Sound1,
        Sound2,
        Sound3,
        Sound4,
        Sound5,
        Acc_delay,
        Break_delay,
        Blower,
        Fan_strong,
        Handbreak,
        Preheat,
        Shaking_grates,
        Sifa,
        Sound_brake,
        Valve,
        Warning
    }
}
