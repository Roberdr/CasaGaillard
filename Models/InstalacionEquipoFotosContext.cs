using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;

namespace CasaGaillard.Models
{
    public class InstalacionEquipoFotosContext : DbContext
    {
        private static readonly object SyncRoot = new object();
        private static bool schemaVerified;

        public InstalacionEquipoFotosContext() : base("name=ApplicationDbContext")
        {
            Database.SetInitializer<InstalacionEquipoFotosContext>(null);
            EnsureSchema();
        }

        public DbSet<InstalacionEquipoFoto> Fotos { get; set; }

        private static void EnsureSchema()
        {
            if (schemaVerified) return;
            lock (SyncRoot)
            {
                if (schemaVerified) return;
                var connectionString = ConfigurationManager.ConnectionStrings["ApplicationDbContext"]?.ConnectionString;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    const string sql = @"
IF OBJECT_ID('dbo.InstalacionEquipoFotos', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.InstalacionEquipoFotos
    (
        ID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_InstalacionEquipoFotos PRIMARY KEY,
        TipoActivo NVARCHAR(20) NOT NULL,
        ActivoID INT NOT NULL,
        RutaArchivo NVARCHAR(260) NOT NULL,
        NombreOriginal NVARCHAR(255) NULL,
        ContentType NVARCHAR(100) NULL,
        FechaCreacion DATETIME NOT NULL
    );
    CREATE INDEX IX_InstalacionEquipoFotos_Activo ON dbo.InstalacionEquipoFotos(TipoActivo, ActivoID);
END";
                    using (var command = new SqlCommand(sql, connection)) command.ExecuteNonQuery();
                }
                schemaVerified = true;
            }
        }
    }
}
