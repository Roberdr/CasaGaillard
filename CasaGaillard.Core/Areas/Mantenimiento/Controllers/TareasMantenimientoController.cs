using CasaGaillard.Core.Security;
using CasaGaillard.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CasaGaillard.Core.Models;
using CasaGaillard.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace CasaGaillard.Core.Areas.Mantenimiento.Controllers;

[Area("Mantenimiento")]
[Authorize(Roles = AppRoles.LecturaMantenimiento)]
public sealed class TareasMantenimientoController : Controller
{
    private readonly ITareasMantenimientoService _service;
    private readonly IOrganizacionService _organizacion;
    private readonly IWebHostEnvironment _environment;
    private readonly AppDbContext _db;

    public TareasMantenimientoController(ITareasMantenimientoService service, IOrganizacionService organizacion, IWebHostEnvironment environment, AppDbContext db)
    {
        _service = service;
        _organizacion = organizacion;
        _environment = environment;
        _db = db;
    }

    public async Task<IActionResult> Index(string? busqueda, string? estado, string? prioridad, int pagina = 1, string? codigo = null, DateTime? fechaDesde = null, DateTime? fechaHasta = null, string? equipo = null, string? responsable = null)
    {
        busqueda = string.IsNullOrWhiteSpace(busqueda) ? codigo : busqueda;
        return View(await _service.GetAllAsync(busqueda, estado, prioridad, pagina, fechaDesde, fechaHasta, equipo, responsable));
    }

    public async Task<IActionResult> Exportar(string? busqueda, string? estado, string? prioridad, DateTime? fechaDesde = null, DateTime? fechaHasta = null, string? equipo = null, string? responsable = null)
    {
        if(fechaDesde.HasValue&&fechaHasta.HasValue&&fechaHasta.Value.Date<fechaDesde.Value.Date) return BadRequest("El rango de fechas no es válido.");
        var query=_db.TareasMantenimiento.AsNoTracking().AsQueryable();
        if(!string.IsNullOrWhiteSpace(busqueda)){var q=busqueda.Trim();query=query.Where(t=>t.Codigo.Contains(q)||t.Titulo.Contains(q)||(t.Equipo!=null&&t.Equipo.Contains(q))||(t.Ubicacion!=null&&t.Ubicacion.Contains(q)));}
        if(!string.IsNullOrWhiteSpace(estado))query=query.Where(t=>t.Estado==estado);
        if(!string.IsNullOrWhiteSpace(prioridad))query=query.Where(t=>t.Prioridad==prioridad);
        if(fechaDesde.HasValue)query=query.Where(t=>t.FechaProgramada>=fechaDesde.Value.Date);
        if(fechaHasta.HasValue)query=query.Where(t=>t.FechaProgramada<fechaHasta.Value.Date.AddDays(1));
        if(!string.IsNullOrWhiteSpace(equipo))query=query.Where(t=>t.Equipo!=null&&t.Equipo.Contains(equipo.Trim()));
        if(!string.IsNullOrWhiteSpace(responsable))query=query.Where(t=>t.AsignadaA!=null&&t.AsignadaA.Contains(responsable.Trim()));
        var tareas=await query.OrderBy(t=>t.FechaProgramada).ThenBy(t=>t.Codigo).Select(t=>new {t.Codigo,t.Titulo,t.Equipo,t.Ubicacion,t.Prioridad,t.Estado,t.AsignadaA,t.FechaProgramada,t.FechaCreacion}).ToListAsync();
        var hoy=DateTime.Today;
        var rows=tareas.Select(t=>
        {
            var clasificacion=string.Equals(t.Estado,"Cerrada",StringComparison.OrdinalIgnoreCase)?"Completada":
                t.FechaProgramada.HasValue&&t.FechaProgramada.Value.Date<hoy?"Atrasada":"Pendiente";
            var row=new TaskReportRow(t.Codigo,t.Titulo,t.Equipo??string.Empty,t.Ubicacion??string.Empty,t.Prioridad??string.Empty,
                t.Estado??string.Empty,t.AsignadaA??string.Empty,t.FechaProgramada?.ToString("dd/MM/yyyy")??string.Empty,t.FechaCreacion.ToString("dd/MM/yyyy HH:mm"));
            return new ClassifiedTaskReportRow(clasificacion,row);
        }).ToList();
        return File(XlsxTaskReportFormatter.Create(rows),"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",$"tareas-mantenimiento-{DateTime.Today:yyyyMMdd}.xlsx");
    }


