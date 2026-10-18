using CasaGaillard.Core.Models;
using CasaGaillard.Core.Security;
using CasaGaillard.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasaGaillard.Core.Areas.Mantenimiento.Controllers;

[Area("Mantenimiento")]
[Authorize(Roles = AppRoles.LecturaMantenimiento)]
public sealed class InstalacionesController : Controller
{
    private readonly IInstalacionesService _service;
    public InstalacionesController(IInstalacionesService service) => _service = service;

    public async Task<IActionResult> Index(bool incluirBajas = false)
    {
        ViewData["IncluirBajas"] = incluirBajas;
        return View(await _service.GetAllAsync(incluirBajas));
    }

    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Create() => View(await _service.GetFormAsync(null));

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Create(InstalacionFormViewModel model)
    {
        await ValidateModelAsync(model);
        if (!ModelState.IsValid) return View(await _service.PrepareFormAsync(model));
        await _service.SaveAsync(model);
        TempData["LoginMessage"] = "La instalación se ha creado.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Edit(int? id)
    {
        if (!id.HasValue) return BadRequest();
        var model = await _service.GetFormAsync(id);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Edit(InstalacionFormViewModel model)
    {
        await ValidateModelAsync(model);
        if (!ModelState.IsValid) return View(await _service.PrepareFormAsync(model));
        if (!await _service.SaveAsync(model)) return NotFound();
        TempData["LoginMessage"] = "La instalación se ha actualizado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> DarDeBaja(int id)
    {
        await _service.DarDeBajaAsync(id);
        TempData["LoginMessage"] = "La instalación se ha marcado fuera de servicio.";
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidateModelAsync(InstalacionFormViewModel model)
    {
        foreach (var error in await _service.ValidateAsync(model)) ModelState.AddModelError(error.Key, error.Value);
    }
}
