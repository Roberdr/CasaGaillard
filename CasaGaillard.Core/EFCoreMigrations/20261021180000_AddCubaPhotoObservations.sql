SET XACT_ABORT ON;
BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.CubaFotoObservaciones', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.CubaFotoObservaciones (
            ID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_CubaFotoObservaciones PRIMARY KEY,
            CubaId int NOT NULL,
            NombreArchivo nvarchar(260) NOT NULL,
            Observacion nvarchar(500) NULL,
            CONSTRAINT AK_CubaFotoObservaciones_CubaId_NombreArchivo UNIQUE (CubaId, NombreArchivo)
        );
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
