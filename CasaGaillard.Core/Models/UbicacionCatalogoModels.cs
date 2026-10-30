using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Core.Models;

public sealed class UbicacionCatalogoViewModel
{
    public int UbicacionId { get; set; }

    [StringLength(50, ErrorMessage = "El código no puede superar los 50 caracteres.")]
    [Display(Name = "Código")]
    public string? Codigo { get; set; }

    [Required, StringLength(150)]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(300)]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    public int InstalacionesAsociadas { get; set; }
    public int AccesoriosAsociados { get; set; }
}
