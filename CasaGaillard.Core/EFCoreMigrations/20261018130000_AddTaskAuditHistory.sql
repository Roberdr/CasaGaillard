IF OBJECT_ID(N'dbo.TareaMantenimientoHistorial', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TareaMantenimientoHistorial (
        ID bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_TareaMantenimientoHistorial PRIMARY KEY,
        TareaMantenimientoID int NOT NULL,
        Usuario nvarchar(100) NULL,
        Accion nvarchar(20) NOT NULL,
        Fecha datetime2 NOT NULL,
        CambiosJson nvarchar(max) NOT NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_TareaMantenimientoHistorial_TareaMantenimientoID_Fecha'
    AND object_id=OBJECT_ID(N'dbo.TareaMantenimientoHistorial'))
BEGIN
    CREATE INDEX IX_TareaMantenimientoHistorial_TareaMantenimientoID_Fecha
        ON dbo.TareaMantenimientoHistorial(TareaMantenimientoID, Fecha);
END;
