using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CasaGaillard.Core.Models;

public sealed class TipoAccesorioFilaViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? SubfamiliaNombre { get; set; }
    public string? Descripcion { get; set; }
    public int AccesoriosAsociados { get; set; }
}

public sealed class TipoAccesorioFormViewModel
{
    public int Id { get; set; }
    [Required, StringLength(45), Display(Name = "Tipo de accesorio")]
    public string Nombre { get; set; } = string.Empty;
    [StringLength(300), Display(Name = "Descripción")]
    public string? Descripcion { get; set; }
    [Display(Name = "Subfamilia")]
    public int? SubfamiliaId { get; set; }
    public IReadOnlyList<SelectListItem> Subfamilias { get; set; } = [];
}

public sealed class TipoAccesorioQuickCreateRequest
{
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public int? SubfamiliaId { get; set; }
}
