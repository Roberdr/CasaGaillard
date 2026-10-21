/* Add direct asset links for cistern and vehicle maintenance plans. */
IF OBJECT_ID(N'gaillard.PlanMantenimiento', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'gaillard.PlanMantenimiento', N'CubaId') IS NULL
        ALTER TABLE gaillard.PlanMantenimiento ADD CubaId INT NULL;

    IF COL_LENGTH(N'gaillard.PlanMantenimiento', N'VehiculoId') IS NULL
        ALTER TABLE gaillard.PlanMantenimiento ADD VehiculoId INT NULL;

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_PlanMantenimiento_Cuba' AND parent_object_id=OBJECT_ID(N'gaillard.PlanMantenimiento'))
        ALTER TABLE gaillard.PlanMantenimiento ADD CONSTRAINT FK_PlanMantenimiento_Cuba FOREIGN KEY (CubaId) REFERENCES gaillard.Cuba(ID);

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_PlanMantenimiento_Vehiculo' AND parent_object_id=OBJECT_ID(N'gaillard.PlanMantenimiento'))
        ALTER TABLE gaillard.PlanMantenimiento ADD CONSTRAINT FK_PlanMantenimiento_Vehiculo FOREIGN KEY (VehiculoId) REFERENCES gaillard.Vehiculo(ID);

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_PlanMantenimiento_CubaId' AND object_id=OBJECT_ID(N'gaillard.PlanMantenimiento'))
        CREATE INDEX IX_PlanMantenimiento_CubaId ON gaillard.PlanMantenimiento(CubaId);

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_PlanMantenimiento_VehiculoId' AND object_id=OBJECT_ID(N'gaillard.PlanMantenimiento'))
        CREATE INDEX IX_PlanMantenimiento_VehiculoId ON gaillard.PlanMantenimiento(VehiculoId);
END;
