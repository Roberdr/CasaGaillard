/*
    Evoluciona PlanMantenimiento para admitir planes de equipos de instalaciones.
    Las columnas de accesorios se conservan para mantener los planes existentes.
    Ejecutar después de 20260929_InstalacionesEquipos.sql.
*/

IF OBJECT_ID(N'gaillard.PlanMantenimiento', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'gaillard.PlanMantenimiento', N'EquipoId') IS NULL
        ALTER TABLE gaillard.PlanMantenimiento ADD EquipoId INT NULL;

    IF COL_LENGTH(N'gaillard.PlanMantenimiento', N'Nombre') IS NULL
        ALTER TABLE gaillard.PlanMantenimiento ADD Nombre NVARCHAR(150) NULL;

    IF COL_LENGTH(N'gaillard.PlanMantenimiento', N'Descripcion') IS NULL
        ALTER TABLE gaillard.PlanMantenimiento ADD Descripcion NVARCHAR(1000) NULL;

    IF COL_LENGTH(N'gaillard.PlanMantenimiento', N'Tipo') IS NULL
        ALTER TABLE gaillard.PlanMantenimiento ADD Tipo NVARCHAR(30) NULL;

    IF COL_LENGTH(N'gaillard.PlanMantenimiento', N'FrecuenciaValor') IS NULL
        ALTER TABLE gaillard.PlanMantenimiento ADD FrecuenciaValor INT NULL;

    IF COL_LENGTH(N'gaillard.PlanMantenimiento', N'FrecuenciaUnidad') IS NULL
        ALTER TABLE gaillard.PlanMantenimiento ADD FrecuenciaUnidad NVARCHAR(20) NULL;

    IF COL_LENGTH(N'gaillard.PlanMantenimiento', N'DuracionEstimadaMinutos') IS NULL
        ALTER TABLE gaillard.PlanMantenimiento ADD DuracionEstimadaMinutos INT NULL;

    IF COL_LENGTH(N'gaillard.PlanMantenimiento', N'Activo') IS NULL
        ALTER TABLE gaillard.PlanMantenimiento ADD Activo BIT NOT NULL
            CONSTRAINT DF_PlanMantenimiento_Activo DEFAULT (1) WITH VALUES;

    IF EXISTS
    (
        SELECT 1 FROM sys.columns
        WHERE object_id = OBJECT_ID(N'gaillard.PlanMantenimiento')
          AND name = N'AccesorioId' AND is_nullable = 0
    )
        ALTER TABLE gaillard.PlanMantenimiento ALTER COLUMN AccesorioId INT NULL;

    IF NOT EXISTS
    (
        SELECT 1 FROM sys.foreign_keys
        WHERE name = N'FK_PlanMantenimiento_Equipo'
          AND parent_object_id = OBJECT_ID(N'gaillard.PlanMantenimiento')
    )
        ALTER TABLE gaillard.PlanMantenimiento
            ADD CONSTRAINT FK_PlanMantenimiento_Equipo
            FOREIGN KEY (EquipoId) REFERENCES gaillard.Equipo(Id);

    IF NOT EXISTS
    (
        SELECT 1 FROM sys.indexes
        WHERE name = N'IX_PlanMantenimiento_EquipoId'
          AND object_id = OBJECT_ID(N'gaillard.PlanMantenimiento')
    )
        CREATE INDEX IX_PlanMantenimiento_EquipoId
            ON gaillard.PlanMantenimiento(EquipoId);

    IF NOT EXISTS
    (
        SELECT 1 FROM sys.check_constraints
        WHERE name = N'CK_PlanMantenimiento_Frecuencia'
          AND parent_object_id = OBJECT_ID(N'gaillard.PlanMantenimiento')
    )
        EXEC(N'ALTER TABLE gaillard.PlanMantenimiento
            ADD CONSTRAINT CK_PlanMantenimiento_Frecuencia
            CHECK (FrecuenciaValor IS NULL OR FrecuenciaValor > 0)');

    IF NOT EXISTS
    (
        SELECT 1 FROM sys.check_constraints
        WHERE name = N'CK_PlanMantenimiento_Duracion'
          AND parent_object_id = OBJECT_ID(N'gaillard.PlanMantenimiento')
    )
        EXEC(N'ALTER TABLE gaillard.PlanMantenimiento
            ADD CONSTRAINT CK_PlanMantenimiento_Duracion
            CHECK (DuracionEstimadaMinutos IS NULL OR DuracionEstimadaMinutos > 0)');
END;
