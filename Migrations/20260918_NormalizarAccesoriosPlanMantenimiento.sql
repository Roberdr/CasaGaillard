IF OBJECT_ID('dbo.TareaMantenimientoAccesorios', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.TareaMantenimientoAccesorios
    (
        ID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_TareaMantenimientoAccesorios PRIMARY KEY,
        TareaMantenimientoID INT NOT NULL,
        AccesorioID INT NOT NULL,
        Cantidad INT NOT NULL CONSTRAINT DF_TareaMantenimientoAccesorios_Cantidad DEFAULT(1),
        Observaciones NVARCHAR(500) NULL,
        CONSTRAINT FK_TareaMantenimientoAccesorios_TareasMantenimiento
            FOREIGN KEY (TareaMantenimientoID) REFERENCES dbo.TareasMantenimiento(ID) ON DELETE CASCADE
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_TareaMantenimientoAccesorios_TareaMantenimientoID' AND object_id = OBJECT_ID('dbo.TareaMantenimientoAccesorios'))
BEGIN
    CREATE INDEX IX_TareaMantenimientoAccesorios_TareaMantenimientoID
        ON dbo.TareaMantenimientoAccesorios(TareaMantenimientoID);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_TareaMantenimientoAccesorios_AccesorioID' AND object_id = OBJECT_ID('dbo.TareaMantenimientoAccesorios'))
BEGIN
    CREATE INDEX IX_TareaMantenimientoAccesorios_AccesorioID
        ON dbo.TareaMantenimientoAccesorios(AccesorioID);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_TareaMantenimientoAccesorios_Tarea_Accesorio' AND object_id = OBJECT_ID('dbo.TareaMantenimientoAccesorios'))
BEGIN
    CREATE UNIQUE INDEX UX_TareaMantenimientoAccesorios_Tarea_Accesorio
        ON dbo.TareaMantenimientoAccesorios(TareaMantenimientoID, AccesorioID);
END
GO

IF OBJECT_ID('gaillard.Accesorio', 'U') IS NOT NULL
    AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_TareaMantenimientoAccesorios_Accesorio')
BEGIN
    ALTER TABLE dbo.TareaMantenimientoAccesorios
    ADD CONSTRAINT FK_TareaMantenimientoAccesorios_Accesorio
        FOREIGN KEY (AccesorioID) REFERENCES gaillard.Accesorio(ID);
END
GO

IF OBJECT_ID('gaillard.PlanMantenimiento', 'U') IS NOT NULL
    AND COL_LENGTH('gaillard.PlanMantenimiento', 'AccesorioGrupoID') IS NULL
BEGIN
    ALTER TABLE gaillard.PlanMantenimiento
    ADD AccesorioGrupoID INT NULL;
END
GO

IF OBJECT_ID('gaillard.PlanMantenimiento', 'U') IS NOT NULL
    AND COL_LENGTH('gaillard.PlanMantenimiento', 'AccesorioGrupoID') IS NOT NULL
    AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PlanMantenimiento_AccesorioGrupoID' AND object_id = OBJECT_ID('gaillard.PlanMantenimiento'))
BEGIN
    CREATE INDEX IX_PlanMantenimiento_AccesorioGrupoID
        ON gaillard.PlanMantenimiento(AccesorioGrupoID);
END
GO

IF OBJECT_ID('gaillard.PlanMantenimiento', 'U') IS NOT NULL
    AND OBJECT_ID('gaillard.AccesorioGrupo', 'U') IS NOT NULL
    AND COL_LENGTH('gaillard.PlanMantenimiento', 'AccesorioGrupoID') IS NOT NULL
    AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_PlanMantenimiento_AccesorioGrupo')
BEGIN
    ALTER TABLE gaillard.PlanMantenimiento
    ADD CONSTRAINT FK_PlanMantenimiento_AccesorioGrupo
        FOREIGN KEY (AccesorioGrupoID) REFERENCES gaillard.AccesorioGrupo(ID);
END
GO

IF OBJECT_ID('gaillard.AccesorioGrupo', 'U') IS NOT NULL
    AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AccesorioGrupo_GrupoID_AccesorioID' AND object_id = OBJECT_ID('gaillard.AccesorioGrupo'))
BEGIN
    CREATE INDEX IX_AccesorioGrupo_GrupoID_AccesorioID
        ON gaillard.AccesorioGrupo(GrupoID, AccesorioID);
END
GO

;WITH AccesoriosLegacy AS
(
    SELECT
        t.ID AS TareaMantenimientoID,
        TRY_CONVERT(INT, LTRIM(RTRIM(n.value('.', 'nvarchar(20)')))) AS AccesorioID
    FROM
    (
        SELECT ID, AccesoriosNecesarios
        FROM dbo.TareasMantenimiento
        WHERE NULLIF(LTRIM(RTRIM(AccesoriosNecesarios)), '') IS NOT NULL
    ) t
    CROSS APPLY
    (
        SELECT TRY_CAST('<i>' + REPLACE(t.AccesoriosNecesarios, ',', '</i><i>') + '</i>' AS XML) AS XmlAccesorios
    ) x
    CROSS APPLY x.XmlAccesorios.nodes('/i') s(n)
)
INSERT INTO dbo.TareaMantenimientoAccesorios (TareaMantenimientoID, AccesorioID, Cantidad)
SELECT DISTINCT l.TareaMantenimientoID, l.AccesorioID, 1
FROM AccesoriosLegacy l
WHERE l.AccesorioID IS NOT NULL
    AND
    (
        OBJECT_ID('gaillard.Accesorio', 'U') IS NULL
        OR EXISTS
        (
            SELECT 1
            FROM gaillard.Accesorio a
            WHERE a.ID = l.AccesorioID
        )
    )
    AND NOT EXISTS
    (
        SELECT 1
        FROM dbo.TareaMantenimientoAccesorios a
        WHERE a.TareaMantenimientoID = l.TareaMantenimientoID
            AND a.AccesorioID = l.AccesorioID
    );
GO
