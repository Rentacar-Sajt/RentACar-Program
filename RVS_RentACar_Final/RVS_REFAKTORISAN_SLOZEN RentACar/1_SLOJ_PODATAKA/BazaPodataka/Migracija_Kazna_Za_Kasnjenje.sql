USE RentACar;
GO

/* =========================================================
   KAZNA ZA KASNJENJE PRI VRACANJU VOZILA
   Tolerancija: 6 sati
   Kazna: 2.000 RSD po zapocetom danu kasnjenja
   ========================================================= */

IF COL_LENGTH('dbo.Ugovori', 'StvarniDatumVracanja') IS NULL
BEGIN
    ALTER TABLE dbo.Ugovori
    ADD StvarniDatumVracanja DATETIME2 NULL;
END;
GO

IF COL_LENGTH('dbo.Ugovori', 'BrojDanaKasnjenja') IS NULL
BEGIN
    ALTER TABLE dbo.Ugovori
    ADD BrojDanaKasnjenja INT NOT NULL
        CONSTRAINT DF_Ugovori_BrojDanaKasnjenja DEFAULT 0;
END;
GO

IF COL_LENGTH('dbo.Ugovori', 'KaznaZaKasnjenje') IS NULL
BEGIN
    ALTER TABLE dbo.Ugovori
    ADD KaznaZaKasnjenje DECIMAL(12,2) NOT NULL
        CONSTRAINT DF_Ugovori_KaznaZaKasnjenje DEFAULT 0;
END;
GO

CREATE OR ALTER VIEW vwSviUgovori
AS
SELECT
    u.Id,
    u.BrojUgovora,
    u.DatumIzdavanja,
    u.DatumPreuzimanja,
    u.DatumVracanja,
    u.StvarniDatumVracanja,
    u.BrojDanaKasnjenja,
    u.KaznaZaKasnjenje,
    u.MestoPreuzimanja,
    u.MestoVracanja,
    u.NacinPlacanja,
    u.Depozit,
    u.StatusUgovora,
    u.Napomena,
    u.PopustProcenat,
    u.UkupnoZaPlacanje,
    u.IzabraneDodatneUsluge,
    u.KlijentId,
    k.Ime + N' ' + k.Prezime AS Klijent,
    u.KorisnikId,
    kor.Ime + N' ' + kor.Prezime AS Zaposleni
FROM Ugovori u
INNER JOIN Klijenti k
    ON k.Id = u.KlijentId
INNER JOIN Korisnici kor
    ON kor.Id = u.KorisnikId;
GO

CREATE OR ALTER PROCEDURE spPromeniStatusUgovora
    @UgovorId INT,
    @NoviStatus NVARCHAR(30),
    @StvarniDatumVracanja DATETIME2 = NULL,
    @BrojDanaKasnjenja INT = 0,
    @KaznaZaKasnjenje DECIMAL(12,2) = 0
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS
        (
            SELECT 1
            FROM Ugovori
            WHERE Id = @UgovorId
        )
        BEGIN
            THROW 50010, N'Ugovor nije pronađen.', 1;
        END;

        IF @NoviStatus NOT IN
        (
            N'Aktivan',
            N'Završen',
            N'Otkazan'
        )
        BEGIN
            THROW 50011, N'Neispravan status ugovora.', 1;
        END;

        IF @NoviStatus = N'Završen'
           AND @StvarniDatumVracanja IS NULL
        BEGIN
            THROW 50013,
                N'Stvarni datum vraćanja je obavezan kada se ugovor završava.',
                1;
        END;

        DECLARE @TrenutniStatus NVARCHAR(30);

        SELECT @TrenutniStatus = StatusUgovora
        FROM Ugovori
        WHERE Id = @UgovorId;

        IF @NoviStatus = N'Aktivan'
           AND @TrenutniStatus <> N'Aktivan'
           AND EXISTS
           (
               SELECT 1
               FROM StavkeUgovora su
               INNER JOIN Vozila v
                   ON v.Id = su.VoziloId
               WHERE su.UgovorId = @UgovorId
                 AND v.StatusVozila <> N'Slobodno'
           )
        BEGIN
            THROW 50012,
                N'Ugovor nije moguće aktivirati jer vozilo nije slobodno.',
                1;
        END;

        UPDATE Ugovori
        SET
            StatusUgovora = @NoviStatus,
            StvarniDatumVracanja =
                CASE
                    WHEN @NoviStatus = N'Završen'
                        THEN @StvarniDatumVracanja
                    ELSE StvarniDatumVracanja
                END,
            BrojDanaKasnjenja =
                CASE
                    WHEN @NoviStatus = N'Završen'
                        THEN ISNULL(@BrojDanaKasnjenja, 0)
                    ELSE BrojDanaKasnjenja
                END,
            KaznaZaKasnjenje =
                CASE
                    WHEN @NoviStatus = N'Završen'
                        THEN ISNULL(@KaznaZaKasnjenje, 0)
                    ELSE KaznaZaKasnjenje
                END
        WHERE Id = @UgovorId;

        IF @NoviStatus = N'Aktivan'
        BEGIN
            UPDATE v
            SET v.StatusVozila = N'Iznajmljeno'
            FROM Vozila v
            INNER JOIN StavkeUgovora su
                ON su.VoziloId = v.Id
            WHERE su.UgovorId = @UgovorId;
        END
        ELSE IF @NoviStatus IN (N'Završen', N'Otkazan')
        BEGIN
            UPDATE v
            SET v.StatusVozila = N'Slobodno'
            FROM Vozila v
            INNER JOIN StavkeUgovora su
                ON su.VoziloId = v.Id
            WHERE su.UgovorId = @UgovorId
              AND v.StatusVozila <> N'Servis';
        END;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;

        THROW;
    END CATCH;
END;
GO
