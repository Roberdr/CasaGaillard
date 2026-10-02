/* Alinea Ubicacion con las claves INT del modelo y enlaza sus instalaciones y accesorios. */
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID(N'gaillard.Ubicacion', N'U') IS NULL
BEGIN
    ;THROW 51000, 'No existe gaillard.Ubicacion.', 1;
END;

IF COL_LENGTH(N'gaillard.Ubicacion', N'Codigo') IS NULL
    ALTER TABLE gaillard.Ubicacion ADD Codigo NVARCHAR(50) NULL;

IF EXISTS
(
    SELECT 1 FROM sys.foreign_keys
    WHERE referenced_object_id = OBJECT_ID(N'gaillard.Ubicacion')
      AND name NOT IN (N'FK_Accesorio_Ubicacion', N'FK_Instalacion_Ubicacion')
)
BEGIN
    ;THROW 51001, 'Ubicacion tiene claves foraneas no contempladas; no se ha cambiado su clave.', 1;
END;

IF EXISTS
(
    SELECT 1 FROM gaillard.Ubicacion
    WHERE UbicacionId > 2147483647 OR UbicacionId < -2147483648
)
BEGIN
    ;THROW 51002, 'Hay identificadores de ubicacion fuera del rango INT.', 1;
END;

IF EXISTS
(
    SELECT 1 FROM gaillard.Accesorio a
    LEFT JOIN gaillard.Ubicacion u ON u.UbicacionId = a.UbicacionId
    WHERE a.UbicacionId IS NOT NULL AND u.UbicacionId IS NULL
)
BEGIN
    ;THROW 51003, 'Hay accesorios con una ubicacion inexistente; no se ha cambiado la clave.', 1;
END;

IF EXISTS
(
    SELECT 1 FROM gaillard.Instalacion i
    LEFT JOIN gaillard.Ubicacion u ON u.UbicacionId = i.UbicacionId
    WHERE i.UbicacionId IS NOT NULL AND u.UbicacionId IS NULL
)
BEGIN
    ;THROW 51004, 'Hay instalaciones con una ubicacion inexistente; no se ha cambiado la clave.', 1;
END;

IF EXISTS
(
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'gaillard.Ubicacion')
      AND name = N'UbicacionId' AND TYPE_NAME(user_type_id) = N'bigint'
)
BEGIN
    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_Accesorio_Ubicacion' AND parent_object_id=OBJECT_ID(N'gaillard.Accesorio'))
        ALTER TABLE gaillard.Accesorio DROP CONSTRAINT FK_Accesorio_Ubicacion;
    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_Instalacion_Ubicacion' AND parent_object_id=OBJECT_ID(N'gaillard.Instalacion'))
        ALTER TABLE gaillard.Instalacion DROP CONSTRAINT FK_Instalacion_Ubicacion;

    CREATE TABLE gaillard.Ubicacion_IntMigracion
    (
        UbicacionId INT IDENTITY(1,1) NOT NULL,
        Nombre NVARCHAR(150) NOT NULL,
        Descripcion NVARCHAR(300) NULL,
        Codigo NVARCHAR(50) NULL
    );

    SET IDENTITY_INSERT gaillard.Ubicacion_IntMigracion ON;
    INSERT INTO gaillard.Ubicacion_IntMigracion (UbicacionId, Nombre, Descripcion, Codigo)
        SELECT CONVERT(INT, UbicacionId), Nombre, Descripcion, Codigo
        FROM gaillard.Ubicacion;
    SET IDENTITY_INSERT gaillard.Ubicacion_IntMigracion OFF;

    DROP TABLE gaillard.Ubicacion;
    EXEC sys.sp_rename N'gaillard.Ubicacion_IntMigracion', N'Ubicacion';
END;

IF NOT EXISTS
(
    SELECT 1 FROM sys.key_constraints
    WHERE parent_object_id = OBJECT_ID(N'gaillard.Ubicacion') AND type = N'PK'
)
BEGIN
    IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'gaillard.Ubicacion') AND name=N'UQ_Ubicacion_UbicacionId')
        DROP INDEX UQ_Ubicacion_UbicacionId ON gaillard.Ubicacion;
    ALTER TABLE gaillard.Ubicacion
        ADD CONSTRAINT PK_Ubicacion PRIMARY KEY CLUSTERED (UbicacionId);
END;

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE object_id=OBJECT_ID(N'gaillard.Ubicacion') AND name=N'UX_Ubicacion_Codigo'
)
    CREATE UNIQUE INDEX UX_Ubicacion_Codigo ON gaillard.Ubicacion(Codigo) WHERE Codigo IS NOT NULL;

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_Accesorio_Ubicacion' AND parent_object_id=OBJECT_ID(N'gaillard.Accesorio'))
    ALTER TABLE gaillard.Accesorio ADD CONSTRAINT FK_Accesorio_Ubicacion
        FOREIGN KEY (UbicacionId) REFERENCES gaillard.Ubicacion(UbicacionId);

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_Instalacion_Ubicacion' AND parent_object_id=OBJECT_ID(N'gaillard.Instalacion'))
    ALTER TABLE gaillard.Instalacion ADD CONSTRAINT FK_Instalacion_Ubicacion
        FOREIGN KEY (UbicacionId) REFERENCES gaillard.Ubicacion(UbicacionId);

COMMIT TRANSACTION;
