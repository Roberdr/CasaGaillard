using CasaGaillard.Core.Models;
using CasaGaillard.Core.Security;
using CasaGaillard.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasaGaillard.Core.Areas.Mantenimiento.Controllers;

[Area("Mantenimiento"), Authorize(Roles = AppRoles.LecturaMantenimiento)]
public sealed class GruposController : Controller
{
    private readonly IGruposService _service;
    public GruposController(IGruposService service) => _service = service;
    public async Task<IActionResult> Index() => View(await _service.GetAllAsync());
    public async Task<IActionResult> Details(int id) { var row=(await _service.GetAllAsync()).FirstOrDefault(x=>x.Id==id);return row is null?NotFound():View(row); }
    [Authorize(Roles = AppRoles.GestionMantenimiento)] public async Task<IActionResult> Create() => View("Form", await _service.GetFormAsync(null));
    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Create(GrupoFormViewModel model) => await Save(model, true);
    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Edit(int id) { var m = await _service.GetFormAsync(id); return m is null ? NotFound() : View("Form", m); }
    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Edit(GrupoFormViewModel model) => await Save(model, false);
    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Delete(int id) { var row=(await _service.GetAllAsync()).FirstOrDefault(x=>x.Id==id);return row is null?NotFound():View(row); }
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> DeleteConfirmed(int id) { await _service.DeleteAsync(id);return RedirectToAction(nameof(Index)); }
    private async Task<IActionResult> Save(GrupoFormViewModel model,bool create)
    {
        var relationError = await _service.ValidateAsync(model);
        if (relationError is not null) ModelState.AddModelError(string.Empty, relationError);
        if (!ModelState.IsValid) { var form=await _service.GetFormAsync(create?null:model.Id); if(form is null)return NotFound();form.TipoGrupoId=model.TipoGrupoId;form.CubaId=model.CubaId;form.CompartimentoId=model.CompartimentoId;form.SituacionId=model.SituacionId;return View("Form",form); }
        await _service.SaveAsync(model,create);return RedirectToAction(nameof(Index));
    }
}
