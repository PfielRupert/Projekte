﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class dbLapProjektEntities : DbContext
    {
        public dbLapProjektEntities()
            : base("name=dbLapProjektEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<tblPersoenlicheDaten> tblPersoenlicheDaten { get; set; }
        public DbSet<tblAbschluss> tblAbschluss { get; set; }
        public DbSet<tblArbeitgeber> tblArbeitgeber { get; set; }
        public DbSet<tblBeschaeftigungsArt> tblBeschaeftigungsArt { get; set; }
        public DbSet<tblBranche> tblBranche { get; set; }
        public DbSet<tblFamilienstand> tblFamilienstand { get; set; }
        public DbSet<tblFinanzielleSituation> tblFinanzielleSituation { get; set; }
        public DbSet<tblGeschlecht> tblGeschlecht { get; set; }
        public DbSet<tblIdentifikationsArt> tblIdentifikationsArt { get; set; }
        public DbSet<tblKontaktdaten> tblKontaktdaten { get; set; }
        public DbSet<tblKontoDaten> tblKontoDaten { get; set; }
        public DbSet<tblKreditdaten> tblKreditdaten { get; set; }
        public DbSet<tblLand> tblLand { get; set; }
        public DbSet<tblOrt> tblOrt { get; set; }
        public DbSet<tblSettings> tblSettings { get; set; }
        public DbSet<tblTitel> tblTitel { get; set; }
        public DbSet<tblWohnart> tblWohnart { get; set; }
    }
}
