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
        none,
        light,
        light2,
        main,
        horn_high,
        horn_low,
        bugle, //Signalhorn
        horn_two_sound,
        whistle_short,
        whistle_long,
        hump_gear,
        hump_funk,
        main_beam2,
        main_beam,
        back_light,
        all_round_light,
        cycle_light, //Umlaufbeleuchtung
        coach_side_light_off, //Wagenseite Licht aus
        coach_side_light_off_2, //Wagenseite Licht aus 2
        sidelights, //Standlicht
        cabin_light,
        cockpit_light_left,
        cockpit_light_right,
        interior_light,
        couple,
        decouple,
        puffer_kick,
        rail_kick,
        mute,
        quieter,
        louder,
        curve_sound,
        compressor,
        air_pump,
        door_close,
        door_open,
        forward_take_power,
        backward_take_power,
        sanden,
        drainage,
        drain_valve,
        dump_steam,
        steam,
        drain_mud,
        firebox,
        generator,
        diesel_generator,
        alternator,
        injector,
        fan,
        feed_pump,
        scoop_coal,
        scoop_coal_sound,
        diesel_regulation_step_down,
        diesel_regulation_step_up,
        crane_arm_down,
        crane_arm_up,
        crane_arm_out,
        crane_arm_in,
        pick_down,
        pick_up,
        hood_close,
        hood_open,
        licence_plate_light,
        destination_plate_light,
        rotate_upper_assembly,
        rotate_left,
        rotate_right,
        snow_blower_down,
        snow_blower_up,
        snow_blower_up_down,
        snow_blower_rotate,
        tish_lamp,
        stair_light,
        weight,
        bell,
        clef,
        neutral,
        sound1,
        sound2,
        sound3,
        sound4,
        sound5,
        acc_delay,
        break_delay,
        blower,
        fan_strong,
        handbreak,
        preheat,
        shaking_grates,
        sifa,
        sound_brake,
        valve,
        warning
    }
}
