﻿/*
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
    /// <summary>
    /// This represents the names for functions available on the CS3 according to maerklins documentation 
    /// </summary>
    public enum FunctionTypeCS2
    {
        None,

        /*--- Light ---*/
        Lichtwechsel = 1,
        Licht_an_Fuehrerstand_hinten_aus = 41,
        Licht_an_Fuehrerstand_vorne_aus = 42,
        Innenbeleuchtung = 2,
        Fuehrerstandsbeleuchtung = 48,
        Maschinenraumbeleuchtung = 126,
        Konsolenbeleuchtung = 117,
        Deckenbeleuchtung = 32,
        Tischbeleuchtung_EP2 = 35,
        Tischbeleuchtung_EP3 = 34,
        Tischbeleuchtung_EP4 = 33,
        Partybeleuchtung = 119,
        Aussenbeleuchtung_Schlusslicht = 3,
        Nummernschildbeleuchtung = 38,
        Zuglaufschildbeleuchtung = 40,
        Triebwerksbeleuchtung = 90,
        Rangierbeleuchtung = 118,
        Blinker = 130,
        Warnlicht = 47,
        Trittstufenbeleuchtung = 110,
        Warnblinklicht = 212,
        Feuerbuechsenbeleuchtung = 31,
        Schweizer_Lichtwechsel = 233,
        Aussenbeleuchtung_Frontlicht = 105,
        Fernlicht = 4,
        Fernlicht_hinten = 86,
        Fernlicht_vorn = 87,
        Fernlicht_vorne_hinten = 281,
        Stirnlicht_vorne_hinten = 282,
        Licht_Abteile = 221,
        Licht_Gang = 222,
        Licht_Kueche = 223,
        Licht_Gepaeck = 224,
        Licht_Bar = 225,

        /*--- Sound 1 ---*/
        Betriebsgeraeusch = 5,
        Bremsenquietschen_aus = 20,
        Pantograph_Auf_Abbuegelgeraeusch = 101,
        Dampfstoss = 201,
        Kupplung_An_Abkuppelgeraeusch = 43,
        Schienenstoss = 37,
        Schaffnerpfiff = 11,
        Schaltstufen = 21,
        Zylinder_ausblasen_Dampf_ablassen = 91,
        Dieselheizung = 202,
        Sicherheitsventil = 123,
        Schuettelrost = 36,
        Generator = 22,
        Pufferstoss = 44,
        Hintergrundmusik = 39,
        Partymusik = 128,
        Druckluft_ablassen = 92,
        Maschine_vorschmieren = 24,
        Kesselheizung = 124,
        //Motorstufe_auf = ,
        //Motorstufe_ab = ,
        Hilfsaggregat_Hilfsdiesel = 125,
        Zugheizung = 203,
        Kurvengeraeusch = 129,
        Ablassventil = 131,
        //Dampf_ablassen = ,
        Sanden = 108,
        Bremsenquietschen_ein = 112,
        //Schaffnerpfiff_Sequenz = ,
        Fahrgeraeusch = 23,
        Segeln = 204,
        Sprachausgabe = 45,
        Bahnhofsansage = 25,
        Abfahrtsdurchsage = 104,
        Ankunft_Zielansage = 121,
        Funkgespraech = 103,
        Wartende_Passagiere = 120,
        Zugansage_Schaffener = 122,
        Unterhaltung_Allgemein = 107,
        Fuehrerstandsgespraech = 133,
        Informationsdurchsage = 134,
        Ambiente_Stadt = 141,
        Ambiente_Tunnel = 142,
        Ambiente_Stahlbruecke = 143,
        Ambiente_Betonbruecke = 144,
        Warnmeldungen = 132,
        Zug_Infoansage = 205,
        Sonderansage = 206,
        Wartende_Passagiere_am_Bahnhof = 207,
        Zugdurchsage = 211,
        Glocke = 13,
        Glocke_kurz = 148,
        Glocke_lang = 249,
        Horn_Typhon = 10,
        Horn_kurz = 250,
        Horn_lang = 251,
        //Doppelhorn = ,
        Pfeife = 12,
        Pfeife_kurz = 252,
        Pfeife_lang = 253,
        Tueren_schliessen_Geraeusch = 28,
        Durchsage_Tueren_schlissen_rechts = 140,
        Durchsage_Tueren_schlissen_links = 209,
        Geraeusch_Klapptuere = 145,
        Geraeusch_Tuere = 146,
        Geraeusch_Falltuere = 147,
        Geraeusch_Rolltor = 148,
        Geraeusch_Schiebetuer = 149,
        Geraeusch_Fenster_auf_zu = 139,
        Lueftergeraeusch = 29,
        Pumpe = 19,
        Luftpumpe = 106,
        Vakuumpumpe = 89,
        Kompressor = 116,
        Injektor = 49,
        Speisewasserpumpe = 111,
        Oelpumpe = 113,
        Schmierpumpe = 114,
        Dieselpumpe = 115,
        Sonstige_Pumpe = 135,
        Kohlen_schaufeln = 26,
        Kombination_schaufeln_Feuerbuechse = 136,
        Starklueften = 273,
        Schwachlueften = 274,
        Licht_Generator_Sound = 278,
        Licht_Generator_Sound_hinten = 291,
        Licht_Generator_Sound_vorne = 294,
        Kohlestaub = 280,
        Kurbeln_Sound = 295,
        Abschlammen_Schlaemmen = 247,
        Ansage_Pfiff_mit_Tuerschliessen = 234,
        Zufall_Aus = 235,
        Sifataster = 236,
        Not_Aus = 237,
        Schalter = 238,
        Kippschlater = 239,
        Drehschalter = 240,
        Not_Halt = 241,
        Sound_Zahnrad = 242,
        Klackern_Tacho = 243,
        Sound_Wischer = 244,

        /*--- Sound 2 ---*/
        Tanken = 226,
        Wasser = 227,
        Diesel = 228,
        Kohle = 229,
        Kohlestaub_2 = 230,
        Oel = 231,
        Sand = 232,
        Hammer = 257,
        Saege = 258,
        Bohrmaschine_Bohrer = 259,
        Schweissgeraet_Schweissen = 260,
        Schleifblock_Schleifen = 261,
        Drahtbuerste_Buersten = 262,
        Kettenzug = 263,
        Trennschleifer_Flex = 264,
        Schaufel = 265,
        Laden_Einladen_Ausladen = 266,
        Presslufthammer_Hämmern = 267,
        Bahnuebergang = 271,
        Glocke_Bahnuebergang = 272,
        Rauchkammer = 279,

        /*--- Mechanics ---*/
        Rangiergang_ein = 8,
        Anfahr_Bremsverzoegerung_aus = 18,
        Anfahr_Bremsverzoegerung_ein = 102,
        Bocksprung = 208,
        Zuggattung = 210,
        Fahrtfreigabe = 100,
        Telexkupplung = 9,
        Telexkupplung_hinten = 82,
        Telexkupplung_vorn = 83,
        Telex_Walzer = 277,
        Telex_Walzer_vorne = 292,
        Telex_Walzer_hinten = 293,
        Rauchgenerator = 7,
        Pantograph = 6,
        Pantograph_hinten = 84,
        Pantograph_vorne = 85,
        Tueren_schliessen = 27,
        Fenster_auf_zu = 138,
        Beweglicher_Schaffner = 137,
        Luefter = 30,
        Zahnrad_Antrieb = 245,
        Wischer = 246,

        /*--- Special Crane ---*/
        Kran = 93,
        KranBuehne_verfahren = 14,
        KranBuehne_neigen = 17,
        KranBuehne_heben_senken = 15,
        KranBuehne_drehen_links = 16,
        KranBuehne_drehen_rechts = 98,
        Kran_Magnet = 99,
        Kran_Haken = 256,
        Kran_Doppelhaken = 46,
        KranBuehne_auf = 94,
        KranBuehne_ab = 95,
        KranBuehne_links = 96,
        KranBuehne_rechts = 97,
        Kran_Ein_Umschalten_Fahrbetrieb_Funktionsbetrieb = 254,
        Kran_Aus_Grundstellung_anfahren = 255,

        /*--- Sound Control ---*/
        Shift = 88,
        Mute_Fade = 109,
        Play = 213,
        Pause = 214,
        Stop = 268,
        Vor = 215,
        Zurueck = 216,
        Naechstes = 269,
        Letztes = 270,
        Laut = 217,
        Leise = 218,

        /*--- Function Overall ---*/
        Funktion_0 = 50,
        Funktion_1 = 51,
        Funktion_2 = 52,
        Funktion_3 = 53,
        Funktion_4 = 54,
        Funktion_5 = 55,
        Funktion_6 = 56,
        Funktion_7 = 57,
        Funktion_8 = 58,
        Funktion_9 = 59,
        Funktion_10 = 60,
        Funktion_11 = 61,
        Funktion_12 = 62,
        Funktion_13 = 63,
        Funktion_14 = 64,
        Funktion_15 = 65,
        Funktion_16 = 66,
        Funktion_17 = 67,
        Funktion_18 = 68,
        Funktion_19 = 69,
        Funktion_20 = 70,
        Funktion_21 = 71,
        Funktion_22 = 72,
        Funktion_23 = 73,
        Funktion_24 = 74,
        Funktion_25 = 75,
        Funktion_26 = 76,
        Funktion_27 = 77,
        Funktion_28 = 78,
        Funktion_29 = 79,
        Funktion_30 = 80,
        Funktion_31 = 81
    }
}
