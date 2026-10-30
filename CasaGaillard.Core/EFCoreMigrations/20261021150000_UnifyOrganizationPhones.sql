SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER ON;
SET NUMERIC_ROUNDABORT OFF;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'gaillard.Telefono', N'U') IS NULL
    BEGIN
        IF OBJECT_ID(N'gaillard.TelefonoPersona', N'U') IS NULL
           OR OBJECT_ID(N'gaillard.TelefonoEntidad', N'U') IS NULL
            THROW 51000, 'No se encontraron las dos tablas de origen de teléfonos.', 1;

        CREATE TABLE gaillard.Telefono
        (
            ID int IDENTITY(1,1) NOT NULL,
            Telefono nchar(10) NOT NULL,
            PersonaID int NULL,
            EntidadID int NULL,
            CONSTRAINT PK_Telefono PRIMARY KEY (ID),
            CONSTRAINT CK_Telefono_UnSoloTitular CHECK
                ((PersonaID IS NOT NULL AND EntidadID IS NULL) OR (PersonaID IS NULL AND EntidadID IS NOT NULL)),
            CONSTRAINT FK_Telefono_Persona FOREIGN KEY (PersonaID) REFERENCES gaillard.Persona(ID),
            CONSTRAINT FK_Telefono_Entidad FOREIGN KEY (EntidadID) REFERENCES gaillard.Entidad(ID)
        );

        CREATE INDEX IX_Telefono_PersonaID ON gaillard.Telefono(PersonaID) WHERE PersonaID IS NOT NULL;
        CREATE INDEX IX_Telefono_EntidadID ON gaillard.Telefono(EntidadID) WHERE EntidadID IS NOT NULL;

        INSERT INTO gaillard.Telefono(Telefono, PersonaID)
            SELECT LTRIM(RTRIM(TelefonoPersona)), PersonaID FROM gaillard.TelefonoPersona;
        INSERT INTO gaillard.Telefono(Telefono, EntidadID)
            SELECT LTRIM(RTRIM(Telefono)), EntidadID FROM gaillard.TelefonoEntidad;
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
