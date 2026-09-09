USE master;
GO
IF DB_ID(N'rentacarsistem_php') IS NULL CREATE DATABASE rentacarsistem_php;
GO
USE rentacarsistem_php;
GO

IF OBJECT_ID('dbo.ObrisiIznajmljivanje','P') IS NOT NULL DROP PROCEDURE dbo.ObrisiIznajmljivanje;
IF OBJECT_ID('dbo.PregledIznajmljivanja','V') IS NOT NULL DROP VIEW dbo.PregledIznajmljivanja;
IF OBJECT_ID('dbo.SlobodnaVozila','V') IS NOT NULL DROP VIEW dbo.SlobodnaVozila;
IF OBJECT_ID('dbo.Aktivnosti','U') IS NOT NULL DROP TABLE dbo.Aktivnosti;
IF OBJECT_ID('dbo.StavkeIznajmljivanja','U') IS NOT NULL DROP TABLE dbo.StavkeIznajmljivanja;
IF OBJECT_ID('dbo.Iznajmljivanja','U') IS NOT NULL DROP TABLE dbo.Iznajmljivanja;
IF OBJECT_ID('dbo.Vozila','U') IS NOT NULL DROP TABLE dbo.Vozila;
IF OBJECT_ID('dbo.Klijenti','U') IS NOT NULL DROP TABLE dbo.Klijenti;
IF OBJECT_ID('dbo.Korisnici','U') IS NOT NULL DROP TABLE dbo.Korisnici;
GO

-- Nezavisna tabela korisnika sistema. Nema FK ka poslovnim tabelama.
CREATE TABLE dbo.Korisnici(
 ID INT IDENTITY(1,1) PRIMARY KEY,
 Ime NVARCHAR(50) NOT NULL,
 Prezime NVARCHAR(50) NOT NULL,
 Mejl NVARCHAR(150) NOT NULL UNIQUE,
 Lozinka NVARCHAR(255) NOT NULL,
 Uloga NVARCHAR(20) NOT NULL CHECK(Uloga IN(N'ADMIN',N'RADNIK')),
 Aktivan BIT NOT NULL DEFAULT 1,
 CONSTRAINT CK_Korisnik_Mejl CHECK(Mejl LIKE N'%_@_%._%')
);
GO

CREATE TABLE dbo.Klijenti(
 ID INT IDENTITY(1,1) PRIMARY KEY,
 Ime NVARCHAR(50) NOT NULL,
 Prezime NVARCHAR(50) NOT NULL,
 Mejl NVARCHAR(150) NOT NULL UNIQUE,
 Telefon NVARCHAR(20) NOT NULL,
 BrojDokumenta NVARCHAR(30) NOT NULL UNIQUE,
 CONSTRAINT CK_Klijent_Mejl CHECK(Mejl LIKE N'%_@_%._%')
);
GO


CREATE TABLE dbo.Vozila(
 ID INT IDENTITY(1,1) PRIMARY KEY,
 Marka NVARCHAR(50) NOT NULL,
 Model NVARCHAR(50) NOT NULL,
 Registracija NVARCHAR(20) NOT NULL UNIQUE,
 Godiste INT NOT NULL CHECK(Godiste BETWEEN 1950 AND 2100),
 Gorivo NVARCHAR(20) NOT NULL CHECK(Gorivo IN(N'Benzin',N'Dizel',N'Hibrid',N'Električni')),
 Menjac NVARCHAR(20) NOT NULL CHECK(Menjac IN(N'Manuelni',N'Automatski')),
 BrojSedista INT NOT NULL CHECK(BrojSedista BETWEEN 1 AND 60),
 Kilometraza INT NOT NULL DEFAULT 0 CHECK(Kilometraza>=0),
 Kategorija NVARCHAR(30) NOT NULL CHECK(Kategorija IN(N'Ekonomična',N'Kompakt',N'Limuzina',N'SUV',N'Karavan',N'Kombi',N'Premium')),
 CenaPoDanu DECIMAL(10,2) NOT NULL CHECK(CenaPoDanu>0),
 Status NVARCHAR(20) NOT NULL CHECK(Status IN(N'Slobodno',N'Iznajmljeno',N'Servis',N'Nedostupno'))
);
GO


