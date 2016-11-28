--Look up Insert Statement dbLapProjekt

use dbLapProjekt
go

-- tblGeschlecht input
insert into tblGeschlecht
VALUES ('Männlich', 'm'),('Weiblich','w');
Go	

-- tblfamilienstand input
insert into tblFamilienstand
values('ledig'),('Verheiratet'),('Verwitwet'),('Geschieden');
go

--tblIdentifikationsart
insert into tblIdentifikationsArt
values ('Reisepass'),('Personalausweis');
go
 --tblWohnart input
 insert into tblWohnart
 values('Miete'),('Eigentum'),('Genossenschaft'),('Wohngemeinschaft');
 go

 -- tblBeschaeftigungsart input
 insert into tblBeschaeftigungsArt
 values ('Angestellter'),('Arbeiter'),('Pensionist'),('Selbstständig'),('Arbeitslos');
 go

 -- tblbranchen input
 insert into tblBranche
 values ('Bank und Versicherung'),('Gewerbe und Handwerk'),('Handel'),
 ('Industrie'),('Information und Consulting'),('Tourismus und Freizeitwirtschaft'),
 ('Transport und Verkehr');

 --tblAbschluss input
 insert into tblAbschluss
 values ('Pflichtschule'),('Lehre'),('Mittlere Schule'),('Matura'),
 ('Fachhochschule/Universität');
 GO

  --Land und Orte mit PLZ Input
	insert into tblLand
	select [Spalte 0],[Spalte 1]
	from LandOrt.dbo.Länderneu
	where [Spalte 0] <> ''
	go

	insert into tblOrt(FKLand, PLZ,Ort)
	select 'AUT',PLZ,Ort
	from LandOrt.dbo.tblOrt
	go




 -- Titel input
 -- Titel aufgeführt wie beim kreditrechner der BAWAG
insert into tblTitel
values ('DI'),('Dipl.Ta.'),('Dipl.Vw.'),('Dir.'),('Dkfm.'),('Dr.'),
('Ing.'),('Mag.'),('Prof.'),('Univ.-Prof.'),('Gen.Dir.'),('Gen.Manager'),
('DI(FH)'),('DDr.'),('Dipl.HTL-Ing.'),('Mag.(FH)'),('Dipl.Kffr.'),
('MBA'),('Dipl.Päd.'),('MA'),('MMag.'),('MSc'),('Dipl.Oek.'),('BA'),
('Bakk.'),('Bakk.(FH)'),('BSc'),('PhD');

-- Settings
-- Fixzins:		4.5
-- Var. Zins:	5.5
--Max.Laufzeit:	360 Monate
--Min.Laufzeit:  20 Monate
--Max.Kredit:	300.000 €
--Min.Kredit:	    500 €
insert into tblSettings
values(4.5,5.2,360,20,300000,500);
go
