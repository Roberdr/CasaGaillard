using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CasaGaillard.Core.Models;

public sealed class InstalacionFilaViewModel
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? TipoNombre { get; set; }
    public string? UbicacionNombre { get; set; }
    public string Estado { get; set; } = string.Empty;
    public byte Criticidad { get; set; }
    public int NumeroEquipos { get; set; }
}

public sealed class InstalacionFormViewModel
{
    public int Id { get; set; }
    [Required, StringLength(50), Display(Name = "Código")]
    public string Codigo { get; set; } = string.Empty;
    [Required, StringLength(255), Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;
    [StringLength(2000), Display(Name = "Descripción")]
    public string? Descripcion { get; set; }
    [Display(Name = "Tipo de instalación")]
    public int? TipoInstalacionId { get; set; }
    [Display(Name = "Ubicación")]
    public int? UbicacionId { get; set; }
    [Display(Name = "Responsable")]
    public int? ResponsablePersonaId { get; set; }
    [Display(Name = "Fecha de puesta en marcha"), DataType(DataType.Date)]
    public DateTime? FechaPuestaMarcha { get; set; }
    [Display(Name = "Fecha de baja"), DataType(DataType.Date)]
    public DateTime? FechaBaja { get; set; }
    [Range(1, 5), Display(Name = "Criticidad (1 a 5)")]
    public byte Criticidad { get; set; } = 3;
    [Required, StringLength(20)]
    public string Estado { get; set; } = "ACTIVA";
    [StringLength(50), Display(Name = "Código de plano")]
    public string? CodigoPlano { get; set; }
    [Display(Name = "Crítica para producción")]
    public bool CriticaProduccion { get; set; }
    [Display(Name = "Crítica para seguridad")]
    public bool CriticaSeguridad { get; set; }
    [StringLength(4000)]
    public string? Observaciones { get; set; }
    public IReadOnlyList<SelectListItem> Tipos { get; set; } = [];
    public IReadOnlyList<SelectListItem> Ubicaciones { get; set; } = [];
    public IReadOnlyList<SelectListItem> Responsables { get; set; } = [];
}
