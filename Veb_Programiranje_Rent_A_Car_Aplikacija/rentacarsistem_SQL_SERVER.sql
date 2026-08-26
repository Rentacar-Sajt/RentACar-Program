USE master;
GO

IF DB_ID('rentacarsistem') IS NULL
BEGIN
    CREATE DATABASE rentacarsistem;
END
GO

USE rentacarsistem;
GO


IF OBJECT_ID('dbo.GetTrenutnaIznajmljivanja', 'P') IS NOT NULL DROP PROCEDURE dbo.GetTrenutnaIznajmljivanja;
IF OBJECT_ID('dbo.ObrisiVozilo', 'P') IS NOT NULL DROP PROCEDURE dbo.ObrisiVozilo;
GO

IF OBJECT_ID('dbo.DostupnaVozilaView', 'V') IS NOT NULL DROP VIEW dbo.DostupnaVozilaView;
IF OBJECT_ID('dbo.Prikaz_Iznajmljivanja', 'V') IS NOT NULL DROP VIEW dbo.Prikaz_Iznajmljivanja;
IF OBJECT_ID('dbo.TrenutnaIznajmljivanja', 'V') IS NOT NULL DROP VIEW dbo.TrenutnaIznajmljivanja;
IF OBJECT_ID('dbo.VozilaView', 'V') IS NOT NULL DROP VIEW dbo.VozilaView;
GO

IF OBJECT_ID('dbo.iznajmljivanjekola', 'U') IS NOT NULL DROP TABLE dbo.iznajmljivanjekola;
IF OBJECT_ID('dbo.kola', 'U') IS NOT NULL DROP TABLE dbo.kola;
IF OBJECT_ID('dbo.korisnici', 'U') IS NOT NULL DROP TABLE dbo.korisnici;
GO

CREATE TABLE dbo.korisnici
(
    ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Ime NVARCHAR(100) NOT NULL,
    Prezime NVARCHAR(100) NOT NULL,
    Mejl NVARCHAR(255) NOT NULL,
    BrojTelefona NVARCHAR(50) NOT NULL,
    Sifra NVARCHAR(255) NOT NULL,
    Tip INT NOT NULL
);
GO

CREATE TABLE dbo.kola
(
    ID INT NOT NULL PRIMARY KEY,
    Brend NVARCHAR(100) NOT NULL,
    Model NVARCHAR(100) NOT NULL,
    Boja NVARCHAR(100) NOT NULL,
    Godina INT NOT NULL,
    Cena FLOAT NOT NULL,
    Dostupnost INT NOT NULL
);
GO

CREATE TABLE dbo.iznajmljivanjekola
(
    ID INT NOT NULL PRIMARY KEY,
    korisnik INT NOT NULL,
    automobil INT NOT NULL,
    datum NVARCHAR(20) NULL,
    sati INT NOT NULL,
    ukupno FLOAT NOT NULL,
    status INT NOT NULL
);
GO

SET IDENTITY_INSERT dbo.korisnici ON;
INSERT INTO dbo.korisnici (ID, Ime, Prezime, Mejl, BrojTelefona, Sifra, Tip) VALUES
(1, N'Admin', N'0', N'admin@rentacarsystem.com', N'000', N'0000', 1),
(2, N'admin', N'0', N'rentcar@sistem.com', N'0', N'1234', 1),
(3, N'Mateja', N'Stojanovic', N'mateja@stojanovic.com', N'123456789', N'mateja', 1),
(4, N'Stefan', N'Jankovic', N'stefan@jankovic.com', N'0101', N'1122', 0),
(5, N'Nemanja', N'Jovanov', N'nemanja@jovanov.com', N'3333', N'3344', 1),
(6, N'Mateja', N'Stojanovic', N'mat@stoj.com', N'45678', N'mat111', 1),
(7, N'Zoran', N'Denkan', N'zoran@denkan.com', N'6453413515', N'zoran', 0),
(8, N'Dragan', N'Popovic', N'dragan@popovic.com', N'1232123133', N'1111', 0),
(9, N'Nikola', N'Milenkovic', N'niko@mile.com', N'124414141414', N'nikola', 0);
SET IDENTITY_INSERT dbo.korisnici OFF;
GO

