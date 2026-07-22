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
                }

                schemaVerified = true;
            }
        }
    }
}
