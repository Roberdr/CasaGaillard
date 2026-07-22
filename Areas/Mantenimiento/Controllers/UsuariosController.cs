using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CasaGaillard.Models;
using CasaGaillard.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace CasaGaillard.Areas.Mantenimiento.Controllers
{
    [Authorize(Roles = AppRoles.Administrador)]
    public class UsuariosController : Controller
    {
        private ApplicationUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

        public async Task<ActionResult> Index()
        {
            var users = await UserManager.Users
                .OrderBy(u => u.Email)
                .ToListAsync();

            var model = users.Select(u => new UserRecoveryViewModel
            {
                Id = u.Id,
                Email = u.Email,
                UserName = u.UserName,
                Roles = UserManager.GetRoles(u.Id)
            }).ToList();

            return View(model);
        }

        public async Task<ActionResult> Roles(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }

            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var roles = await UserManager.GetRolesAsync(user.Id);
            var model = new UserRolesEditViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                IsAdministrador = roles.Contains(AppRoles.Administrador),
                IsMantenimiento = roles.Contains(AppRoles.Mantenimiento),
                IsConsulta = roles.Contains(AppRoles.Consulta)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Roles(UserRolesEditViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.UserId))
            {
                return HttpNotFound();
            }

            var user = await UserManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return HttpNotFound();
            }

            await AsegurarRolesAsync(AppRoles.Administrador, AppRoles.Mantenimiento, AppRoles.Consulta);
            var actuales = await UserManager.GetRolesAsync(user.Id);
            var deseados = new[]
            {
                new { Name = AppRoles.Administrador, Enabled = model.IsAdministrador },
                new { Name = AppRoles.Mantenimiento, Enabled = model.IsMantenimiento },
                new { Name = AppRoles.Consulta, Enabled = model.IsConsulta }
            };

            foreach (var role in actuales.ToArray())
            {
                if (!deseados.Any(r => r.Name == role && r.Enabled))
                {
                    await UserManager.RemoveFromRoleAsync(user.Id, role);
                }
            }

            foreach (var role in deseados.Where(r => r.Enabled))
            {
                if (!actuales.Contains(role.Name))
                {
                    await UserManager.AddToRoleAsync(user.Id, role.Name);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GenerarEnlaceRestablecimiento(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }

            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var resetUrl = Url.Action("ResetPassword", "Account", new { area = "", code = token }, protocol: Request.Url.Scheme);

            return View("ResetLink", new PasswordResetLinkViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                ResetUrl = resetUrl
            });
        }

        private async Task AsegurarRolesAsync(params string[] roles)
        {
            using (var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext())))
            {
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
            }
        }
    }
}
