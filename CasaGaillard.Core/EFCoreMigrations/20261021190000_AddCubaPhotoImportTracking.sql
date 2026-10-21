SET XACT_ABORT ON;
BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.CubaFotoImportadas', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.CubaFotoImportadas (
            CubaId int NOT NULL,
            NombreArchivo nvarchar(260) NOT NULL,
            ImportadaEn datetime2 NOT NULL,
            CONSTRAINT PK_CubaFotoImportadas PRIMARY KEY (CubaId, NombreArchivo)
        );
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
