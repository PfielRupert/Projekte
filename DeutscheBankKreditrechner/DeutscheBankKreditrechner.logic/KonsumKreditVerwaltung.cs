﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeutscheBankKreditrechner.logic
{
    public class KonsumKReditVerwaltung
    {

        /// <summary>
        /// Erzeugt einen "leeren" dummy Kunden
        /// zu dem in Folge alle Konsumkredit Daten
        /// verknüpft werden können.
        /// </summary>
        /// <returns>einen leeren Kunden wenn erfolgreich, ansonsten null</returns>
        public static tblPersoenlicheDaten ErzeugeKunde()
        {
            Debug.WriteLine("KonsumKreditVerwaltung - ErzeugeKunde");
            Debug.Indent();

            tblPersoenlicheDaten neuerKunde = null;

            try
            {
                using (var context = new dbLapProjektEntities())
                {
                    neuerKunde = new logic.tblPersoenlicheDaten()
                    {
                        Vorname = "anonym",
                        Nachname = "anonym",
                        FKGeschlecht = 1
                    };
                    context.tblPersoenlicheDaten.Add(neuerKunde);

                    int anzahlZeilenBetroffen = context.SaveChanges();
                    Debug.WriteLine($"{anzahlZeilenBetroffen} Kunden angelegt!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in ErzeugeKunde");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return neuerKunde;
        }

        /// <summary>
        /// Lädt den Kreditrahmen für die übergebene ID
        /// </summary>
        /// <param name="id">die id des zu ladenden Kreditrahmens</param>
        /// <returns>der Kreditwunsch für die übergebene ID</returns>
        public static tblKreditdaten KreditRahmenLaden(int id)
        {
            Debug.WriteLine("KonsumKreditVerwaltung - KreditRahmenLaden");
            Debug.Indent();

            tblKreditdaten wunsch = null;

            try
            {
                using (var context = new dbLapProjektEntities())
                {
                    wunsch = context.tblKreditdaten.Where(x => x.ID_Kredit == id).FirstOrDefault();
                    Debug.WriteLine("KreditRahmen geladen!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in KreditRahmenLaden");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return wunsch;
        }

        /// Lädt die FinanzielleSituation für die übergebene ID
        /// </summary>
        /// <param name="id">die id der zu ladenden FinanzielleSituation</param>
        /// <returns>die FinanzielleSituation für die übergebene ID</returns>
        public static tblFinanzielleSituation FinanzielleSituationLaden(int id)
        {
            Debug.WriteLine("KonsumKreditVerwaltung - FinanzielleSituationLaden");
            Debug.Indent();

            tblFinanzielleSituation finanzielleSituation = null;

            try
            {
                using (var context = new dbLapProjektEntities())
                {
                    finanzielleSituation = context.tblFinanzielleSituation.Where(x => x.ID_FinanzielleSituation == id).FirstOrDefault();
                    Debug.WriteLine("FinanzielleSituation geladen!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in FinanzielleSituationLaden");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return finanzielleSituation;
        }
        /// <summary>
        /// Speichert zu einer übergebenene ID_Kunde den Wunsch Kredit und dessen Laufzeit ab
        /// </summary>
        /// <param name="kreditBetrag">die Höhe des gewünschten Kredits</param>
        /// <param name="laufzeit">die Laufzeit des gewünschten Kredits</param>
        /// <param name="idKunde">die ID des Kunden zu dem die Angaben gespeichert werden sollen</param>
        /// <returns>true wenn Eintragung gespeichert werden konnte und der Kunde existiert, ansonsten false</returns>
        public static bool KreditRahmenSpeichern(double kreditBetrag, int laufzeit, int idKunde)
        {
            Debug.WriteLine("KonsumKreditVerwaltung - KreditRahmenSpeichern");
            Debug.Indent();

            bool erfolgreich = false;

            try
            {
                using (var context = new dbLapProjektEntities())
                {

                    /// speichere zum Kunden die Angaben
                    tblPersoenlicheDaten aktKunde = context.tblPersoenlicheDaten.Where(x => x.ID_PersoenlicheDaten == idKunde).FirstOrDefault();

                    if (aktKunde != null)
                    {
                        /// ermittle ob es bereits einen Kreditwunsch gibt
                        tblKreditdaten neuerKreditWunsch = context.tblKreditdaten.FirstOrDefault(x => x.ID_Kredit == idKunde);
                        /// nur wenn noch keiner existiert
                        if (neuerKreditWunsch == null)
                        {
                            /// lege einen neuen an
                            neuerKreditWunsch = new tblKreditdaten()
                            {
                                GesamtBetrag = (double)kreditBetrag,
                                Laufzeit = laufzeit,
                                ID_Kredit = idKunde
                            };

                            context.tblKreditdaten.Add(neuerKreditWunsch);
                        }
                    }

                    int anzahlZeilenBetroffen = context.SaveChanges();
                    erfolgreich = anzahlZeilenBetroffen >= 1;
                    Debug.WriteLine($"{anzahlZeilenBetroffen} KreditRahmen gespeichert!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in KreditRahmenSpeichern");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return erfolgreich;
        }

        /// <summary>
        /// Speichert die Daten aus der Finanziellen Situation zu einem Kunden
        /// </summary>
        /// <param name="nettoEinkommen">das Netto Einkommen des Kunden</param>
        /// <param name="ratenVerpflichtungen">Raten Verpflichtungen des Kunden</param>
        /// <param name="wohnkosten">die Wohnkosten des Kunden</param>
        /// <param name="einkünfteAlimenteUnterhalt">Einkünfte aus Alimente und Unterhalt</param>
        /// <param name="unterhaltsZahlungen">Zahlungen für Alimente und Unterhalt</param>
        /// <param name="idKunde">die id des Kunden</param>
        /// <returns>true wenn die finanzielle Situation erfolgreich gespeichert werden konnte, ansonsten false</returns>
        public static bool FinanzielleSituationSpeichern(double nettoEinkommen, double ratenVerpflichtungen, double wohnkosten, double einkünfteAlimenteUnterhalt, double unterhaltsZahlungen, int idKunde)
        {
            Debug.WriteLine("KonsumKreditVerwaltung - FinanzielleSituationSpeichern");
            Debug.Indent();

            bool erfolgreich = false;

            try
            {
                using (var context = new dbLapProjektEntities())
                {

                    /// speichere zum Kunden die Angaben
                    tblPersoenlicheDaten aktKunde = context.tblPersoenlicheDaten.Where(x => x.ID_PersoenlicheDaten == idKunde).FirstOrDefault();

                    if (aktKunde != null)
                    {
                        tblFinanzielleSituation finanzielleSituation = context.tblFinanzielleSituation.FirstOrDefault(x => x.ID_FinanzielleSituation == idKunde);

                        if (finanzielleSituation == null)
                        {
                            finanzielleSituation = new tblFinanzielleSituation();
                            context.tblFinanzielleSituation.Add(finanzielleSituation);
                        }
                        finanzielleSituation.NettoEinkommenJährlich = (double)nettoEinkommen;
                        finanzielleSituation.Unterhaltszahlungen = (double)unterhaltsZahlungen;
                        finanzielleSituation.EinkuenfteAlimente = (double)einkünfteAlimenteUnterhalt;
                        finanzielleSituation.WohnkostenMonatlich = (double)wohnkosten;
                        finanzielleSituation.BestehendeRatenVerpflichtungen = (double)ratenVerpflichtungen;
                        finanzielleSituation.ID_FinanzielleSituation = idKunde;

                        int anzahlZeilenBetroffen = context.SaveChanges();
                        erfolgreich = anzahlZeilenBetroffen >= 1;
                        Debug.WriteLine($"{anzahlZeilenBetroffen} FinanzielleSituation gespeichert!");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in FinanzielleSituation");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return erfolgreich;
        }

        /// Lädt den Kunden für die übergebene ID
        /// </summary>
        /// <param name="id">die id des zu ladenden Kunden</param>
        /// <returns>der Kunde für die übergebene ID</returns>
        public static tblPersoenlicheDaten PersönlicheDatenLaden(int id)
        {
            Debug.WriteLine("KonsumKreditVerwaltung - PersönlicheDatenLaden");
            Debug.Indent();

            tblPersoenlicheDaten persönlicheDaten = null;

            try
            {
                using (var context = new dbLapProjektEntities())
                {
                    persönlicheDaten = context.tblPersoenlicheDaten.Where(x => x.ID_PersoenlicheDaten == id).FirstOrDefault();
                    Debug.WriteLine("PersönlicheDatenLaden geladen!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in PersönlicheDatenLaden");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return persönlicheDaten;
        }

        /// <summary>
        /// Liefert alle Branchen zurück
        /// </summary>
        /// <returns>alle Branchen oder null bei einem Fehler</returns>
        public static List<tblBranche> BranchenLaden()
        {
            Debug.WriteLine("KonsumKreditVerwaltung - BranchenLaden");
            Debug.Indent();

            List<tblBranche> alleBranchen = null;

            try
            {
                using (var context = new dbLapProjektEntities())
                {
                    alleBranchen = context.tblBranche.ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in BranchenLaden");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return alleBranchen;
        }

        /// <summary>
        /// Liefert alle Beschaefitgungsarten zurück
        /// </summary>
        /// <returns>alle Beschaefitgungsarten oder null bei einem Fehler</returns>
        public static List<tblBeschaeftigungsArt> BeschaeftigungsArtenLaden()
        {
            Debug.WriteLine("KonsumKreditVerwaltung - Beschaeftigungsart");
            Debug.Indent();

            List<tblBeschaeftigungsArt> alleBeschaeftigungsArten = null;

            try
            {
                using (var context = new dbLapProjektEntities())
                {
                    alleBeschaeftigungsArten = context.tblBeschaeftigungsArt.ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in Beschaeftigungsart");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return alleBeschaeftigungsArten;
        }

        /// <summary>
        /// Liefert alle Schulabschlüsse zurück
        /// </summary>
        /// <returns>alle Schulabschlüsse oder null bei einem Fehler</returns>
        public static List<tblAbschluss> BildungsAngabenLaden()
        {
            Debug.WriteLine("KonsumKreditVerwaltung - BildungsAngabenLaden");
            Debug.Indent();

            List<tblAbschluss> alleAbschlüsse = null;

            try
            {
                using (var context = new dbLapProjektEntities())
                {
                    alleAbschlüsse = context.tblAbschluss.ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in BildungsAngabenLaden");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return alleAbschlüsse;
        }

        /// <summary>
        /// Liefert alle FamilienStand zurück
        /// </summary>
        /// <returns>alle FamilienStand oder null bei einem Fehler</returns>
        public static List<tblFamilienstand> FamilienStandAngabenLaden()
        {
            Debug.WriteLine("KonsumKreditVerwaltung - FamilienStandAngabenLaden");
            Debug.Indent();

            List<tblFamilienstand> alleFamilienStandsAngaben = null;

            try
            {
                using (var context = new dbLapProjektEntities())
                {
                    alleFamilienStandsAngaben = context.tblFamilienstand.ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in FamilienStandAngabenLaden");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return alleFamilienStandsAngaben;
        }

        /// <summary>
        /// Liefert alle Länder zurück
        /// </summary>
        /// <returns>alle Länder oder null bei einem Fehler</returns>
        public static List<tblLand> LaenderLaden()
        {
            Debug.WriteLine("KonsumKreditVerwaltung - LaenderLaden");
            Debug.Indent();

            List<tblLand> alleLänder = null;

            try
            {
                using (var context = new dbLapProjektEntities())
                {
                    alleLänder = context.tblLand.ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in LaenderLaden");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return alleLänder;
        }

        /// <summary>
        /// Liefert alle Länder zurück
        /// </summary>
        /// <returns>alle Länder oder null bei einem Fehler</returns>
        public static List<tblOrt> PLZLaden()
        {
            Debug.WriteLine("KonsumKreditVerwaltung - PLZ Laden");
            Debug.Indent();

            List<tblOrt> allePLZ = null;

            try
            {
                using (var context = new dbLapProjektEntities())
                {

                    allePLZ = context.tblOrt.OrderBy(x=> x.PLZ).ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in LaenderLaden");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return allePLZ;
        }
        /// <summary>
        /// Liefert alle Wohnarten zurück
        /// </summary>
        /// <returns>alle Wohnarten oder null bei einem Fehler</returns>
        public static List<tblWohnart> WohnartenLaden()
        {
            Debug.WriteLine("KonsumKreditVerwaltung - WohnartenLaden");
            Debug.Indent();

            List<tblWohnart> alleWohnarten = null;

            try
            {
                using (var context = new dbLapProjektEntities())
                {
                    alleWohnarten = context.tblWohnart.ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in WohnartenLaden");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return alleWohnarten;
        }

        /// <summary>
        /// Liefert alle IdentifikatikonsArt zurück
        /// </summary>
        /// <returns>alle IdentifikatikonsArt oder null bei einem Fehler</returns>
        public static List<tblIdentifikationsArt> IdentifikiationsAngabenLaden()
        {
            Debug.WriteLine("KonsumKreditVerwaltung - IdentifikiationsAngabenLaden");
            Debug.Indent();

            List<tblIdentifikationsArt> alleIdentifikationsArten = null;

            try
            {
                using (var context = new dbLapProjektEntities())
                {
                    alleIdentifikationsArten = context.tblIdentifikationsArt.ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in IdentifikiationsAngabenLaden");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return alleIdentifikationsArten;
        }

        /// <summary>
        /// Liefert alle Titel zurück
        /// </summary>
        /// <returns>alle Titel oder null bei einem Fehler</returns>
        public static List<tblTitel> TitelLaden()
        {
            Debug.WriteLine("KonsumKreditVerwaltung - TitelLaden");
            Debug.Indent();

            List<tblTitel> alleTitel = null;

            try
            {
                using (var context = new dbLapProjektEntities())
                {
                    alleTitel = context.tblTitel.ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in TitelLaden");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return alleTitel;
        }

        

        /// <summary>
        /// Speichert die Daten für die übergebene idKunde
        /// </summary>
        /// <param name="idTitel">der Titel des Kunden</param>
        /// <param name="geschlecht">das Geschlecht des Kunden</param>
        /// <param name="geburtsDatum">das Geburtsdatum des Kunden</param>
        /// <param name="vorname">der Vorname des Kunden</param>
        /// <param name="nachname">der Nachname des Kunden</param>
        /// <param name="idTitelNachstehend">der nachstehende Titel des Kunden</param>
        /// <param name="idBildung">die Bildung des Kunden</param>
        /// <param name="idFamilienstand">der Familienstand des Kunden</param>
        /// <param name="idIdentifikationsart">die Identifikations des Kunden</param>
        /// <param name="identifikationsNummer">der Identifikations-Nummer des Kunden</param>
        /// <param name="idStaatsbuergerschaft">die Staatsbürgerschaft des Kunden</param>
        /// <param name="idWohnart">die Wohnart des Kunden</param>
        /// <param name="idKunde">die ID des Kunden</param>
        /// <returns>true wenn das Anpassen der Werte erfolgreich war, ansonsten false</returns>
        public static bool PersönlicheDatenSpeichern(int? idTitel, int geschlecht, DateTime geburtsDatum, string vorname, string nachname, int idBildung, int idFamilienstand, int idIdentifikationsart, string identifikationsNummer, string idStaatsbuergerschaft, int idWohnart, int idKunde)
        {
            Debug.WriteLine("KonsumKreditVerwaltung - PersönlicheDatenSpeichern");
            Debug.Indent();

            bool erfolgreich = false;

            try
            {
                using (var context = new dbLapProjektEntities())
                {

                    /// speichere zum Kunden die Angaben
                    tblPersoenlicheDaten aktKunde = context.tblPersoenlicheDaten.Where(x => x.ID_PersoenlicheDaten == idKunde).FirstOrDefault();

                    if (aktKunde != null)
                    {
                        aktKunde.Vorname = vorname;
                        aktKunde.Nachname = nachname;
                        aktKunde.FKFamilienstand = idFamilienstand;
                        aktKunde.FKAbschluss = idBildung;
                        aktKunde.FKStaatsbuegerschaft = idStaatsbuergerschaft;
                        aktKunde.FKTitel = idTitel;
                        aktKunde.FkIdentifikationsArt = idIdentifikationsart;
                        aktKunde.Identifikationsnummer = identifikationsNummer;
                        aktKunde.FKGeschlecht = geschlecht;
                        aktKunde.FKWohnart = idWohnart;
                    }

                    int anzahlZeilenBetroffen = context.SaveChanges();
                    erfolgreich = anzahlZeilenBetroffen >= 1;
                    Debug.WriteLine($"{anzahlZeilenBetroffen} PersönlicheDaten gespeichert!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in PersönlicheDatenSpeichern");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return erfolgreich;
        }

        /// <summary>
        /// Speichert die Angaben des Arbeitsgebers zu einem Kunden
        /// </summary>
        /// <param name="firmenName">der Firmenname des Arbeitgeber des Kunden</param>
        /// <param name="idBeschäftigungsArt">die Beschäftigungsart des Arbeitgeber des Kunden</param>
        /// <param name="idBranche">die Branche des Arbeitgeber des Kunden</param>
        /// <param name="beschäftigtSeit"> BeschäftigtSeit Wert des Kunden</param>
        /// <param name="idKunde">die ID des Kunden</param>
        /// <returns>true wenn das Speichern erfolgreich war, ansonsten false</returns>
        public static bool ArbeitgeberAngabenSpeichern(string firmenName, int idBeschäftigungsArt, int idBranche, string beschäftigtSeit, int idKunde)
        {
            Debug.WriteLine("KonsumKreditVerwaltung - ArbeitgeberAngabenSpeichern");
            Debug.Indent();

            bool erfolgreich = false;

            try
            {
                using (var context = new dbLapProjektEntities())
                {

                    /// speichere zum Kunden die Angaben
                    tblPersoenlicheDaten aktKunde = context.tblPersoenlicheDaten.Where(x => x.ID_PersoenlicheDaten == idKunde).FirstOrDefault();

                    if (aktKunde != null)
                    {
                        tblArbeitgeber neuerArbeitgeber = new tblArbeitgeber()
                        {
                            BeschaeftigtSeitMonaten = DateTime.Now.Month - DateTime.Parse(beschäftigtSeit).Month,
                            FKBranche = idBranche,
                            FKBeschaeftigungsArt = idBeschäftigungsArt,
                            Firma = firmenName
                        };
                        aktKunde.tblArbeitgeber = neuerArbeitgeber;
                    }

                    int anzahlZeilenBetroffen = context.SaveChanges();
                    erfolgreich = anzahlZeilenBetroffen >= 1;
                    Debug.WriteLine($"{anzahlZeilenBetroffen} ArbeitgeberDaten gespeichert!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in ArbeitgeberAngabenSpeichern");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return erfolgreich;
        }

        public static bool KontaktdatenSpeichern(string strasse, string hausNr, string stiege, string etage, string tuer, string eMail 
            , string telNr,int id_PLZ, int idKunde)
        {
            Debug.WriteLine("KonsumKreditVerwaltung - KontaktDatenSpeichern");
            Debug.Indent();

            bool erfolgreich = false;

            try
            {
                using (var context = new dbLapProjektEntities())
                {

                    /// speichere zum Kunden die Angaben
                    tblPersoenlicheDaten aktKunde = context.tblPersoenlicheDaten.Where(x => x.ID_PersoenlicheDaten == idKunde).FirstOrDefault();

                    if (aktKunde != null)
                    {
                        tblKontaktdaten neueKontaktdaten = new tblKontaktdaten()
                        {
                            Strasse = strasse,
                            Hausnummer = hausNr,
                            Stiege = stiege,
                            Etage = etage,
                            Türnummer = tuer,
                            email = eMail,
                            Tel = telNr,
                            FKOrt = id_PLZ,
                            ID_Kontaktdaten = idKunde                          
                        };

                        context.tblKontaktdaten.Add(neueKontaktdaten);
                    }

                    int anzahlZeilenBetroffen = context.SaveChanges();
                    erfolgreich = anzahlZeilenBetroffen >= 1;
                    Debug.WriteLine($"{anzahlZeilenBetroffen} Kontakt-Daten gespeichert!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in KontoInformationenSpeichern");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return erfolgreich;
        }

        public static bool KontoinformationenSpeichern(string bankName, string iban, string bic, bool neuesKonto, int idKunde)
        {
            Debug.WriteLine("KonsumKreditVerwaltung - KontoInformationenSpeichern");
            Debug.Indent();

            bool erfolgreich = false;

            try
            {
                using (var context = new dbLapProjektEntities())
                {

                    /// speichere zum Kunden die Angaben
                    tblPersoenlicheDaten aktKunde = context.tblPersoenlicheDaten.Where(x => x.ID_PersoenlicheDaten == idKunde).FirstOrDefault();

                    if (aktKunde != null)
                    {
                        tblKontoDaten neueKontoDaten = new tblKontoDaten()
                        {
                            BankName = bankName,
                            IBAN = iban,
                            BIC = bic,
                            NeuesKonto = neuesKonto,
                            ID_KontoDaten = idKunde
                        };

                        context.tblKontoDaten.Add(neueKontoDaten);
                    }

                    int anzahlZeilenBetroffen = context.SaveChanges();
                    erfolgreich = anzahlZeilenBetroffen >= 1;
                    Debug.WriteLine($"{anzahlZeilenBetroffen} Konto-Daten gespeichert!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in KontoInformationenSpeichern");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return erfolgreich;
        }
        /// Lädt zu einer übergebenen ID alle Informationen zu diesem Kunden aus der DB
        /// </summary>
        /// <param name="iKunde">die ID des zu landenden Kunden</param>
        /// <returns>alle Daten aus der DB zu diesem Kunden</returns>
        public static tblPersoenlicheDaten KundeLaden(int idKunde)
        {
            Debug.WriteLine("KonsumKreditVerwaltung - KundeLaden");
            Debug.Indent();

            tblPersoenlicheDaten aktuellerKunde = null;

            try
            {
                using (var context = new dbLapProjektEntities())
                {
                    aktuellerKunde = context.tblPersoenlicheDaten
                        .Include("tblArbeitgeber")
                        .Include("tblArbeitgeber.tblBeschaeftigungsArt")
                        .Include("tblArbeitgeber.tblBranche")
                        .Include("tblFamilienstand")
                        .Include("tblFinanzielleSituation")
                        .Include("tblIdentifikationsArt")
                        .Include("tblKontaktdaten")
                        .Include("tblKontoDaten")
                        .Include("tblKreditdaten")
                        .Include("tblAbschluss")
                        .Include("tblTitel")
                        .Include("tblWohnart")
                        .Include("tblLand")
                        .Include("tblKontaktdaten.tblOrt")
                        .Where(x => x.ID_PersoenlicheDaten == idKunde).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in KundeLaden");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return aktuellerKunde;
        }
    }
}
