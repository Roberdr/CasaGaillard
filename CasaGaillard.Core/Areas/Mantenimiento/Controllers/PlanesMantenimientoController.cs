using CasaGaillard.Core.Models;
using CasaGaillard.Core.Security;
using CasaGaillard.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasaGaillard.Core.Areas.Mantenimiento.Controllers;

[Area("Mantenimiento"), Authorize(Roles = AppRoles.LecturaMantenimiento)]
public sealed class PlanesMantenimientoController : Controller
{
    private readonly IPlanesMantenimientoService _service;
    public PlanesMantenimientoController(IPlanesMantenimientoService service) => _service = service;
    public async Task<IActionResult> Index() => View(await _service.GetAllAsync());
    public async Task<IActionResult> Details(int? id) { if(!id.HasValue)return BadRequest();var model=await _service.GetDetailsAsync(id.Value);return model is null?NotFound():View(model); }
    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Create() => View(await _service.GetFormAsync(null));
    [HttpPost,ValidateAntiForgeryToken,Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Create(PlanMantenimientoFormViewModel model) { await ValidateAsync(model);if(!ModelState.IsValid)return View(await _service.PrepareFormAsync(model));await _service.SaveAsync(model);TempData["LoginMessage"]="El plan de mantenimiento se ha creado.";return RedirectToAction(nameof(Index)); }
    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Edit(int? id) { if(!id.HasValue)return BadRequest();var model=await _service.GetFormAsync(id);return model is null?NotFound():View(model); }
    [HttpPost,ValidateAntiForgeryToken,Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Edit(PlanMantenimientoFormViewModel model) { await ValidateAsync(model);if(!ModelState.IsValid)return View(await _service.PrepareFormAsync(model));if(!await _service.SaveAsync(model))return NotFound();TempData["LoginMessage"]="El plan de mantenimiento se ha actualizado.";return RedirectToAction(nameof(Index)); }
    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Delete(int? id) { if(!id.HasValue)return BadRequest();var model=await _service.GetDetailsAsync(id.Value);return model is null?NotFound():View(model); }
    [HttpPost,ActionName("Delete"),ValidateAntiForgeryToken,Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> DeleteConfirmed(int id) { if(!await _service.DeleteAsync(id))return NotFound();TempData["LoginMessage"]="El plan de mantenimiento se ha eliminado.";return RedirectToAction(nameof(Index)); }
    private async Task ValidateAsync(PlanMantenimientoFormViewModel model) { foreach(var error in await _service.ValidateAsync(model))ModelState.AddModelError(error.Key,error.Value); }
}
