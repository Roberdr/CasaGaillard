using System.Data.Entity;

namespace CasaGaillard.Models
{
    public partial class GaillardEntities
    {
        // Forwarding properties with los nombres que espera el código existente
        public virtual DbSet<Direccion> Direcciones
        {
            get { return this.Direccions; }
            set { this.Direccions = value; }
        }

        public virtual DbSet<Cargo> Cargos
        {
            get { return this.Cargoes; }
            set { this.Cargoes = value; }
        }

        public virtual DbSet<TelefonoPersona> TelefonosPersona
        {
            get { return this.TelefonoPersonas; }
            set { this.TelefonoPersonas = value; }
        }

        public virtual DbSet<TelefonoEntidad> TelefonosEntidad
        {
            get { return this.TelefonoEntidads; }
            set { this.TelefonoEntidads = value; }
        }

        public virtual DbSet<PersonasEntidad> PersonasEntidad
        {
            get { return this.PersonasEntidads; }
            set { this.PersonasEntidads = value; }
        }

        public virtual DbSet<Poblacion> Poblaciones
        {
            get { return this.Poblacions; }
            set { this.Poblacions = value; }
        }
        public virtual DbSet<Situacion> Situacion
        {
            get { return this.Situaciones; }
            set { this.Situaciones = value; }
        }
    }
}
