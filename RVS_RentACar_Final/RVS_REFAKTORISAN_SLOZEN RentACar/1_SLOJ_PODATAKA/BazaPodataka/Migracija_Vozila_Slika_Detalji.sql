USE RentACar;
GO

IF COL_LENGTH('dbo.Vozila', 'Godiste') IS NULL ALTER TABLE dbo.Vozila ADD Godiste INT NULL;
IF COL_LENGTH('dbo.Vozila', 'Gorivo') IS NULL ALTER TABLE dbo.Vozila ADD Gorivo NVARCHAR(30) NULL;
IF COL_LENGTH('dbo.Vozila', 'Menjac') IS NULL ALTER TABLE dbo.Vozila ADD Menjac NVARCHAR(30) NULL;
IF COL_LENGTH('dbo.Vozila', 'Kilometraza') IS NULL ALTER TABLE dbo.Vozila ADD Kilometraza INT NULL;
IF COL_LENGTH('dbo.Vozila', 'Boja') IS NULL ALTER TABLE dbo.Vozila ADD Boja NVARCHAR(30) NULL;
IF COL_LENGTH('dbo.Vozila', 'BrojSedista') IS NULL ALTER TABLE dbo.Vozila ADD BrojSedista INT NULL;
IF COL_LENGTH('dbo.Vozila', 'ZapreminaMotora') IS NULL ALTER TABLE dbo.Vozila ADD ZapreminaMotora DECIMAL(6,2) NULL;
IF COL_LENGTH('dbo.Vozila', 'SnagaMotora') IS NULL ALTER TABLE dbo.Vozila ADD SnagaMotora INT NULL;
IF COL_LENGTH('dbo.Vozila', 'SlikaPutanja') IS NULL ALTER TABLE dbo.Vozila ADD SlikaPutanja NVARCHAR(300) NULL;
GO

CREATE OR ALTER PROCEDURE spDajSvaVozila AS
BEGIN SET NOCOUNT ON;
SELECT Id,Marka,Model,Registracija,CenaPoDanu,StatusVozila,Godiste,Gorivo,Menjac,Kilometraza,Boja,BrojSedista,ZapreminaMotora,SnagaMotora,SlikaPutanja FROM Vozila ORDER BY Marka,Model;
END;
GO
CREATE OR ALTER PROCEDURE spDajVoziloPoId @Id INT AS
BEGIN SET NOCOUNT ON;
SELECT Id,Marka,Model,Registracija,CenaPoDanu,StatusVozila,Godiste,Gorivo,Menjac,Kilometraza,Boja,BrojSedista,ZapreminaMotora,SnagaMotora,SlikaPutanja FROM Vozila WHERE Id=@Id;
END;
GO
CREATE OR ALTER PROCEDURE spDajVozilaPoRegistraciji @Registracija NVARCHAR(20) AS
BEGIN SET NOCOUNT ON;
SELECT Id,Marka,Model,Registracija,CenaPoDanu,StatusVozila,Godiste,Gorivo,Menjac,Kilometraza,Boja,BrojSedista,ZapreminaMotora,SnagaMotora,SlikaPutanja FROM Vozila WHERE Registracija LIKE N'%'+@Registracija+N'%' ORDER BY Registracija;
END;
GO
CREATE OR ALTER PROCEDURE spDodajVozilo
@Marka NVARCHAR(50),@Model NVARCHAR(50),@Registracija NVARCHAR(20),@CenaPoDanu DECIMAL(10,2),@StatusVozila NVARCHAR(30),@Godiste INT=NULL,@Gorivo NVARCHAR(30)=NULL,@Menjac NVARCHAR(30)=NULL,@Kilometraza INT=NULL,@Boja NVARCHAR(30)=NULL,@BrojSedista INT=NULL,@ZapreminaMotora DECIMAL(6,2)=NULL,@SnagaMotora INT=NULL,@SlikaPutanja NVARCHAR(300)=NULL AS
BEGIN SET NOCOUNT ON;
INSERT INTO Vozila(Marka,Model,Registracija,CenaPoDanu,StatusVozila,Godiste,Gorivo,Menjac,Kilometraza,Boja,BrojSedista,ZapreminaMotora,SnagaMotora,SlikaPutanja)
VALUES(@Marka,@Model,@Registracija,@CenaPoDanu,@StatusVozila,@Godiste,@Gorivo,@Menjac,@Kilometraza,@Boja,@BrojSedista,@ZapreminaMotora,@SnagaMotora,@SlikaPutanja);
SELECT CAST(SCOPE_IDENTITY() AS INT) NoviId; END;
GO
CREATE OR ALTER PROCEDURE spIzmeniVozilo
@Id INT,@Marka NVARCHAR(50),@Model NVARCHAR(50),@Registracija NVARCHAR(20),@CenaPoDanu DECIMAL(10,2),@StatusVozila NVARCHAR(30),@Godiste INT=NULL,@Gorivo NVARCHAR(30)=NULL,@Menjac NVARCHAR(30)=NULL,@Kilometraza INT=NULL,@Boja NVARCHAR(30)=NULL,@BrojSedista INT=NULL,@ZapreminaMotora DECIMAL(6,2)=NULL,@SnagaMotora INT=NULL,@SlikaPutanja NVARCHAR(300)=NULL AS
BEGIN SET NOCOUNT ON;
UPDATE Vozila SET Marka=@Marka,Model=@Model,Registracija=@Registracija,CenaPoDanu=@CenaPoDanu,StatusVozila=@StatusVozila,Godiste=@Godiste,Gorivo=@Gorivo,Menjac=@Menjac,Kilometraza=@Kilometraza,Boja=@Boja,BrojSedista=@BrojSedista,ZapreminaMotora=@ZapreminaMotora,SnagaMotora=@SnagaMotora,SlikaPutanja=@SlikaPutanja WHERE Id=@Id; END;
GO