    public async Task<IActionResult> Details(int? id)
    {
        if (!id.HasValue) return BadRequest();
        var model = await _service.GetDetailsAsync(id.Value);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> AsumirAviso(int id)
    {
        var usuario = User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(usuario)) return Challenge();
        var asumida = await _service.ClaimAsync(id, usuario);
        TempData["LoginMessage"] = asumida ? "La tarea ya está asignada a tu usuario." : "La tarea ya está asignada, cerrada o no existe.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> CambiarEstadoRapido(int id, string estado)
    {
        var permitidos = new[] { "Nueva", "En estudio", "Asignada", "En curso", "Pendiente repuestos", "Cerrada" };
        if (!permitidos.Contains(estado, StringComparer.Ordinal)) return BadRequest("El estado indicado no es válido.");
        var tarea = await _service.GetGestionAsync(id);
        if (tarea is null) return NotFound();
        tarea.Estado = estado;
        if (!await _service.UpdateGestionAsync(tarea, User.Identity?.Name)) return NotFound();
        TempData["LoginMessage"] = estado == "Cerrada" ? "La tarea se ha cerrado." : "El estado de la tarea se ha actualizado.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> AdjuntarFotoRapida(int id, IFormFile? foto, string? observacion)
    {
        if (observacion?.Length > 500)
        {
            TempData["LoginMessage"] = "El pie de foto no puede superar los 500 caracteres.";
            return RedirectToAction(nameof(Details), new { id });
        }
        if (foto is null || foto.Length == 0)
        {
            TempData["LoginMessage"] = "Selecciona una foto antes de enviarla.";
            return RedirectToAction(nameof(Details), new { id });
        }
        if (!TareaFotoValidator.TipoPermitido(foto) || !TareaFotoValidator.TamanoPermitido(foto))
        {
            TempData["LoginMessage"] = "La foto debe ser una imagen de hasta 10 MB.";
            return RedirectToAction(nameof(Details), new { id });
        }
        if (await _service.GetDetailsAsync(id) is null) return NotFound();
        await SavePhotosAsync(id, new[] { foto }, new[] { observacion });
        TempData["LoginMessage"] = "La foto se ha añadido a la tarea.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Create()
    {
        return View(await _service.GetFormAsync(null));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> Create(TareaMantenimientoFormViewModel model)
    {
        ValidatePhotos(model.FotosAdjuntas, model.ObservacionesFotos);
        if (!ModelState.IsValid) return View(await _service.PrepareFormAsync(model));

        var id = await _service.CreateAsync(model, User.Identity?.Name);
        await SavePhotosAsync(id, model.FotosAdjuntas, model.ObservacionesFotos);
        TempData["LoginMessage"] = "La tarea de mantenimiento se ha creado correctamente.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.GestionMantenimiento)]
    public async Task<IActionResult> CrearPersona(PersonaForm model)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { error = "Indica nombre, primer apellido y NIF válidos." });

        var id = await _organizacion.SavePersonaAsync(model);
        if (!id.HasValue) return BadRequest(new { error = "No se pudo crear la persona." });
        var nombre = string.Join(" ", new[] { model.Nombre, model.Apellido1, model.Apellido2 }.Where(value => !string.IsNullOrWhiteSpace(value)));
        return Json(new { id, nombre });
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
        ValidatePhotos(model.FotosAdjuntas, model.ObservacionesFotos);
        if (!ModelState.IsValid) return View(await _service.PrepareFormAsync(model));
        if (!await _service.UpdateAsync(model, User.Identity?.Name)) return NotFound();
        await SavePhotosAsync(model.ID, model.FotosAdjuntas, model.ObservacionesFotos);
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

        var photoRoot = Path.GetFullPath(Path.Combine(_environment.ContentRootPath, "App_Data", "TareasMantenimientoFotos"));
        var relativePath = (foto.RutaArchivo ?? string.Empty).TrimStart('~', '/', '\\')
            .Replace('/', Path.DirectorySeparatorChar);
        var physicalPath = Path.GetFullPath(Path.Combine(_environment.ContentRootPath, relativePath));
        if (!physicalPath.StartsWith(photoRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            return NotFound();
        if (!System.IO.File.Exists(physicalPath)) return NotFound();

        return PhysicalFile(physicalPath,
            string.IsNullOrWhiteSpace(foto.ContentType) ? "application/octet-stream" : foto.ContentType);
    }

    private void ValidatePhotos(IEnumerable<IFormFile>? files, IEnumerable<string?>? captions = null)
    {
        if ((captions ?? Enumerable.Empty<string?>()).Any(caption => caption?.Length > 500))
            ModelState.AddModelError(nameof(TareaMantenimientoFormViewModel.ObservacionesFotos), "El pie de foto no puede superar los 500 caracteres.");
        foreach (var file in (files ?? Enumerable.Empty<IFormFile>()).Where(file => file.Length > 0))
        {
            if (!TareaFotoValidator.TipoPermitido(file))
                ModelState.AddModelError(nameof(TareaMantenimientoFormViewModel.FotosAdjuntas), "Solo se admiten archivos de imagen.");
            if (!TareaFotoValidator.TamanoPermitido(file))
                ModelState.AddModelError(nameof(TareaMantenimientoFormViewModel.FotosAdjuntas), "Cada imagen debe pesar como máximo 10 MB.");
        }
    }

    private async Task SavePhotosAsync(int tareaId, IEnumerable<IFormFile>? files, IReadOnlyList<string?>? observaciones = null)
    {
        var root = Path.GetFullPath(Path.Combine(_environment.ContentRootPath, "App_Data", "TareasMantenimientoFotos"));
        var folder = Path.Combine(root, tareaId.ToString());
        Directory.CreateDirectory(folder);
        var index = 0;
        foreach (var file in files ?? Enumerable.Empty<IFormFile>())
        {
            var observacionFoto = observaciones is not null && index < observaciones.Count ? observaciones[index] : null;
            index++;
            if (file.Length == 0) continue;
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var storedName = Guid.NewGuid().ToString("N") + extension;
            var fullPath = Path.Combine(folder, storedName);
            await using (var output = System.IO.File.Create(fullPath))
                await file.CopyToAsync(output);
            await _service.AddFotoAsync(tareaId, storedName, file.FileName, file.ContentType, observacionFoto, DateTime.Now);
        }
    }
}
