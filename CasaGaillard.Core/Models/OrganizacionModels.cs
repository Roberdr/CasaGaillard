using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CasaGaillard.Core.Models;

public sealed class DireccionRow
{
    public int Id { get; set; }
    public string? TipoVia { get; set; }
    public string? NombreVia { get; set; }
    public int? Numero { get; set; }
    public string? CodigoPostal { get; set; }
    public string? Poblacion { get; set; }
    public string? Pais { get; set; }
}

public sealed class DireccionForm
{
    public int Id { get; set; }
    [Range(1, int.MaxValue), Display(Name = "Tipo de vía")] public int TipoViaId { get; set; }
    [StringLength(30), Display(Name = "Vía")] public string? NombreVia { get; set; }
    [Display(Name = "Distrito")] public string? Distrito { get; set; }
    [Display(Name = "Número")] public int? Numero { get; set; }
    public string? Escalera { get; set; }
    public string? Piso { get; set; }
    public string? Puerta { get; set; }
    [Range(1, int.MaxValue), Display(Name = "Población")] public int PoblacionId { get; set; }
    [StringLength(30), Display(Name = "Nueva población")]
    public string? NuevaPoblacion { get; set; }
    [Display(Name = "Código postal")] public string? CodigoPostal { get; set; }
    public string? Pais { get; set; }
    public int? PersonaId { get; set; }
    public bool ParaEntidad { get; set; }
    public string? TipoViaNombre { get; set; }
    public string? PoblacionNombre { get; set; }
    public List<SelectListItem> TiposVia { get; set; } = [];
    public List<SelectListItem> Poblaciones { get; set; } = [];
}

public sealed class PersonaRow
{
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public string? Apellido1 { get; set; }
    public string? Apellido2 { get; set; }
    public string? Nif { get; set; }
    public string? Direccion { get; set; }
}

public sealed class PersonaForm
{
    public int Id { get; set; }
    [Required, StringLength(50), Display(Name = "Nombre")] public string? Nombre { get; set; }
    [Required, StringLength(50), Display(Name = "Primer apellido")] public string? Apellido1 { get; set; }
    [Display(Name = "Segundo apellido")] public string? Apellido2 { get; set; }
    [Required, StringLength(10), Display(Name = "NIF")] public string? Nif { get; set; }
    [Display(Name = "Dirección")] public int? DireccionId { get; set; }
    public string? DireccionTexto { get; set; }
    public List<SelectListItem> Direcciones { get; set; } = [];
    public List<OrganizacionTelefono> Telefonos { get; set; } = [];
    public List<PersonaRelacion> Relaciones { get; set; } = [];
    public List<SelectListItem> Entidades { get; set; } = [];
    public List<SelectListItem> Cargos { get; set; } = [];
}

public sealed class EntidadRow
{
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public string? Cif { get; set; }
    public string? Direccion { get; set; }
}

public sealed class EntidadForm
{
    public int Id { get; set; }
    [Required, StringLength(60), Display(Name = "Entidad")] public string? Nombre { get; set; }
    [Display(Name = "CIF")] public string? Cif { get; set; }
    [Range(1, int.MaxValue), Display(Name = "Dirección")] public int DireccionId { get; set; }
    public string? DireccionTexto { get; set; }
    public List<SelectListItem> Direcciones { get; set; } = [];
    public List<OrganizacionTelefono> Telefonos { get; set; } = [];
    public List<PersonaRelacion> Relaciones { get; set; } = [];
    public List<SelectListItem> Personas { get; set; } = [];
    public List<SelectListItem> Cargos { get; set; } = [];
}

public sealed class OrganizacionTelefono
{
    public int Id { get; set; }
    public string? Numero { get; set; }
    public int? PersonaId { get; set; }
    public int? EntidadId { get; set; }
    public string? PersonaNombre { get; set; }
    public string? EntidadNombre { get; set; }
}

public sealed class PersonaRelacion
{
    public int Id { get; set; }
    public int? PersonaId { get; set; }
    public int? EntidadId { get; set; }
    public string? Nombre { get; set; }
    public string? Cargo { get; set; }
    public int? CargoId { get; set; }
    public DateTime? FechaAlta { get; set; }
    public DateTime? FechaBaja { get; set; }
    public bool Activa => !FechaBaja.HasValue;
}
