using System;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;

namespace CasaGaillard.Models
{
    public class AccesoriosFotosContext : DbContext
    {
        private static readonly object SyncRoot = new object();
        private static bool schemaVerified;

        public AccesoriosFotosContext()
            : base("name=ApplicationDbContext")
        {
            Database.SetInitializer<AccesoriosFotosContext>(null);
            EnsureSchema();
        }

        public DbSet<AccesorioFoto> AccesorioFotos { get; set; }

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
IF OBJECT_ID('dbo.AccesorioFotos', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.AccesorioFotos
    (
        ID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AccesorioFotos PRIMARY KEY,
        AccesorioID INT NOT NULL,
        RutaArchivo NVARCHAR(260) NOT NULL,
        NombreOriginal NVARCHAR(255) NULL,
        ContentType NVARCHAR(100) NULL,
        FechaCreacion DATETIME NOT NULL
    );

    CREATE INDEX IX_AccesorioFotos_AccesorioID ON dbo.AccesorioFotos(AccesorioID);
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
