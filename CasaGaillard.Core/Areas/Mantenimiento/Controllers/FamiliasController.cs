using CasaGaillard.Core.Models;
using CasaGaillard.Core.Security;
using CasaGaillard.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasaGaillard.Core.Areas.Mantenimiento.Controllers;

[Area("Mantenimiento")]
[Authorize(Roles = AppRoles.LecturaMantenimiento)]
public sealed class FamiliasController : Controller
{
    private readonly IFamiliasService _service;
    public FamiliasController(IFamiliasService service) => _service = service;

    public async Task<IActionResult> Index() => View(await _service.GetIndexAsync());

    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public IActionResult CreateFamilia() => View("FamiliaForm", new FamiliaFormViewModel());

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> CreateFamilia(FamiliaFormViewModel model)
    {
        await ValidateFamiliaAsync(model);
        if (!ModelState.IsValid) return View("FamiliaForm", model);
        await _service.SaveFamiliaAsync(model);
        TempData["LoginMessage"] = "La familia se ha creado.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> EditFamilia(int? id)
    {
        if (!id.HasValue) return BadRequest();
        var model = await _service.GetFamiliaAsync(id.Value);
        return model is null ? NotFound() : View("FamiliaForm", model);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> EditFamilia(FamiliaFormViewModel model)
    {
        await ValidateFamiliaAsync(model);
        if (!ModelState.IsValid) return View("FamiliaForm", model);
        if (!await _service.UpdateFamiliaAsync(model)) return NotFound();
        TempData["LoginMessage"] = "La familia se ha actualizado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> DeleteFamilia(int id)
    {
        var result = await _service.DeleteFamiliaAsync(id);
        if (!result.Exists) return NotFound();
        if (!result.Deleted) TempData["CatalogoError"] = "No se puede eliminar: la familia tiene subfamilias o accesorios asociados.";
        else TempData["LoginMessage"] = "La familia se ha eliminado.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> CreateSubfamilia() => View("SubfamiliaForm", await PrepareSubfamiliaAsync(new SubfamiliaFormViewModel()));

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> CreateSubfamilia(SubfamiliaFormViewModel model)
    {
        await ValidateSubfamiliaAsync(model);
        if (!ModelState.IsValid) return View("SubfamiliaForm", await PrepareSubfamiliaAsync(model));
        await _service.SaveSubfamiliaAsync(model);
        TempData["LoginMessage"] = "La subfamilia se ha creado y vinculado a su familia.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> EditSubfamilia(int? id)
    {
        if (!id.HasValue) return BadRequest();
        var model = await _service.GetSubfamiliaAsync(id.Value);
        return model is null ? NotFound() : View("SubfamiliaForm", model);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> EditSubfamilia(SubfamiliaFormViewModel model)
    {
        await ValidateSubfamiliaAsync(model);
        if (!ModelState.IsValid) return View("SubfamiliaForm", await PrepareSubfamiliaAsync(model));
        if (!await _service.UpdateSubfamiliaAsync(model)) return NotFound();
        TempData["LoginMessage"] = "La subfamilia se ha actualizado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> DeleteSubfamilia(int id)
    {
        var result = await _service.DeleteSubfamiliaAsync(id);
        if (!result.Exists) return NotFound();
        if (!result.Deleted) TempData["CatalogoError"] = "No se puede eliminar: la subfamilia tiene accesorios o tipos asociados.";
        else TempData["LoginMessage"] = "La subfamilia se ha eliminado.";
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidateFamiliaAsync(FamiliaFormViewModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.Nombre) && await _service.FamiliaNombreExisteAsync(model.Nombre, model.FamiliaId))
            ModelState.AddModelError(nameof(model.Nombre), "Ya existe una familia con ese nombre.");
    }

    private async Task ValidateSubfamiliaAsync(SubfamiliaFormViewModel model)
    {
        if (model.FamiliaId.HasValue && !await _service.FamiliaExisteAsync(model.FamiliaId.Value))
            ModelState.AddModelError(nameof(model.FamiliaId), "Selecciona una familia válida.");
        if (!string.IsNullOrWhiteSpace(model.Nombre) && model.FamiliaId.HasValue &&
            await _service.SubfamiliaNombreExisteAsync(model.FamiliaId.Value, model.Nombre, model.SubfamiliaId))
            ModelState.AddModelError(nameof(model.Nombre), "Ya existe una subfamilia con ese nombre dentro de la familia seleccionada.");
    }

    private async Task<SubfamiliaFormViewModel> PrepareSubfamiliaAsync(SubfamiliaFormViewModel model)
    {
        model.Familias = await _service.GetFamiliasAsync(model.FamiliaId);
        return model;
    }
}
