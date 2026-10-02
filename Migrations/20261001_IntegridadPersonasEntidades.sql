/* Integridad e índices para personas, entidades, teléfonos y su relación histórica.
   La aplicación también asigna Persona.ID y Entidad.ID en transacciones porque
   esas columnas heredadas todavía no son IDENTITY. */
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID(N'gaillard.PersonasEntidad', N'U') IS NULL
    THROW 51000, 'No existe gaillard.PersonasEntidad.', 1;

/* No elegir automáticamente qué duplicado conservar: resolverlo antes de aplicar. */
IF EXISTS (
    SELECT 1
    FROM gaillard.PersonasEntidad
    WHERE FechaBaja IS NULL
    GROUP BY PersonaID, EntidadID
    HAVING COUNT(*) > 1
)
    THROW 51001, 'Hay vínculos activos duplicados Persona-Entidad; corrígelos antes de aplicar esta migración.', 1;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'gaillard.PersonasEntidad') AND name = N'UX_PersonasEntidad_Activa')
    CREATE UNIQUE INDEX UX_PersonasEntidad_Activa
        ON gaillard.PersonasEntidad (PersonaID, EntidadID)
        WHERE FechaBaja IS NULL;

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id = OBJECT_ID(N'gaillard.PersonasEntidad') AND name = N'CK_PersonasEntidad_Fechas')
    ALTER TABLE gaillard.PersonasEntidad WITH CHECK ADD CONSTRAINT CK_PersonasEntidad_Fechas
        CHECK (FechaAlta IS NULL OR FechaBaja IS NULL OR FechaBaja >= FechaAlta);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'gaillard.PersonasEntidad') AND name = N'IX_PersonasEntidad_Entidad_FechaBaja')
    CREATE INDEX IX_PersonasEntidad_Entidad_FechaBaja
        ON gaillard.PersonasEntidad (EntidadID, FechaBaja) INCLUDE (PersonaID, CargoID, FechaAlta);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'gaillard.PersonasEntidad') AND name = N'IX_PersonasEntidad_CargoID')
    CREATE INDEX IX_PersonasEntidad_CargoID ON gaillard.PersonasEntidad (CargoID) WHERE CargoID IS NOT NULL;

IF OBJECT_ID(N'gaillard.TelefonoPersona', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'gaillard.TelefonoPersona') AND name = N'IX_TelefonoPersona_PersonaID')
    CREATE INDEX IX_TelefonoPersona_PersonaID ON gaillard.TelefonoPersona (PersonaID);

IF OBJECT_ID(N'gaillard.TelefonoEntidad', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'gaillard.TelefonoEntidad') AND name = N'IX_TelefonoEntidad_EntidadID')
    CREATE INDEX IX_TelefonoEntidad_EntidadID ON gaillard.TelefonoEntidad (EntidadID);

COMMIT TRANSACTION;
