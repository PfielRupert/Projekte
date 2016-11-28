
-- Funktion, die ausrechnet wie viel Geld
-- dem Kunden zur verfügung steht (Input: IDKunde, Output: Betrag)

CREATE FUNCTION dbo.GeldProKundeVerfuegbar(@KundenID int)  
RETURNS float   
AS    
BEGIN 
	declare @Ergebnis float 
    select @Ergebnis = (NettoEinkommenJährlich+EinkuenfteAlimente+SonstigeEinnahmenMonatlich)-
	(Unterhaltszahlungen+SonstigeAusgabenMonatlich
	+ BestehendeRatenVerpflichtungen + WohnkostenMonatlich)
	from tblFinanzielleSituation
	where ID_FinanzielleSituation = @KundenID

    RETURN @Ergebnis;  
END;  
GO  

-- Prozedur, die einen Kunden einträgt
USE dbLapProjekt
Go
create proc pInsertKunde
	@FirstName varchar(50),
	@LastName varchar(50),
	@UHPChilds int,
	@Birthdate date,
	@FKGeschlecht int,
	@FkTitel int,
	@FKStaatsbuergerschaft int,
	@FKFamilienstand int,
	@FKWohnart int,
	@FkAbschluss int,
	@FkIdentifitkationsart int,
	@Identifikationsnr varchar(50)
as
	insert into tblPersoenlicheDaten(Vorname,Nachname,UHPKinder,GeburtsDatum,FKGeschlecht
	,FKTitel,FKStaatsbuegerschaft,FKFamilienstand,FKWohnart,FKAbschluss,FkIdentifikationsArt,Identifikationsnummer)
	Values(@FirstName,@LastName,@UHPChilds,@Birthdate,@FKGeschlecht,@FkTitel,@FKStaatsbuergerschaft
	,@FKFamilienstand,@FKWohnart,@FkAbschluss,@FkIdentifitkationsart,@Identifikationsnr)
GO



-- Proz, die einen Kredit bewilligt oder ablehnt (update)


-- Proz, die die Kreditrate berechnet (einfach!!!)


-- Proz, die den Login überprüft 