INSERT INTO dbo.kola (ID, Brend, Model, Boja, Godina, Cena, Dostupnost) VALUES
(1, N'Audi', N'A6', N'Crna', 2021, 450, 1),
(2, N'Ford', N'Fokus', N'Plava', 2005, 30, 0),
(3, N'Mercedes', N'S600', N'Bela', 2015, 120, 1),
(4, N'Audi', N'A4', N'Bela', 2024, 420, 1),
(5, N'Audi', N'A5', N'Crna', 2018, 250, 1),
(8, N'Volksvagen', N'Arteon', N'Siva', 2020, 230, 0),
(9, N'Toyota', N'Hilux', N'Bela', 2018, 215, 1),
(10, N'Nissan', N'Skyline R34', N'Svetlo plava', 1998, 1200, 0);
GO

INSERT INTO dbo.iznajmljivanjekola (ID, korisnik, automobil, datum, sati, ukupno, status) VALUES
(1, 5, 4, N'2024-08-16', 3, 1260, 0),
(2, 5, 8, N'2024-08-17', 12, 2760, 1),
(3, 6, 5, N'2024-08-20', 12, 3000, 1),
(4, 6, 10, N'2024-08-20', 3, 3600, 1),
(5, 6, 7, N'2024-08-21', 12, 2280, 1),
(6, 6, 1, N'2024-08-21', 14, 6300, 1),
(7, 6, 2, N'2024-08-21', 13, 390, 1),
(8, 6, 4, N'2024-08-21', 4, 1680, 1),
(9, 6, 2, N'2024-08-21', 2, 60, 1),
(10, 6, 9, N'2024-08-21', 11, 2365, 1),
(11, 6, 1, N'2024-08-21', 3, 1350, 1),
(12, 6, 4, N'2024-08-21', 5, 1680, 0),
(13, 6, 5, N'2024-08-22', 4, 1000, 0),
(14, 6, 3, N'2024-08-22', 7, 840, 0),
(15, 6, 9, N'2024-08-22', 5, 1075, 0);
GO

CREATE VIEW dbo.DostupnaVozilaView
AS
SELECT ID, Brend, Model, Boja, Godina, Cena, Dostupnost
FROM dbo.kola
WHERE Dostupnost = 0;
GO

CREATE VIEW dbo.VozilaView
AS
SELECT ID, Brend, Model, Boja, Godina, Cena, Dostupnost
FROM dbo.kola;
GO

CREATE VIEW dbo.Prikaz_Iznajmljivanja
AS
SELECT
    ik.ID AS iznajmljivanjeID,
    k.ID AS korisnikID,
    k.Ime,
    k.Prezime,
    k.Mejl,
    k.BrojTelefona,
    a.ID AS automobilID,
    a.Brend,
    a.Model,
    a.Boja,
    a.Godina,
    a.Cena,
    a.Dostupnost,
    ik.datum,
    ik.sati,
    ik.ukupno,
    ik.status
FROM dbo.iznajmljivanjekola AS ik
INNER JOIN dbo.korisnici AS k ON ik.korisnik = k.ID
INNER JOIN dbo.kola AS a ON ik.automobil = a.ID;
GO

CREATE VIEW dbo.TrenutnaIznajmljivanja
AS
SELECT
    iz.ID AS IznajmljivanjeID,
    k.Ime,
    k.Mejl,
    k.BrojTelefona,
    a.ID AS VoziloID,
    CONCAT(a.Brend, N' ', a.Model, N' ', a.Boja) AS Automobil,
    iz.datum AS Datum,
    iz.sati AS Sati,
    iz.ukupno AS Ukupno,
    CASE
        WHEN iz.status = 0 THEN N'Aktivno'
        WHEN iz.status = 1 THEN N'Završeno'
        ELSE N'Nepoznato'
    END AS Status
FROM dbo.iznajmljivanjekola AS iz
INNER JOIN dbo.korisnici AS k ON iz.korisnik = k.ID
INNER JOIN dbo.kola AS a ON iz.automobil = a.ID
WHERE iz.status = 0
  AND TRY_CONVERT(date, iz.datum, 23) <= CAST(GETDATE() AS date);
GO

CREATE PROCEDURE dbo.GetTrenutnaIznajmljivanja
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.TrenutnaIznajmljivanja;
END;
GO

CREATE PROCEDURE dbo.ObrisiVozilo
    @voziloID INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.kola WHERE ID = @voziloID;

    IF @@ROWCOUNT = 0
        THROW 50001, N'Vozilo sa ovim ID-om ne postoji ili je već obrisano.', 1;
END;
GO

SELECT N'Baza rentacarsistem je uspešno kreirana za Microsoft SQL Server.' AS Poruka;
GO


SELECT name AS NazivViewa
FROM sys.views
WHERE name IN ('VozilaView', 'DostupnaVozilaView', 'Prikaz_Iznajmljivanja', 'TrenutnaIznajmljivanja')
ORDER BY name;
GO
