using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Core.Models;

public sealed class LoginViewModel
{
    [Required, Display(Name = "Correo electrónico")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Recordarme")]
    public bool RememberMe { get; set; }
}
