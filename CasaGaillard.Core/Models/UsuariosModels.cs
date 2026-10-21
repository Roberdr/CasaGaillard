using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Core.Models;

public sealed class UsuarioFilaViewModel
{
    public string Id { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string? Persona { get; set; }
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

public sealed class UsuarioCreateViewModel
{
    [Required, EmailAddress, StringLength(256, ErrorMessage = "El correo no puede superar los 256 caracteres.")]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;
    [StringLength(50), Display(Name = "Nombre de la persona")]
    public string? NombrePersona { get; set; }
    [StringLength(50), Display(Name = "Primer apellido")]
    public string? Apellido1 { get; set; }
    [StringLength(50), Display(Name = "Segundo apellido")]
    public string? Apellido2 { get; set; }
    [StringLength(10), Display(Name = "NIF")]
    public string? Nif { get; set; }
    [Display(Name = "Administrador")] public bool IsAdministrador { get; set; }
    [Display(Name = "Mantenimiento")] public bool IsMantenimiento { get; set; } = true;
    [Display(Name = "Consulta")] public bool IsConsulta { get; set; }
    public string? ResetUrl { get; set; }
}

public sealed class UsuarioPersonaViewModel
{
    [Required] public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    [Display(Name = "Persona existente")] public int? PersonaId { get; set; }
    public IReadOnlyList<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Personas { get; set; } = [];
    [StringLength(50), Display(Name = "Nombre")] public string? NombrePersona { get; set; }
    [StringLength(50), Display(Name = "Primer apellido")] public string? Apellido1 { get; set; }
    [StringLength(50), Display(Name = "Segundo apellido")] public string? Apellido2 { get; set; }
    [StringLength(10), Display(Name = "NIF")] public string? Nif { get; set; }
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
