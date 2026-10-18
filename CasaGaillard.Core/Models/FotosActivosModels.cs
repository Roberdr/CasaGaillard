using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CasaGaillard.Core.Models;

public sealed class InstalacionEquipoFotosViewModel
{
    public string Tipo { get; set; } = string.Empty;
    public int ActivoId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public IReadOnlyList<InstalacionEquipoFotoFilaViewModel> Fotos { get; set; } = [];
    [Display(Name = "Fotos")] public List<IFormFile> Archivos { get; set; } = [];
}

public sealed class InstalacionEquipoFotoFilaViewModel
{
    public int Id { get; set; }
    public string? NombreOriginal { get; set; }
    public DateTime FechaCreacion { get; set; }
}
