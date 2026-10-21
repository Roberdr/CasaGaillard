using CasaGaillard.Core.Models;
using CasaGaillard.Core.Security;
using CasaGaillard.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;

namespace CasaGaillard.Core.Controllers;

[Authorize(Roles = AppRoles.LecturaMantenimiento)]
public sealed class EntidadesController : Controller
{
    private readonly IOrganizacionService _service;
    public EntidadesController(IOrganizacionService service) => _service = service;
    public async Task<IActionResult> Index() => View(await _service.GetEntidadesAsync());
    public async Task<IActionResult> Details(int? id)
    {
        if (!id.HasValue) return BadRequest();
        var model = await _service.GetEntidadAsync(id); return model is null ? NotFound() : View(model);
    }
    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Create(int? direccionId = null)
    {
        var model = new EntidadForm { DireccionId = direccionId ?? 0 }; await _service.PrepareEntidadAsync(model); return View(model);
    }
    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Create(EntidadForm model)
    {
        if (!ModelState.IsValid) { await _service.PrepareEntidadAsync(model); return View(model); }
        await _service.SaveEntidadAsync(model); return RedirectToAction(nameof(Index));
    }
    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Edit(int? id)
    {
        if (!id.HasValue) return BadRequest();
        var model = await _service.GetEntidadAsync(id); if (model is null) return NotFound();
        await _service.PrepareEntidadAsync(model); return View(model);
    }
    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Edit(EntidadForm model)
    {
        if (!ModelState.IsValid) { await _service.PrepareEntidadAsync(model); return View(model); }
        if (await _service.SaveEntidadAsync(model) is null) return NotFound();
        return RedirectToAction(nameof(Index));
    }
    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Delete(int? id)
    {
        if (!id.HasValue) return BadRequest();
        var model = await _service.GetEntidadAsync(id); return model is null ? NotFound() : View(model);
    }
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            if (!await _service.DeleteEntidadAsync(id)) return NotFound();
        }
        catch (DbException)
        {
            TempData["OrganizacionError"] = "No se puede eliminar la entidad porque tiene teléfonos o relaciones asociadas.";
            return RedirectToAction(nameof(Delete), new { id });
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> AgregarTelefono(int entidadId, int personaId, string telefono)
    {
        if (!await _service.AddEntidadTelefonoAsync(entidadId, personaId, telefono ?? "")) TempData["OrganizacionError"] = "Selecciona una persona válida e indica un teléfono de hasta 10 caracteres.";
        return RedirectToAction(nameof(Details), new { id = entidadId });
    }
    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> ActualizarTelefono(int entidadId, int id, int? personaId, string telefono)
    {
        if (!await _service.UpdateEntidadTelefonoAsync(entidadId, id, personaId, telefono ?? "")) TempData["OrganizacionError"] = "Selecciona una persona válida para el teléfono de trabajo.";
        return RedirectToAction(nameof(Details), new { id = entidadId });
    }
    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> EliminarTelefono(int entidadId, int id)
    {
        if (!await _service.DeleteEntidadTelefonoAsync(entidadId, id)) TempData["OrganizacionError"] = "No se pudo eliminar el teléfono.";
        return RedirectToAction(nameof(Details), new { id = entidadId });
    }
    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> AgregarPersona(int entidadId, int personaId, int? cargoId, DateTime? fechaAlta)
    {
        if (!await _service.AddEntidadPersonaAsync(entidadId, personaId, cargoId, fechaAlta)) TempData["OrganizacionError"] = "No se pudo crear la relación; comprueba si ya existe una relación activa.";
        return RedirectToAction(nameof(Details), new { id = entidadId });
    }
    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> EditarRelacion(int entidadId, int relacionId, int? cargoId, DateTime? fechaAlta, DateTime? fechaBaja)
    {
        if (!await _service.UpdateEntidadPersonaAsync(entidadId, relacionId, cargoId, fechaAlta, fechaBaja)) TempData["OrganizacionError"] = "No se pudo actualizar la relación; revisa sus fechas y estado.";
        return RedirectToAction(nameof(Details), new { id = entidadId });
    }
    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> DarDeBajaPersona(int entidadId, int relacionId)
    {
        if (!await _service.DarDeBajaEntidadPersonaAsync(entidadId, relacionId)) TempData["OrganizacionError"] = "No se pudo dar de baja la relación.";
        return RedirectToAction(nameof(Details), new { id = entidadId });
    }
}
