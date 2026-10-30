using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Core.Models;

public sealed class TipoInstalacionCatalogoViewModel
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    public int InstalacionesAsociadas { get; set; }
}
