using CasaGaillard.Core.Models;
using CasaGaillard.Core.Security;
using CasaGaillard.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasaGaillard.Core.Areas.Mantenimiento.Controllers;

[Area("Mantenimiento")]
[Authorize(Roles = AppRoles.LecturaMantenimiento)]
public sealed class UbicacionesController : Controller
{
    private readonly IUbicacionesService _service;
    public UbicacionesController(IUbicacionesService service) => _service = service;

    public async Task<IActionResult> Index() => View(await _service.GetAllAsync());

    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public IActionResult Create() => View(new UbicacionCatalogoViewModel());

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Create(UbicacionCatalogoViewModel model)
    {
        await ValidateCodigoAsync(model);
        if (!ModelState.IsValid) return View(model);
        await _service.SaveAsync(model);
        TempData["LoginMessage"] = "La ubicación se ha creado.";
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
    public async Task<IActionResult> Edit(UbicacionCatalogoViewModel model)
    {
        await ValidateCodigoAsync(model);
        if (!ModelState.IsValid) return View(model);
        if (!await _service.UpdateAsync(model)) return NotFound();
        TempData["LoginMessage"] = "La ubicación se ha actualizado.";
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
            TempData["CatalogoError"] = "No se puede eliminar: hay instalaciones o accesorios que utilizan esta ubicación.";
            return RedirectToAction(nameof(Delete), new { id });
        }
        TempData["LoginMessage"] = "La ubicación se ha eliminado.";
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidateCodigoAsync(UbicacionCatalogoViewModel model)
    {
        if (await _service.CodigoExisteAsync(model.Codigo, model.UbicacionId))
            ModelState.AddModelError(nameof(model.Codigo), "Ya existe una ubicación con ese código.");
    }
}