CREATE TABLE dbo.Iznajmljivanja(
 ID INT IDENTITY(1,1) PRIMARY KEY,
 KlijentID INT NOT NULL,
 DatumOd DATE NOT NULL,
 DatumDo DATE NOT NULL,
 DatumVracanja DATE NULL,
 KasnjenjeDana INT NOT NULL DEFAULT 0 CHECK(KasnjenjeDana>=0),
 NapomenaVracanja NVARCHAR(250) NULL,
 Status NVARCHAR(20) NOT NULL CHECK(Status IN(N'Aktivno',N'Zavrseno')),
 UkupnaCena DECIMAL(12,2) NOT NULL DEFAULT 0 CHECK(UkupnaCena>=0),
 KreiraoKorisnik NVARCHAR(150) NULL,
 IzmenioKorisnik NVARCHAR(150) NULL,
 CONSTRAINT FK_Iznajmljivanje_Klijent FOREIGN KEY(KlijentID) REFERENCES dbo.Klijenti(ID),
 CONSTRAINT CK_Datumi CHECK(DatumDo>=DatumOd)
);
GO


CREATE TABLE dbo.StavkeIznajmljivanja(
 ID INT IDENTITY(1,1) PRIMARY KEY,
 IznajmljivanjeID INT NOT NULL,
 VoziloID INT NOT NULL,
 BrojDana INT NOT NULL CHECK(BrojDana>0),
 CenaPoDanu DECIMAL(10,2) NOT NULL CHECK(CenaPoDanu>0),
 Iznos DECIMAL(12,2) NOT NULL CHECK(Iznos>=0),
 CONSTRAINT FK_Stavka_Iznajmljivanje FOREIGN KEY(IznajmljivanjeID) REFERENCES dbo.Iznajmljivanja(ID) ON DELETE CASCADE,
 CONSTRAINT FK_Stavka_Vozilo FOREIGN KEY(VoziloID) REFERENCES dbo.Vozila(ID),
 CONSTRAINT UQ_Stavka UNIQUE(IznajmljivanjeID,VoziloID)
);
GO


CREATE TABLE dbo.Aktivnosti(
 ID INT IDENTITY(1,1) PRIMARY KEY,
 KorisnikMejl NVARCHAR(150) NOT NULL,
 Akcija NVARCHAR(80) NOT NULL,
 Entitet NVARCHAR(50) NOT NULL,
 EntitetID INT NULL,
 Opis NVARCHAR(300) NULL,
 DatumVreme DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);
GO

CREATE VIEW dbo.SlobodnaVozila AS
SELECT ID,Marka,Model,Registracija,Godiste,Gorivo,Menjac,BrojSedista,Kilometraza,Kategorija,CenaPoDanu,Status
FROM dbo.Vozila WHERE Status=N'Slobodno';
GO

CREATE VIEW dbo.PregledIznajmljivanja AS
SELECT i.ID,k.ID AS KlijentID,k.Ime,k.Prezime,k.Mejl,k.Telefon,k.BrojDokumenta,
       i.DatumOd,i.DatumDo,i.DatumVracanja,i.KasnjenjeDana,i.NapomenaVracanja,i.Status,i.UkupnaCena,
       i.KreiraoKorisnik,i.IzmenioKorisnik,COUNT(s.ID) AS BrojStavki
FROM dbo.Iznajmljivanja i
INNER JOIN dbo.Klijenti k ON k.ID=i.KlijentID
LEFT JOIN dbo.StavkeIznajmljivanja s ON s.IznajmljivanjeID=i.ID
GROUP BY i.ID,k.ID,k.Ime,k.Prezime,k.Mejl,k.Telefon,k.BrojDokumenta,i.DatumOd,i.DatumDo,
         i.DatumVracanja,i.KasnjenjeDana,i.NapomenaVracanja,i.Status,i.UkupnaCena,i.KreiraoKorisnik,i.IzmenioKorisnik;
GO

CREATE PROCEDURE dbo.ObrisiIznajmljivanje @ID INT AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON; BEGIN TRAN;
 UPDATE v SET Status=N'Slobodno'
 FROM dbo.Vozila v INNER JOIN dbo.StavkeIznajmljivanja s ON s.VoziloID=v.ID
 WHERE s.IznajmljivanjeID=@ID AND v.Status=N'Iznajmljeno';
 DELETE FROM dbo.StavkeIznajmljivanja WHERE IznajmljivanjeID=@ID;
 DELETE FROM dbo.Iznajmljivanja WHERE ID=@ID;
 COMMIT;
