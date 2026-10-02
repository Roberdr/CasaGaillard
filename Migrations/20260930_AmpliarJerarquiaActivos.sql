/*
    Amplía los nodos de la jerarquía de equipos para representar subsistemas,
    equipos, componentes e instalaciones auxiliares dentro de cada instalación.
    Ejecutar una sola vez contra la base GAILLARD antes de desplegar la aplicación.
*/
IF OBJECT_ID(N'gaillard.Equipo', N'U') IS NULL
    THROW 50001, 'No existe gaillard.Equipo. Ejecuta primero la migración de instalaciones y equipos.', 1;

IF COL_LENGTH(N'gaillard.Equipo', N'TipoElemento') IS NULL
BEGIN
    ALTER TABLE gaillard.Equipo
        ADD TipoElemento NVARCHAR(30) NOT NULL
            CONSTRAINT DF_Equipo_TipoElemento DEFAULT (N'EQUIPO');
END;

IF NOT EXISTS
(
    SELECT 1 FROM sys.check_constraints
    WHERE name = N'CK_Equipo_TipoElemento'
      AND parent_object_id = OBJECT_ID(N'gaillard.Equipo')
)
BEGIN
    EXEC(N'ALTER TABLE gaillard.Equipo WITH CHECK
        ADD CONSTRAINT CK_Equipo_TipoElemento CHECK
            (TipoElemento IN (N''SUBSISTEMA'', N''EQUIPO'', N''COMPONENTE'', N''INSTALACION_AUXILIAR''))');
END;

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_Equipo_Instalacion_Padre'
      AND object_id = OBJECT_ID(N'gaillard.Equipo')
)
    CREATE INDEX IX_Equipo_Instalacion_Padre
        ON gaillard.Equipo(InstalacionId, EquipoPadreId);
