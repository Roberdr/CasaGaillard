/* Vincula las tareas de mantenimiento existentes con instalaciones, equipos y planes. */

IF OBJECT_ID(N'dbo.TareasMantenimiento', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'dbo.TareasMantenimiento', N'InstalacionId') IS NULL
        ALTER TABLE dbo.TareasMantenimiento ADD InstalacionId INT NULL;

    IF COL_LENGTH(N'dbo.TareasMantenimiento', N'EquipoId') IS NULL
        ALTER TABLE dbo.TareasMantenimiento ADD EquipoId INT NULL;

    IF COL_LENGTH(N'dbo.TareasMantenimiento', N'PlanMantenimientoId') IS NULL
        ALTER TABLE dbo.TareasMantenimiento ADD PlanMantenimientoId INT NULL;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_TareasMantenimiento_PlanMantenimientoId' AND object_id = OBJECT_ID(N'dbo.TareasMantenimiento'))
        CREATE INDEX IX_TareasMantenimiento_PlanMantenimientoId ON dbo.TareasMantenimiento(PlanMantenimientoId);

    IF OBJECT_ID(N'gaillard.PlanMantenimiento', N'U') IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_TareasMantenimiento_PlanMantenimiento' AND parent_object_id = OBJECT_ID(N'dbo.TareasMantenimiento'))
        EXEC(N'ALTER TABLE dbo.TareasMantenimiento ADD CONSTRAINT FK_TareasMantenimiento_PlanMantenimiento FOREIGN KEY (PlanMantenimientoId) REFERENCES gaillard.PlanMantenimiento(PlanId)');

    IF OBJECT_ID(N'gaillard.Instalacion', N'U') IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_TareasMantenimiento_Instalacion' AND parent_object_id = OBJECT_ID(N'dbo.TareasMantenimiento'))
        EXEC(N'ALTER TABLE dbo.TareasMantenimiento ADD CONSTRAINT FK_TareasMantenimiento_Instalacion FOREIGN KEY (InstalacionId) REFERENCES gaillard.Instalacion(Id)');

    IF OBJECT_ID(N'gaillard.Equipo', N'U') IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_TareasMantenimiento_Equipo' AND parent_object_id = OBJECT_ID(N'dbo.TareasMantenimiento'))
        EXEC(N'ALTER TABLE dbo.TareasMantenimiento ADD CONSTRAINT FK_TareasMantenimiento_Equipo FOREIGN KEY (EquipoId) REFERENCES gaillard.Equipo(Id)');
END;
