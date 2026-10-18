using Microsoft.EntityFrameworkCore;
using CasaGaillard.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CasaGaillard.Core.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
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
        public DbSet<TareaMantenimiento> TareasMantenimiento { get; set; } = null!;
        public DbSet<TareaMantenimientoFoto> TareaMantenimientoFotos { get; set; } = null!;
        public DbSet<TareaMantenimientoAccesorio> TareaMantenimientoAccesorios { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ASP.NET Identity 2 in MVC uses the default dbo schema. A separate,
            // empty set of Identity tables exists in gaillard, so Core must use dbo.
            modelBuilder.Entity<IdentityUser>().ToTable("AspNetUsers", "dbo");
            modelBuilder.Entity<IdentityRole>().ToTable("AspNetRoles", "dbo");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles", "dbo");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims", "dbo");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins", "dbo");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims", "dbo");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens", "dbo");

            modelBuilder.Entity<IntervencionViewModel>(eb =>
            {
                eb.HasKey(e => e.Id);
                eb.Property(e => e.Titulo).HasMaxLength(200);
            });

            // Vehiculo minimal mapping
            modelBuilder.Entity<Vehiculo>(eb =>
            {
                eb.ToTable("Vehiculo", "gaillard");
                eb.HasKey(e => e.ID);
                eb.Property(e => e.MatriculaVehiculo).HasMaxLength(20);
                eb.Property(e => e.Marca).HasMaxLength(100);
                eb.Property(e => e.Modelo).HasMaxLength(100);
                eb.HasOne(e => e.TipoVehiculo).WithMany().HasForeignKey(e => e.TipoVehiculoID).OnDelete(DeleteBehavior.Restrict);
                eb.HasOne(e => e.Taller).WithMany().HasForeignKey(e => e.TallerHabitualID).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TipoVehiculo>(eb =>
            {
                eb.ToTable("TipoVehiculo", "gaillard");
                eb.HasKey(e => e.ID);
                eb.Property(e => e.Vehiculo).HasMaxLength(100);
            });

            modelBuilder.Entity<Entidad>(eb =>
            {
                eb.ToTable("Entidad", "gaillard");
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

            modelBuilder.Entity<TareaMantenimiento>(eb =>
            {
                eb.ToTable("TareasMantenimiento", "dbo");
                eb.HasKey(tarea => tarea.ID);
                eb.Property(tarea => tarea.Codigo).HasMaxLength(30).IsRequired();
                eb.HasIndex(tarea => tarea.Codigo).IsUnique();
                eb.Property(tarea => tarea.Titulo).HasMaxLength(150).IsRequired();
                eb.Property(tarea => tarea.Descripcion).HasMaxLength(2000).IsRequired();
                eb.Property(tarea => tarea.Ubicacion).HasMaxLength(150);
                eb.Property(tarea => tarea.Equipo).HasMaxLength(150);
                eb.Property(tarea => tarea.DetectadoPor).HasMaxLength(150);
                eb.Property(tarea => tarea.Contacto).HasMaxLength(150);
                eb.Property(tarea => tarea.Prioridad).HasMaxLength(20);
                eb.Property(tarea => tarea.Estado).HasMaxLength(30);
                eb.Property(tarea => tarea.AsignadaA).HasMaxLength(150);
                eb.Property(tarea => tarea.EmpresaExterior).HasMaxLength(150);
                eb.Property(tarea => tarea.AccionesARealizar).HasMaxLength(2000);
                eb.Property(tarea => tarea.AccesoriosNecesarios).HasMaxLength(4000);
                eb.Property(tarea => tarea.Observaciones).HasMaxLength(2000);
                eb.Property(tarea => tarea.CreadoPor).HasMaxLength(100);
                eb.Property(tarea => tarea.ActualizadoPor).HasMaxLength(100);
            });

            modelBuilder.Entity<TareaMantenimientoFoto>(eb =>
            {
                eb.ToTable("TareaMantenimientoFotos", "dbo");
                eb.HasKey(foto => foto.ID);
                eb.Property(foto => foto.RutaArchivo).HasMaxLength(260).IsRequired();
                eb.Property(foto => foto.NombreOriginal).HasMaxLength(255);
                eb.Property(foto => foto.ContentType).HasMaxLength(100);
                eb.HasOne<TareaMantenimiento>().WithMany().HasForeignKey(foto => foto.TareaMantenimientoID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TareaMantenimientoAccesorio>(eb =>
            {
                eb.ToTable("TareaMantenimientoAccesorios", "dbo");
                eb.HasKey(accesorio => accesorio.ID);
                eb.Property(accesorio => accesorio.Observaciones).HasMaxLength(500);
                eb.HasOne<TareaMantenimiento>().WithMany().HasForeignKey(accesorio => accesorio.TareaMantenimientoID)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
