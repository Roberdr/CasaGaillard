using CasaGaillard.Core.Models;
using CasaGaillard.Core.Security;
using CasaGaillard.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;

namespace CasaGaillard.Core.Controllers;

[Authorize(Roles = AppRoles.LecturaMantenimiento)]
public sealed class PersonasController : Controller
{
    private readonly IOrganizacionService _service;
    public PersonasController(IOrganizacionService service) => _service = service;
    public async Task<IActionResult> Index() => View(await _service.GetPersonasAsync());
    public async Task<IActionResult> Details(int? id)
    {
        if (!id.HasValue) return BadRequest();
        var model = await _service.GetPersonaAsync(id);
        return model is null ? NotFound() : View(model);
    }
    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Create()
    {
        var model = new PersonaForm(); await _service.PreparePersonaAsync(model); return View(model);
    }
    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Create(PersonaForm model)
    {
        if (!ModelState.IsValid) { await _service.PreparePersonaAsync(model); return View(model); }
        await _service.SavePersonaAsync(model); return RedirectToAction(nameof(Index));
    }
    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Edit(int? id, int? direccionId = null)
    {
        if (!id.HasValue) return BadRequest();
        var model = await _service.GetPersonaAsync(id);
        if (model is null) return NotFound();
        if (direccionId.HasValue) model.DireccionId = direccionId;
        await _service.PreparePersonaAsync(model); return View(model);
    }
    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Edit(PersonaForm model)
    {
        if (!ModelState.IsValid) { await _service.PreparePersonaAsync(model); return View(model); }
        if (await _service.SavePersonaAsync(model) is null) return NotFound();
        return RedirectToAction(nameof(Index));
    }
    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Delete(int? id)
    {
        if (!id.HasValue) return BadRequest();
        var model = await _service.GetPersonaAsync(id); return model is null ? NotFound() : View(model);
    }
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            if (!await _service.DeletePersonaAsync(id)) return NotFound();
        }
        catch (DbException)
        {
            TempData["OrganizacionError"] = "No se puede eliminar la persona porque tiene teléfonos o relaciones con entidades.";
            return RedirectToAction(nameof(Delete), new { id });
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> AgregarTelefono(int personaId, string telefono, string returnTo = "Details")
    {
        if (!await _service.AddPersonaTelefonoAsync(personaId, telefono ?? "")) TempData["OrganizacionError"] = "El teléfono no es válido o la persona ya no existe.";
        return RedirectToAction(ReturnAction(returnTo), new { id = personaId });
    }
    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> ActualizarTelefono(int personaId, int id, string telefono, string returnTo = "Details")
    {
        if (!await _service.UpdatePersonaTelefonoAsync(personaId, id, telefono ?? "")) TempData["OrganizacionError"] = "No se pudo actualizar el teléfono.";
        return RedirectToAction(ReturnAction(returnTo), new { id = personaId });
    }
    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> EliminarTelefono(int personaId, int id, string returnTo = "Details")
    {
        if (!await _service.DeletePersonaTelefonoAsync(personaId, id)) TempData["OrganizacionError"] = "No se pudo eliminar el teléfono.";
        return RedirectToAction(ReturnAction(returnTo), new { id = personaId });
    }
    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> AgregarEntidad(int personaId, int entidadId, int? cargoId, DateTime? fechaAlta)
    {
        if (!await _service.AddPersonaEntidadAsync(personaId, entidadId, cargoId, fechaAlta)) TempData["OrganizacionError"] = "No se pudo crear la relación; comprueba si ya existe una relación activa.";
        return RedirectToAction(nameof(Details), new { id = personaId });
    }
    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> EditarRelacion(int personaId, int relacionId, int? cargoId, DateTime? fechaAlta, DateTime? fechaBaja)
    {
        if (!await _service.UpdatePersonaEntidadAsync(personaId, relacionId, cargoId, fechaAlta, fechaBaja)) TempData["OrganizacionError"] = "No se pudo actualizar la relación; revisa sus fechas y estado.";
        return RedirectToAction(nameof(Details), new { id = personaId });
    }
    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> DarDeBajaRelacion(int personaId, int relacionId)
    {
        if (!await _service.DarDeBajaPersonaEntidadAsync(personaId, relacionId)) TempData["OrganizacionError"] = "No se pudo dar de baja la relación.";
        return RedirectToAction(nameof(Details), new { id = personaId });
    }

    private static string ReturnAction(string value) => string.Equals(value, "Edit", StringComparison.OrdinalIgnoreCase) ? "Edit" : "Details";
}
