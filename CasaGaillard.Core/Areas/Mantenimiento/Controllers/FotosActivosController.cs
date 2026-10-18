using CasaGaillard.Core.Models;
using CasaGaillard.Core.Security;
using CasaGaillard.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasaGaillard.Core.Areas.Mantenimiento.Controllers;

[Area("Mantenimiento"), Authorize(Roles = AppRoles.LecturaMantenimiento)]
public sealed class FotosActivosController : Controller
{
    private const long MaxFileSize = 5 * 1024 * 1024;
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private readonly IFotosActivosService _service;

    public FotosActivosController(IFotosActivosService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Index(string? tipo, int id)
    {
        tipo = NormalizeTipo(tipo);
        if (tipo is null || id <= 0) return BadRequest();
        var model = await _service.GetAsync(tipo, id);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento), RequestSizeLimit(55 * 1024 * 1024)]
    public async Task<IActionResult> Subir(InstalacionEquipoFotosViewModel model, CancellationToken cancellationToken)
    {
        model.Tipo = NormalizeTipo(model.Tipo) ?? string.Empty;
        if (model.Tipo.Length == 0 || model.ActivoId <= 0) return BadRequest();
        var files = model.Archivos.Where(f => f.Length > 0).ToList();
        if (files.Count == 0) ModelState.AddModelError(nameof(model.Archivos), "Selecciona al menos una imagen.");
        if (files.Count > 10) ModelState.AddModelError(nameof(model.Archivos), "Puedes subir un máximo de 10 imágenes cada vez.");
        foreach (var file in files)
        {
            var extension = Path.GetExtension(Path.GetFileName(file.FileName));
            if (file.Length > MaxFileSize) ModelState.AddModelError(nameof(model.Archivos), $"{Path.GetFileName(file.FileName)} supera el límite de 5 MB.");
            if (!AllowedExtensions.Contains(extension) || !file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                ModelState.AddModelError(nameof(model.Archivos), $"{Path.GetFileName(file.FileName)} no es un formato de imagen permitido.");
        }
        var current = await _service.GetAsync(model.Tipo, model.ActivoId);
        if (current is null) return NotFound();
        if (!ModelState.IsValid)
        {
            current.Archivos = model.Archivos;
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors)) ModelState.AddModelError(string.Empty, error.ErrorMessage);
            return View("Index", current);
        }
        await _service.UploadAsync(model.Tipo, model.ActivoId, files, cancellationToken);
        TempData["Success"] = "Las imágenes se han subido correctamente.";
        return RedirectToAction(nameof(Index), new { tipo = model.Tipo, id = model.ActivoId });
    }

    [HttpGet]
    public async Task<IActionResult> Foto(int id)
    {
        var foto = await _service.GetFotoAsync(id);
        if (foto is null || !System.IO.File.Exists(foto.Value.PhysicalPath)) return NotFound();
        return PhysicalFile(foto.Value.PhysicalPath, foto.Value.ContentType);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Eliminar(int id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result.Exists) return NotFound();
        TempData["Success"] = "La imagen se ha eliminado.";
        return RedirectToAction(nameof(Index), new { tipo = result.Tipo, id = result.ActivoId });
    }

    private static string? NormalizeTipo(string? tipo) => tipo?.Trim().ToLowerInvariant() switch
    {
        "instalacion" => "Instalacion",
        "equipo" => "Equipo",
        _ => null
    };
}