END;
GO

-- admin123 / radnik123
INSERT INTO dbo.Korisnici(Ime,Prezime,Mejl,Lozinka,Uloga,Aktivan) VALUES
(N'Admin',N'Administrator',N'admin@rentacar.com',N'$2y$12$NHIFxRyIit.fFGA/qR/yLu2EtvAxUen6tQXtvXDzDM3a1BaxFBhCK',N'ADMIN',1),
(N'Radnik',N'Rentacar',N'radnik@rentacar.com',N'$2y$12$pCp8pmNPdWUD5z7IsCc0qe6ZgwykFUSuXti49kYEF8MUvWUAFUbeS',N'RADNIK',1);

INSERT INTO dbo.Klijenti(Ime,Prezime,Mejl,Telefon,BrojDokumenta) VALUES
(N'Marko',N'Marković',N'marko@example.com',N'064/111-222',N'LK100001'),
(N'Jelena',N'Jovanović',N'jelena@example.com',N'063/333-444',N'LK100002');

INSERT INTO dbo.Vozila(Marka,Model,Registracija,Godiste,Gorivo,Menjac,BrojSedista,Kilometraza,Kategorija,CenaPoDanu,Status) VALUES
(N'Audi',N'A4',N'BG-123-AA',2020,N'Dizel',N'Automatski',5,98000,N'Limuzina',4500,N'Slobodno'),
(N'Volkswagen',N'Passat',N'BG-456-BB',2019,N'Dizel',N'Manuelni',5,125000,N'Karavan',4000,N'Slobodno'),
(N'Mercedes',N'C220',N'NS-789-CC',2021,N'Dizel',N'Automatski',5,76000,N'Premium',5500,N'Slobodno');
GO

USE rentacarsistem_php;
GO



IF COL_LENGTH('dbo.Korisnici','Uloga') IS NULL
BEGIN
    ALTER TABLE dbo.Korisnici ADD Uloga NVARCHAR(20) NOT NULL CONSTRAINT DF_Korisnici_Uloga DEFAULT N'RADNIK' WITH VALUES;
END;
GO
IF COL_LENGTH('dbo.Korisnici','Aktivan') IS NULL
BEGIN
    ALTER TABLE dbo.Korisnici ADD Aktivan BIT NOT NULL CONSTRAINT DF_Korisnici_Aktivan DEFAULT 1 WITH VALUES;
END;
GO
UPDATE dbo.Korisnici SET Uloga=N'ADMIN', Aktivan=1 WHERE Mejl=N'admin@rentacar.com';
GO
IF NOT EXISTS(SELECT 1 FROM dbo.Korisnici WHERE Mejl=N'radnik@rentacar.com')
BEGIN
    INSERT INTO dbo.Korisnici(Ime,Prezime,Mejl,Lozinka,Uloga,Aktivan)
    VALUES(N'Radnik',N'Rentacar',N'radnik@rentacar.com',N'$2y$12$pCp8pmNPdWUD5z7IsCc0qe6ZgwykFUSuXti49kYEF8MUvWUAFUbeS',N'RADNIK',1);
END;
GO

/* Vozila */
IF COL_LENGTH('dbo.Vozila','Godiste') IS NULL
    ALTER TABLE dbo.Vozila ADD Godiste INT NOT NULL CONSTRAINT DF_Vozila_Godiste DEFAULT 2020 WITH VALUES;
GO
IF COL_LENGTH('dbo.Vozila','Gorivo') IS NULL
    ALTER TABLE dbo.Vozila ADD Gorivo NVARCHAR(20) NOT NULL CONSTRAINT DF_Vozila_Gorivo DEFAULT N'Dizel' WITH VALUES;
GO
IF COL_LENGTH('dbo.Vozila','Menjac') IS NULL
    ALTER TABLE dbo.Vozila ADD Menjac NVARCHAR(20) NOT NULL CONSTRAINT DF_Vozila_Menjac DEFAULT N'Manuelni' WITH VALUES;
