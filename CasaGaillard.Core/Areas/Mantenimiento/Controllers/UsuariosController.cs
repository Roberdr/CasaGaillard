using CasaGaillard.Core.Data;
using CasaGaillard.Core.Models;
using CasaGaillard.Core.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CasaGaillard.Core.Areas.Mantenimiento.Controllers;

[Area("Mantenimiento")]
[Authorize(Roles = AppRoles.Administrador)]
public sealed class UsuariosController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<IdentityUser> _users;
    private readonly RoleManager<IdentityRole> _roles;
    private readonly ILogger<UsuariosController> _logger;
    public UsuariosController(AppDbContext db, UserManager<IdentityUser> users, RoleManager<IdentityRole> roles, ILogger<UsuariosController> logger)
    { _db = db; _users = users; _roles = roles; _logger = logger; }

    public async Task<IActionResult> Index()
    {
        var rows = await _db.Database.SqlQueryRaw<UsuarioRow>(@"SELECT u.Id,u.Email,u.UserName,
            LTRIM(RTRIM(CONCAT(p.NombrePersona,N' ',p.Apellido1,N' ',COALESCE(p.Apellido2,N'')))) Persona
            FROM dbo.AspNetUsers u LEFT JOIN gaillard.Persona p ON LOWER(LTRIM(RTRIM(p.Email)))=LOWER(LTRIM(RTRIM(u.Email)))
            ORDER BY u.Email").ToListAsync();
        var users = rows.Select(row => new UsuarioFilaViewModel { Id = row.Id, Email = row.Email, UserName = row.UserName, Persona = row.Persona }).ToList();
        foreach (var user in users)
        {
            var identityUser = await _users.FindByIdAsync(user.Id);
            if (identityUser is not null) user.Roles = (await _users.GetRolesAsync(identityUser)).ToList();
        }
        return View(users);
    }

    public IActionResult Create() => View(new UsuarioCreateViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UsuarioCreateViewModel model)
    {
        var email = model.Email?.Trim();
        if (!model.IsAdministrador && !model.IsMantenimiento && !model.IsConsulta)
            ModelState.AddModelError(string.Empty, "Selecciona al menos un rol para la cuenta.");
        var personaExiste = !string.IsNullOrWhiteSpace(email) && await _db.Database.SqlQueryRaw<int>(
            "SELECT COUNT(*) AS Value FROM gaillard.Persona WHERE LOWER(LTRIM(RTRIM(Email)))=LOWER(@p0)", email).SingleAsync() > 0;
        if (!personaExiste)
        {
            if (string.IsNullOrWhiteSpace(model.NombrePersona)) ModelState.AddModelError(nameof(model.NombrePersona), "Indica el nombre para crear la persona.");
            if (string.IsNullOrWhiteSpace(model.Apellido1)) ModelState.AddModelError(nameof(model.Apellido1), "Indica el primer apellido para crear la persona.");
            if (string.IsNullOrWhiteSpace(model.Nif)) ModelState.AddModelError(nameof(model.Nif), "Indica el NIF para crear la persona.");
        }
        if (!string.IsNullOrWhiteSpace(email) && await _users.FindByEmailAsync(email) is not null)
            ModelState.AddModelError(nameof(model.Email), "Ya existe una cuenta con ese correo electrónico.");
        if (!ModelState.IsValid) return View(model);

        await EnsureRolesAsync();
        var user = new IdentityUser
        {
            // The login form authenticates by email as the Identity username.
            UserName = email!,
            Email = email!,
            EmailConfirmed = false
        };
        var created = await _users.CreateAsync(user);
        if (!created.Succeeded)
        {
            foreach (var error in created.Errors) ModelState.AddModelError(string.Empty, error.Description);
            return View(model);
        }

        var roles = new Dictionary<string, bool>
        {
            [AppRoles.Administrador] = model.IsAdministrador,
            [AppRoles.Mantenimiento] = model.IsMantenimiento,
            [AppRoles.Consulta] = model.IsConsulta
        };
        var assignedRoles = new List<string>();
        foreach (var role in roles.Where(item => item.Value).Select(item => item.Key))
        {
            var result = await _users.AddToRoleAsync(user, role);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
                foreach (var assignedRole in assignedRoles) await _users.RemoveFromRoleAsync(user, assignedRole);
                await _users.DeleteAsync(user);
                return View(model);
            }
            assignedRoles.Add(role);
        }

        try
        {
            await EnsurePersonaAsync(email!, model, personaExiste);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "No se pudo vincular el email {Email} con una persona al crear el usuario.", email);
            foreach (var assignedRole in assignedRoles) await _users.RemoveFromRoleAsync(user, assignedRole);
            await _users.DeleteAsync(user);
            ModelState.AddModelError(string.Empty, "No se pudo vincular el correo con una persona. Comprueba que las migraciones de base de datos estén aplicadas.");
            return View(model);
        }

        var token = await _users.GeneratePasswordResetTokenAsync(user);
        model.ResetUrl = Url.Action("ResetPassword", "Account", new { area = "", code = token, email = user.Email }, Request.Scheme);
        return View("Create", model);
    }

    public async Task<IActionResult> Roles(string? id)
    {
        if (string.IsNullOrWhiteSpace(id)) return NotFound();
        var user = await _users.FindByIdAsync(id);
        if (user is null) return NotFound();
        var roles = await _users.GetRolesAsync(user);
        return View(new UsuarioRolesViewModel
        {
            UserId = user.Id, Email = user.Email, UserName = user.UserName,
            IsAdministrador = roles.Contains(AppRoles.Administrador), IsMantenimiento = roles.Contains(AppRoles.Mantenimiento), IsConsulta = roles.Contains(AppRoles.Consulta)
        });
    }

    public async Task<IActionResult> VincularPersona(string? id)
    {
        if (string.IsNullOrWhiteSpace(id)) return NotFound();
        var user = await _users.FindByIdAsync(id);
        if (user?.Email is null) return NotFound();
        var model = new UsuarioPersonaViewModel { UserId = user.Id, Email = user.Email };
        await PrepararPersonasAsync(model);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> VincularPersona(UsuarioPersonaViewModel model)
    {
        var user = await _users.FindByIdAsync(model.UserId);
        if (user?.Email is null) return NotFound();
        model.Email = user.Email;

        if (model.PersonaId.HasValue)
        {
            var updated = await _db.Database.ExecuteSqlInterpolatedAsync($@"UPDATE gaillard.Persona SET Email={user.Email}
                WHERE ID={model.PersonaId.Value} AND (Email IS NULL OR LOWER(LTRIM(RTRIM(Email)))=LOWER({user.Email}))");
            if (updated == 0) ModelState.AddModelError(nameof(model.PersonaId), "La persona ya está vinculada a otro correo o no existe.");
        }
        else
        {
            if (string.IsNullOrWhiteSpace(model.NombrePersona)) ModelState.AddModelError(nameof(model.NombrePersona), "Indica el nombre para crear la persona.");
            if (string.IsNullOrWhiteSpace(model.Apellido1)) ModelState.AddModelError(nameof(model.Apellido1), "Indica el primer apellido para crear la persona.");
            if (string.IsNullOrWhiteSpace(model.Nif)) ModelState.AddModelError(nameof(model.Nif), "Indica el NIF para crear la persona.");
            if (ModelState.IsValid)
            {
                try
                {
                    await CrearPersonaAsync(user.Email, model.NombrePersona!, model.Apellido1!, model.Apellido2, model.Nif!);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "No se pudo crear la persona para el usuario {UserId}.", user.Id);
                    ModelState.AddModelError(string.Empty, "No se pudo crear o vincular la persona.");
                }
            }
        }

        if (!ModelState.IsValid)
        {
            await PrepararPersonasAsync(model);
            return View(model);
        }
        TempData["LoginMessage"] = "La persona se ha vinculado al usuario.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Roles(UsuarioRolesViewModel model)
    {
        var user = await _users.FindByIdAsync(model.UserId);
        if (user is null) return NotFound();
        if (!ModelState.IsValid) return View(model);
        await EnsureRolesAsync();
        var current = await _users.GetRolesAsync(user);
        var selected = new Dictionary<string, bool>
        {
            [AppRoles.Administrador] = model.IsAdministrador,
            [AppRoles.Mantenimiento] = model.IsMantenimiento,
            [AppRoles.Consulta] = model.IsConsulta
        };
        foreach (var role in selected)
        {
            if (role.Value && !current.Contains(role.Key))
            {
                var result = await _users.AddToRoleAsync(user, role.Key);
                if (!result.Succeeded) foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            }
            else if (!role.Value && current.Contains(role.Key))
            {
                var result = await _users.RemoveFromRoleAsync(user, role.Key);
                if (!result.Succeeded) foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        if (!ModelState.IsValid) return View(model);
        TempData["LoginMessage"] = "Los roles del usuario se han actualizado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerarEnlaceRestablecimiento(string? id)
    {
        if (string.IsNullOrWhiteSpace(id)) return NotFound();
        var user = await _users.FindByIdAsync(id);
        if (user is null) return NotFound();
        var token = await _users.GeneratePasswordResetTokenAsync(user);
        var resetUrl = Url.Action("ResetPassword", "Account", new { area = "", code = token, email = user.Email }, Request.Scheme);
        return View("ResetLink", new PasswordResetLinkViewModel { UserId = user.Id, Email = user.Email ?? string.Empty, ResetUrl = resetUrl ?? string.Empty });
    }

    private async Task EnsureRolesAsync()
    {
        foreach (var name in new[] { AppRoles.Administrador, AppRoles.Mantenimiento, AppRoles.Consulta })
            if (!await _roles.RoleExistsAsync(name))
                await _roles.CreateAsync(new IdentityRole(name));
    }

    private async Task EnsurePersonaAsync(string email, UsuarioCreateViewModel model, bool personaExistia)
    {
        if (personaExistia) return;
        await CrearPersonaAsync(email, model.NombrePersona!, model.Apellido1!, model.Apellido2, model.Nif!);
    }

    private async Task CrearPersonaAsync(string email, string nombre, string apellido1, string? apellido2, string nif)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        var ids = await _db.Database.SqlQueryRaw<int>("SELECT ISNULL(MAX(ID),0)+1 AS Id FROM gaillard.Persona WITH (UPDLOCK,HOLDLOCK)").ToListAsync();
        await _db.Database.ExecuteSqlInterpolatedAsync($@"INSERT INTO gaillard.Persona(ID,NombrePersona,Apellido1,Apellido2,NIF,DireccionID,Email)
            VALUES({ids.Single()},{nombre.Trim()},{apellido1.Trim()},{apellido2?.Trim()},{nif.Trim()},NULL,{email})");
        await transaction.CommitAsync();
    }

    private async Task PrepararPersonasAsync(UsuarioPersonaViewModel model)
    {
        var rows = await _db.Database.SqlQueryRaw<PersonaOption>(@"SELECT ID,CONCAT(NombrePersona,N' ',RTRIM(Apellido1),N' ',RTRIM(COALESCE(Apellido2,N''))) Texto
            FROM gaillard.Persona WHERE Email IS NULL OR LOWER(LTRIM(RTRIM(Email)))=LOWER(@p0)
            ORDER BY NombrePersona,Apellido1,Apellido2", model.Email).ToListAsync();
        model.Personas = rows.Select(row => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(row.Texto, row.Id.ToString(), row.Id == model.PersonaId)).ToList();
    }

    private sealed class UsuarioRow { public string Id { get; set; } = string.Empty; public string? Email { get; set; } public string? UserName { get; set; } public string? Persona { get; set; } }
    private sealed class PersonaOption { public int Id { get; set; } public string Texto { get; set; } = string.Empty; }
}
