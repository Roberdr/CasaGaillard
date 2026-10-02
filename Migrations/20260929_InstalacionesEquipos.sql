/*
    CasaGaillard - modelo base de instalaciones y equipos.
    Esta migracion es idempotente y debe ejecutarse sobre la base de datos
    GAILLARD antes de actualizar el modelo EDMX.
*/

IF SCHEMA_ID(N'gaillard') IS NULL
BEGIN
    EXEC(N'CREATE SCHEMA gaillard');
END;

IF OBJECT_ID(N'gaillard.Ubicacion', N'U') IS NOT NULL
    AND COL_LENGTH(N'gaillard.Ubicacion', N'Codigo') IS NULL
BEGIN
    ALTER TABLE gaillard.Ubicacion ADD Codigo NVARCHAR(50) NULL;
END;

IF OBJECT_ID(N'gaillard.Ubicacion', N'U') IS NOT NULL
    AND NOT EXISTS
    (
        SELECT 1
        FROM sys.indexes
        WHERE name = N'UX_Ubicacion_Codigo'
          AND object_id = OBJECT_ID(N'gaillard.Ubicacion')
    )
BEGIN
    CREATE UNIQUE INDEX UX_Ubicacion_Codigo
        ON gaillard.Ubicacion(Codigo)
        WHERE Codigo IS NOT NULL;
END;

-- UbicacionId es la clave que usa el modelo y que referencian las tablas
-- relacionadas. Algunas bases existentes no tienen una clave candidata para
-- esa columna, así que créala antes de añadir las claves foráneas.
IF OBJECT_ID(N'gaillard.Ubicacion', N'U') IS NOT NULL
    AND COL_LENGTH(N'gaillard.Ubicacion', N'UbicacionId') IS NOT NULL
    AND NOT EXISTS
    (
        SELECT 1
        FROM sys.indexes AS i
        WHERE i.object_id = OBJECT_ID(N'gaillard.Ubicacion')
          AND i.is_unique = 1
          AND i.has_filter = 0
          AND 1 =
          (
              SELECT COUNT(*)
              FROM sys.index_columns AS ic
              WHERE ic.object_id = i.object_id
                AND ic.index_id = i.index_id
                AND ic.key_ordinal > 0
          )
          AND EXISTS
          (
              SELECT 1
              FROM sys.index_columns AS ic
              INNER JOIN sys.columns AS c
                  ON c.object_id = ic.object_id
                 AND c.column_id = ic.column_id
              WHERE ic.object_id = i.object_id
                AND ic.index_id = i.index_id
                AND ic.key_ordinal = 1
                AND c.name = N'UbicacionId'
          )
    )
BEGIN
    ALTER TABLE gaillard.Ubicacion
        ADD CONSTRAINT UQ_Ubicacion_UbicacionId UNIQUE (UbicacionId);
END;

IF OBJECT_ID(N'gaillard.TipoInstalacion', N'U') IS NULL
BEGIN
    CREATE TABLE gaillard.TipoInstalacion
    (
        Id INT IDENTITY(1,1) NOT NULL,
        Nombre NVARCHAR(100) NOT NULL,
        Descripcion NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2(0) NOT NULL
            CONSTRAINT DF_TipoInstalacion_CreatedAt DEFAULT (SYSDATETIME()),

        CONSTRAINT PK_TipoInstalacion PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_TipoInstalacion_Nombre UNIQUE (Nombre)
    );
END;

