/* Registra las actuaciones ejecutadas y los repuestos consumidos en cada tarea. */

IF OBJECT_ID(N'dbo.Intervencion', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Intervencion
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Intervencion PRIMARY KEY,
        TareaMantenimientoId INT NOT NULL,
        Tecnico NVARCHAR(150) NULL,
        FechaInicio DATETIME2(0) NOT NULL,
        FechaFin DATETIME2(0) NULL,
        Descripcion NVARCHAR(2000) NOT NULL,
        HorasEmpleadas DECIMAL(10,2) NULL,
        CosteManoObra DECIMAL(12,2) NULL,
        CreadoPor NVARCHAR(100) NULL,
        FechaCreacion DATETIME2(0) NOT NULL CONSTRAINT DF_Intervencion_FechaCreacion DEFAULT (SYSDATETIME()),
        CONSTRAINT FK_Intervencion_Tarea FOREIGN KEY (TareaMantenimientoId)
            REFERENCES dbo.TareasMantenimiento(ID)
    );
    CREATE INDEX IX_Intervencion_Tarea ON dbo.Intervencion(TareaMantenimientoId, FechaInicio);
END;

IF OBJECT_ID(N'dbo.IntervencionRepuesto', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.IntervencionRepuesto
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_IntervencionRepuesto PRIMARY KEY,
        IntervencionId INT NOT NULL,
        AccesorioId INT NOT NULL,
        Cantidad DECIMAL(12,3) NOT NULL,
        CosteUnitario DECIMAL(12,2) NULL,
        Observaciones NVARCHAR(500) NULL,
        CONSTRAINT FK_IntervencionRepuesto_Intervencion FOREIGN KEY (IntervencionId)
            REFERENCES dbo.Intervencion(Id) ON DELETE CASCADE,
        CONSTRAINT FK_IntervencionRepuesto_Accesorio FOREIGN KEY (AccesorioId)
            REFERENCES gaillard.Accesorio(ID),
        CONSTRAINT CK_IntervencionRepuesto_Cantidad CHECK (Cantidad > 0),
        CONSTRAINT CK_IntervencionRepuesto_Coste CHECK (CosteUnitario IS NULL OR CosteUnitario >= 0)
    );
    CREATE INDEX IX_IntervencionRepuesto_Intervencion ON dbo.IntervencionRepuesto(IntervencionId);
END;
