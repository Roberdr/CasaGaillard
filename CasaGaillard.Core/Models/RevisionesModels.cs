using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CasaGaillard.Core.Models;

public class RevisionHistorialItemViewModel
{
    public int Id { get; set; }
    public int CubaId { get; set; }
    public string MatriculaCuba { get; set; } = string.Empty;
    public DateTime FechaRevision { get; set; }
    public string? Descripcion { get; set; }
    public DateTime? ValidaHasta { get; set; }
    public string? DescripcionProxima { get; set; }
    public string? Autorizado { get; set; }
}

public sealed class RevisionCubaFilaViewModel
{
    public int CubaId { get; set; }
    public string MatriculaCuba { get; set; } = string.Empty;
    public bool CubaBaja { get; set; }
    public RevisionHistorialItemViewModel? UltimaRevision { get; set; }
    public IReadOnlyList<RevisionHistorialItemViewModel> Historial { get; set; } = [];
}

public sealed class RevisionesIndexViewModel
{
    public int? CubaId { get; set; }
    public string? MatriculaCuba { get; set; }
    public bool IncluirBajas { get; set; }
    public IReadOnlyList<SelectListItem> Cubas { get; set; } = [];
    public IReadOnlyList<RevisionCubaFilaViewModel> Revisiones { get; set; } = [];
}

public sealed class RevisionFormViewModel
{
    public int Id { get; set; }
    [Required, Display(Name = "Cuba")]
    public int CubaId { get; set; }
    [Required, DataType(DataType.Date), Display(Name = "Fecha de revisión")]
    public DateTime FechaRevision { get; set; } = DateTime.Today;
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }
    [DataType(DataType.Date), Display(Name = "Válida hasta")]
    public DateTime? ValidaHasta { get; set; }
    [Display(Name = "Descripción próxima")]
    public string? DescripcionProxima { get; set; }
    [Display(Name = "Persona y entidad autorizante")]
    public int? AutorizadoPersonasEntidadId { get; set; }
    public string? Autorizado { get; set; }
    public IReadOnlyList<SelectListItem> Cubas { get; set; } = [];
    public IReadOnlyList<SelectListItem> PersonasEntidad { get; set; } = [];
}

public sealed class RevisionDetalleViewModel : RevisionHistorialItemViewModel
{
    public int? AutorizadoPersonasEntidadId { get; set; }
}
