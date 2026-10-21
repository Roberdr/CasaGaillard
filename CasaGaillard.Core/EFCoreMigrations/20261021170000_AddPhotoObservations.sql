SET XACT_ABORT ON;
BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.AccesorioFotos', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.AccesorioFotos', N'Observacion') IS NULL
        ALTER TABLE dbo.AccesorioFotos ADD Observacion nvarchar(500) NULL;
    IF OBJECT_ID(N'dbo.InstalacionEquipoFotos', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.InstalacionEquipoFotos', N'Observacion') IS NULL
        ALTER TABLE dbo.InstalacionEquipoFotos ADD Observacion nvarchar(500) NULL;
    IF OBJECT_ID(N'dbo.TareaMantenimientoFotos', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.TareaMantenimientoFotos', N'Observacion') IS NULL
        ALTER TABLE dbo.TareaMantenimientoFotos ADD Observacion nvarchar(500) NULL;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
