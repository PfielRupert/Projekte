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
    
    public partial class tblGeschlecht
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblGeschlecht()
        {
            this.tblPersoenlicheDaten = new HashSet<tblPersoenlicheDaten>();
        }
    
        public int ID_Geschlecht { get; set; }
        public string GeschlechtLong { get; set; }
        public string GeschlechtShort { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPersoenlicheDaten> tblPersoenlicheDaten { get; set; }
    }
}
