using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CasaGaillard.Core.Models;

public sealed class GrupoFilaViewModel
{
    public int Id { get; set; }
    public string Cuba { get; set; } = "";
    public int Compartimento { get; set; }
    public string Tipo { get; set; } = "";
    public string Situacion { get; set; } = "";
    [NotMapped]
    public IReadOnlyList<GrupoAccesorioViewModel> Accesorios { get; set; } = [];
}

public sealed class GrupoAccesorioViewModel
{
    public int GrupoId { get; set; }
    public int Cantidad { get; set; }
    public string TipoAccesorio { get; set; } = "";
    public string Material { get; set; } = "";
}

public sealed class GrupoFormViewModel
{
    public int Id { get; set; }
    [Required, Display(Name = "Tipo de grupo")] public int? TipoGrupoId { get; set; }
    [Required, Display(Name = "Cuba")] public int? CubaId { get; set; }
    [Required, Display(Name = "Compartimento")] public int? CompartimentoId { get; set; }
    [Required, Display(Name = "Situación")] public int? SituacionId { get; set; }
    public IReadOnlyList<SelectListItem> Tipos { get; set; } = [];
    public IReadOnlyList<SelectListItem> Cubas { get; set; } = [];
    public IReadOnlyList<SelectListItem> Compartimentos { get; set; } = [];
    public IReadOnlyList<SelectListItem> Situaciones { get; set; } = [];
}
