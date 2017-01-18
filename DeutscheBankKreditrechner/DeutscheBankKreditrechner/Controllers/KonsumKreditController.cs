using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeutscheBankKreditrechner.web.Models;
using DeutscheBankKreditrechner.logic;
using System.Globalization;
using DeutescheBankKreditrechner.freigabe;
using DeutscheBankKreditrechner.web.Controllers;

namespace DeutscheBankKreditrechner.Controllers
{
    public class KonsumKreditController : Controller
    {
        [HttpGet]
        public ActionResult Hinweis()
        {
            Debug.WriteLine("GET - KonsumKredit - Hiwneis");

            return View();
        }


        [HttpGet]
        public ActionResult KreditRahmen()
        {
            Debug.WriteLine("GET - KonsumKredit - KreditRahmen");

            KreditRahmenModel model = new KreditRahmenModel()
            {
                GewünschterBetrag = 25000,  // default Werte
                Laufzeit = 12   // default Werte
            };
            int id = -1;
            if (Request.Cookies["idKunde"] != null && int.TryParse(Request.Cookies["idKunde"].Value, out id))
            {
                /// lade Daten aus Datenbank
                tblKreditdaten wunsch = KonsumKReditVerwaltung.KreditRahmenLaden(id);
                model.GewünschterBetrag = (int)wunsch.GesamtBetrag;
                model.Laufzeit = wunsch.Laufzeit;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KreditRahmen(KreditRahmenModel model)
        {
            Debug.WriteLine("POST - KonsumKredit - KreditRahmen");

            if (ModelState.IsValid)
            {
                /// speichere Daten über BusinessLogic
                if (Request.Cookies["idKunde"] == null)
                {
                    tblPersoenlicheDaten neuerKunde = KonsumKReditVerwaltung.ErzeugeKunde();

                    if (neuerKunde != null && KonsumKReditVerwaltung.KreditRahmenSpeichern(model.GewünschterBetrag, model.Laufzeit, neuerKunde.ID_PersoenlicheDaten))
                    {
                        Response.Cookies.Add(new HttpCookie("idKunde", neuerKunde.ID_PersoenlicheDaten.ToString()));
                        /// gehe zum nächsten Schritt
                        if (!HomeController.alleDatenAngeben)
                        {
                            return RedirectToAction("FinanzielleSituation");
                        }
                        else
                        {
                            return RedirectToAction("Zusammenfassung");
                        }
                    }
                }
                else
                {
                    int idKunde = int.Parse(Request.Cookies["idKunde"].Value);
                    if (KonsumKReditVerwaltung.KreditRahmenSpeichern(model.GewünschterBetrag, model.Laufzeit, idKunde))
                    {
                        /// gehe zum nächsten Schritt
                        if (!HomeController.alleDatenAngeben)
                        {
                            return RedirectToAction("FinanzielleSituation");
                        }
                        else
                        {
                            return RedirectToAction("Zusammenfassung");
                        }
                    }
                }
            }

            /// falls der ModelState NICHT valid ist, bleibe hier und
            /// gib die Daten (falls vorhanden) wieder auf das UI
            return View(model);
        }


        [HttpGet]
        public ActionResult FinanzielleSituation()
        {
            Debug.WriteLine("GET - KonsumKredit - FinanzielleSituation");

            FinanzielleSituationModel model = new FinanzielleSituationModel()
            {
                ID_Kunde = int.Parse(Request.Cookies["idKunde"].Value)
            };
            tblFinanzielleSituation situation = KonsumKReditVerwaltung.FinanzielleSituationLaden(model.ID_Kunde);
            if (situation != null)
            {
                model.EinkünfteAlimenteUnterhalt = (double)situation.EinkuenfteAlimente.Value;
                model.NettoEinkommen = (double)situation.NettoEinkommenJährlich;
                model.RatenVerpflichtungen = (double)situation.BestehendeRatenVerpflichtungen.Value;
                model.UnterhaltsZahlungen = (double)situation.Unterhaltszahlungen.Value;
                model.Wohnkosten = (double)situation.WohnkostenMonatlich.Value;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FinanzielleSituation(FinanzielleSituationModel model)
        {
            Debug.WriteLine("POST - KonsumKredit - FinanzielleSituation");

            if (ModelState.IsValid)
            {
                /// speichere Daten über BusinessLogic
                if (KonsumKReditVerwaltung.FinanzielleSituationSpeichern(
                                                model.NettoEinkommen,
                                                model.RatenVerpflichtungen,
                                                model.Wohnkosten,
                                                model.EinkünfteAlimenteUnterhalt,
                                                model.UnterhaltsZahlungen,
                                                model.ID_Kunde))
                {

                    if (!HomeController.alleDatenAngeben)
                    {
                        return RedirectToAction("PersönlicheDaten");
                    }
                    else
                    {
                        return RedirectToAction("Zusammenfassung");
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult PersönlicheDaten()
        {
            Debug.WriteLine("GET - KonsumKredit - PersönlicheDaten");

            List<BildungsModel> alleBildungsAngaben = new List<BildungsModel>();
            List<FamilienStandModel> alleFamilienStandAngaben = new List<FamilienStandModel>();
            List<IdentifikationsModel> alleIdentifikationsAngaben = new List<IdentifikationsModel>();
            List<StaatsbuergerschaftsModel> alleStaatsbuergerschaftsAngaben = new List<StaatsbuergerschaftsModel>();
            List<TitelModel> alleTitelAngaben = new List<TitelModel>();
            List<WohnartModel> alleWohnartAngaben = new List<WohnartModel>();


            /// Lade Daten aus Logic
            foreach (var bildungsAngabe in KonsumKReditVerwaltung.BildungsAngabenLaden())
            {
                alleBildungsAngaben.Add(new BildungsModel()
                {
                    ID = bildungsAngabe.ID_Abschluss.ToString(),
                    Bezeichnung = bildungsAngabe.Abschluss
                });
            }

            foreach (var familienStand in KonsumKReditVerwaltung.FamilienStandAngabenLaden())
            {
                alleFamilienStandAngaben.Add(new FamilienStandModel()
                {
                    ID = familienStand.ID_Familienstand.ToString(),
                    Bezeichnung = familienStand.Familienstand
                });
            }
            foreach (var identifikationsAngabe in KonsumKReditVerwaltung.IdentifikiationsAngabenLaden())
            {
                alleIdentifikationsAngaben.Add(new IdentifikationsModel()
                {
                    ID = identifikationsAngabe.ID_IdentitifaktionsArt.ToString(),
                    Bezeichnung = identifikationsAngabe.IdentitfikationsArt
                });
            }
            foreach (var land in KonsumKReditVerwaltung.LaenderLaden())
            {
                alleStaatsbuergerschaftsAngaben.Add(new StaatsbuergerschaftsModel()
                {
                    ID = land.ID_Land,
                    Bezeichnung = land.Land
                });
            }
            foreach (var titel in KonsumKReditVerwaltung.TitelLaden())
            {
                alleTitelAngaben.Add(new TitelModel()
                {
                    ID = titel.ID_Titel.ToString(),
                    Bezeichnung = titel.Titel
                });
            }
            foreach (var wohnart in KonsumKReditVerwaltung.WohnartenLaden())
            {
                alleWohnartAngaben.Add(new WohnartModel()
                {
                    ID = wohnart.ID_Wohnart.ToString(),
                    Bezeichnung = wohnart.Wohnart
                });
            }



            PersönlicheDatenModel model = new PersönlicheDatenModel()
            {
                AlleBildungAngaben = alleBildungsAngaben,
                AlleFamilienStandAngaben = alleFamilienStandAngaben,
                AlleIdentifikationsAngaben = alleIdentifikationsAngaben,
                AlleStaatsbuergerschaftsAngaben = alleStaatsbuergerschaftsAngaben,
                AlleTitelAngaben = alleTitelAngaben,
                AlleWohnartAngaben = alleWohnartAngaben,
                ID_Kunde = int.Parse(Request.Cookies["idKunde"].Value)
            };
            tblPersoenlicheDaten kunde = KonsumKReditVerwaltung.PersönlicheDatenLaden(model.ID_Kunde);
            if (kunde.FKStaatsbuegerschaft != null)
            {
                model.Geschlecht = kunde.FKGeschlecht == 1 ? Geschlecht.Männlich : Geschlecht.Weiblich;
                model.Vorname = kunde.Vorname;
                model.Nachname = kunde.Nachname;
                model.ID_Titel = kunde.FKTitel.HasValue ? kunde.FKTitel.Value : 0;
                model.GeburtsDatum = kunde.GeburtsDatum.Value.ToShortDateString();
                model.ID_Staatsbuergerschaft = kunde.FKStaatsbuegerschaft;
                model.ID_Familienstand = kunde.FKFamilienstand.HasValue ? kunde.FKFamilienstand.Value : 0;
                model.ID_Wohnart = kunde.FKWohnart.HasValue ? kunde.FKWohnart.Value : 0;
                model.ID_Bildung = kunde.FKAbschluss.HasValue ? kunde.FKAbschluss.Value : 0;
                model.ID_Identifikationsart = kunde.FkIdentifikationsArt.HasValue ? kunde.FkIdentifikationsArt.Value : 0;
                model.IdentifikationsNummer = kunde.Identifikationsnummer;
            }
            

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PersönlicheDaten(PersönlicheDatenModel model)
        {
            Debug.WriteLine("POST - KonsumKredit - PersönlicheDaten");

            if (ModelState.IsValid)
            {
                /// speichere Daten über BusinessLogic
                if (KonsumKReditVerwaltung.PersönlicheDatenSpeichern(
                                                model.ID_Titel,
                                                model.Geschlecht == Geschlecht.Männlich ? 1 : 2,
                                                model.GeburtsDatum,
                                                model.Vorname,
                                                model.Nachname,
                                                model.ID_Bildung,
                                                model.ID_Familienstand,
                                                model.ID_Identifikationsart,
                                                model.IdentifikationsNummer,
                                                model.ID_Staatsbuergerschaft,
                                                model.ID_Wohnart,
                                                model.ID_Kunde))
                {
                    if (!HomeController.alleDatenAngeben)
                    {
                        return RedirectToAction("KontaktDaten");
                    }
                    else
                    {
                        return RedirectToAction("Zusammenfassung");
                    }
                }
            }
            #region Listen Nachladen
            // Titel Nachladen
            if (model.AlleTitelAngaben == null)
            {
                model.AlleTitelAngaben = new List<TitelModel>();
                foreach (var titel in KonsumKReditVerwaltung.TitelLaden())
                {
                    model.AlleTitelAngaben.Add(new TitelModel()
                    {
                        ID = titel.ID_Titel.ToString(),
                        Bezeichnung = titel.Titel
                    });
                }
            }
            // Abschluss Nachladen
            if (model.AlleBildungAngaben == null)
            {
                model.AlleBildungAngaben = new List<BildungsModel>();
                foreach (var titel in KonsumKReditVerwaltung.BildungsAngabenLaden())
                {
                    model.AlleBildungAngaben.Add(new BildungsModel()
                    {
                        ID = titel.ID_Abschluss.ToString(),
                        Bezeichnung = titel.Abschluss
                    });
                }
            }
            // Familienstand nachladen
            if (model.AlleFamilienStandAngaben == null)
            {
                model.AlleFamilienStandAngaben = new List<FamilienStandModel>();
                foreach (var titel in KonsumKReditVerwaltung.FamilienStandAngabenLaden())
                {
                    model.AlleFamilienStandAngaben.Add(new FamilienStandModel()
                    {
                        ID = titel.ID_Familienstand.ToString(),
                        Bezeichnung = titel.Familienstand
                    });
                }
            }

            // Identifikation nachladen
            if (model.AlleIdentifikationsAngaben == null)
            {
                model.AlleIdentifikationsAngaben = new List<IdentifikationsModel>();
                foreach (var titel in KonsumKReditVerwaltung.IdentifikiationsAngabenLaden())
                {
                    model.AlleIdentifikationsAngaben.Add(new IdentifikationsModel()
                    {
                        ID = titel.ID_IdentitifaktionsArt.ToString(),
                        Bezeichnung = titel.IdentitfikationsArt
                    });
                }
            }
            // Staatsbuergerschaften nachladen
            if (model.AlleStaatsbuergerschaftsAngaben == null)
            {
                model.AlleStaatsbuergerschaftsAngaben = new List<StaatsbuergerschaftsModel>();
                foreach (var titel in KonsumKReditVerwaltung.LaenderLaden())
                {
                    model.AlleStaatsbuergerschaftsAngaben.Add(new StaatsbuergerschaftsModel()
                    {
                        ID = titel.ID_Land.ToString(),
                        Bezeichnung = titel.Land
                    });
                }
            }

            // Wohnarten nachladen
            if (model.AlleWohnartAngaben == null)
            {
                model.AlleWohnartAngaben = new List<WohnartModel>();
                foreach (var titel in KonsumKReditVerwaltung.WohnartenLaden())
                {
                    model.AlleWohnartAngaben.Add(new WohnartModel()
                    {
                        ID = titel.ID_Wohnart.ToString(),
                        Bezeichnung = titel.Wohnart
                    });
                }
            }
            #endregion

            return View(model);
        }

        [HttpGet]
        public ActionResult Arbeitgeber()
        {
            Debug.WriteLine("GET - KonsumKredit - Arbeitgeber");

            List<BeschaeftigungsArtModel> alleBeschaeftigungen = new List<BeschaeftigungsArtModel>();
            List<BrancheModel> alleBranchen = new List<BrancheModel>();

            foreach (var branche in KonsumKReditVerwaltung.BranchenLaden())
            {
                alleBranchen.Add(new BrancheModel()
                {
                    ID = branche.ID_Branche.ToString(),
                    Bezeichnung = branche.Branche
                });
            }

            foreach (var beschaeftigungsArt in KonsumKReditVerwaltung.BeschaeftigungsArtenLaden())
            {
                alleBeschaeftigungen.Add(new BeschaeftigungsArtModel()
                {
                    ID = beschaeftigungsArt.ID_BeschaeftigungsArt.ToString(),
                    Bezeichnung = beschaeftigungsArt.Beschaeftigungsart
                });
            }

            ArbeitgeberModel model = new ArbeitgeberModel()
            {
                AlleBeschaeftigungen = alleBeschaeftigungen,
                AlleBranchen = alleBranchen,
                ID_Kunde = int.Parse(Request.Cookies["idKunde"].Value)
            };
            tblArbeitgeber arbeitgeberDaten = KonsumKReditVerwaltung.ArbeitgeberAngabenLaden(model.ID_Kunde);
            if (arbeitgeberDaten != null)
            {
                model.BeschäftigtSeit = arbeitgeberDaten.BeschaeftigtSeit.ToString("MM.yyyy");
                model.FirmenName = arbeitgeberDaten.Firma;
                model.ID_BeschäftigungsArt = arbeitgeberDaten.FKBeschaeftigungsArt; ;
                model.ID_Branche = arbeitgeberDaten.FKBranche;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Arbeitgeber(ArbeitgeberModel model)
        {
            Debug.WriteLine("POST - KonsumKredit - Arbeitgeber");

            if (ModelState.IsValid)
            {
                /// speichere Daten über BusinessLogic
                if (KonsumKReditVerwaltung.ArbeitgeberAngabenSpeichern(
                                                model.FirmenName,
                                                model.ID_BeschäftigungsArt,
                                                model.ID_Branche,
                                                model.BeschäftigtSeit,
                                                model.ID_Kunde))
                {
                    if (!HomeController.alleDatenAngeben)
                    {
                        return RedirectToAction("Kontoinformationen");
                    }
                    else {
                        return RedirectToAction("Zusammenfassung");
                    }
                }
            }
            #region Listen nachladen
            // Beschaeftigungsafrten nachladen
            if (model.AlleBeschaeftigungen == null)
            {
                model.AlleBeschaeftigungen = new List<BeschaeftigungsArtModel>();
                foreach (var titel in KonsumKReditVerwaltung.BeschaeftigungsArtenLaden())
                {
                    model.AlleBeschaeftigungen.Add(new BeschaeftigungsArtModel()
                    {
                        ID = titel.ID_BeschaeftigungsArt.ToString(),
                        Bezeichnung = titel.Beschaeftigungsart
                    });
                }
            }

            // Branchen nachladen
            if (model.AlleBranchen == null)
            {
                model.AlleBranchen = new List<BrancheModel>();
                foreach (var titel in KonsumKReditVerwaltung.BranchenLaden())
                {
                    model.AlleBranchen.Add(new BrancheModel()
                    {
                        ID = titel.ID_Branche.ToString(),
                        Bezeichnung = titel.Branche
                    });
                }
            }

            #endregion

            return View(model);
        }

        [HttpGet]
        public ActionResult KontoInformationen()
        {
            Debug.WriteLine("GET - KonsumKredit - KontoInformationen");

            KontoInformationenModel model = new KontoInformationenModel()
            {
                ID_Kunde = int.Parse(Request.Cookies["idKunde"].Value)
            };
            tblKontoDaten daten = KonsumKReditVerwaltung.KontoInformationenLaden(model.ID_Kunde);
            if (daten != null)
            {
                model.BankName = daten.BankName;
                model.BIC = daten.BIC;
                model.IBAN = daten.IBAN;
                model.NeuesKonto = daten.NeuesKonto.Value;
               
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KontoInformationen(KontoInformationenModel model)
        {
            Debug.WriteLine("POST - KonsumKredit - KontoInformationen");

            if (ModelState.IsValid)
            {
                /// speichere Daten über BusinessLogic
                if (KonsumKReditVerwaltung.KontoinformationenSpeichern(
                                                model.BankName,
                                                model.IBAN,
                                                model.BIC,
                                                model.NeuesKonto,
                                                model.ID_Kunde
                                               ))
                {
                    return RedirectToAction("Zusammenfassung");
                }
            }

            return View();
        }

        [HttpGet]
        public ActionResult KontaktDaten()
        {
            Debug.WriteLine("GET - KonsumKredit - Kontaktdaten");
            List<PLZModel> AllePostleitZahlen = new List<PLZModel>();

            // Lade Orte aus Logic
            foreach (var ort in KonsumKReditVerwaltung.PLZLaden())
            {
                AllePostleitZahlen.Add(new PLZModel()
                {
                    ID = ort.ID_Ort.ToString(),
                    Bezeichnung = ort.Ort + "(" + ort.PLZ + ")"
                });
            }

            KontaktDatenModel model = new KontaktDatenModel()
            {
                AllePostleitZahlen = AllePostleitZahlen,
                ID_Kunde = int.Parse(Request.Cookies["idKunde"].Value)
            };

            tblKontaktdaten daten = KonsumKReditVerwaltung.KontaktdatenLaden(model.ID_Kunde);
            if (daten != null)
            {
                model.Mail = daten.email;
                model.TelefonNummer = daten.Tel;
                model.Strasse = daten.Strasse;
                model.Hausnummer = daten.Hausnummer;
                model.Stiege = daten.Stiege;
                model.Etage = daten.Etage;
                model.Tuer = daten.Türnummer;
                model.ID_PLZ = daten.FKOrt;
            }


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KontaktDaten(KontaktDatenModel model)
        {
            Debug.WriteLine("POST - KonsumKredit - Kontaktdaten");

            if (ModelState.IsValid)
            {
                /// speichere Daten über BusinessLogic
                if (KonsumKReditVerwaltung.KontaktdatenSpeichern(
                                                model.Strasse,
                                                model.Hausnummer,
                                                model.Stiege,
                                                model.Etage,
                                                model.Tuer,
                                                model.Mail,
                                                model.TelefonNummer,
                                                model.ID_PLZ,
                                                model.ID_Kunde))
                {
                    if (!HomeController.alleDatenAngeben)
                    {
                        return RedirectToAction("Arbeitgeber");
                    }
                    else
                    {
                        return RedirectToAction("Zusammenfassung");
                    }
                    
                }
            }

            return View();
        }

        [HttpGet]
        public ActionResult Zusammenfassung()
        {
            Debug.WriteLine("GET - KonsumKredit - Zusammenfassung");

            HomeController.alleDatenAngeben = true;

            /// ermittle für diese Kunden_ID
            /// alle gespeicherten Daten (ACHTUNG! das sind viele ....)
            /// gib Sie alle in das ZusammenfassungsModel (bzw. die UNTER-Modelle) 
            /// hinein.
            ZusammenfassungModel model = new ZusammenfassungModel();
            model.ID_Kunde = int.Parse(Request.Cookies["idKunde"].Value);

            /// lädt ALLE Daten zu diesem Kunden (also auch die angehängten/referenzierten
            /// Entities) aus der DB
            tblPersoenlicheDaten aktKunde = KonsumKReditVerwaltung.KundeLaden(model.ID_Kunde);

            model.GewünschterBetrag = (int)aktKunde.tblKreditdaten.GesamtBetrag;
            model.Laufzeit = aktKunde.tblKreditdaten.Laufzeit;

            model.NettoEinkommen = (double)aktKunde.tblFinanzielleSituation.NettoEinkommenJährlich;
            model.Wohnkosten = (double)aktKunde.tblFinanzielleSituation.WohnkostenMonatlich.Value;
            model.EinkünfteAlimenteUnterhalt = (double)aktKunde.tblFinanzielleSituation.EinkuenfteAlimente.Value;
            model.UnterhaltsZahlungen = (double)aktKunde.tblFinanzielleSituation.Unterhaltszahlungen.Value;
            model.RatenVerpflichtungen = (double)aktKunde.tblFinanzielleSituation.BestehendeRatenVerpflichtungen.Value;

            model.Geschlecht = aktKunde.FKGeschlecht == 1 ? "Herr" : "Frau";
            model.Vorname = aktKunde.Vorname;
            model.Nachname = aktKunde.Nachname;
            model.Titel = aktKunde.tblTitel?.Titel;
            if(aktKunde.GeburtsDatum != null)
            model.GeburtsDatum = aktKunde.GeburtsDatum.Value.ToShortDateString();
            model.Staatsbuergerschaft = aktKunde.tblLand?.Land;
            if (aktKunde.UHPKinder != null)
            {
                model.AnzahlUnterhaltspflichtigeKinder = (int)aktKunde.UHPKinder;
            }
            else
            {
                model.AnzahlUnterhaltspflichtigeKinder = 0;
            }
            model.Familienstand = aktKunde.tblFamilienstand?.Familienstand;
            model.Wohnart = aktKunde.tblWohnart?.Wohnart;
            model.Bildung = aktKunde.tblAbschluss?.Abschluss;
            model.Identifikationsart = aktKunde.tblIdentifikationsArt?.IdentitfikationsArt;
            model.IdentifikationsNummer = aktKunde.Identifikationsnummer;

            model.FirmenName = aktKunde.tblArbeitgeber?.Firma;
            model.BeschäftigungsArt = aktKunde.tblArbeitgeber?.tblBeschaeftigungsArt?.Beschaeftigungsart;
            model.Branche = aktKunde.tblArbeitgeber?.tblBranche?.Branche;
            model.BeschäftigtSeit = aktKunde.tblArbeitgeber?.BeschaeftigtSeit.ToString("MM.yyyy");

            model.Strasse = aktKunde.tblKontaktdaten?.Strasse;
            model.Hausnummer = aktKunde.tblKontaktdaten?.Hausnummer;
            model.Stiege = aktKunde.tblKontaktdaten?.Stiege;
            model.Etage = aktKunde.tblKontaktdaten?.Etage;
            model.Türnummer = aktKunde.tblKontaktdaten?.Türnummer;
            model.Ort = aktKunde.tblKontaktdaten?.tblOrt?.Ort;
            model.PLZ = aktKunde.tblKontaktdaten?.tblOrt?.PLZ;
            model.Mail = aktKunde.tblKontaktdaten?.email;
            model.TelefonNummer = aktKunde.tblKontaktdaten?.Tel;

            model.NeuesKonto = (bool)aktKunde.tblKontoDaten?.NeuesKonto.Value;
            model.BankName = aktKunde.tblKontoDaten?.BankName;
            model.IBAN = aktKunde.tblKontoDaten?.IBAN;
            model.BIC = aktKunde.tblKontoDaten?.BIC;
           

            /// gib model an die View
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Bestätigung(int id, bool? bestätigt)
        {
            if (bestätigt.HasValue && bestätigt.Value)
            {
                Debug.WriteLine("POST - KonsumKredit - Bestätigung");
                Debug.Indent();


                //int idKunde = int.Parse(Request.Cookies["idKunde"].Value);
                tblPersoenlicheDaten aktKunde = KonsumKReditVerwaltung.KundeLaden(id);
                Response.Cookies.Remove("idKunde");

                bool istFreigegeben = KreditFreigabe.FreigabeErteilt(
                                                          aktKunde.tblGeschlecht.GeschlechtShort,
                                                            aktKunde.Vorname,
                                                            aktKunde.Nachname,
                                                            aktKunde.tblFamilienstand.Familienstand,
                                                            (double)aktKunde.tblFinanzielleSituation.NettoEinkommenJährlich,
                                                            (double)aktKunde.tblFinanzielleSituation.WohnkostenMonatlich,
                                                            (double)aktKunde.tblFinanzielleSituation.EinkuenfteAlimente,
                                                            (double)aktKunde.tblFinanzielleSituation.Unterhaltszahlungen,
                                                            (double)aktKunde.tblFinanzielleSituation.BestehendeRatenVerpflichtungen);

                /// Rüfe Service/DLL auf und prüfe auf Kreditfreigabe
                Debug.WriteLine($"Kreditfreigabe {(istFreigegeben ? "" : "nicht")}erteilt!");

                Debug.Unindent();
                return RedirectToAction("Index", "Freigabe", new { erfolgreich = istFreigegeben });

            }
            else
            {
                return RedirectToAction("Zusammenfassung");
            }
        }
    }
}