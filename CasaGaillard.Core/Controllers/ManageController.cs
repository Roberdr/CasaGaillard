using CasaGaillard.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CasaGaillard.Core.Controllers;

[Authorize]
public sealed class ManageController : Controller
{
    private readonly UserManager<IdentityUser> _users;
    private readonly SignInManager<IdentityUser> _signIn;

    public ManageController(UserManager<IdentityUser> users, SignInManager<IdentityUser> signIn)
    {
        _users = users;
        _signIn = signIn;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _users.GetUserAsync(User);
        if (user is null) return NotFound();
        return View(new ManageIndexModel { HasPassword = await _users.HasPasswordAsync(user) });
    }

    public async Task<IActionResult> ChangePassword()
    {
        var user = await _users.GetUserAsync(User);
        return user is null ? NotFound() : await _users.HasPasswordAsync(user) ? View(new ChangePasswordForm()) : RedirectToAction(nameof(SetPassword));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordForm model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = await _users.GetUserAsync(User);
        if (user is null) return NotFound();
        var result = await _users.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            return View(model);
        }
        await _signIn.RefreshSignInAsync(user);
        TempData["ManageMessage"] = "La contraseña se ha actualizado.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> SetPassword()
    {
        var user = await _users.GetUserAsync(User);
        if (user is null) return NotFound();
        return await _users.HasPasswordAsync(user) ? RedirectToAction(nameof(ChangePassword)) : View(new SetPasswordForm());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SetPassword(SetPasswordForm model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = await _users.GetUserAsync(User);
        if (user is null) return NotFound();
        var result = await _users.AddPasswordAsync(user, model.NewPassword);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            return View(model);
        }
        await _signIn.RefreshSignInAsync(user);
        TempData["ManageMessage"] = "La contraseña se ha configurado.";
        return RedirectToAction(nameof(Index));
    }
}
