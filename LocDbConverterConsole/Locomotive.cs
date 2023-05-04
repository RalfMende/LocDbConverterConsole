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

namespace LocDbConverterConsole
{
    public class Locomotive
    {
        public Locomotive()
        {
            Name = "New Locomotive";
            Decodertype = DecoderType.None;
            Address = 1;
            //Icon = "leere Lok" //space must be replaced by #20 for Maerklin files
            //Symbol = 1;
            Functions = new int[32];
        }

        public string Name { get; internal set; }
        public DecoderType Decodertype { get; internal set; }
        public int Address { get; internal set; } //Maerklin: derived from "uid"
        public int Uid { get; internal set; }
        public int Mfxuid { get; internal set; }
        public string Icon { get; internal set; } //Picture
        //public string Symbol { get; internal set; }
        public int AccelerationDelay { get; internal set; } //Maerklin:"av"
        public int DecelerationDelay { get; internal set; } //Maerklin:"bv"
        public int VMin { get; internal set; }
        public int VMax { get; internal set; }
        public int SpeedometerMax { get; internal set; } //Maerklin:"tachomax"
        //public int Volume { get; internal set; }
        //public int ArticleNumber { get; internal set; } //Maerklin:"artikelnr"
        public string Spa { get; internal set; } //Todo: What does this mean?
        public string Ft { get; internal set; } //Todo: What does this mean?
        public int[] Functions { get; internal set; } // each refers to index of FunctionMapping
        //public List<int> FunctionsList { get; internal set; }
    }
}
