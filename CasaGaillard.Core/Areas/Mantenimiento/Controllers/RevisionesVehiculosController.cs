using CasaGaillard.Core.Models;
using CasaGaillard.Core.Security;
using CasaGaillard.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasaGaillard.Core.Areas.Mantenimiento.Controllers;

[Area("Mantenimiento"), Authorize(Roles = AppRoles.LecturaMantenimiento)]
public sealed class RevisionesVehiculosController : Controller
{
    private readonly IRevisionesVehiculosService _service;
    public RevisionesVehiculosController(IRevisionesVehiculosService service) => _service = service;

    public async Task<IActionResult> Index(string? matriculaVehiculo, bool incluirBajas = false)
    {
        ViewData["MatriculaVehiculo"] = matriculaVehiculo;
        ViewData["IncluirBajas"] = incluirBajas;
        return View(await _service.GetAllAsync(string.IsNullOrWhiteSpace(matriculaVehiculo) ? null : matriculaVehiculo.Trim(), incluirBajas));
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> ActualizarReserva(int id, DateTime? fechaReservada, string? matriculaVehiculo, bool incluirBajas = false, bool volverPanel = false)
    {
        if (!await _service.UpdateReservaAsync(id, fechaReservada)) return NotFound();
        TempData["Success"] = "La reserva de la revisión se ha actualizado.";
        return volverPanel ? RedirectToAction("Index", "Home") : RedirectToAction(nameof(Index), new { matriculaVehiculo, incluirBajas });
    }

    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Proximas() => View("ProximasRevisiones", await _service.GetProximasAsync());

    public async Task<IActionResult> Details(int id)
    {
        var row = (await _service.GetAllAsync(null, true)).FirstOrDefault(x => x.Id == id);
        return row is null ? NotFound() : View(row);
    }

    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Create(int? vehiculoId, int? tipoRevisionId) => View("Form", await _service.GetFormAsync(null, vehiculoId, tipoRevisionId));

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Create(RevisionVehiculoFormViewModel model) => await Save(model, true);

    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await _service.GetFormAsync(id, null, null);
        return model is null ? NotFound() : View("Form", model);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Edit(RevisionVehiculoFormViewModel model) => await Save(model, false);

    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Delete(int id)
    {
        var row = (await _service.GetAllAsync(null, true)).FirstOrDefault(x => x.Id == id);
        return row is null ? NotFound() : View(row);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _service.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    private async Task<IActionResult> Save(RevisionVehiculoFormViewModel model, bool create)
    {
        if (!ModelState.IsValid)
        {
            var invalid = await _service.GetFormAsync(create ? null : model.Id, model.VehiculoId, model.TipoRevisionId);
            if (invalid is null) return NotFound();
            invalid.FechaRevision = model.FechaRevision; invalid.Caducidad = model.Caducidad; invalid.Detalles = model.Detalles;
            invalid.EjecutorPersonasEntidadId = model.EjecutorPersonasEntidadId;
            return View("Form", invalid);
        }
        if (!await _service.SaveAsync(model, create))
        {
            ModelState.AddModelError(nameof(model.EjecutorPersonasEntidadId), "Selecciona una persona vinculada a una entidad activa.");
            var invalid = await _service.GetFormAsync(create ? null : model.Id, model.VehiculoId, model.TipoRevisionId);
            if (invalid is null) return NotFound();
            invalid.FechaRevision = model.FechaRevision; invalid.Caducidad = model.Caducidad; invalid.Detalles = model.Detalles;
            invalid.EjecutorPersonasEntidadId = model.EjecutorPersonasEntidadId;
            return View("Form", invalid);
        }
        TempData["Success"] = "La revisión se ha guardado.";
        return RedirectToAction(nameof(Index));
    }
}