IF OBJECT_ID(N'gaillard.Instalacion', N'U') IS NULL
BEGIN
    CREATE TABLE gaillard.Instalacion
    (
        Id INT IDENTITY(1,1) NOT NULL,
        Codigo NVARCHAR(50) NOT NULL,
        Nombre NVARCHAR(255) NOT NULL,
        Descripcion NVARCHAR(MAX) NULL,
        TipoInstalacionId INT NULL,
        UbicacionId INT NULL,
        ResponsablePersonaId INT NULL,
        FechaPuestaMarcha DATE NULL,
        FechaBaja DATE NULL,
        Criticidad TINYINT NOT NULL
            CONSTRAINT DF_Instalacion_Criticidad DEFAULT (3),
        Estado NVARCHAR(20) NOT NULL
            CONSTRAINT DF_Instalacion_Estado DEFAULT (N'ACTIVA'),
        CodigoPlano NVARCHAR(50) NULL,
        CriticaProduccion BIT NOT NULL
            CONSTRAINT DF_Instalacion_CriticaProduccion DEFAULT (0),
        CriticaSeguridad BIT NOT NULL
            CONSTRAINT DF_Instalacion_CriticaSeguridad DEFAULT (0),
        Observaciones NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2(0) NOT NULL
            CONSTRAINT DF_Instalacion_CreatedAt DEFAULT (SYSDATETIME()),
        UpdatedAt DATETIME2(0) NOT NULL
            CONSTRAINT DF_Instalacion_UpdatedAt DEFAULT (SYSDATETIME()),

        CONSTRAINT PK_Instalacion PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_Instalacion_Codigo UNIQUE (Codigo),
        CONSTRAINT CK_Instalacion_Criticidad CHECK (Criticidad BETWEEN 1 AND 5),
        CONSTRAINT CK_Instalacion_Estado CHECK
            (Estado IN (N'ACTIVA', N'PARADA', N'MANTENIMIENTO', N'FUERA_SERVICIO')),
        CONSTRAINT CK_Instalacion_Fechas CHECK
            (FechaBaja IS NULL OR FechaPuestaMarcha IS NULL OR FechaBaja >= FechaPuestaMarcha)
    );

    CREATE INDEX IX_Instalacion_Nombre ON gaillard.Instalacion(Nombre);
    CREATE INDEX IX_Instalacion_Estado ON gaillard.Instalacion(Estado);
    CREATE INDEX IX_Instalacion_Criticidad ON gaillard.Instalacion(Criticidad);
    CREATE INDEX IX_Instalacion_TipoInstalacionId ON gaillard.Instalacion(TipoInstalacionId);
    CREATE INDEX IX_Instalacion_UbicacionId ON gaillard.Instalacion(UbicacionId);
END;

IF OBJECT_ID(N'gaillard.Instalacion', N'U') IS NOT NULL
    AND OBJECT_ID(N'gaillard.TipoInstalacion', N'U') IS NOT NULL
    AND NOT EXISTS
    (
        SELECT 1 FROM sys.foreign_keys
        WHERE name = N'FK_Instalacion_TipoInstalacion'
          AND parent_object_id = OBJECT_ID(N'gaillard.Instalacion')
    )
BEGIN
    ALTER TABLE gaillard.Instalacion
        ADD CONSTRAINT FK_Instalacion_TipoInstalacion
        FOREIGN KEY (TipoInstalacionId) REFERENCES gaillard.TipoInstalacion(Id);
END;

IF OBJECT_ID(N'gaillard.Instalacion', N'U') IS NOT NULL
    AND OBJECT_ID(N'gaillard.Ubicacion', N'U') IS NOT NULL
    AND EXISTS
    (
        SELECT 1
        FROM sys.columns AS childColumn
        INNER JOIN sys.types AS childType
            ON childType.user_type_id = childColumn.user_type_id
        INNER JOIN sys.columns AS parentColumn
            ON parentColumn.object_id = OBJECT_ID(N'gaillard.Ubicacion')
           AND parentColumn.name = N'UbicacionId'
        INNER JOIN sys.types AS parentType
            ON parentType.user_type_id = parentColumn.user_type_id
        WHERE childColumn.object_id = OBJECT_ID(N'gaillard.Instalacion')
          AND childColumn.name = N'UbicacionId'
          AND childType.name = parentType.name
    )
    AND NOT EXISTS
    (
        SELECT 1 FROM sys.foreign_keys
        WHERE name = N'FK_Instalacion_Ubicacion'
          AND parent_object_id = OBJECT_ID(N'gaillard.Instalacion')
    )
BEGIN
    ALTER TABLE gaillard.Instalacion
        ADD CONSTRAINT FK_Instalacion_Ubicacion
        FOREIGN KEY (UbicacionId) REFERENCES gaillard.Ubicacion(UbicacionId);
END;

IF OBJECT_ID(N'gaillard.Instalacion', N'U') IS NOT NULL
    AND OBJECT_ID(N'gaillard.Persona', N'U') IS NOT NULL
    AND NOT EXISTS
    (
        SELECT 1 FROM sys.foreign_keys
        WHERE name = N'FK_Instalacion_ResponsablePersona'
          AND parent_object_id = OBJECT_ID(N'gaillard.Instalacion')
    )
BEGIN
    ALTER TABLE gaillard.Instalacion
        ADD CONSTRAINT FK_Instalacion_ResponsablePersona
        FOREIGN KEY (ResponsablePersonaId) REFERENCES gaillard.Persona(ID);
END;

