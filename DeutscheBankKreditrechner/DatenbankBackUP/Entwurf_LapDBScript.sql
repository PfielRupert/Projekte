-- LAP Projekt Entwurf
-- Copyright  Pfiel

-- ********LOOK UP TABELLEN
-- Geschlecht
create database dbLapProjekt
GO

use dbLapProjekt
GO

create table tblGeschlecht(
	ID_Geschlecht int identity primary key,
	GeschlechtLong varchar(20) not null unique,
	GeschlechtShort char(1) not null unique
	)
go

-- Titel vorstehend
create table tblTitel(
	ID_Titel int identity primary key,
	Titel varchar(50) not null unique
)


-- Familienstand
create table tblFamilienstand(
	ID_Familienstand int identity primary key,
	Familienstand varchar(50) not null unique
)
Go
-- Abschluss
create table tblAbschluss(
	ID_Abschluss int identity primary key,
	Abschluss varchar(50) not null unique
)
Go

--Identitfikationsart
create table tblIdentifikationsArt(
	ID_IdentitifaktionsArt int identity primary key,
	IdentitfikationsArt varchar(50) not null unique
)
Go

-- Wohnart
create table tblWohnart(
	ID_Wohnart int identity primary key,
	Wohnart varchar(50) not null unique
)
Go

--Land
create table tblLand(
	ID_Land char(3) primary key,
	Land varchar(75) not null unique
)
GO
--Ort , mit Fk auf Land
create table tblOrt(
	ID_Ort int identity primary key,
	Ort varchar(100) not null,
	PLZ varchar(10) not null,
	FKLand char(3) references tblLand
)
GO

--Beschäftigungsart
create table tblBeschaeftigungsArt(
	ID_BeschaeftigungsArt int identity primary key,
	Beschaeftigungsart varchar(50) not null unique
)
GO

-- Branche

create table tblBranche(
	ID_Branche int identity primary key,
	Branche varchar(50) not null unique
)
--********** Ende LookUp Tabellen

-- Haupttabellen

create table tblPersoenlicheDaten(
	ID_PersoenlicheDaten int identity primary key,
	Vorname varchar(50) not null,
	Nachname  varchar(50) not null,
	UHPKinder int,
	GeburtsDatum Date check(Geburtsdatum <= dateadd(year,-18,getdate())),
	FKGeschlecht int references tblGeschlecht not null,
	FKTitel int references tblTitel,-- Darf null sein nicht jeder hat einen Titel
	FKStaatsbuegerschaft char(3) references tblLand,
	FKFamilienstand int references tblFamilienstand,
	FKWohnart int references tblWohnart,
	FKAbschluss int references tblAbschluss,
	FkIdentifikationsArt int references tblIdentifikationsArt,
	Identifikationsnummer varchar(50) -- wird möglicherweise noch ueber die UI
												-- validiert je nach auswahl der Art
)
GO

-- Finanzielle Situation
create table tblFinanzielleSituation(
	ID_FinanzielleSituation int references tblPersoenlicheDaten primary key,
	NettoEinkommenJährlich float not null,
	WohnkostenMonatlich float,
	EinkuenfteAlimente float,
	Unterhaltszahlungen float,
	SonstigeAusgabenMonatlich float,-- noch unklar
	SonstigeEinnahmenMonatlich float,-- noch unklar
	BestehendeRatenVerpflichtungen float	
)

-- Arbeitgeber, 1:1 mit Persoenliche Daten
create table tblArbeitgeber(
	ID_Arbeitgeber int references tblPersoenlicheDaten primary key,
	Firma varchar(50) not null,
	BeschaeftigtSeit date, 
	FKBeschaeftigungsArt int references tblBeschaeftigungsArt not null,
	FKBranche int references tblBranche not null,
)
Go
-- KontaktDaten
create table tblKontaktdaten(
	ID_Kontaktdaten int references tblPersoenlicheDaten primary key,
	Strasse varchar(50) not null,
	Hausnummer varchar(10) not null,
	Stiege varchar(10),
	Etage varchar(10),
	Türnummer varchar(10),
	email varchar(50) not null,
	Tel varchar(50),
	FKOrt int references tblOrt not null,-- Auswahl in der UI nach Land Begrenzen
)
Go

-- KontoDaten

create table tblKontoDaten(
	ID_KontoDaten int references tblPersoenlicheDaten primary key,
	IBAN varchar(20),-- noch auf die genaue Länge einschränken
	BIC varchar(8),-- Noch auf die genaue Länge beschränken
	BankName varchar(50),
	NeuesKonto bit
)

-- KreditDaten
create table tblKreditdaten(
	ID_Kredit int references tblPersoenlicheDaten primary key,
	Laufzeit int not null,
	GesamtBetrag float not null,
	Kreditrate float not null,
	istBewilligt bit not null,
)
GO

-- ######## Admin Settings
create table tblSettings(
	ID_Settings int identity primary key,
	FixZins float not null,
	VariablerZins float not null,
	MaxLaufzeit int not null,
	MinLautzeit int not null,
	MaxKreditBetrag int not null,
	MinKreditBetreg int not null
	-- möglicherweise werden noch einige andere sachen wie maximale Laufzeit usw hinzukommen
)
GO


-- use master drop database dbLapProjekt