using CasaGaillard.Core.Models;
using CasaGaillard.Core.Security;
using CasaGaillard.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasaGaillard.Core.Areas.Mantenimiento.Controllers;

[Area("Mantenimiento"), Authorize(Roles = AppRoles.LecturaMantenimiento)]
public sealed class CubasController : Controller
{
    private readonly ICubasService _service;
    public CubasController(ICubasService service) => _service = service;
    public async Task<IActionResult> Index(bool incluirBajas = false)
    {
        ViewData["IncluirBajas"] = incluirBajas;
        return View(await _service.GetAllAsync(incluirBajas));
    }
    public async Task<IActionResult> Details(int? id)
    {
        if (!id.HasValue) return BadRequest();
        var model = await _service.GetByIdAsync(id.Value);
        return model is null ? NotFound() : View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Imagen(int id, string archivo)
    {
        var photo = await _service.GetImageAsync(id, archivo);
        return photo is null ? NotFound() : PhysicalFile(photo.Value.Path, photo.Value.ContentType);
    }

    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Create() => View("Form", await _service.GetFormAsync(null));

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Create(CubaFormViewModel model) => await Save(model, true);

    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await _service.GetFormAsync(id);
        return model is null ? NotFound() : View("Form", model);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Edit(CubaFormViewModel model) => await Save(model, false);

    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Delete(int id)
    {
        var model = await _service.GetByIdAsync(id);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!await _service.DeleteAsync(id))
        {
            TempData["Error"] = "No se puede eliminar esta cuba porque tiene compartimentos, grupos o revisiones vinculados.";
            return RedirectToAction(nameof(Delete), new { id });
        }
        return RedirectToAction(nameof(Index), new { incluirBajas = true });
    }

    private async Task<IActionResult> Save(CubaFormViewModel model, bool create)
    {
        if (model.PesoBruto < 0 || model.Tara < 0 || model.PesoMaxProducto < 0)
            ModelState.AddModelError(string.Empty, "Los pesos no pueden ser negativos.");
        if (model.PesoBruto.HasValue && model.Tara.HasValue && model.Tara > model.PesoBruto)
            ModelState.AddModelError(nameof(model.Tara), "La tara no puede superar el peso bruto.");
        if (!ModelState.IsValid)
        {
            var form = await _service.GetFormAsync(create ? null : model.Id);
            if (form is null) return NotFound();
            CopyFormValues(model, form);
            return View("Form", form);
        }
        if (!await _service.SaveAsync(model, create))
        {
            ModelState.AddModelError(nameof(model.Matricula), "Ya existe una cuba con esta matrícula.");
            var form = await _service.GetFormAsync(create ? null : model.Id);
            if (form is null) return NotFound();
            CopyFormValues(model, form);
            return View("Form", form);
        }
        return RedirectToAction(nameof(Index), new { incluirBajas = true });
    }

    private static void CopyFormValues(CubaFormViewModel source, CubaFormViewModel target)
    {
        var targetProperties = target.GetType().GetProperties().Where(p => p.CanWrite && (p.PropertyType == typeof(string) || !typeof(System.Collections.IEnumerable).IsAssignableFrom(p.PropertyType))).ToDictionary(p => p.Name);
        foreach (var property in source.GetType().GetProperties())
            if (targetProperties.TryGetValue(property.Name, out var destination) && destination.PropertyType == property.PropertyType)
                destination.SetValue(target, property.GetValue(source));
    }
}