IF OBJECT_ID(N'gaillard.Equipo', N'U') IS NULL
BEGIN
    CREATE TABLE gaillard.Equipo
    (
        Id INT IDENTITY(1,1) NOT NULL,
        InstalacionId INT NOT NULL,
        EquipoPadreId INT NULL,
        Codigo NVARCHAR(50) NOT NULL,
        Nombre NVARCHAR(255) NOT NULL,
        Descripcion NVARCHAR(MAX) NULL,
        Fabricante NVARCHAR(150) NULL,
        Modelo NVARCHAR(150) NULL,
        NumeroSerie NVARCHAR(100) NULL,
        Potencia DECIMAL(18,3) NULL,
        Caudal DECIMAL(18,3) NULL,
        Presion DECIMAL(18,3) NULL,
        Voltaje DECIMAL(18,3) NULL,
        FechaInstalacion DATE NULL,
        FechaBaja DATE NULL,
        Activo BIT NOT NULL CONSTRAINT DF_Equipo_Activo DEFAULT (1),
        Observaciones NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2(0) NOT NULL
            CONSTRAINT DF_Equipo_CreatedAt DEFAULT (SYSDATETIME()),
        UpdatedAt DATETIME2(0) NOT NULL
            CONSTRAINT DF_Equipo_UpdatedAt DEFAULT (SYSDATETIME()),

        CONSTRAINT PK_Equipo PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_Equipo_Instalacion_Codigo UNIQUE (InstalacionId, Codigo),
        CONSTRAINT CK_Equipo_Fechas CHECK
            (FechaBaja IS NULL OR FechaInstalacion IS NULL OR FechaBaja >= FechaInstalacion)
    );

    CREATE INDEX IX_Equipo_InstalacionId ON gaillard.Equipo(InstalacionId);
    CREATE INDEX IX_Equipo_EquipoPadreId ON gaillard.Equipo(EquipoPadreId);
    CREATE INDEX IX_Equipo_NumeroSerie ON gaillard.Equipo(NumeroSerie);
END;

IF OBJECT_ID(N'gaillard.Equipo', N'U') IS NOT NULL
    AND NOT EXISTS
    (
        SELECT 1 FROM sys.foreign_keys
        WHERE name = N'FK_Equipo_Instalacion'
          AND parent_object_id = OBJECT_ID(N'gaillard.Equipo')
    )
BEGIN
    ALTER TABLE gaillard.Equipo
        ADD CONSTRAINT FK_Equipo_Instalacion
        FOREIGN KEY (InstalacionId) REFERENCES gaillard.Instalacion(Id);
END;

IF OBJECT_ID(N'gaillard.Equipo', N'U') IS NOT NULL
    AND NOT EXISTS
    (
        SELECT 1 FROM sys.foreign_keys
        WHERE name = N'FK_Equipo_EquipoPadre'
          AND parent_object_id = OBJECT_ID(N'gaillard.Equipo')
    )
BEGIN
    ALTER TABLE gaillard.Equipo
        ADD CONSTRAINT FK_Equipo_EquipoPadre
        FOREIGN KEY (EquipoPadreId) REFERENCES gaillard.Equipo(Id);
END;

IF OBJECT_ID(N'gaillard.EquipoAccesorio', N'U') IS NULL
BEGIN
    CREATE TABLE gaillard.EquipoAccesorio
    (
        Id INT IDENTITY(1,1) NOT NULL,
        EquipoId INT NOT NULL,
        AccesorioId INT NOT NULL,
        Cantidad INT NOT NULL CONSTRAINT DF_EquipoAccesorio_Cantidad DEFAULT (1),
        Posicion NVARCHAR(100) NULL,
        NumeroSerie NVARCHAR(100) NULL,
        FechaInstalacion DATE NULL,
        FechaRetirada DATE NULL,
        Activo BIT NOT NULL CONSTRAINT DF_EquipoAccesorio_Activo DEFAULT (1),
        Observaciones NVARCHAR(500) NULL,

        CONSTRAINT PK_EquipoAccesorio PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_EquipoAccesorio_Equipo_Accesorio UNIQUE (EquipoId, AccesorioId, Posicion),
        CONSTRAINT CK_EquipoAccesorio_Cantidad CHECK (Cantidad > 0),
        CONSTRAINT CK_EquipoAccesorio_Fechas CHECK
            (FechaRetirada IS NULL OR FechaInstalacion IS NULL OR FechaRetirada >= FechaInstalacion)
    );

    CREATE INDEX IX_EquipoAccesorio_EquipoId ON gaillard.EquipoAccesorio(EquipoId);
    CREATE INDEX IX_EquipoAccesorio_AccesorioId ON gaillard.EquipoAccesorio(AccesorioId);
END;

IF OBJECT_ID(N'gaillard.EquipoAccesorio', N'U') IS NOT NULL
    AND NOT EXISTS
    (
        SELECT 1 FROM sys.foreign_keys
        WHERE name = N'FK_EquipoAccesorio_Equipo'
          AND parent_object_id = OBJECT_ID(N'gaillard.EquipoAccesorio')
    )
