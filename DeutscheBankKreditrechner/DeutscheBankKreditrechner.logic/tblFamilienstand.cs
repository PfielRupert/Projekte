//------------------------------------------------------------------------------
// <auto-generated>
//    Dieser Code wurde aus einer Vorlage generiert.
//
//    Manuelle Änderungen an dieser Datei führen möglicherweise zu unerwartetem Verhalten Ihrer Anwendung.
//    Manuelle Änderungen an dieser Datei werden überschrieben, wenn der Code neu generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DeutscheBankKreditrechner.logic
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblFamilienstand
    {
        public tblFamilienstand()
        {
            this.tblPersoenlicheDaten = new HashSet<tblPersoenlicheDaten>();
        }
    
        public int ID_Familienstand { get; set; }
        public string Familienstand { get; set; }
    
        public virtual ICollection<tblPersoenlicheDaten> tblPersoenlicheDaten { get; set; }
    }
}
