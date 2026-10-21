using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Core.Models;

public sealed class AccesoriosIndexViewModel
{
    public IReadOnlyList<AccesorioFilaViewModel> Accesorios { get; set; } = [];
    public IReadOnlyList<SelectListItem> TiposAccesorio { get; set; } = [];
    public IReadOnlyList<SelectListItem> Materiales { get; set; } = [];
    public IReadOnlyList<SelectListItem> Familias { get; set; } = [];
    public IReadOnlyList<SelectListItem> Subfamilias { get; set; } = [];
    public int? TipoAccesorioId { get; set; }
    public int? MaterialId { get; set; }
    public int? FamiliaId { get; set; }
    public int? SubfamiliaId { get; set; }
    public string? Query { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / Math.Max(1, PageSize));
}

public sealed class AccesorioFilaViewModel
{
    public int Id { get; set; }
    public string TipoNombre { get; set; } = string.Empty;
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public string MaterialNombre { get; set; } = string.Empty;
    public string? FamiliaNombre { get; set; }
    public string? SubfamiliaNombre { get; set; }
    public string? UbicacionNombre { get; set; }
    public DateTime? FechaAlta { get; set; }
    public bool Activo { get; set; }
    public int? FotoId { get; set; }
}

public sealed class AccesorioDetailsViewModel
{
    public AccesorioFilaViewModel Accesorio { get; set; } = new();
    public IReadOnlyList<DetalleAccesorioFilaViewModel> Detalles { get; set; } = [];
    public IReadOnlyList<AccesorioFotoViewModel> Fotos { get; set; } = [];
}

public sealed class AccesorioFormViewModel
{
    public int Id { get; set; }
    public int? IntervencionId { get; set; }
    [StringLength(150), Display(Name = "Nombre")] public string? Nombre { get; set; }
    [StringLength(500), Display(Name = "Descripción")] public string? Descripcion { get; set; }
    [Display(Name = "Tipo de accesorio"), Required] public int? TipoAccesorioId { get; set; }
    [Display(Name = "Material"), Required] public int? MaterialId { get; set; }
    [Display(Name = "Familia")] public int? FamiliaId { get; set; }
    [Display(Name = "Subfamilia")] public int? SubfamiliaId { get; set; }
    [Display(Name = "Ubicación")] public int? UbicacionId { get; set; }
    [Display(Name = "Fecha de alta"), DataType(DataType.Date)] public DateTime? FechaAlta { get; set; }
    [Display(Name = "Activo")] public bool Activo { get; set; } = true;
    public List<DetalleAccesorioFormViewModel> Detalles { get; set; } = [];
    [Display(Name = "Fotos del accesorio")] public List<IFormFile> FotosAdjuntas { get; set; } = [];
    public List<string?> ObservacionesFotos { get; set; } = [];
    public IReadOnlyList<SelectListItem> TiposAccesorio { get; set; } = [];
    public IReadOnlyList<SelectListItem> Materiales { get; set; } = [];
    public IReadOnlyList<SelectListItem> Familias { get; set; } = [];
    public IReadOnlyList<SelectListItem> Subfamilias { get; set; } = [];
    public IReadOnlyList<SelectListItem> Ubicaciones { get; set; } = [];
    public IReadOnlyList<SelectListItem> Caracteristicas { get; set; } = [];
    public IReadOnlyList<SelectListItem> Unidades { get; set; } = [];
    public IReadOnlyList<AccesorioFotoViewModel> FotosExistentes { get; set; } = [];
}

public sealed class DetalleAccesorioFormViewModel
{
    public int Id { get; set; }
    [Display(Name = "Característica")] public int? CaracteristicaId { get; set; }
    [Display(Name = "Cantidad")] public int? Cantidad { get; set; }
    [Display(Name = "Medida")] public decimal? Medida { get; set; }
    [Display(Name = "Unidad")] public int? UnidadId { get; set; }
    [Display(Name = "Tipo"), StringLength(100)] public string? Tipo { get; set; }
    public bool Eliminar { get; set; }
}

public sealed class DetalleAccesorioFilaViewModel
{
    public int Id { get; set; }
    public string? Caracteristica { get; set; }
    public int? Cantidad { get; set; }
    public decimal? Medida { get; set; }
    public string? Unidad { get; set; }
    public string? Tipo { get; set; }
}

public sealed class AccesorioFotoViewModel
{
    public int Id { get; set; }
    public string? NombreOriginal { get; set; }
    public string? Observacion { get; set; }
    public string? ContentType { get; set; }
    public DateTime FechaCreacion { get; set; }
}
