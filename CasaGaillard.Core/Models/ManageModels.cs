using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Core.Models;

public sealed class ManageIndexModel
{
    public bool HasPassword { get; set; }
}

public sealed class ChangePasswordForm
{
    [Required, DataType(DataType.Password), Display(Name = "Contraseña actual")]
    public string CurrentPassword { get; set; } = "";

    [Required, StringLength(100, MinimumLength = 6), DataType(DataType.Password), Display(Name = "Nueva contraseña")]
    public string NewPassword { get; set; } = "";

    [Required, DataType(DataType.Password), Compare(nameof(NewPassword)), Display(Name = "Confirmar contraseña")]
    public string ConfirmPassword { get; set; } = "";
}

public sealed class SetPasswordForm
{
    [Required, StringLength(100, MinimumLength = 6), DataType(DataType.Password), Display(Name = "Nueva contraseña")]
    public string NewPassword { get; set; } = "";

    [Required, DataType(DataType.Password), Compare(nameof(NewPassword)), Display(Name = "Confirmar contraseña")]
    public string ConfirmPassword { get; set; } = "";
}
