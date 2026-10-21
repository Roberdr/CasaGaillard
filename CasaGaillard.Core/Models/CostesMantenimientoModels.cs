using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CasaGaillard.Core.Models;

public sealed class CostesMantenimientoViewModel
{
    [Display(Name="Desde"),DataType(DataType.Date)] public DateTime? FechaDesde { get; set; }
    [Display(Name="Hasta"),DataType(DataType.Date)] public DateTime? FechaHasta { get; set; }
    [Display(Name="Instalación")] public int? InstalacionId { get; set; }
    [Display(Name="Equipo")] public int? EquipoId { get; set; }
    public IReadOnlyList<SelectListItem> Instalaciones { get; set; }=[];
    public IReadOnlyList<SelectListItem> Equipos { get; set; }=[];
    public IReadOnlyList<CosteActivoResumenViewModel> Resumen { get; set; }=[];
    public IReadOnlyList<CosteIntervencionViewModel> Intervenciones { get; set; }=[];
    public IReadOnlyList<CosteGraficoItemViewModel> CostePorMes { get; set; }=[];
    public IReadOnlyList<CosteGraficoItemViewModel> CostePorInstalacion { get; set; }=[];
    public IReadOnlyList<CosteGraficoItemViewModel> CostePorEquipo { get; set; }=[];
}

public sealed class CosteGraficoItemViewModel
{
    public string Etiqueta { get; set; }=string.Empty;
    public decimal Total { get; set; }
}

public sealed class CosteActivoResumenViewModel
{
    public int? InstalacionId { get; set; } public string? InstalacionCodigo { get; set; } public string InstalacionNombre { get; set; }=string.Empty;
    public int? EquipoId { get; set; } public string? EquipoCodigo { get; set; } public string EquipoNombre { get; set; }=string.Empty;
    public int NumeroIntervenciones { get; set; } public decimal HorasEmpleadas { get; set; } public decimal CosteManoObra { get; set; }
    public decimal CosteRepuestos { get; set; } public int RepuestosSinCoste { get; set; } public decimal CosteTotal { get; set; }
}

public sealed class CosteIntervencionViewModel
{
    public int Id { get; set; } public int TareaMantenimientoId { get; set; } public string TareaCodigo { get; set; }=string.Empty;
    public string TareaTitulo { get; set; }=string.Empty; public string InstalacionNombre { get; set; }=string.Empty;
    public string? EquipoCodigo { get; set; } public string? EquipoNombre { get; set; } public string? Tecnico { get; set; }
    public DateTime FechaInicio { get; set; } public DateTime? FechaFin { get; set; } public string? Descripcion { get; set; }
    public decimal? HorasEmpleadas { get; set; } public decimal CosteManoObra { get; set; } public decimal CosteRepuestos { get; set; }
    public int RepuestosSinCoste { get; set; } public decimal CosteTotal { get; set; }
}
