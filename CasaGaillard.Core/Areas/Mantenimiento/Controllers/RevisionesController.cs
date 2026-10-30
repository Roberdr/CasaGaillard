using CasaGaillard.Core.Models;
using CasaGaillard.Core.Security;
using CasaGaillard.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasaGaillard.Core.Areas.Mantenimiento.Controllers;

[Area("Mantenimiento"),Authorize(Roles=AppRoles.LecturaMantenimiento)]
public sealed class RevisionesController : Controller
{
    private readonly IRevisionesService _service;
    public RevisionesController(IRevisionesService service)=>_service=service;

    public async Task<IActionResult> Index(int? cubaId,string? matriculaCuba,bool incluirBajas=false)
        =>View(await _service.GetIndexAsync(cubaId,matriculaCuba,incluirBajas));

    [HttpPost,ValidateAntiForgeryToken,Authorize(Roles=AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> ActualizarReserva(int id,DateTime? fechaReservada,int? cubaId,string? matriculaCuba,bool incluirBajas=false,bool volverPanel=false)
    {
        if(!await _service.UpdateReservaAsync(id,fechaReservada)) return NotFound();
        TempData["LoginMessage"]="La reserva de la revisión se ha actualizado.";
        return volverPanel ? RedirectToAction("Index", "Home") : RedirectToAction(nameof(Index),new{cubaId,matriculaCuba,incluirBajas});
    }
    public async Task<IActionResult> Details(int? id){if(!id.HasValue)return BadRequest();var model=await _service.GetDetailsAsync(id.Value);return model is null?NotFound():View(model);}

    [Authorize(Roles=AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Create(int? cubaId)=>View(await _service.GetFormAsync(null,cubaId));
    [HttpPost,ValidateAntiForgeryToken,Authorize(Roles=AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Create(RevisionFormViewModel model){await ValidateAsync(model,true);if(!ModelState.IsValid)return View(await _service.PrepareFormAsync(model));await _service.SaveAsync(model);TempData["LoginMessage"]="La revisión se ha registrado.";return RedirectToAction(nameof(Index),new{cubaId=model.CubaId});}

    [Authorize(Roles=AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Edit(int? id){if(!id.HasValue)return BadRequest();var model=await _service.GetFormAsync(id);return model is null?NotFound():View(model);}
    [HttpPost,ValidateAntiForgeryToken,Authorize(Roles=AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Edit(RevisionFormViewModel model){await ValidateAsync(model,false);if(!ModelState.IsValid)return View(await _service.PrepareFormAsync(model));if(!await _service.SaveAsync(model))return NotFound();TempData["LoginMessage"]="La revisión se ha actualizado.";return RedirectToAction(nameof(Index),new{cubaId=model.CubaId});}

    [Authorize(Roles=AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Delete(int? id){if(!id.HasValue)return BadRequest();var model=await _service.GetDetailsAsync(id.Value);return model is null?NotFound():View(model);}
    [HttpPost,ActionName("Delete"),ValidateAntiForgeryToken,Authorize(Roles=AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> DeleteConfirmed(int id){var revision=await _service.GetDetailsAsync(id);if(revision is null)return NotFound();await _service.DeleteAsync(id);TempData["LoginMessage"]="La revisión se ha eliminado.";return RedirectToAction(nameof(Index),new{cubaId=revision.CubaId});}

    private async Task ValidateAsync(RevisionFormViewModel model,bool creating){foreach(var error in await _service.ValidateAsync(model,creating))ModelState.AddModelError(error.Key,error.Value);}
}
