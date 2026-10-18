using CasaGaillard.Core.Models;
using CasaGaillard.Core.Security;
using CasaGaillard.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasaGaillard.Core.Controllers;

[Area("Mantenimiento")]
[Authorize(Roles = AppRoles.LecturaMantenimiento)]
public class IntervencionesController : Controller
{
    private readonly IIntervencionesService _service;

    public IntervencionesController(IIntervencionesService service) => _service = service;

    public async Task<IActionResult> Index(int? tareaId)
    {
        var model = await _service.GetIndexAsync(tareaId);
        return model is null ? NotFound() : View(model);
    }

    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Create(int? tareaId)
    {
        if (!tareaId.HasValue) return BadRequest();
        var model = await _service.GetCreateAsync(tareaId.Value);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Create(IntervencionFormViewModel model)
    {
        if (model.FechaFin.HasValue && model.FechaFin.Value < model.FechaInicio)
            ModelState.AddModelError(nameof(model.FechaFin), "La fecha de fin no puede ser anterior al inicio.");

        if (ModelState.IsValid && await _service.SaveCreateAsync(model, User.Identity?.Name))
        {
            TempData["LoginMessage"] = "La intervención se ha registrado.";
            return RedirectToAction(nameof(Index), new { tareaId = model.TareaMantenimientoId });
        }

        await RepopulateFormAsync(model);
        if (ModelState.IsValid)
            ModelState.AddModelError(nameof(model.TecnicoPersonaID), "La tarea o el técnico seleccionado ya no existe.");
        return View(model);
    }

    public async Task<IActionResult> Details(int? id, int? nuevoAccesorioId)
    {
        if (!id.HasValue) return BadRequest();
        var model = await _service.GetDetailsAsync(id.Value, nuevoAccesorioId);
        return model is null ? NotFound() : View(model);
    }

    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Edit(int? id)
    {
        if (!id.HasValue) return BadRequest();
        var model = await _service.GetForEditAsync(id.Value);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Edit(IntervencionFormViewModel model)
    {
        if (model.FechaFin.HasValue && model.FechaFin.Value < model.FechaInicio)
            ModelState.AddModelError(nameof(model.FechaFin), "La fecha de fin no puede ser anterior al inicio.");

        if (ModelState.IsValid && await _service.SaveUpdateAsync(model))
        {
            TempData["LoginMessage"] = "La intervención se ha actualizado.";
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        await RepopulateFormAsync(model);
        if (ModelState.IsValid)
            ModelState.AddModelError(nameof(model.TecnicoPersonaID), "La tarea o el técnico seleccionado ya no existe.");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> AddRepuesto(IntervencionRepuestoFormViewModel model)
    {
        if (ModelState.IsValid && await _service.AddRepuestoAsync(model))
            TempData["LoginMessage"] = "El repuesto utilizado se ha añadido.";
        else
            TempData["LoginMessage"] = "No se pudo añadir el repuesto. Revisa la cantidad, el coste y el accesorio.";
        return RedirectToAction(nameof(Details), new { id = model.IntervencionId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> RemoveRepuesto(int id)
    {
        var interventionId = await _service.RemoveRepuestoAsync(id);
        return interventionId.HasValue ? RedirectToAction(nameof(Details), new { id = interventionId.Value }) : NotFound();
    }

    private async Task RepopulateFormAsync(IntervencionFormViewModel model)
    {
        var task = await _service.GetTareaSummaryAsync(model.TareaMantenimientoId);
        if (task is not null)
        {
            model.TareaCodigo = task.Codigo;
            model.TareaTitulo = task.Titulo;
        }
        model.Personas = await _service.GetPersonasAsync(model.TecnicoPersonaID);
    }
}
