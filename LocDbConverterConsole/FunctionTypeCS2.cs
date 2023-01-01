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
    /// This represents the names for functions available on the CS3 according to maerklins documentation 
    /// </summary>
    public enum FunctionTypeCS2
    {
        None,
        Lichtwechsel = 1,
        Licht_an_Fuehrerstand_hinten_aus,
        Licht_an_Fuehrerstand_vorne_aus,
        Innenbeleuchtung,
        Fuehrerstandsbeleuchtung = 48,
        Maschienraumbeleuchtung,
        Konsolenbeleuchtung,
        Deckenbeleuchtung,
        Tischbeleuchtung_EP2,
        Tischbeleuchtung_EP3,
        Tischbeleuchtung_EP4,
        Partybeleuchtung,
        Aussenbeleuchtung_Schlusslicht,
        Nummernschildbeleuchtung,
        Zielschildbeleuchtung,
        Triebwerksbeleuchtung,
        Rangierbeleuchtung,
        Blinker,
        Warnlicht,
        Trittstufenbeleuchtung,
        Warnblinklicht,
        Feuerbuechsenbeleuchtung,
        Schweizer_Lichtwechsel,
        Aussenbeleuchtung_Frontlicht,
        Fernlicht,
        Fernlicht_hinten,
        Fernlicht_vorn,

        Betriebsgeraeusch = 23,
        Bremsenquietschen_aus = 20,
        Pantograph_Auf_Abbuegelgeraeusch,
        Dampfstoss,
        Kupplung_An_Abkuppelgeraeusch,
        Schienestoss,
        Schaffnerpfiff,
        Schaltstufen,
        Zylinder_ausblasen_Dampf_ablassen,

        Dieselheizung,
        Sicherheitsventil,
        Schuettelrost,
        Generator,
        Pufferstoss,
        Hintergrundmusik,
        Partymusik,
        Druckluft_ablassen,
        Maschine_vorschmieren,
        Kesselheizung,
        Motorstufe_auf,
        Motorstufe_ab,
        Hilfsaggregat_Hilfsdiesel,
        Zugheizung,
        Kurvengeraeusch,
        Ablassventil,
        Dampf_ablassen,
        Sanden,
        Bremsenquietschen_ein = 112,
        Schaffnerpfiff_Sequenz,
        Fahrgeraeusch,
        Segeln,
        Sprachausgabe,
        Bahnhofsansage,
        Abfahrtsansage,
        Ankunft_Zielansage,
        Funkgespraech,
        Wartende_Passagiere,
        ZUgansage_schaffener,
        Unterhaltung_Allgemein,
        Fuehrerstandsgespraech,
        Informationsdurchsage,
        Ambiente_Stadt,
        Ambiente_Tunnel,
        Ambiente_Stahlbruecke,
        Ambiente_Betonbruecke,
        Warnmeldung,
        Zug_Infoansage,
        Sonderansage,
        Wartende_Passagiere_am_Bahnhof,
        Zugdurchsage,
        Glocke = 13,
        Horn_Typhon,
        Pfeife = 12,
        Tueren_schliessen_Geraeusch,
        Durchsage_Tueren_schlissen_rechts,
        Durchsage_Tueren_schlissen_links,
        Geraeusch_Klapptuere,
        Geraeusch_Tuere,
        Geraeusch_Falltuere,
        Geraeusch_Rolltor,
        Geraeusch_Schiebetuer,
        Geraeusch_Fenster_auf_zu,
        Lueftergeraeusch,
        Pumpe,
        Luftpumpe = 106,
        Vakuumpumpe,
        Kompressor,
        Injektor = 49,
        Speisewasserpumpe,
        Oelpumpe,
        Schmierpumpe,
        Dieselpumpe,
        Sonstige_Pumpe,
        Kohlen_schaufeln = 26,
        Kombination_schaufeln_Feuerbuechse,

        Rangiergang_ein = 8,
        Anfahr_Bremsverzoegerung_aus = 18,
        Bocksprung,
        Zuggattung,
        Fahrtfreigabe,
        Telexkupplung,
        Telexkupplung_hinten,
        Telexkupplung_vorn,
        Rauchgenerator = 7,
        Pantograph,
        Pantograph_hinten,
        Pantograph_vorne,
        Tueren_schliessen,
        Fenster_auf_zu,
        Beweglicher_Schaffner,
        Luefter,

        Kran,
        KranBuehne_neigen,
        KranBuehne_heben_senken,
        KranBuehne_drehen_links,
        KranBuehne_drehen_rechts,
        Kran_Magnet,
        Kran_Haken,
        KranBuehne_auf,
        KranBuehne_ab,
        KranBuehne_links,
        KranBuehne_rechts,
        Umschalten_Fahrbetrieb_Funktionsbetrieb,
        Grundstallung_anfahren
    }
}
