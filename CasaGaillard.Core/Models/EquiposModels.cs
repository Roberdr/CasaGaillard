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
