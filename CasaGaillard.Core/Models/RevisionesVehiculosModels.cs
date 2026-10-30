using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CasaGaillard.Core.Models;

public sealed class RevisionVehiculoFilaViewModel
{
    public int Id { get; set; }
    public int VehiculoId { get; set; }
    public string Matricula { get; set; } = "";
    public int TipoRevisionId { get; set; }
    public string TipoRevision { get; set; } = "";
    public DateTime? FechaRevision { get; set; }
    public DateTime? Caducidad { get; set; }
    public string? Detalles { get; set; }
    public string? Ejecutor { get; set; }
    public DateTime? FechaReservada { get; set; }
}

public sealed class RevisionVehiculoFormViewModel
{
    public int Id { get; set; }
    [Required, Display(Name = "Vehículo")] public int? VehiculoId { get; set; }
    [Required, Display(Name = "Tipo de revisión")] public int? TipoRevisionId { get; set; }
    [DataType(DataType.Date), Display(Name = "Fecha de revisión")] public DateTime? FechaRevision { get; set; }
    [DataType(DataType.Date), Display(Name = "Caducidad")] public DateTime? Caducidad { get; set; }
    [StringLength(4000), Display(Name = "Detalles")] public string? Detalles { get; set; }
    [Display(Name = "Ejecutor")] public int? EjecutorPersonasEntidadId { get; set; }
    public string? Ejecutor { get; set; }
    public IReadOnlyList<SelectListItem> Vehiculos { get; set; } = [];
    public IReadOnlyList<SelectListItem> Tipos { get; set; } = [];
    public IReadOnlyList<SelectListItem> Ejecutores { get; set; } = [];
}
