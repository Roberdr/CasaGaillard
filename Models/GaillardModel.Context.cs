﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CasaGaillard.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class GaillardEntities : DbContext
    {
        public GaillardEntities()
            : base("name=GaillardEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Accesorio> Accesorios { get; set; }
        public virtual DbSet<AccesorioGrupo> AccesoriosGrupo { get; set; }
        public virtual DbSet<CaracteristicaAccesorio> CaracteristicasAccesorio { get; set; }
        public virtual DbSet<Compartimento> Compartimentos { get; set; }
        public virtual DbSet<Cuba> Cubas { get; set; }
        public virtual DbSet<DetalleAccesorio> DetallesAccesorio { get; set; }
        public virtual DbSet<Grupo> Grupos { get; set; }
        public virtual DbSet<MantenimientoVehiculo> MantenimientosVehiculo { get; set; }
        public virtual DbSet<Material> Materiales { get; set; }
        public virtual DbSet<Operacion> Operaciones { get; set; }
        public virtual DbSet<Operario> Operarios { get; set; }
        public virtual DbSet<Plataforma> Plataformas { get; set; }
        public virtual DbSet<Producto> Productos { get; set; }
        public virtual DbSet<Revision> Revisiones { get; set; }
        public virtual DbSet<RevisionVehiculo> RevisionesVehiculo { get; set; }
        public virtual DbSet<Situacion> Situaciones { get; set; }
        public virtual DbSet<TipoAccesorio> TiposAccesorio { get; set; }
        public virtual DbSet<TipoGrupo> TiposGrupo { get; set; }
        public virtual DbSet<TipoMantenimiento> TiposMantenimiento { get; set; }
        public virtual DbSet<TipoOperacion> TiposOperacion { get; set; }
        public virtual DbSet<TipoRevision> TiposRevision { get; set; }
        public virtual DbSet<TipoVehiculo> TiposVehiculo { get; set; }
        public virtual DbSet<Unidad> Unidades { get; set; }
        public virtual DbSet<Vehiculo> Vehiculos { get; set; }
        public virtual DbSet<Cargo> Cargoes { get; set; }
        public virtual DbSet<Direccion> Direccions { get; set; }
        public virtual DbSet<Entidad> Entidads { get; set; }
        public virtual DbSet<Persona> Personas { get; set; }
        public virtual DbSet<PersonasEntidad> PersonasEntidads { get; set; }
        public virtual DbSet<Poblacion> Poblacions { get; set; }
        public virtual DbSet<TelefonoEntidad> TelefonoEntidads { get; set; }
        public virtual DbSet<TelefonoPersona> TelefonoPersonas { get; set; }
        public virtual DbSet<TipoVia> TipoVias { get; set; }
    }
}
