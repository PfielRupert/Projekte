//------------------------------------------------------------------------------
// <auto-generated>
//     Der Code wurde von einer Vorlage generiert.
//
//     Manuelle Änderungen an dieser Datei führen möglicherweise zu unerwartetem Verhalten der Anwendung.
//     Manuelle Änderungen an dieser Datei werden überschrieben, wenn der Code neu generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DeutscheBankKreditrechner.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblKontaktdaten
    {
        public int ID_Kontaktdaten { get; set; }
        public string Strasse { get; set; }
        public string Hausnummer { get; set; }
        public string Stiege { get; set; }
        public string Etage { get; set; }
        public string Türnummer { get; set; }
        public string email { get; set; }
        public string Tel { get; set; }
        public int FKOrt { get; set; }
    
        public virtual tblOrt tblOrt { get; set; }
        public virtual tblPersoenlicheDaten tblPersoenlicheDaten { get; set; }
    }
}
