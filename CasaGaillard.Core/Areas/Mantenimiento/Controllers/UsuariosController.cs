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
    public UsuariosController(AppDbContext db, UserManager<IdentityUser> users, RoleManager<IdentityRole> roles)
    { _db = db; _users = users; _roles = roles; }

    public async Task<IActionResult> Index()
    {
        var rows = await _db.Database.SqlQueryRaw<UsuarioRow>("SELECT Id,Email,UserName FROM dbo.AspNetUsers ORDER BY Email").ToListAsync();
        var users = rows.Select(row => new UsuarioFilaViewModel { Id = row.Id, Email = row.Email, UserName = row.UserName }).ToList();
        foreach (var user in users)
        {
            var identityUser = await _users.FindByIdAsync(user.Id);
            if (identityUser is not null) user.Roles = (await _users.GetRolesAsync(identityUser)).ToList();
        }
        return View(users);
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

    private sealed class UsuarioRow { public string Id { get; set; } = string.Empty; public string? Email { get; set; } public string? UserName { get; set; } }
}
