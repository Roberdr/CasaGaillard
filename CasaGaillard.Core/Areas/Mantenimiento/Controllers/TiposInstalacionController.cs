using CasaGaillard.Core.Models;
using CasaGaillard.Core.Security;
using CasaGaillard.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasaGaillard.Core.Areas.Mantenimiento.Controllers;

[Area("Mantenimiento")]
[Authorize(Roles = AppRoles.LecturaMantenimiento)]
public sealed class TiposInstalacionController : Controller
{
    private readonly ITiposInstalacionService _service;
    public TiposInstalacionController(ITiposInstalacionService service) => _service = service;

    public async Task<IActionResult> Index() => View(await _service.GetAllAsync());

    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public IActionResult Create() => View(new TipoInstalacionCatalogoViewModel());

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Create(TipoInstalacionCatalogoViewModel model)
    {
        await ValidateNameAsync(model);
        if (!ModelState.IsValid) return View(model);
        await _service.SaveAsync(model);
        TempData["LoginMessage"] = "El tipo de instalación se ha creado.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Edit(int? id)
    {
        if (!id.HasValue) return BadRequest();
        var model = await _service.GetAsync(id.Value);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Edit(TipoInstalacionCatalogoViewModel model)
    {
        await ValidateNameAsync(model);
        if (!ModelState.IsValid) return View(model);
        if (!await _service.UpdateAsync(model)) return NotFound();
        TempData["LoginMessage"] = "El tipo de instalación se ha actualizado.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Delete(int? id)
    {
        if (!id.HasValue) return BadRequest();
        var model = await _service.GetAsync(id.Value);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result.Exists) return NotFound();
        if (!result.Deleted)
        {
            TempData["CatalogoError"] = "No se puede eliminar: hay instalaciones que utilizan este tipo.";
            return RedirectToAction(nameof(Delete), new { id });
        }
        TempData["LoginMessage"] = "El tipo de instalación se ha eliminado.";
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidateNameAsync(TipoInstalacionCatalogoViewModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.Nombre) && await _service.NombreExisteAsync(model.Nombre, model.Id))
            ModelState.AddModelError(nameof(model.Nombre), "Ya existe un tipo de instalación con ese nombre.");
    }
}
