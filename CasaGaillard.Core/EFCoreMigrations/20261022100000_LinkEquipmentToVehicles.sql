SET XACT_ABORT ON;
BEGIN TRY
    BEGIN TRANSACTION;

    IF COL_LENGTH(N'gaillard.Equipo', N'VehiculoId') IS NULL
        ALTER TABLE gaillard.Equipo ADD VehiculoId int NULL;

    IF EXISTS (
        SELECT 1 FROM sys.columns
        WHERE object_id = OBJECT_ID(N'gaillard.Equipo') AND name = N'InstalacionId' AND is_nullable = 0
    )
        ALTER TABLE gaillard.Equipo ALTER COLUMN InstalacionId int NULL;

    IF NOT EXISTS (
        SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Equipo_Vehiculo'
          AND parent_object_id = OBJECT_ID(N'gaillard.Equipo')
    )
        EXEC(N'ALTER TABLE gaillard.Equipo ADD CONSTRAINT FK_Equipo_Vehiculo
            FOREIGN KEY (VehiculoId) REFERENCES gaillard.Vehiculo(ID);');

    IF NOT EXISTS (
        SELECT 1 FROM sys.check_constraints WHERE name = N'CK_Equipo_Ubicacion'
          AND parent_object_id = OBJECT_ID(N'gaillard.Equipo')
    )
        EXEC(N'ALTER TABLE gaillard.Equipo ADD CONSTRAINT CK_Equipo_Ubicacion
            CHECK ((InstalacionId IS NOT NULL AND VehiculoId IS NULL) OR
                   (InstalacionId IS NULL AND VehiculoId IS NOT NULL));');

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