GO
IF COL_LENGTH('dbo.Vozila','BrojSedista') IS NULL
    ALTER TABLE dbo.Vozila ADD BrojSedista INT NOT NULL CONSTRAINT DF_Vozila_BrojSedista DEFAULT 5 WITH VALUES;
GO
IF COL_LENGTH('dbo.Vozila','Kilometraza') IS NULL
    ALTER TABLE dbo.Vozila ADD Kilometraza INT NOT NULL CONSTRAINT DF_Vozila_Kilometraza DEFAULT 0 WITH VALUES;
GO
IF COL_LENGTH('dbo.Vozila','Kategorija') IS NULL
    ALTER TABLE dbo.Vozila ADD Kategorija NVARCHAR(30) NOT NULL CONSTRAINT DF_Vozila_Kategorija DEFAULT N'Limuzina' WITH VALUES;
GO

/* Iznajmljivanja */
IF COL_LENGTH('dbo.Iznajmljivanja','DatumVracanja') IS NULL
    ALTER TABLE dbo.Iznajmljivanja ADD DatumVracanja DATE NULL;
GO
IF COL_LENGTH('dbo.Iznajmljivanja','KasnjenjeDana') IS NULL
    ALTER TABLE dbo.Iznajmljivanja ADD KasnjenjeDana INT NOT NULL CONSTRAINT DF_Iznajmljivanja_Kasnjenje DEFAULT 0 WITH VALUES;
GO
IF COL_LENGTH('dbo.Iznajmljivanja','NapomenaVracanja') IS NULL
    ALTER TABLE dbo.Iznajmljivanja ADD NapomenaVracanja NVARCHAR(250) NULL;
GO
IF COL_LENGTH('dbo.Iznajmljivanja','KreiraoKorisnik') IS NULL
    ALTER TABLE dbo.Iznajmljivanja ADD KreiraoKorisnik NVARCHAR(150) NULL;
GO
IF COL_LENGTH('dbo.Iznajmljivanja','IzmenioKorisnik') IS NULL
    ALTER TABLE dbo.Iznajmljivanja ADD IzmenioKorisnik NVARCHAR(150) NULL;
GO

/* Evidencija aktivnosti */
IF OBJECT_ID('dbo.Aktivnosti','U') IS NULL
BEGIN
    CREATE TABLE dbo.Aktivnosti(
        ID INT IDENTITY(1,1) PRIMARY KEY,
        KorisnikMejl NVARCHAR(150) NOT NULL,
        Akcija NVARCHAR(80) NOT NULL,
        Entitet NVARCHAR(50) NOT NULL,
        EntitetID INT NULL,
        Opis NVARCHAR(300) NULL,
        DatumVreme DATETIME2 NOT NULL DEFAULT SYSDATETIME()
    );
END;
GO


CREATE OR ALTER VIEW dbo.SlobodnaVozila AS
SELECT ID,Marka,Model,Registracija,Godiste,Gorivo,Menjac,BrojSedista,Kilometraza,Kategorija,CenaPoDanu,Status
FROM dbo.Vozila
WHERE Status=N'Slobodno';
GO

CREATE OR ALTER VIEW dbo.PregledIznajmljivanja AS
SELECT i.ID,k.ID AS KlijentID,k.Ime,k.Prezime,k.Mejl,k.Telefon,k.BrojDokumenta,
       i.DatumOd,i.DatumDo,i.DatumVracanja,i.KasnjenjeDana,i.NapomenaVracanja,
       i.Status,i.UkupnaCena,i.KreiraoKorisnik,i.IzmenioKorisnik,
       COUNT(s.ID) AS BrojStavki
FROM dbo.Iznajmljivanja i
INNER JOIN dbo.Klijenti k ON k.ID=i.KlijentID
LEFT JOIN dbo.StavkeIznajmljivanja s ON s.IznajmljivanjeID=i.ID
GROUP BY i.ID,k.ID,k.Ime,k.Prezime,k.Mejl,k.Telefon,k.BrojDokumenta,
         i.DatumOd,i.DatumDo,i.DatumVracanja,i.KasnjenjeDana,i.NapomenaVracanja,
         i.Status,i.UkupnaCena,i.KreiraoKorisnik,i.IzmenioKorisnik;
GO

