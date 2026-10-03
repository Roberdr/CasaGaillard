using Microsoft.EntityFrameworkCore;
using CasaGaillard.Core.Models;

namespace CasaGaillard.Core.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<IntervencionViewModel> Intervenciones { get; set; } = null!;
        public DbSet<Vehiculo> Vehiculos { get; set; } = null!;
        public DbSet<TipoVehiculo> TiposVehiculo { get; set; } = null!;
        public DbSet<Entidad> Entidads { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map minimal model for initial compilation. Detailed mapping will be added later.
            modelBuilder.Entity<IntervencionViewModel>(eb =>
            {
                eb.HasKey(e => e.Id);
                eb.Property(e => e.Titulo).HasMaxLength(200);
            });

            // Vehiculo minimal mapping
            modelBuilder.Entity<Vehiculo>(eb =>
            {
                eb.HasKey(e => e.ID);
                eb.Property(e => e.MatriculaVehiculo).HasMaxLength(20);
                eb.Property(e => e.Marca).HasMaxLength(100);
                eb.Property(e => e.Modelo).HasMaxLength(100);
                eb.HasOne(e => e.TipoVehiculo).WithMany().HasForeignKey(e => e.TipoVehiculoID).OnDelete(DeleteBehavior.Restrict);
                eb.HasOne(e => e.Taller).WithMany().HasForeignKey(e => e.TallerHabitualID).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TipoVehiculo>(eb =>
            {
                eb.HasKey(e => e.ID);
                eb.Property(e => e.Vehiculo).HasMaxLength(100);
            });

            modelBuilder.Entity<Entidad>(eb =>
            {
                eb.HasKey(e => e.ID);
                eb.Property(e => e.NombreEntidad).HasMaxLength(200);
            });
        }
    }
}
