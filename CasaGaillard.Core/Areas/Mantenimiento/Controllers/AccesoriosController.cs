using CasaGaillard.Core.Models;
using CasaGaillard.Core.Security;
using CasaGaillard.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasaGaillard.Core.Areas.Mantenimiento.Controllers;

[Area("Mantenimiento")]
[Authorize(Roles = AppRoles.LecturaMantenimiento)]
public sealed class AccesoriosController : Controller
{
    private const long MaxPhotoBytes = 10 * 1024 * 1024;
    private const long MaxPhotosRequestBytes = 32 * 1024 * 1024;
    private static readonly HashSet<string> ImageExtensions = new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private readonly IAccesoriosService _service;
    public AccesoriosController(IAccesoriosService service) => _service = service;

    public async Task<IActionResult> Index(int? tipoAccesorioId, int? materialId, int? familiaId, int? subfamiliaId, string? q, int page = 1, int pageSize = 12) =>
        View(await _service.GetIndexAsync(tipoAccesorioId, materialId, familiaId, subfamiliaId, q, page, pageSize));

    public async Task<IActionResult> Details(int? id)
    {
        if (!id.HasValue) return BadRequest();
        var model = await _service.GetDetailsAsync(id.Value);
        return model is null ? NotFound() : View(model);
    }

    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Create(int? intervencionId) => View(await _service.GetFormAsync(null, intervencionId));

    [HttpGet]
    public async Task<IActionResult> Subfamilias(int familiaId)
    {
        var options = await _service.GetSubfamiliasAsync(familiaId, null);
        return Json(options.Select(option => new { value = option.Value, text = option.Text }));
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    [RequestSizeLimit(MaxPhotosRequestBytes)]
    public async Task<IActionResult> Create(AccesorioFormViewModel model, CancellationToken cancellationToken)
    {
        await ValidateFormAsync(model);
        ValidatePhotos(model.FotosAdjuntas);
        ValidatePhotoCaptions(model);
        if (!ModelState.IsValid)
        {
            var prepared = await _service.GetFormAsync(null);
            var form = MergeForm(model, prepared!);
            form.Subfamilias = await _service.GetSubfamiliasAsync(model.FamiliaId, model.SubfamiliaId);
            return View(form);
        }
        model.Id = 0;
        var id = await _service.SaveAsync(model, cancellationToken);
        TempData["LoginMessage"] = "El accesorio se ha creado correctamente.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Edit(int? id)
    {
        if (!id.HasValue) return BadRequest();
        var model = await _service.GetFormAsync(id.Value);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    [RequestSizeLimit(MaxPhotosRequestBytes)]
    public async Task<IActionResult> Edit(int id, AccesorioFormViewModel model, CancellationToken cancellationToken)
    {
        model.Id = id;
        await ValidateFormAsync(model);
        ValidatePhotos(model.FotosAdjuntas);
        ValidatePhotoCaptions(model);
        if (!ModelState.IsValid)
        {
            var prepared = await _service.GetFormAsync(model.Id);
            var form = prepared is null ? model : MergeForm(model, prepared);
            form.Subfamilias = await _service.GetSubfamiliasAsync(model.FamiliaId, model.SubfamiliaId);
            return View(form);
        }
        var savedId = await _service.SaveAsync(model, cancellationToken);
        if (savedId == 0) return NotFound();
        TempData["LoginMessage"] = "El accesorio se ha actualizado.";
        return RedirectToAction(nameof(Details), new { id = savedId });
    }

    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Delete(int? id)
    {
        if (!id.HasValue) return BadRequest();
        var model = await _service.GetDetailsAsync(id.Value);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!await _service.DeleteAsync(id)) return NotFound();
        TempData["LoginMessage"] = "El accesorio se ha eliminado.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Foto(int id)
    {
        var foto = await _service.GetFotoAsync(id);
        if (foto is null) return NotFound();
        var path = _service.ResolveFotoPath(foto.Value.RutaArchivo);
        if (path is null || !System.IO.File.Exists(path)) return NotFound();
        return PhysicalFile(path, string.IsNullOrWhiteSpace(foto.Value.ContentType) ? "application/octet-stream" : foto.Value.ContentType);
    }

    private async Task ValidateFormAsync(AccesorioFormViewModel model)
    {
        var prepared = await _service.GetFormAsync(null);
        if (model.TipoAccesorioId.HasValue && !prepared!.TiposAccesorio.Any(x => x.Value == model.TipoAccesorioId.Value.ToString()))
            ModelState.AddModelError(nameof(model.TipoAccesorioId), "Selecciona un tipo de accesorio válido.");
        if (model.MaterialId.HasValue && !prepared!.Materiales.Any(x => x.Value == model.MaterialId.Value.ToString()))
            ModelState.AddModelError(nameof(model.MaterialId), "Selecciona un material válido.");
        if (model.FamiliaId.HasValue && !prepared!.Familias.Any(x => x.Value == model.FamiliaId.Value.ToString()))
            ModelState.AddModelError(nameof(model.FamiliaId), "Selecciona una familia válida.");
        if (model.UbicacionId.HasValue && !prepared!.Ubicaciones.Any(x => x.Value == model.UbicacionId.Value.ToString()))
            ModelState.AddModelError(nameof(model.UbicacionId), "Selecciona una ubicación válida.");
        if (model.SubfamiliaId.HasValue)
        {
            if (!model.FamiliaId.HasValue) ModelState.AddModelError(nameof(model.SubfamiliaId), "Selecciona primero una familia.");
            else
            {
                var subfamilias = await _service.GetSubfamiliasAsync(model.FamiliaId, null);
                if (!subfamilias.Any(x => x.Value == model.SubfamiliaId.Value.ToString()))
                    ModelState.AddModelError(nameof(model.SubfamiliaId), "La subfamilia no pertenece a la familia indicada.");
            }
        }
    }

    private void ValidatePhotos(IEnumerable<Microsoft.AspNetCore.Http.IFormFile> photos)
    {
        foreach (var photo in photos.Where(x => x.Length > 0))
        {
            if (photo.Length > MaxPhotoBytes || !ImageExtensions.Contains(Path.GetExtension(Path.GetFileName(photo.FileName))) ||
                string.IsNullOrWhiteSpace(photo.ContentType) || !photo.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                ModelState.AddModelError(nameof(AccesorioFormViewModel.FotosAdjuntas), "Adjunta imágenes JPG, PNG, GIF o WEBP de hasta 10 MB cada una.");
        }
    }

    private void ValidatePhotoCaptions(AccesorioFormViewModel model)
    {
        if (model.ObservacionesFotos.Any(text => text?.Length > 500))
            ModelState.AddModelError(nameof(model.ObservacionesFotos), "El pie de foto no puede superar los 500 caracteres.");
    }

    private static AccesorioFormViewModel MergeForm(AccesorioFormViewModel posted, AccesorioFormViewModel prepared)
    {
        posted.TiposAccesorio = prepared.TiposAccesorio; posted.Materiales = prepared.Materiales; posted.Familias = prepared.Familias;
        posted.Ubicaciones = prepared.Ubicaciones; posted.Caracteristicas = prepared.Caracteristicas; posted.Unidades = prepared.Unidades;
        posted.Subfamilias = prepared.Subfamilias;
        posted.FotosExistentes = prepared.FotosExistentes;
        return posted;
    }
}
