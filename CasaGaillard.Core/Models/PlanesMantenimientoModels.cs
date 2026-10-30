using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CasaGaillard.Core.Models;

public sealed class PlanMantenimientoFilaViewModel
{
    public int PlanId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Tipo { get; set; }
    public string? EquipoNombre { get; set; }
    public string? AccesorioNombre { get; set; }
    public string? AccesorioTipo { get; set; }
    public int PeriodicidadDias { get; set; }
    public DateTime? ProximaEjecucion { get; set; }
    public bool Activo { get; set; }
}

public class PlanMantenimientoFormViewModel
{
    public int PlanId { get; set; }
    [Required, StringLength(150)] public string Nombre { get; set; } = string.Empty;
    [StringLength(1000)] public string? Descripcion { get; set; }
    [StringLength(30)] public string? Tipo { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "La periodicidad debe ser mayor que cero.")] public int PeriodicidadDias { get; set; } = 365;
    [DataType(DataType.Date)] public DateTime? UltimaEjecucion { get; set; } = DateTime.Today;
    [DataType(DataType.Date)] public DateTime? ProximaEjecucion { get; set; } = DateTime.Today.AddDays(365);
    public int? AccesorioId { get; set; }
    public int? AccesorioGrupoID { get; set; }
    public int? EquipoId { get; set; }
    public int? CubaId { get; set; }
    public int? VehiculoId { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "La frecuencia debe ser mayor que cero.")] public int? FrecuenciaValor { get; set; }
    [StringLength(20)] public string? FrecuenciaUnidad { get; set; }
    [Range(0, int.MaxValue)] public int? DuracionEstimadaMinutos { get; set; }
    public bool Activo { get; set; } = true;
    public IReadOnlyList<SelectListItem> Accesorios { get; set; } = [];
    public IReadOnlyList<SelectListItem> Equipos { get; set; } = [];
    public IReadOnlyList<SelectListItem> Cubas { get; set; } = [];
    public IReadOnlyList<SelectListItem> Vehiculos { get; set; } = [];
    public IReadOnlyList<SelectListItem> GruposAccesorio { get; set; } = [];
}

public sealed class PlanMantenimientoDetalleViewModel : PlanMantenimientoFormViewModel
{
    public string? EquipoNombre { get; set; }
    public string? AccesorioNombre { get; set; }
    public string? AccesorioTipo { get; set; }
    public string? AccesorioGrupoTexto { get; set; }
}
