/*
    Reinicia las tablas del control de instalaciones para aplicar de nuevo
    el modelo definido en 20260929_InstalacionesEquipos.sql.
    Conserva dbo.TareasMantenimiento y sus columnas INT InstalacionId/EquipoId.
*/

DECLARE @tables TABLE (ObjectId INT NOT NULL PRIMARY KEY);

INSERT INTO @tables (ObjectId)
SELECT OBJECT_ID(name)
FROM (VALUES
    (N'gaillard.EquipoAccesorio'),
    (N'gaillard.InstalacionDocumento'),
    (N'gaillard.Equipo'),
    (N'gaillard.Instalacion'),
    (N'gaillard.TipoInstalacion')
) AS targets(name)
WHERE OBJECT_ID(name, N'U') IS NOT NULL;

DECLARE @dropConstraints NVARCHAR(MAX) = N'';

SELECT @dropConstraints +=
    N'ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(fk.parent_object_id)) + N'.'
    + QUOTENAME(OBJECT_NAME(fk.parent_object_id)) + N' DROP CONSTRAINT '
    + QUOTENAME(fk.name) + N';' + CHAR(13) + CHAR(10)
FROM sys.foreign_keys AS fk
WHERE fk.parent_object_id IN (SELECT ObjectId FROM @tables)
   OR fk.referenced_object_id IN (SELECT ObjectId FROM @tables);

IF @dropConstraints <> N''
    EXEC sys.sp_executesql @dropConstraints;

DROP TABLE IF EXISTS gaillard.EquipoAccesorio;
DROP TABLE IF EXISTS gaillard.InstalacionDocumento;
DROP TABLE IF EXISTS gaillard.Equipo;
DROP TABLE IF EXISTS gaillard.Instalacion;
DROP TABLE IF EXISTS gaillard.TipoInstalacion;
