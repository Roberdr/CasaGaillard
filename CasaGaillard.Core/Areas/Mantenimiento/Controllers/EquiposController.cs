using CasaGaillard.Core.Models;
using CasaGaillard.Core.Security;
using CasaGaillard.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CasaGaillard.Core.Areas.Mantenimiento.Controllers;

[Area("Mantenimiento")]
[Authorize(Roles = AppRoles.LecturaMantenimiento)]
public sealed class EquiposController : Controller
{
    private readonly IEquiposService _service;
    public EquiposController(IEquiposService service) => _service = service;

    public async Task<IActionResult> Index(int? instalacionId, bool incluirBajas = false)
    {
        ViewData["InstalacionId"] = instalacionId;
        ViewData["IncluirBajas"] = incluirBajas;
        ViewData["Instalaciones"] = new SelectList(await _service.GetInstalacionesAsync(instalacionId), "Value", "Text", instalacionId);
        return View(await _service.GetAllAsync(instalacionId, incluirBajas));
    }

    [HttpGet]
    public async Task<IActionResult> EquiposPadre(int instalacionId)
    {
        var options = await _service.GetEquiposPadreAsync(instalacionId, null);
        return Json(options.Select(option => new { value = option.Value, text = option.Text }));
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (!id.HasValue) return BadRequest();
        var model = await _service.GetDetailsAsync(id.Value);
        return model is null ? NotFound() : View(model);
    }

    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Create(int? instalacionId) => View(await _service.GetFormAsync(null, instalacionId));

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Create(EquipoFormViewModel model)
    {
        await ValidateModelAsync(model);
        if (!ModelState.IsValid) return View(await _service.PrepareFormAsync(model));
        await _service.SaveAsync(model);
        TempData["LoginMessage"] = "El equipo se ha creado.";
        return RedirectToAction(nameof(Index), new { instalacionId = model.InstalacionId });
    }

    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Edit(int? id)
    {
        if (!id.HasValue) return BadRequest();
        var model = await _service.GetFormAsync(id);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Edit(EquipoFormViewModel model)
    {
        await ValidateModelAsync(model);
        if (!ModelState.IsValid) return View(await _service.PrepareFormAsync(model));
        if (!await _service.SaveAsync(model)) return NotFound();
        TempData["LoginMessage"] = "El equipo se ha actualizado.";
        return RedirectToAction(nameof(Index), new { instalacionId = model.InstalacionId });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> DarDeBaja(int id, int? instalacionId)
    {
        await _service.DarDeBajaAsync(id);
        TempData["LoginMessage"] = "El equipo se ha marcado como inactivo.";
        return RedirectToAction(nameof(Index), new { instalacionId });
    }

    private async Task ValidateModelAsync(EquipoFormViewModel model)
    {
        foreach (var error in await _service.ValidateAsync(model)) ModelState.AddModelError(error.Key, error.Value);
    }
}
