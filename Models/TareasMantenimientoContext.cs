using System;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;

namespace CasaGaillard.Models
{
    public class TareasMantenimientoContext : DbContext
    {
        private static readonly object SyncRoot = new object();
        private static bool schemaVerified;

        public TareasMantenimientoContext()
            : base("name=ApplicationDbContext")
        {
            Database.SetInitializer<TareasMantenimientoContext>(null);
            EnsureSchema();
        }

        public DbSet<TareaMantenimiento> TareasMantenimiento { get; set; }
        public DbSet<TareaMantenimientoFoto> TareaMantenimientoFotos { get; set; }
        public DbSet<TareaMantenimientoAccesorio> TareaMantenimientoAccesorios { get; set; }

        private static void EnsureSchema()
        {
            if (schemaVerified)
            {
                return;
            }

            lock (SyncRoot)
            {
                if (schemaVerified)
                {
                    return;
                }

                var connectionString = ConfigurationManager.ConnectionStrings["ApplicationDbContext"]?.ConnectionString;
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException("No se ha encontrado la cadena de conexión ApplicationDbContext.");
                }

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    var createTable = @"
IF OBJECT_ID('dbo.TareasMantenimiento', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.TareasMantenimiento
    (
        ID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_TareasMantenimiento PRIMARY KEY,
        Codigo NVARCHAR(30) NOT NULL,
        Titulo NVARCHAR(150) NOT NULL,
        Descripcion NVARCHAR(2000) NOT NULL,
        Ubicacion NVARCHAR(150) NULL,
        Equipo NVARCHAR(150) NULL,
        InstalacionId INT NULL,
        EquipoId INT NULL,
        CubaId INT NULL,
        GrupoId INT NULL,
        VehiculoId INT NULL,
        PlanMantenimientoId INT NULL,
        DetectadoPorPersonaID INT NULL,
        ContactoPersonaID INT NULL,
        AsignadaAPersonaID INT NULL,
        EmpresaExteriorEntidadID INT NULL,
        DetectadoPor NVARCHAR(150) NULL,
        Contacto NVARCHAR(150) NULL,
        Prioridad NVARCHAR(20) NULL,
        Estado NVARCHAR(30) NULL,
        AsignadaA NVARCHAR(150) NULL,
        EmpresaExterior NVARCHAR(150) NULL,
        AccionesARealizar NVARCHAR(2000) NULL,
        AccesoriosNecesarios NVARCHAR(4000) NULL,
        Observaciones NVARCHAR(2000) NULL,
        CreadoPor NVARCHAR(100) NULL,
        ActualizadoPor NVARCHAR(100) NULL,
        FechaDeteccion DATETIME NOT NULL,
        FechaCreacion DATETIME NOT NULL,
        FechaActualizacion DATETIME NULL
    );

    CREATE UNIQUE INDEX IX_TareasMantenimiento_Codigo ON dbo.TareasMantenimiento(Codigo);
END";

                    using (var command = new SqlCommand(createTable, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    const string ensureAssetColumns = @"
IF OBJECT_ID('dbo.TareasMantenimiento', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.TareasMantenimiento', 'InstalacionId') IS NULL
        ALTER TABLE dbo.TareasMantenimiento ADD InstalacionId INT NULL;

    IF COL_LENGTH('dbo.TareasMantenimiento', 'EquipoId') IS NULL
        ALTER TABLE dbo.TareasMantenimiento ADD EquipoId INT NULL;

    IF COL_LENGTH('dbo.TareasMantenimiento', 'CubaId') IS NULL
        ALTER TABLE dbo.TareasMantenimiento ADD CubaId INT NULL;

    IF COL_LENGTH('dbo.TareasMantenimiento', 'GrupoId') IS NULL
        ALTER TABLE dbo.TareasMantenimiento ADD GrupoId INT NULL;

    IF COL_LENGTH('dbo.TareasMantenimiento', 'VehiculoId') IS NULL
        ALTER TABLE dbo.TareasMantenimiento ADD VehiculoId INT NULL;

    IF COL_LENGTH('dbo.TareasMantenimiento', 'PlanMantenimientoId') IS NULL
        ALTER TABLE dbo.TareasMantenimiento ADD PlanMantenimientoId INT NULL;

    IF COL_LENGTH('dbo.TareasMantenimiento', 'DetectadoPorPersonaID') IS NULL
        ALTER TABLE dbo.TareasMantenimiento ADD DetectadoPorPersonaID INT NULL;
    IF COL_LENGTH('dbo.TareasMantenimiento', 'ContactoPersonaID') IS NULL
        ALTER TABLE dbo.TareasMantenimiento ADD ContactoPersonaID INT NULL;
    IF COL_LENGTH('dbo.TareasMantenimiento', 'AsignadaAPersonaID') IS NULL
        ALTER TABLE dbo.TareasMantenimiento ADD AsignadaAPersonaID INT NULL;
    IF COL_LENGTH('dbo.TareasMantenimiento', 'EmpresaExteriorEntidadID') IS NULL
        ALTER TABLE dbo.TareasMantenimiento ADD EmpresaExteriorEntidadID INT NULL;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_TareasMantenimiento_PlanMantenimientoId' AND object_id = OBJECT_ID('dbo.TareasMantenimiento'))
        CREATE INDEX IX_TareasMantenimiento_PlanMantenimientoId ON dbo.TareasMantenimiento(PlanMantenimientoId);

    IF OBJECT_ID('gaillard.PlanMantenimiento', 'U') IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_TareasMantenimiento_PlanMantenimiento' AND parent_object_id = OBJECT_ID('dbo.TareasMantenimiento'))
        EXEC(N'ALTER TABLE dbo.TareasMantenimiento ADD CONSTRAINT FK_TareasMantenimiento_PlanMantenimiento FOREIGN KEY (PlanMantenimientoId) REFERENCES gaillard.PlanMantenimiento(PlanId)');

    IF OBJECT_ID('gaillard.Instalacion', 'U') IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_TareasMantenimiento_Instalacion' AND parent_object_id = OBJECT_ID('dbo.TareasMantenimiento'))
        EXEC(N'ALTER TABLE dbo.TareasMantenimiento ADD CONSTRAINT FK_TareasMantenimiento_Instalacion FOREIGN KEY (InstalacionId) REFERENCES gaillard.Instalacion(Id)');

    IF OBJECT_ID('gaillard.Equipo', 'U') IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_TareasMantenimiento_Equipo' AND parent_object_id = OBJECT_ID('dbo.TareasMantenimiento'))
        EXEC(N'ALTER TABLE dbo.TareasMantenimiento ADD CONSTRAINT FK_TareasMantenimiento_Equipo FOREIGN KEY (EquipoId) REFERENCES gaillard.Equipo(Id)');

    IF OBJECT_ID('gaillard.Cuba', 'U') IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_TareasMantenimiento_Cuba' AND parent_object_id = OBJECT_ID('dbo.TareasMantenimiento'))
        EXEC(N'ALTER TABLE dbo.TareasMantenimiento ADD CONSTRAINT FK_TareasMantenimiento_Cuba FOREIGN KEY (CubaId) REFERENCES gaillard.Cuba(ID)');

    IF OBJECT_ID('gaillard.Grupo', 'U') IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_TareasMantenimiento_Grupo' AND parent_object_id = OBJECT_ID('dbo.TareasMantenimiento'))
        EXEC(N'ALTER TABLE dbo.TareasMantenimiento ADD CONSTRAINT FK_TareasMantenimiento_Grupo FOREIGN KEY (GrupoId) REFERENCES gaillard.Grupo(ID)');

    IF OBJECT_ID('gaillard.Vehiculo', 'U') IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_TareasMantenimiento_Vehiculo' AND parent_object_id = OBJECT_ID('dbo.TareasMantenimiento'))
        EXEC(N'ALTER TABLE dbo.TareasMantenimiento ADD CONSTRAINT FK_TareasMantenimiento_Vehiculo FOREIGN KEY (VehiculoId) REFERENCES gaillard.Vehiculo(ID)');
END";

                    using (var command = new SqlCommand(ensureAssetColumns, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    var createPhotosTable = @"
IF OBJECT_ID('dbo.TareaMantenimientoFotos', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.TareaMantenimientoFotos
    (
        ID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_TareaMantenimientoFotos PRIMARY KEY,
        TareaMantenimientoID INT NOT NULL,
        RutaArchivo NVARCHAR(260) NOT NULL,
        NombreOriginal NVARCHAR(255) NULL,
        ContentType NVARCHAR(100) NULL,
        FechaCreacion DATETIME NOT NULL,
        CONSTRAINT FK_TareaMantenimientoFotos_TareasMantenimiento
            FOREIGN KEY (TareaMantenimientoID) REFERENCES dbo.TareasMantenimiento(ID) ON DELETE CASCADE
    );

    CREATE INDEX IX_TareaMantenimientoFotos_TareaMantenimientoID
        ON dbo.TareaMantenimientoFotos(TareaMantenimientoID);
END";

                    using (var command = new SqlCommand(createPhotosTable, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    var createAccessoriesTable = @"
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

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_TareaMantenimientoAccesorios_TareaMantenimientoID' AND object_id = OBJECT_ID('dbo.TareaMantenimientoAccesorios'))
BEGIN
    CREATE INDEX IX_TareaMantenimientoAccesorios_TareaMantenimientoID
        ON dbo.TareaMantenimientoAccesorios(TareaMantenimientoID);
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_TareaMantenimientoAccesorios_AccesorioID' AND object_id = OBJECT_ID('dbo.TareaMantenimientoAccesorios'))
BEGIN
    CREATE INDEX IX_TareaMantenimientoAccesorios_AccesorioID
        ON dbo.TareaMantenimientoAccesorios(AccesorioID);
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_TareaMantenimientoAccesorios_Tarea_Accesorio' AND object_id = OBJECT_ID('dbo.TareaMantenimientoAccesorios'))
BEGIN
    CREATE UNIQUE INDEX UX_TareaMantenimientoAccesorios_Tarea_Accesorio
        ON dbo.TareaMantenimientoAccesorios(TareaMantenimientoID, AccesorioID);
END

IF OBJECT_ID('gaillard.Accesorio', 'U') IS NOT NULL
    AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_TareaMantenimientoAccesorios_Accesorio')
BEGIN
    ALTER TABLE dbo.TareaMantenimientoAccesorios
    ADD CONSTRAINT FK_TareaMantenimientoAccesorios_Accesorio
        FOREIGN KEY (AccesorioID) REFERENCES gaillard.Accesorio(ID);
END

IF OBJECT_ID('gaillard.PlanMantenimiento', 'U') IS NOT NULL
    AND COL_LENGTH('gaillard.PlanMantenimiento', 'AccesorioGrupoID') IS NULL
BEGIN
    ALTER TABLE gaillard.PlanMantenimiento
    ADD AccesorioGrupoID INT NULL;
END

IF OBJECT_ID('gaillard.PlanMantenimiento', 'U') IS NOT NULL
    AND COL_LENGTH('gaillard.PlanMantenimiento', 'AccesorioGrupoID') IS NOT NULL
    AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PlanMantenimiento_AccesorioGrupoID' AND object_id = OBJECT_ID('gaillard.PlanMantenimiento'))
BEGIN
    CREATE INDEX IX_PlanMantenimiento_AccesorioGrupoID
        ON gaillard.PlanMantenimiento(AccesorioGrupoID);
END

IF OBJECT_ID('gaillard.PlanMantenimiento', 'U') IS NOT NULL
    AND OBJECT_ID('gaillard.AccesorioGrupo', 'U') IS NOT NULL
    AND COL_LENGTH('gaillard.PlanMantenimiento', 'AccesorioGrupoID') IS NOT NULL
    AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_PlanMantenimiento_AccesorioGrupo')
BEGIN
    ALTER TABLE gaillard.PlanMantenimiento
    ADD CONSTRAINT FK_PlanMantenimiento_AccesorioGrupo
        FOREIGN KEY (AccesorioGrupoID) REFERENCES gaillard.AccesorioGrupo(ID);
END

IF OBJECT_ID('gaillard.AccesorioGrupo', 'U') IS NOT NULL
    AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AccesorioGrupo_GrupoID_AccesorioID' AND object_id = OBJECT_ID('gaillard.AccesorioGrupo'))
BEGIN
    CREATE INDEX IX_AccesorioGrupo_GrupoID_AccesorioID
        ON gaillard.AccesorioGrupo(GrupoID, AccesorioID);
END

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
    );";

                    using (var command = new SqlCommand(createAccessoriesTable, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                schemaVerified = true;
            }
        }
    }
}
