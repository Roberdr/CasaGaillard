using CasaGaillard.Core.Models;
using CasaGaillard.Core.Security;
using CasaGaillard.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasaGaillard.Core.Areas.Mantenimiento.Controllers;

[Area("Mantenimiento")]
[Authorize(Roles = AppRoles.LecturaMantenimiento)]
public sealed class InstalacionDocumentosController : Controller
{
    private const long MaximoBytes = 20 * 1024 * 1024;
    private static readonly HashSet<string> ExtensionesPermitidas = new(StringComparer.OrdinalIgnoreCase)
        { ".pdf", ".png", ".jpg", ".jpeg", ".doc", ".docx", ".xls", ".xlsx", ".dwg" };
    private readonly IInstalacionDocumentosService _service;
    public InstalacionDocumentosController(IInstalacionDocumentosService service) => _service = service;

    public async Task<IActionResult> Index(int? instalacionId)
    {
        if (!instalacionId.HasValue) return BadRequest();
        var model = await _service.GetAsync(instalacionId.Value);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    [RequestSizeLimit(MaximoBytes + 64 * 1024)]
    public async Task<IActionResult> Subir(InstalacionDocumentoFormViewModel model, CancellationToken cancellationToken)
    {
        var instalacion = await _service.GetAsync(model.InstalacionId);
        if (instalacion is null) return NotFound();
        model.InstalacionCodigo = instalacion.InstalacionCodigo;
        model.InstalacionNombre = instalacion.InstalacionNombre;

        var archivo = model.Archivo;
        var extension = archivo is null ? null : Path.GetExtension(Path.GetFileName(archivo.FileName));
        if (archivo is null || archivo.Length <= 0)
            ModelState.AddModelError(nameof(model.Archivo), "Selecciona un archivo.");
        else if (archivo.Length > MaximoBytes)
            ModelState.AddModelError(nameof(model.Archivo), "El archivo no puede superar los 20 MB.");
        else if (string.IsNullOrWhiteSpace(Path.GetFileName(archivo.FileName)))
            ModelState.AddModelError(nameof(model.Archivo), "El archivo seleccionado no tiene un nombre válido.");
        else if (string.IsNullOrWhiteSpace(extension) || !ExtensionesPermitidas.Contains(extension))
            ModelState.AddModelError(nameof(model.Archivo), "Formato no permitido. Usa PDF, imagen, Office o DWG.");

        if (!ModelState.IsValid)
        {
            model.Documentos = instalacion.Documentos;
            return View("Index", model);
        }

        await _service.SaveAsync(model, cancellationToken);
        TempData["LoginMessage"] = "El documento se ha guardado.";
        return RedirectToAction(nameof(Index), new { instalacionId = model.InstalacionId });
    }

    public async Task<IActionResult> Archivo(int id)
    {
        var documento = await _service.GetFileAsync(id);
        if (documento is null) return NotFound();
        var path = _service.ResolveStoredPath(documento.Value.RutaArchivo);
        if (path is null || !System.IO.File.Exists(path)) return NotFound();
        return PhysicalFile(path, "application/octet-stream", Path.GetFileName(documento.Value.Nombre));
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Eliminar(int id)
    {
        var instalacionId = await _service.DeleteAsync(id);
        if (!instalacionId.HasValue) return NotFound();
        TempData["LoginMessage"] = "El documento se ha eliminado.";
        return RedirectToAction(nameof(Index), new { instalacionId });
    }
}
