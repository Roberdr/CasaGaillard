/* Vincula las referencias nominales con los catálogos existentes.
   Se conservan las columnas de texto para preservar y mostrar los datos históricos. */

/* Completa la integridad de la relación persona-entidad existente. */
IF OBJECT_ID(N'gaillard.PersonasEntidad', N'U') IS NOT NULL
   AND OBJECT_ID(N'gaillard.Persona', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_PersonasEntidad_Persona')
    ALTER TABLE gaillard.PersonasEntidad ADD CONSTRAINT FK_PersonasEntidad_Persona
        FOREIGN KEY (PersonaID) REFERENCES gaillard.Persona(ID);

/* Cada referencia nueva es opcional para no invalidar registros históricos. */
IF OBJECT_ID(N'gaillard.Revision', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'gaillard.Revision', N'AutorizadoPersonasEntidadID') IS NULL
        ALTER TABLE gaillard.Revision ADD AutorizadoPersonasEntidadID INT NULL;
    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Revision_AutorizadoPersonasEntidad')
        ALTER TABLE gaillard.Revision ADD CONSTRAINT FK_Revision_AutorizadoPersonasEntidad
            FOREIGN KEY (AutorizadoPersonasEntidadID) REFERENCES gaillard.PersonasEntidad(ID);
END;

IF OBJECT_ID(N'gaillard.RevisionVehiculo', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'gaillard.RevisionVehiculo', N'EjecutorPersonasEntidadID') IS NULL
        ALTER TABLE gaillard.RevisionVehiculo ADD EjecutorPersonasEntidadID INT NULL;
    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_RevisionVehiculo_EjecutorPersonasEntidad')
        ALTER TABLE gaillard.RevisionVehiculo ADD CONSTRAINT FK_RevisionVehiculo_EjecutorPersonasEntidad
            FOREIGN KEY (EjecutorPersonasEntidadID) REFERENCES gaillard.PersonasEntidad(ID);
END;

IF OBJECT_ID(N'dbo.TareasMantenimiento', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'dbo.TareasMantenimiento', N'DetectadoPorPersonaID') IS NULL
        ALTER TABLE dbo.TareasMantenimiento ADD DetectadoPorPersonaID INT NULL;
    IF COL_LENGTH(N'dbo.TareasMantenimiento', N'ContactoPersonaID') IS NULL
        ALTER TABLE dbo.TareasMantenimiento ADD ContactoPersonaID INT NULL;
    IF COL_LENGTH(N'dbo.TareasMantenimiento', N'AsignadaAPersonaID') IS NULL
        ALTER TABLE dbo.TareasMantenimiento ADD AsignadaAPersonaID INT NULL;
    IF COL_LENGTH(N'dbo.TareasMantenimiento', N'EmpresaExteriorEntidadID') IS NULL
        ALTER TABLE dbo.TareasMantenimiento ADD EmpresaExteriorEntidadID INT NULL;

    IF OBJECT_ID(N'gaillard.Persona', N'U') IS NOT NULL
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_TareasMantenimiento_DetectadoPorPersona')
            ALTER TABLE dbo.TareasMantenimiento ADD CONSTRAINT FK_TareasMantenimiento_DetectadoPorPersona FOREIGN KEY (DetectadoPorPersonaID) REFERENCES gaillard.Persona(ID);
        IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_TareasMantenimiento_ContactoPersona')
            ALTER TABLE dbo.TareasMantenimiento ADD CONSTRAINT FK_TareasMantenimiento_ContactoPersona FOREIGN KEY (ContactoPersonaID) REFERENCES gaillard.Persona(ID);
        IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_TareasMantenimiento_AsignadaAPersona')
            ALTER TABLE dbo.TareasMantenimiento ADD CONSTRAINT FK_TareasMantenimiento_AsignadaAPersona FOREIGN KEY (AsignadaAPersonaID) REFERENCES gaillard.Persona(ID);
    END;
    IF OBJECT_ID(N'gaillard.Entidad', N'U') IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_TareasMantenimiento_EmpresaExteriorEntidad')
        ALTER TABLE dbo.TareasMantenimiento ADD CONSTRAINT FK_TareasMantenimiento_EmpresaExteriorEntidad FOREIGN KEY (EmpresaExteriorEntidadID) REFERENCES gaillard.Entidad(ID);
END;

IF OBJECT_ID(N'dbo.Intervencion', N'U') IS NOT NULL
   AND COL_LENGTH(N'dbo.Intervencion', N'TecnicoPersonaID') IS NULL
    ALTER TABLE dbo.Intervencion ADD TecnicoPersonaID INT NULL;
IF OBJECT_ID(N'dbo.Intervencion', N'U') IS NOT NULL
   AND OBJECT_ID(N'gaillard.Persona', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Intervencion_TecnicoPersona')
    ALTER TABLE dbo.Intervencion ADD CONSTRAINT FK_Intervencion_TecnicoPersona
        FOREIGN KEY (TecnicoPersonaID) REFERENCES gaillard.Persona(ID);
