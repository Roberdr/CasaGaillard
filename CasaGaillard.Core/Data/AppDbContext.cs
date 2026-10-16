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
        public DbSet<Instalacion> Instalaciones { get; set; } = null!;
        public DbSet<TipoInstalacion> TiposInstalacion { get; set; } = null!;
        public DbSet<Ubicacion> Ubicaciones { get; set; } = null!;

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

            modelBuilder.Entity<TipoInstalacion>(eb =>
            {
                eb.ToTable("TipoInstalacion", "gaillard");
                eb.HasKey(tipo => tipo.Id);
                eb.Property(tipo => tipo.Nombre).HasMaxLength(100).IsRequired();
            });

            modelBuilder.Entity<Ubicacion>(eb =>
            {
                eb.ToTable("Ubicacion", "gaillard");
                eb.HasKey(ubicacion => ubicacion.UbicacionId);
                eb.Property(ubicacion => ubicacion.Nombre).HasMaxLength(150).IsRequired();
                eb.Property(ubicacion => ubicacion.Descripcion).HasMaxLength(300);
            });

            modelBuilder.Entity<Instalacion>(eb =>
            {
                eb.ToTable("Instalacion", "gaillard");
                eb.HasKey(instalacion => instalacion.ID);
                eb.Property(instalacion => instalacion.ID).HasColumnName("Id");
                eb.Property(instalacion => instalacion.Codigo).HasMaxLength(50).IsRequired();
                eb.Property(instalacion => instalacion.Nombre).HasMaxLength(255).IsRequired();
                eb.Property(instalacion => instalacion.Estado).HasMaxLength(20).IsRequired();
                eb.Property(instalacion => instalacion.CodigoPlano).HasMaxLength(50);
                eb.Property(instalacion => instalacion.FechaPuestaMarcha).HasColumnType("date");
                eb.Property(instalacion => instalacion.FechaBaja).HasColumnType("date");
                eb.HasOne(instalacion => instalacion.TipoInstalacion)
                    .WithMany(tipo => tipo.Instalaciones)
                    .HasForeignKey(instalacion => instalacion.TipoInstalacionId)
                    .OnDelete(DeleteBehavior.Restrict);
                eb.HasOne(instalacion => instalacion.Ubicacion)
                    .WithMany(ubicacion => ubicacion.Instalaciones)
                    .HasForeignKey(instalacion => instalacion.UbicacionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
