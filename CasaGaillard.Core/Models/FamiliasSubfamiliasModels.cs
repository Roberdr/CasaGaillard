using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CasaGaillard.Core.Models;

public sealed class FamiliasSubfamiliasIndexViewModel
{
    public IReadOnlyList<FamiliaFilaViewModel> Familias { get; set; } = [];
    public IReadOnlyList<SubfamiliaFilaViewModel> Subfamilias { get; set; } = [];
}

public sealed class FamiliaFilaViewModel
{
    public int FamiliaId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int NumeroSubfamilias { get; set; }
    public int NumeroAccesorios { get; set; }
}

public sealed class SubfamiliaFilaViewModel
{
    public int SubfamiliaId { get; set; }
    public int FamiliaId { get; set; }
    public string FamiliaNombre { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int NumeroAccesorios { get; set; }
    public int NumeroTipos { get; set; }
}

public sealed class FamiliaFormViewModel
{
    public int FamiliaId { get; set; }
    [Required, StringLength(150)] public string Nombre { get; set; } = string.Empty;
    [StringLength(500)] public string? Descripcion { get; set; }
}

public sealed class SubfamiliaFormViewModel
{
    public int SubfamiliaId { get; set; }
    [Required, Display(Name = "Familia")] public int? FamiliaId { get; set; }
    [Required, StringLength(150)] public string Nombre { get; set; } = string.Empty;
    [StringLength(500)] public string? Descripcion { get; set; }
    public IReadOnlyList<SelectListItem> Familias { get; set; } = [];
}
