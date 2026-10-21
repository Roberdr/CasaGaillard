using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CasaGaillard.Core.Models;

public sealed class EquipoFilaViewModel
{
    public int Id { get; set; }
    public int InstalacionId { get; set; }
    public string InstalacionNombre { get; set; } = string.Empty;
    public int Nivel { get; set; }
    public string TipoElemento { get; set; } = string.Empty;
    public string? PadreNombre { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Fabricante { get; set; }
    public string? Modelo { get; set; }
    public string? NumeroSerie { get; set; }
    public bool Activo { get; set; }
}

public sealed class EquipoDetailsViewModel
{
    public EquipoFormViewModel Equipo { get; set; } = new();
    public string InstalacionNombre { get; set; } = string.Empty;
    public IReadOnlyList<EquipoTareaViewModel> Tareas { get; set; } = [];
    public IReadOnlyList<EquipoPlanViewModel> Planes { get; set; } = [];
    public IReadOnlyList<EquipoIntervencionViewModel> Intervenciones { get; set; } = [];
    public IReadOnlyList<InstalacionEquipoFotoFilaViewModel> Fotos { get; set; } = [];
    public IReadOnlyList<InstalacionDocumentoFilaViewModel> Documentos { get; set; } = [];
}

public sealed class EquipoTareaViewModel
{
    public int Id { get; set; } public string Codigo { get; set; } = string.Empty; public string Titulo { get; set; } = string.Empty;
    public string? Estado { get; set; } public string? Prioridad { get; set; } public string? AsignadaA { get; set; }
    public DateTime FechaCreacion { get; set; } public DateTime? FechaProgramada { get; set; }
}

public sealed class EquipoPlanViewModel
{
    public int PlanId { get; set; } public string Nombre { get; set; } = string.Empty; public string? Tipo { get; set; }
    public int PeriodicidadDias { get; set; } public DateTime? UltimaEjecucion { get; set; } public DateTime? ProximaEjecucion { get; set; }
    public bool Activo { get; set; }
}

public sealed class EquipoIntervencionViewModel
{
    public int Id { get; set; } public int TareaMantenimientoId { get; set; } public string TareaCodigo { get; set; } = string.Empty;
    public string TareaTitulo { get; set; } = string.Empty; public string? Tecnico { get; set; } public DateTime FechaInicio { get; set; }
    public string Descripcion { get; set; } = string.Empty; public decimal? HorasEmpleadas { get; set; } public decimal CosteTotal { get; set; }
}

public sealed class EquipoFormViewModel
{
    public int Id { get; set; }
    [Required, Display(Name = "Instalación")]
    public int InstalacionId { get; set; }
    [Display(Name = "Equipo padre")]
    public int? EquipoPadreId { get; set; }
    [Required, StringLength(30), Display(Name = "Tipo de elemento")]
    public string TipoElemento { get; set; } = "EQUIPO";
    [Required, StringLength(50), Display(Name = "Código")]
    public string Codigo { get; set; } = string.Empty;
    [Required, StringLength(255), Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;
    [StringLength(2000), Display(Name = "Descripción")]
    public string? Descripcion { get; set; }
    [StringLength(150), Display(Name = "Fabricante")] public string? Fabricante { get; set; }
    [StringLength(150), Display(Name = "Modelo")] public string? Modelo { get; set; }
    [StringLength(100), Display(Name = "Número de serie")] public string? NumeroSerie { get; set; }
    [DecimalRange("0", "999999999999999"), Display(Name = "Potencia")] public decimal? Potencia { get; set; }
    [DecimalRange("0", "999999999999999"), Display(Name = "Caudal")] public decimal? Caudal { get; set; }
    [DecimalRange("0", "999999999999999"), Display(Name = "Presión")] public decimal? Presion { get; set; }
    [DecimalRange("0", "999999999999999"), Display(Name = "Voltaje")] public decimal? Voltaje { get; set; }
    [Display(Name = "Fecha de instalación"), DataType(DataType.Date)] public DateTime? FechaInstalacion { get; set; }
    [Display(Name = "Fecha de baja"), DataType(DataType.Date)] public DateTime? FechaBaja { get; set; }
    [Display(Name = "Activo")] public bool Activo { get; set; } = true;
    [StringLength(4000), Display(Name = "Observaciones")] public string? Observaciones { get; set; }
    public IReadOnlyList<SelectListItem> Instalaciones { get; set; } = [];
    public IReadOnlyList<SelectListItem> EquiposPadre { get; set; } = [];
    public IReadOnlyList<SelectListItem> TiposElemento { get; set; } = [];
}
