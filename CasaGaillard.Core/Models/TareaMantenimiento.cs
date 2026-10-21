using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CasaGaillard.Core.Models;

public sealed class TareaMantenimiento
{
    public int ID { get; set; }
    [StringLength(30)] public string Codigo { get; set; } = string.Empty;
    [StringLength(150)] public string Titulo { get; set; } = string.Empty;
    [StringLength(2000)] public string Descripcion { get; set; } = string.Empty;
    [StringLength(150)] public string? Ubicacion { get; set; }
    [StringLength(150)] public string? Equipo { get; set; }
    public int? InstalacionId { get; set; }
    public int? EquipoId { get; set; }
    public int? CubaId { get; set; }
    public int? GrupoId { get; set; }
    public int? VehiculoId { get; set; }
    public int? PlanMantenimientoId { get; set; }
    public int? DetectadoPorPersonaID { get; set; }
    public int? ContactoPersonaID { get; set; }
    public int? AsignadaAPersonaID { get; set; }
    public int? EmpresaExteriorEntidadID { get; set; }
    [StringLength(150)] public string? DetectadoPor { get; set; }
    [StringLength(150)] public string? Contacto { get; set; }
    [StringLength(20)] public string? Prioridad { get; set; }
    [StringLength(30)] public string? Estado { get; set; }
    [StringLength(150)] public string? AsignadaA { get; set; }
    [StringLength(150)] public string? EmpresaExterior { get; set; }
    [StringLength(2000)] public string? AccionesARealizar { get; set; }
    [StringLength(4000)] public string? AccesoriosNecesarios { get; set; }
    [StringLength(2000)] public string? Observaciones { get; set; }
    [StringLength(100)] public string? CreadoPor { get; set; }
    [StringLength(100)] public string? ActualizadoPor { get; set; }
    public DateTime FechaDeteccion { get; set; }
    public DateTime? FechaProgramada { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}

public sealed class TareaMantenimientoFoto
{
    public int ID { get; set; }
    public int TareaMantenimientoID { get; set; }
    [StringLength(260)] public string RutaArchivo { get; set; } = string.Empty;
    [StringLength(255)] public string? NombreOriginal { get; set; }
    [StringLength(500)] public string? Observacion { get; set; }
    [StringLength(100)] public string? ContentType { get; set; }
    public DateTime FechaCreacion { get; set; }
}

public sealed class TareaMantenimientoHistorial
{
    public long ID { get; set; }
    public int TareaMantenimientoID { get; set; }
    [StringLength(100)] public string? Usuario { get; set; }
    [StringLength(20)] public string Accion { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string CambiosJson { get; set; } = "{}";
}

public sealed class TareaMantenimientoAccesorio
{
    public int ID { get; set; }
    public int TareaMantenimientoID { get; set; }
    public int AccesorioID { get; set; }
    public int Cantidad { get; set; }
    [StringLength(500)] public string? Observaciones { get; set; }
}

public sealed class TareaMantenimientoResumenViewModel
{
    public int ID { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string? Ubicacion { get; set; }
    public string? Equipo { get; set; }
    public string? Prioridad { get; set; }
    public string? Estado { get; set; }
    public string? AsignadaA { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaProgramada { get; set; }
}

public sealed class TareasMantenimientoIndexViewModel
{
    public IReadOnlyList<TareaMantenimientoResumenViewModel> Tareas { get; set; } = [];
    public string? Busqueda { get; set; }
    public string? Estado { get; set; }
    public string? Prioridad { get; set; }
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }
    public string? FiltroEquipo { get; set; }
    public string? Responsable { get; set; }
    public int Pagina { get; set; }
    public int TotalPaginas { get; set; }
    public int TotalResultados { get; set; }
}

public sealed class TareaMantenimientoDetailsViewModel
{
    public TareaMantenimiento Tarea { get; set; } = new();
    public IReadOnlyList<TareaMantenimientoFoto> Fotos { get; set; } = [];
    public string AccesoriosTexto { get; set; } = "Sin accesorios indicados.";
    public IReadOnlyList<TareaMantenimientoHistorial> Historial { get; set; } = [];
    public IReadOnlyList<TareaMantenimientoResumenViewModel> TareasDelActivo { get; set; } = [];
}

public sealed class TareaMantenimientoFormViewModel
{
    public int ID { get; set; }
    public string Codigo { get; set; } = string.Empty;
    [Required, StringLength(150), Display(Name = "Asunto")]
    public string Titulo { get; set; } = string.Empty;
    [Required, StringLength(2000), DataType(DataType.MultilineText), Display(Name = "Descripción de la avería")]
    public string Descripcion { get; set; } = string.Empty;
    [StringLength(150), Display(Name = "Ubicación")]
    public string? Ubicacion { get; set; }
    [StringLength(150), Display(Name = "Equipo / instalación")]
    public string? Equipo { get; set; }
    public int? InstalacionId { get; set; }
    public int? EquipoId { get; set; }
    public int? CubaId { get; set; }
    public int? GrupoId { get; set; }
    public int? VehiculoId { get; set; }
    public int? PlanMantenimientoId { get; set; }
    public int? DetectadoPorPersonaID { get; set; }
    public int? ContactoPersonaID { get; set; }
    public int? AsignadaAPersonaID { get; set; }
    public int? EmpresaExteriorEntidadID { get; set; }
    public IReadOnlyList<SelectListItem> Personas { get; set; } = [];
    public IReadOnlyList<SelectListItem> Contactos { get; set; } = [];
    public IReadOnlyList<SelectListItem> Asignados { get; set; } = [];
    public IReadOnlyList<SelectListItem> Entidades { get; set; } = [];
    public IReadOnlyList<SelectListItem> Instalaciones { get; set; } = [];
    public IReadOnlyList<SelectListItem> Equipos { get; set; } = [];
    public IReadOnlyList<SelectListItem> Cubas { get; set; } = [];
    public IReadOnlyList<SelectListItem> Grupos { get; set; } = [];
    public IReadOnlyList<SelectListItem> Vehiculos { get; set; } = [];
    public IReadOnlyList<SelectListItem> PlanesMantenimiento { get; set; } = [];
    [StringLength(150), Display(Name = "Detectado por")]
    public string? DetectadoPor { get; set; }
    [StringLength(150), Display(Name = "Contacto")]
    public string? Contacto { get; set; }
    [StringLength(20), Display(Name = "Prioridad")]
    public string? Prioridad { get; set; }
    [StringLength(30), Display(Name = "Estado")]
    public string? Estado { get; set; }
    [StringLength(150), Display(Name = "Asignada a")]
    public string? AsignadaA { get; set; }
    [StringLength(150), Display(Name = "Empresa exterior")]
    public string? EmpresaExterior { get; set; }
    [StringLength(2000), DataType(DataType.MultilineText), Display(Name = "Acciones a realizar")]
    public string? AccionesARealizar { get; set; }
    [StringLength(4000), DataType(DataType.MultilineText), Display(Name = "Accesorios necesarios (IDs separados por comas)")]
    public string? AccesoriosNecesarios { get; set; }
    [StringLength(2000), DataType(DataType.MultilineText), Display(Name = "Observaciones")]
    public string? Observaciones { get; set; }
    [Display(Name = "Fecha de detección"), DataType(DataType.DateTime)]
    public DateTime FechaDeteccion { get; set; }
    [Display(Name = "Fecha prevista de realización"), DataType(DataType.Date)]
    public DateTime? FechaProgramada { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    public string? CreadoPor { get; set; }
    public string? ActualizadoPor { get; set; }
    [Display(Name = "Fotos de la avería")]
    public List<IFormFile>? FotosAdjuntas { get; set; }
    public List<string?> ObservacionesFotos { get; set; } = [];
}

public sealed class TareaMantenimientoGestionViewModel
{
    public int ID { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? Ubicacion { get; set; }
    public string? Equipo { get; set; }
    public int? AsignadaAPersonaID { get; set; }
    public int? EmpresaExteriorEntidadID { get; set; }
    public IReadOnlyList<SelectListItem> Personas { get; set; } = [];
    public IReadOnlyList<SelectListItem> Entidades { get; set; } = [];
    [Required, StringLength(20)]
    public string? Prioridad { get; set; }
    [Required, StringLength(30)]
    public string? Estado { get; set; }
    [Display(Name = "Fecha prevista de realización"), DataType(DataType.Date)]
    public DateTime? FechaProgramada { get; set; }
    [StringLength(150)]
    public string? AsignadaA { get; set; }
    [StringLength(150)]
    public string? EmpresaExterior { get; set; }
    [StringLength(2000), DataType(DataType.MultilineText)]
    public string? AccionesARealizar { get; set; }
    [StringLength(4000), DataType(DataType.MultilineText)]
    public string? AccesoriosNecesarios { get; set; }
    [StringLength(2000), DataType(DataType.MultilineText)]
    public string? Observaciones { get; set; }
}
