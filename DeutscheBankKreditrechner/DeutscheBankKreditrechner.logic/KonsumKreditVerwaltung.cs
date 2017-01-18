using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using DeutscheBankKreditrechner.logic;
using System.Net;

namespace DeutscheBankKreditrechner.logic
{
    public class KonsumKReditVerwaltung
    {
        public static void FirstMailSenden()
        {
            using (var context = new dbLapProjektEntities())
            {
                bool frist14Tage = false;
                bool frist21Tage = false;
                string lAktDatum = DateTime.Now.AddDays(-14.0).ToShortDateString();
                DateTime AktDatum = DateTime.Parse(lAktDatum);

                //                DateTime Test = new DateTime(2017, 1, 4);
                
                //prüfen ob es Antragsteller gibt die in verzug sind (14 Tage)
                frist14Tage = context.tblPersoenlicheDaten.Any(x => x.ErstellDatum == lAktDatum 
                && x.hatGebührBezahlt == false 
                && x.hat14TageFristMailBekommen == false 
                && x.hatGültigenAntragGestellt == true
                );

                DateTime Prüfdatum21 = AktDatum.AddDays(-7.0);
                string Prüfstring = Prüfdatum21.ToShortDateString();
                // prüfen ob Antragsteller in 21 Tage Verzug sind und storniert wird
                frist21Tage = context.tblPersoenlicheDaten.Any(x => x.ErstellDatum == Prüfstring && x.hatGebührBezahlt == false && x.hat21TageFristMailBekommen == false && x.hatGültigenAntragGestellt == true);

                if (frist14Tage)
                {
                    List<tblPersoenlicheDaten> _KundenListe = new List<tblPersoenlicheDaten>();
                    _KundenListe = context.tblPersoenlicheDaten.Where(x => x.ErstellDatum == lAktDatum
                    && x.hat14TageFristMailBekommen == false
                    && x.hatGebührBezahlt == false
                    && x.hatGültigenAntragGestellt == true)
                    .ToList();

                    foreach (var item in _KundenListe)
                    {
                        

                        string mailAdresse;
                        try
                        {
                            mailAdresse = context.tblKontaktdaten.FirstOrDefault(x => x.ID_Kontaktdaten == item.ID_PersoenlicheDaten).email;
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        try
                        { 
                            using (SmtpClient smtpClient = new SmtpClient("SRV08.itccn.loc", 25))
                            { // <-- note the use of localhost
                                NetworkCredential creds = new NetworkCredential("Rupert.Pfiel@qualifizierung.or.at", "Gilgamosch0088q");
                                smtpClient.Credentials = creds;
                                string text = EmailText(item,false);
                                MailMessage msg = new MailMessage("Rupert.Pfiel@qualifizierung.or.at", mailAdresse.ToString(), "Zahlungserinnerung", text);
                                
                                msg.IsBodyHtml = true;
                                
                                smtpClient.Send(msg);

                                item.hat14TageFristMailBekommen = true;
                                context.SaveChanges();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                        
                    }

                }

                if (frist21Tage)
                {


                    List<tblPersoenlicheDaten> _KundenListe = new List<tblPersoenlicheDaten>();
                    _KundenListe = context.tblPersoenlicheDaten.Where(x => x.ErstellDatum == Prüfstring
                    && x.hat21TageFristMailBekommen == false
                    && x.hatGebührBezahlt == false
                    && x.hatGültigenAntragGestellt == true)
                    .ToList();

                    foreach (var item in _KundenListe)
                    {


                        string mailAdresse;
                        try
                        {
                            mailAdresse = context.tblKontaktdaten.FirstOrDefault(x => x.ID_Kontaktdaten == item.ID_PersoenlicheDaten).email;
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        try
                        {
                            using (SmtpClient smtpClient = new SmtpClient("SRV08.itccn.loc", 25))
                            { // <-- note the use of localhost
                                NetworkCredential creds = new NetworkCredential("Rupert.Pfiel@qualifizierung.or.at", "Gilgamosch0088q");
                                smtpClient.Credentials = creds;
                                string text = EmailText(item, true);
                                MailMessage msg = new MailMessage("Rupert.Pfiel@qualifizierung.or.at", mailAdresse.ToString(), "Stornierung Kreditantrag", text);
                                msg.IsBodyHtml = true;

                                smtpClient.Send(msg);

                                item.hat21TageFristMailBekommen = true;
                                item.istStorniert = true;
                                context.SaveChanges();

                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                        
                    }
                }

            }

        }

        public static string EmailText(tblPersoenlicheDaten aKunde,bool aWirdStorniert)
        {
            string Anrede;

            //Anrede
            if (aKunde.FKGeschlecht == 1)
            {
                Anrede = "Sehr geehrter Herr " + aKunde.Nachname + " " + aKunde.Vorname;
            }
            else
            {
                Anrede = "Sehr geehrte Frau " + aKunde.Nachname + " " + aKunde.Vorname;
            }
            string Text = "";
            if (!aWirdStorniert)
            {
                Text = "<p>Vielen Dank für Ihr Vertrauen in unseren Service.</p><br> <p>Bezüglich Ihres Kreditantrages vom " + aKunde.ErstellDatum.ToString() + " möchten wir Sie hiermit höflichst dazu auffordern die angefallene Bearbeitungsgebühr von 4,65€ zu überweisen.</p>" +
                "<br><br><p> Sollten Sie diese nicht innerhalb von 7 Tagen überweisen wird Ihr Kreditantrag storniert.</p><br><p>Natürlich stehen Wir Ihnen jederzeit für weitere Informationen zur Verfügung.\n Hochachtungsvoll Ihre Deutsche Bank AG</p>";
            }
            else
            {
                Text = "<p>Ihr Antrag vom " + aKunde.ErstellDatum.ToString() + " wurde <b>storniert</b>!</p> <p>Natürlich stehen Wir Ihnen jederzeit für weitere Informationen zur Verfügung.<p><br> Hochachtungsvoll Ihre Deutsche Bank AG";
            }
            string Endtext = Anrede + Text;

            return Endtext;
        }

        public static void AntragBewilligt(int aKundenID)
        {
            using (var context = new dbLapProjektEntities())
            {
                try
                {
                    context.tblPersoenlicheDaten.FirstOrDefault(x => x.ID_PersoenlicheDaten == aKundenID).hatGültigenAntragGestellt = true;
                    context.SaveChanges();
                }
                catch (Exception)
                {

                    throw;
                }


            }

        }



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
                        FKGeschlecht = 1,
                        ErstellDatum = DateTime.Now.ToShortDateString(),
                        hatGebührBezahlt = false,
                        istStorniert = false,
                        hatGültigenAntragGestellt = false,
                        hat14TageFristMailBekommen = false,
                        hat21TageFristMailBekommen = false
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
                    wunsch = context.tblKreditdaten.FirstOrDefault(x => x.ID_Kredit == id);
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
                            neuerKreditWunsch = new tblKreditdaten();
                            context.tblKreditdaten.Add(neuerKreditWunsch);
                        }
                        neuerKreditWunsch.GesamtBetrag = (double)kreditBetrag;
                        neuerKreditWunsch.Laufzeit = laufzeit;
                        neuerKreditWunsch.ID_Kredit = idKunde;
                    }

                    int anzahlZeilenBetroffen = context.SaveChanges();
                    erfolgreich = anzahlZeilenBetroffen >= 0;
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
                        erfolgreich = anzahlZeilenBetroffen >= 0;
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
        /// Lädt den Kreditrahmen für die übergebene ID
        /// </summary>
        /// <param name="id">die id des zu ladenden Kreditrahmens</param>
        /// <returns>der Kreditwunsch für die übergebene ID</returns>
        public static tblArbeitgeber ArbeitgeberAngabenLaden(int id)
        {
            Debug.WriteLine("KonsumKreditVerwaltung - ArbeitgeberAngabenLaden");
            Debug.Indent();

            tblArbeitgeber arbeitGeber = null;

            try
            {
                using (var context = new dbLapProjektEntities())
                {
                    arbeitGeber = context.tblArbeitgeber.Where(x => x.ID_Arbeitgeber == id).FirstOrDefault();
                    Debug.WriteLine("ArbeitgeberAngaben geladen!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in ArbeitgeberAngabenLaden");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return arbeitGeber;
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
        public static bool PersönlicheDatenSpeichern(int? idTitel, int geschlecht, string geburtsDatum, string vorname, string nachname, int idBildung, int idFamilienstand, int idIdentifikationsart, string identifikationsNummer, string idStaatsbuergerschaft, int idWohnart, int idKunde)
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
                        if(geburtsDatum != null)
                        aktKunde.GeburtsDatum = DateTime.Parse(geburtsDatum);
                        
                    }

                    int anzahlZeilenBetroffen = context.SaveChanges();
                    erfolgreich = anzahlZeilenBetroffen >= 0;
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
                        tblArbeitgeber arbeitgeber = context.tblArbeitgeber.FirstOrDefault(x => x.ID_Arbeitgeber == idKunde);

                        if (arbeitgeber == null)
                        {
                            arbeitgeber = new tblArbeitgeber();
                            context.tblArbeitgeber.Add(arbeitgeber);
                        }
                        arbeitgeber.BeschaeftigtSeit = DateTime.Parse(beschäftigtSeit);
                        arbeitgeber.FKBranche = idBranche;
                        arbeitgeber.FKBeschaeftigungsArt = idBeschäftigungsArt;
                        arbeitgeber.Firma = firmenName;
                        aktKunde.tblArbeitgeber = arbeitgeber;
                    }

                    int anzahlZeilenBetroffen = context.SaveChanges();
                    erfolgreich = anzahlZeilenBetroffen >= 0;
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

        /// <summary>
        /// Lädt die KontoDaten für die übergebene ID
        /// </summary>
        /// <param name="id">die id der zu ladenden KontoDaten</param>
        /// <returns>die KontoDaten für die übergebene ID</returns>
        public static tblKontoDaten KontoInformationenLaden(int id)
        {
            Debug.WriteLine("KonsumKreditVerwaltung - KontoInformationenLaden");
            Debug.Indent();

            tblKontoDaten kontoDaten = null;

            try
            {
                using (var context = new dbLapProjektEntities())
                {
                    kontoDaten = context.tblKontoDaten.Where(x => x.ID_KontoDaten == id).FirstOrDefault();
                    Debug.WriteLine("KontoInformationen geladen!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in KontoInformationenLaden");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return kontoDaten;
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
                        tblKontaktdaten neueKontaktdaten = context.tblKontaktdaten.FirstOrDefault(x => x.ID_Kontaktdaten == idKunde);

                        if (neueKontaktdaten == null)
                        {
                            neueKontaktdaten = new tblKontaktdaten();
                            context.tblKontaktdaten.Add(neueKontaktdaten);

                        }
                        neueKontaktdaten.Strasse = strasse;
                        neueKontaktdaten.Hausnummer = hausNr;
                        neueKontaktdaten.Stiege = stiege;
                        neueKontaktdaten.Etage = etage;
                        neueKontaktdaten.Türnummer = tuer;
                        neueKontaktdaten.email = eMail;
                        neueKontaktdaten.Tel = telNr;
                        neueKontaktdaten.FKOrt = id_PLZ;
                        neueKontaktdaten.ID_Kontaktdaten = idKunde;

                        int anzahlZeilenBetroffen = context.SaveChanges();
                        erfolgreich = anzahlZeilenBetroffen >= 0;
                        Debug.WriteLine($"{anzahlZeilenBetroffen} Kontakt-Daten gespeichert!");
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fehler in KontaktdatenSpeichern");
                Debug.Indent();
                Debug.WriteLine(ex.Message);
                Debug.Unindent();
                Debugger.Break();
            }

            Debug.Unindent();
            return erfolgreich;
        }

        public static tblKontaktdaten KontaktdatenLaden(int id)
        {
            Debug.WriteLine("KonsumKreditVerwaltung - KontaktdatenLaden");
            Debug.Indent();

            tblKontaktdaten kontaktDaten = null;

            try
            {
                using (var context = new dbLapProjektEntities())
                {
                    kontaktDaten = context.tblKontaktdaten.FirstOrDefault(x => x.ID_Kontaktdaten == id);
                    Debug.WriteLine("Kontaktdaten geladen!");
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
            return kontaktDaten;


        }

        public static bool KontoinformationenSpeichern(string bankName, string iban, string bic, bool neuesKonto, int idKunde
            )
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
                        tblKontoDaten kontoDaten = context.tblKontoDaten.FirstOrDefault(x => x.ID_KontoDaten == idKunde);

                        if (kontoDaten == null)
                        {
                            kontoDaten = new tblKontoDaten();
                            context.tblKontoDaten.Add(kontoDaten);
                        }

                            kontoDaten.BankName = bankName;
                            kontoDaten.IBAN = iban;
                            kontoDaten.BIC = bic;
                            kontoDaten.NeuesKonto = neuesKonto;
                            kontoDaten.ID_KontoDaten = idKunde;
                                                    
                        
                        int anzahlZeilenBetroffen = context.SaveChanges();
                        erfolgreich = anzahlZeilenBetroffen >= 0;
                        
                        Debug.WriteLine($"{anzahlZeilenBetroffen} Konto-Daten gespeichert!");
                    }
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
                        .Include("tblGeschlecht")
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
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<tblPersoenlicheDaten> KundenLaden()
        {
            Debug.WriteLine("KonsumKreditVerwaltung - KundeLaden");
            Debug.Indent();

            List<tblPersoenlicheDaten> aktuellerKunde = null;

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
                        .Include("tblGeschlecht")
                        .Where(x=> x.tblKontoDaten != null)
                        .OrderBy(x => x.tblKreditdaten.GesamtBetrag)
                        .ToList();
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
