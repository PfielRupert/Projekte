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
    
    public partial class tblArbeitgeber
    {
        public int ID_Arbeitgeber { get; set; }
        public string Firma { get; set; }
        public int FKBeschaeftigungsArt { get; set; }
        public int FKBranche { get; set; }
        public DateTime BeschaeftigtSeit { get; set; }
    
        public virtual tblBeschaeftigungsArt tblBeschaeftigungsArt { get; set; }
        public virtual tblBranche tblBranche { get; set; }
        public virtual tblPersoenlicheDaten tblPersoenlicheDaten { get; set; }
    }
}
