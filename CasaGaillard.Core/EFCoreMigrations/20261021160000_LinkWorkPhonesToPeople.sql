SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'gaillard.Telefono', N'U') IS NULL
        THROW 51000, 'No se encontró la tabla gaillard.Telefono.', 1;

    IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'gaillard.Telefono') AND name=N'CK_Telefono_UnSoloTitular')
        ALTER TABLE gaillard.Telefono DROP CONSTRAINT CK_Telefono_UnSoloTitular;

    IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'gaillard.Telefono') AND name=N'CK_Telefono_AlMenosUnTitular')
        ALTER TABLE gaillard.Telefono ADD CONSTRAINT CK_Telefono_AlMenosUnTitular
            CHECK (PersonaID IS NOT NULL OR EntidadID IS NOT NULL);

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
