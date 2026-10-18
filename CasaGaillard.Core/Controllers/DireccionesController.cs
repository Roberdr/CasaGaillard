using CasaGaillard.Core.Models;
using CasaGaillard.Core.Security;
using CasaGaillard.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;

namespace CasaGaillard.Core.Controllers;

[Authorize(Roles = AppRoles.LecturaMantenimiento)]
public sealed class DireccionesController : Controller
{
    private readonly IOrganizacionService _service;
    public DireccionesController(IOrganizacionService service) => _service = service;

    public async Task<IActionResult> Index() => View(await _service.GetDireccionesAsync());
    public async Task<IActionResult> Details(int? id)
    {
        if (!id.HasValue) return BadRequest();
        var model = await _service.GetDireccionAsync(id);
        return model is null ? NotFound() : View(model);
    }
    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Create(int? personaId = null, bool paraEntidad = false)
    {
        var model = new DireccionForm { PersonaId = personaId, ParaEntidad = paraEntidad };
        await _service.PrepareDireccionAsync(model);
        return View(model);
    }
    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Create(DireccionForm model)
    {
        if (!ModelState.IsValid) { await _service.PrepareDireccionAsync(model); return View(model); }
        var id = await _service.SaveDireccionAsync(model);
        if (model.ParaEntidad) return RedirectToAction("Create", "Entidades", new { direccionId = id });
        if (model.PersonaId.HasValue) return RedirectToAction("Edit", "Personas", new { id = model.PersonaId, direccionId = id });
        return RedirectToAction(nameof(Index));
    }
    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Edit(int? id)
    {
        if (!id.HasValue) return BadRequest();
        var model = await _service.GetDireccionAsync(id);
        if (model is null) return NotFound();
        await _service.PrepareDireccionAsync(model);
        return View(model);
    }
    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Edit(DireccionForm model)
    {
        if (!ModelState.IsValid) { await _service.PrepareDireccionAsync(model); return View(model); }
        if (await _service.SaveDireccionAsync(model) is null) return NotFound();
        return RedirectToAction(nameof(Index));
    }
    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Delete(int? id)
    {
        if (!id.HasValue) return BadRequest();
        var model = await _service.GetDireccionAsync(id);
        return model is null ? NotFound() : View(model);
    }
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            if (!await _service.DeleteDireccionAsync(id)) return NotFound();
        }
        catch (DbException)
        {
            TempData["OrganizacionError"] = "No se puede eliminar la dirección porque está asociada a una persona o entidad.";
            return RedirectToAction(nameof(Delete), new { id });
        }
        return RedirectToAction(nameof(Index));
    }
}
