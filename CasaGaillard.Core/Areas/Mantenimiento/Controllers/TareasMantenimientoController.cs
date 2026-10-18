using CasaGaillard.Core.Security;
using CasaGaillard.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CasaGaillard.Core.Models;

namespace CasaGaillard.Core.Areas.Mantenimiento.Controllers;

[Area("Mantenimiento")]
[Authorize(Roles = AppRoles.LecturaMantenimiento)]
public sealed class TareasMantenimientoController : Controller
{
    private readonly ITareasMantenimientoService _service;
    private readonly IWebHostEnvironment _environment;

    public TareasMantenimientoController(ITareasMantenimientoService service, IWebHostEnvironment environment)
    {
        _service = service;
        _environment = environment;
    }

    public async Task<IActionResult> Index(string? codigo)
    {
        ViewData["Codigo"] = codigo;
        return View(await _service.GetAllAsync(codigo));
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (!id.HasValue) return BadRequest();
        var model = await _service.GetDetailsAsync(id.Value);
        return model is null ? NotFound() : View(model);
    }

    public async Task<IActionResult> Create()
    {
        return View(await _service.GetFormAsync(null));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TareaMantenimientoFormViewModel model)
    {
        ValidatePhotos(model.FotosAdjuntas);
        if (!ModelState.IsValid) return View(await _service.PrepareFormAsync(model));

        var id = await _service.CreateAsync(model, User.Identity?.Name);
        await SavePhotosAsync(id, model.FotosAdjuntas);
        TempData["LoginMessage"] = "La tarea de mantenimiento se ha creado correctamente.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Edit(int? id)
    {
        if (!id.HasValue) return BadRequest();
        var model = await _service.GetFormAsync(id.Value);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Edit(TareaMantenimientoFormViewModel model)
    {
        ValidatePhotos(model.FotosAdjuntas);
        if (!ModelState.IsValid) return View(await _service.PrepareFormAsync(model));
        if (!await _service.UpdateAsync(model, User.Identity?.Name)) return NotFound();
        await SavePhotosAsync(model.ID, model.FotosAdjuntas);
        TempData["LoginMessage"] = "La tarea se ha actualizado.";
        return RedirectToAction(nameof(Details), new { id = model.ID });
    }

    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Gestion(int? id)
    {
        if (!id.HasValue) return BadRequest();
        var model = await _service.GetGestionAsync(id.Value);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Gestion(TareaMantenimientoGestionViewModel model)
    {
        if (!ModelState.IsValid) return View(await _service.PrepareGestionAsync(model));
        if (!await _service.UpdateGestionAsync(model, User.Identity?.Name)) return NotFound();
        TempData["LoginMessage"] = "La tarea se ha actualizado.";
        return RedirectToAction(nameof(Details), new { id = model.ID });
    }

    [HttpGet]
    public async Task<IActionResult> Foto(int id)
    {
        var foto = await _service.GetFotoAsync(id);
        if (foto is null) return NotFound();

        var photoRoot = Path.GetFullPath(Path.Combine(_environment.ContentRootPath, "..", "App_Data", "TareasMantenimientoFotos"));
        var relativePath = (foto.RutaArchivo ?? string.Empty).TrimStart('~', '/', '\\')
            .Replace('/', Path.DirectorySeparatorChar);
        var physicalPath = Path.GetFullPath(Path.Combine(_environment.ContentRootPath, "..", relativePath));
        if (!physicalPath.StartsWith(photoRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            return NotFound();
        if (!System.IO.File.Exists(physicalPath)) return NotFound();

        return PhysicalFile(physicalPath,
            string.IsNullOrWhiteSpace(foto.ContentType) ? "application/octet-stream" : foto.ContentType);
    }

    private void ValidatePhotos(IEnumerable<IFormFile> files)
    {
        var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };
        foreach (var file in files.Where(file => file.Length > 0))
        {
            if (!file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase) ||
                !allowedExtensions.Contains(Path.GetExtension(file.FileName)))
                ModelState.AddModelError(nameof(TareaMantenimientoFormViewModel.FotosAdjuntas), "Solo se admiten archivos de imagen.");
            if (file.Length > 10 * 1024 * 1024)
                ModelState.AddModelError(nameof(TareaMantenimientoFormViewModel.FotosAdjuntas), "Cada imagen debe pesar como máximo 10 MB.");
        }
    }

    private async Task SavePhotosAsync(int tareaId, IEnumerable<IFormFile> files)
    {
        var root = Path.GetFullPath(Path.Combine(_environment.ContentRootPath, "..", "App_Data", "TareasMantenimientoFotos"));
        var folder = Path.Combine(root, tareaId.ToString());
        Directory.CreateDirectory(folder);
        foreach (var file in files.Where(file => file.Length > 0))
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var storedName = Guid.NewGuid().ToString("N") + extension;
            var fullPath = Path.Combine(folder, storedName);
            await using (var output = System.IO.File.Create(fullPath))
                await file.CopyToAsync(output);
            await _service.AddFotoAsync(tareaId, storedName, file.FileName, file.ContentType, DateTime.Now);
        }
    }
}