BEGIN
    ALTER TABLE gaillard.EquipoAccesorio
        ADD CONSTRAINT FK_EquipoAccesorio_Equipo
        FOREIGN KEY (EquipoId) REFERENCES gaillard.Equipo(Id);
END;

IF OBJECT_ID(N'gaillard.EquipoAccesorio', N'U') IS NOT NULL
    AND OBJECT_ID(N'gaillard.Accesorio', N'U') IS NOT NULL
    AND NOT EXISTS
    (
        SELECT 1 FROM sys.foreign_keys
        WHERE name = N'FK_EquipoAccesorio_Accesorio'
          AND parent_object_id = OBJECT_ID(N'gaillard.EquipoAccesorio')
    )
BEGIN
    ALTER TABLE gaillard.EquipoAccesorio
        ADD CONSTRAINT FK_EquipoAccesorio_Accesorio
        FOREIGN KEY (AccesorioId) REFERENCES gaillard.Accesorio(ID);
END;

IF OBJECT_ID(N'gaillard.InstalacionDocumento', N'U') IS NULL
BEGIN
    CREATE TABLE gaillard.InstalacionDocumento
    (
        Id INT IDENTITY(1,1) NOT NULL,
        InstalacionId INT NOT NULL,
        Nombre NVARCHAR(255) NOT NULL,
        RutaArchivo NVARCHAR(500) NOT NULL,
        TipoDocumento NVARCHAR(100) NULL,
        CodigoPlano NVARCHAR(50) NULL,
        FechaDocumento DATE NULL,
        FechaAlta DATETIME2(0) NOT NULL
            CONSTRAINT DF_InstalacionDocumento_FechaAlta DEFAULT (SYSDATETIME()),

        CONSTRAINT PK_InstalacionDocumento PRIMARY KEY CLUSTERED (Id)
    );

    CREATE INDEX IX_InstalacionDocumento_InstalacionId
        ON gaillard.InstalacionDocumento(InstalacionId);
END;

IF OBJECT_ID(N'gaillard.InstalacionDocumento', N'U') IS NOT NULL
    AND NOT EXISTS
    (
        SELECT 1 FROM sys.foreign_keys
        WHERE name = N'FK_InstalacionDocumento_Instalacion'
          AND parent_object_id = OBJECT_ID(N'gaillard.InstalacionDocumento')
    )
BEGIN
    ALTER TABLE gaillard.InstalacionDocumento
        ADD CONSTRAINT FK_InstalacionDocumento_Instalacion
        FOREIGN KEY (InstalacionId) REFERENCES gaillard.Instalacion(Id);
END;

IF OBJECT_ID(N'dbo.TareasMantenimiento', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'dbo.TareasMantenimiento', N'InstalacionId') IS NULL
        ALTER TABLE dbo.TareasMantenimiento ADD InstalacionId INT NULL;

    IF COL_LENGTH(N'dbo.TareasMantenimiento', N'EquipoId') IS NULL
        ALTER TABLE dbo.TareasMantenimiento ADD EquipoId INT NULL;

    IF NOT EXISTS
    (
        SELECT 1 FROM sys.indexes
        WHERE name = N'IX_TareasMantenimiento_InstalacionId'
          AND object_id = OBJECT_ID(N'dbo.TareasMantenimiento')
    )
        CREATE INDEX IX_TareasMantenimiento_InstalacionId
            ON dbo.TareasMantenimiento(InstalacionId);

    IF NOT EXISTS
    (
        SELECT 1 FROM sys.indexes
        WHERE name = N'IX_TareasMantenimiento_EquipoId'
          AND object_id = OBJECT_ID(N'dbo.TareasMantenimiento')
    )
        CREATE INDEX IX_TareasMantenimiento_EquipoId
            ON dbo.TareasMantenimiento(EquipoId);

    IF NOT EXISTS
    (
        SELECT 1 FROM sys.foreign_keys
        WHERE name = N'FK_TareasMantenimiento_Instalacion'
          AND parent_object_id = OBJECT_ID(N'dbo.TareasMantenimiento')
    )
        ALTER TABLE dbo.TareasMantenimiento
            ADD CONSTRAINT FK_TareasMantenimiento_Instalacion
            FOREIGN KEY (InstalacionId) REFERENCES gaillard.Instalacion(Id);

    IF NOT EXISTS
    (
        SELECT 1 FROM sys.foreign_keys
        WHERE name = N'FK_TareasMantenimiento_Equipo'
          AND parent_object_id = OBJECT_ID(N'dbo.TareasMantenimiento')
    )
        ALTER TABLE dbo.TareasMantenimiento
            ADD CONSTRAINT FK_TareasMantenimiento_Equipo
            FOREIGN KEY (EquipoId) REFERENCES gaillard.Equipo(Id);
END;
