using CasaGaillard.Core.Models;
using CasaGaillard.Core.Security;
using CasaGaillard.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasaGaillard.Core.Areas.Mantenimiento.Controllers;

[Area("Mantenimiento")]
[Authorize(Roles = AppRoles.LecturaMantenimiento)]
public sealed class TiposAccesorioController : Controller
{
    private readonly ITiposAccesorioService _service;
    public TiposAccesorioController(ITiposAccesorioService service) => _service = service;

    public async Task<IActionResult> Index() => View(await _service.GetAllAsync());

    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Create() => View(await PrepareAsync(new TipoAccesorioFormViewModel()));

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Create(TipoAccesorioFormViewModel model)
    {
        await ValidateAsync(model);
        if (!ModelState.IsValid) return View(await PrepareAsync(model));
        var existente = await _service.FindByNameAsync(model.Nombre);
        if (existente is not null)
        {
            ModelState.AddModelError(nameof(model.Nombre), "Ya existe un tipo de accesorio con ese nombre.");
            return View(await PrepareAsync(model));
        }
        await _service.CreateAsync(model);
        TempData["LoginMessage"] = "El tipo de accesorio se ha creado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> CrearRapido(TipoAccesorioQuickCreateRequest request)
    {
        var nombre = request.Nombre?.Trim();
        if (string.IsNullOrWhiteSpace(nombre)) return Json(new { ok = false, message = "Introduce el nombre del tipo de accesorio." });
        var existing = await _service.FindByNameAsync(nombre);
        if (existing is not null) return Json(new { ok = true, id = existing.Id, text = existing.Nombre, existing = true });
        if (nombre.Length > 45) return Json(new { ok = false, message = "El nombre no puede superar los 45 caracteres." });
        if (request.Descripcion?.Length > 300) return Json(new { ok = false, message = "La descripción no puede superar los 300 caracteres." });
        var model = new TipoAccesorioFormViewModel { Nombre = nombre, Descripcion = request.Descripcion, SubfamiliaId = request.SubfamiliaId };
        await ValidateAsync(model);
        if (!ModelState.IsValid)
            return Json(new { ok = false, message = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage ?? "Datos no válidos." });
        var id = await _service.CreateAsync(model);
        return Json(new { ok = true, id, text = nombre, existing = false });
    }

    private async Task ValidateAsync(TipoAccesorioFormViewModel model)
    {
        if (model.SubfamiliaId.HasValue && !await _service.SubfamiliaExisteAsync(model.SubfamiliaId.Value))
            ModelState.AddModelError(nameof(model.SubfamiliaId), "Selecciona una subfamilia válida.");
    }

    private async Task<TipoAccesorioFormViewModel> PrepareAsync(TipoAccesorioFormViewModel model)
    {
        model.Subfamilias = await _service.GetSubfamiliasAsync(model.SubfamiliaId);
        return model;
    }
}
