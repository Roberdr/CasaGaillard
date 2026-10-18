using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Core.Models;

public sealed class UsuarioFilaViewModel
{
    public string Id { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public IReadOnlyList<string> Roles { get; set; } = [];
}

public sealed class UsuarioRolesViewModel
{
    [Required] public string UserId { get; set; } = string.Empty;
    [Display(Name = "Correo")] public string? Email { get; set; }
    public string? UserName { get; set; }
    [Display(Name = "Administrador")] public bool IsAdministrador { get; set; }
    [Display(Name = "Mantenimiento")] public bool IsMantenimiento { get; set; }
    [Display(Name = "Consulta")] public bool IsConsulta { get; set; }
}

public sealed class PasswordResetLinkViewModel
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ResetUrl { get; set; } = string.Empty;
}

public sealed class ResetPasswordCoreViewModel
{
    [Required] public string? Code { get; set; }
    [Required, EmailAddress] public string? Email { get; set; }
    [Required, StringLength(100, MinimumLength = 6), DataType(DataType.Password), Display(Name = "Nueva contraseña")] public string? Password { get; set; }
    [Required, Compare(nameof(Password)), DataType(DataType.Password), Display(Name = "Confirmar contraseña")] public string? ConfirmPassword { get; set; }
}
