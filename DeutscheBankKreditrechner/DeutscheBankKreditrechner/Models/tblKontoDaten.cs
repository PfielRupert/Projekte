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
    
    public partial class tblKontoDaten
    {
        public int ID_KontoDaten { get; set; }
        public string IBAN { get; set; }
        public string BIC { get; set; }
        public bool HatKonto { get; set; }
    
        public virtual tblPersoenlicheDaten tblPersoenlicheDaten { get; set; }
    }
}
