using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Models.ViewModels
{
    public class UserRecoveryViewModel
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public IList<string> Roles { get; set; }
    }

    public class PasswordResetLinkViewModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string ResetUrl { get; set; }
    }

    public class UserRolesEditViewModel
    {
        public string UserId { get; set; }

        [Display(Name = "Correo")]
        public string Email { get; set; }

        public string UserName { get; set; }

        public bool IsAdministrador { get; set; }

        public bool IsMantenimiento { get; set; }

        public bool IsConsulta { get; set; }
    }

    public class BootstrapAdminViewModel
    {
        [Required]
        [Display(Name = "Usuario")]
        public string UserId { get; set; }

        public IEnumerable<UserRecoveryViewModel> Users { get; set; }
    }
}
