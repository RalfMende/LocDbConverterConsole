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
    internal struct FunctionTypeMapping
    {
        public int Key { get; set; }
        public string ShortName { get; set; }
        public int Duration { get; set; } // duration   0: constantly, -1 momentary, else time    
        public int FunctionTypeIndexCS2 { get; set; }
        //public FunctionTypeCS3 FunctionTypeCS3 { get; set; }
        public FunctionTypeZ21 FunctionTypeZ21 { get; set; }
    }
}
