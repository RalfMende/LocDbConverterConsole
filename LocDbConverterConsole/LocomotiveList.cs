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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocDbConverterConsole
{
    static class LocomotiveList
    {
        static List<Locomotive> _list;

        static LocomotiveList()
        {
            _list = new List<Locomotive>();
        }

        public static int Set(Locomotive loc)
        {
            _list.Add(loc);
            return _list.Count();
        }

        public static Locomotive Get(int index)
        {
            return _list[index];
        }

        public static bool Contains(Locomotive loc)
        {
            return _list.Contains(loc);
        }

        public static bool Replace(Locomotive loc)
        {
            bool returnValue = false;
            int index = _list.FindIndex(s => s == loc);
            if (index != -1)
            {
                _list[index] = loc;
                returnValue = true;
            }
            return returnValue;
        }

        public static int SizeOf()
        {
            return _list.Count();
        }
    }
}
