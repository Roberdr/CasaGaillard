using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CasaGaillard.Core.Models;

public sealed class IntervencionesIndexViewModel
{
    public TareaMantenimientoResumen? Tarea { get; set; }
    public bool EsGlobal => Tarea is null;
    public IReadOnlyList<IntervencionFilaViewModel> Intervenciones { get; set; } = [];
}

public sealed class TareaMantenimientoResumen
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
}

public class IntervencionFilaViewModel
{
    public int Id { get; set; }
    public int TareaMantenimientoId { get; set; }
    public string TareaCodigo { get; set; } = string.Empty;
    public string TareaTitulo { get; set; } = string.Empty;
    public string? Tecnico { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public decimal? HorasEmpleadas { get; set; }
    public decimal? CosteManoObra { get; set; }
}

public sealed class IntervencionFormViewModel
{
    public int Id { get; set; }
    [Required]
    public int TareaMantenimientoId { get; set; }
    [Required(ErrorMessage = "Selecciona un técnico.")]
    public int? TecnicoPersonaID { get; set; }
    [NotMapped]
    public IReadOnlyList<SelectListItem> Personas { get; set; } = [];
    [Required, Display(Name = "Inicio"), DataType(DataType.DateTime)]
    public DateTime FechaInicio { get; set; }
    [Display(Name = "Fin"), DataType(DataType.DateTime)]
    public DateTime? FechaFin { get; set; }
    [Required, StringLength(2000), DataType(DataType.MultilineText)]
    public string Descripcion { get; set; } = string.Empty;
    [DecimalRange("0", "99999999")]
    public decimal? HorasEmpleadas { get; set; }
    [DecimalRange("0", "9999999999")]
    public decimal? CosteManoObra { get; set; }
    public string TareaCodigo { get; set; } = string.Empty;
    public string TareaTitulo { get; set; } = string.Empty;
}

public sealed class IntervencionDetailsViewModel : IntervencionFilaViewModel
{
    public IReadOnlyList<IntervencionRepuestoViewModel> Repuestos { get; set; } = [];
    public IntervencionRepuestoFormViewModel RepuestoForm { get; set; } = new();
}

public sealed class IntervencionRepuestoViewModel
{
    public int Id { get; set; }
    public int IntervencionId { get; set; }
    public int AccesorioId { get; set; }
    public string AccesorioNombre { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal? CosteUnitario { get; set; }
    public string? Observaciones { get; set; }
}

public sealed class IntervencionRepuestoFormViewModel
{
    [Required]
    public int IntervencionId { get; set; }
    [Required, Display(Name = "Repuesto")]
    public int AccesorioId { get; set; }
    [DecimalRange("0.001", "999999999")]
    public decimal Cantidad { get; set; } = 1;
    [DecimalRange("0", "9999999999"), Display(Name = "Coste unitario")]
    public decimal? CosteUnitario { get; set; }
    [StringLength(500)]
    public string? Observaciones { get; set; }
    public IReadOnlyList<SelectListItem> Accesorios { get; set; } = [];
}
