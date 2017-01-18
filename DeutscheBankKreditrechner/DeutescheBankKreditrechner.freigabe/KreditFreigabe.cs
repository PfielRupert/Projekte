using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeutescheBankKreditrechner.freigabe
{
    public class KreditFreigabe
    {
        /// <summary>
        /// Gibt zurück ob für die angegebenen Daten eine Kreditfreigabe erteilt wird
        /// </summary>
        /// <param name="vorname">der Vorname des Antragsteller</param>
        /// <param name="nachname">der Nachname des Antragsteller</param>
        /// <param name="monatsEinkommen">das monatliche Netto-Einkommen des Antragsteller</param>
        /// <param name="wohnKosten">die monatlichen Wohnkosten  des Antragsteller</param>
        /// <param name="einkuenfteAlimente">die monatlichen Einkünfte aus Alimente, Unterhalt des Antragsteller</param>
        /// <param name="ausgabenAlimente">die monatlichen Ausgaben für Alimente, Unterhalt des Antragsteller</param>
        /// <param name="ratenZahlungen">die monatlichen Ratenzahlungen des Antragsteller</param>
        /// <returns></returns>
        public static bool FreigabeErteilt(
            string geschlecht,
            string vorname,
            string nachname,
            string familienStand,
            double monatsEinkommen,
            double wohnKosten,
            double einkuenfteAlimente,
            double ausgabenAlimente,
            double ratenZahlungen,
            int aKundenId)
        {
            Debug.WriteLine("KreditFreigabe - FreigabeErteilt");
            Debug.Indent();
            bool freigabe = false;

            if (string.IsNullOrEmpty(vorname))
                throw new ArgumentNullException(nameof(vorname));
            if (string.IsNullOrEmpty(nachname))
                throw new ArgumentNullException(nameof(nachname));
            if (monatsEinkommen <= 0 || monatsEinkommen > 50000)
                throw new ArgumentException($"Ungültigter Wert für {nameof(monatsEinkommen)}");
            //if (wohnKosten <= 0 || wohnKosten > 10000)
            //    throw new ArgumentException($"Ungültigter Wert für {nameof(wohnKosten)}");
            //if (einkuenfteAlimente <= 0 || einkuenfteAlimente > 10000)
            //    throw new ArgumentException($"Ungültigter Wert für {nameof(einkuenfteAlimente)}");
            //if (ausgabenAlimente <= 0 || ausgabenAlimente > 10000)
            //    throw new ArgumentException($"Ungültigter Wert für {nameof(ausgabenAlimente)}");
            //if (ratenZahlungen <= 0 || ratenZahlungen > 10000)
            //    throw new ArgumentException($"Ungültigter Wert für {nameof(ratenZahlungen)}");

            double verfügbarerBetrag = monatsEinkommen + einkuenfteAlimente - wohnKosten - einkuenfteAlimente - ausgabenAlimente - ratenZahlungen;
            double verhältnisWohkostenVerfügbarerBetrag = wohnKosten / verfügbarerBetrag;

            switch (familienStand.ToLower())
            {
                case "ledig":
                case "verwitwet":
                    switch (geschlecht)
                    {
                        case "m":
                            freigabe = verfügbarerBetrag > wohnKosten * 2;
                            break;
                        case "w":
                            freigabe = verfügbarerBetrag > wohnKosten * 1.8;
                            break;
                        default:
                            throw new ArgumentException($"Ungültiger Wert für {nameof(geschlecht)}!\n\nNur 'm' oder 'w' erlaubt.");
                            break;
                    }

                    break;
                case "in Partnerschaft":
                case "verheiratet":
                case "geschieden":
                    if (verhältnisWohkostenVerfügbarerBetrag < 0.5)
                    {
                        freigabe = verfügbarerBetrag > wohnKosten * 2.5;
                    }
                    else
                    {
                        freigabe = verfügbarerBetrag > wohnKosten * 3.5;
                    }
                    break;

                default:
                    throw new ArgumentException($"Ungültiger Wert für {nameof(familienStand)}!\n\nNur 'ledig', 'verwitwet', 'in Partnerschaft', 'verheiratet' erlaubt.");
                    break;
            }
            if(freigabe)
            {
                DeutscheBankKreditrechner.logic.KonsumKReditVerwaltung.AntragBewilligt(aKundenId);
            }

            Debug.Unindent();
            return freigabe;
        }
    }
}
