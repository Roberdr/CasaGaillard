using CasaGaillard.Core.Security;
using CasaGaillard.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CasaGaillard.Core.Data;
using CasaGaillard.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CasaGaillard.Core.Areas.Mantenimiento.Controllers;

[Area("Mantenimiento"), Authorize(Roles = AppRoles.LecturaMantenimiento)]
public sealed class HomeController : Controller
{
    private readonly IMantenimientoDashboardService _service;
    private readonly AppDbContext _db;
    public HomeController(IMantenimientoDashboardService service, AppDbContext db) { _service = service; _db = db; }
    public async Task<IActionResult> Index() => View(await _service.GetAsync());

    public async Task<IActionResult> Calendario(int? anio, int? mes)
    {
        var hoy = DateTime.Today;
        var inicio = new DateTime(anio ?? hoy.Year, Math.Clamp(mes ?? hoy.Month, 1, 12), 1);
        var inicioVista = inicio.AddDays(-(((int)inicio.DayOfWeek + 6) % 7));
        var finVista = inicioVista.AddDays(42);
        var tareas = await _db.TareasMantenimiento.AsNoTracking()
            .Where(t => t.FechaProgramada >= inicioVista && t.FechaProgramada < finVista)
            .OrderBy(t => t.FechaProgramada).ThenBy(t => t.Prioridad)
            .Select(t => new TareaMantenimientoResumenViewModel { ID=t.ID,Codigo=t.Codigo,Titulo=t.Titulo,Equipo=t.Equipo,
                Estado=t.Estado,Prioridad=t.Prioridad,FechaProgramada=t.FechaProgramada,AsignadaA=t.AsignadaA })
            .ToListAsync();
        var dias = Enumerable.Range(0, 42).Select(offset =>
        {
            var fecha = inicioVista.AddDays(offset);
            return new CalendarioDiaViewModel { Fecha=fecha, Tareas=tareas.Where(t => t.FechaProgramada?.Date == fecha).ToList() };
        }).ToList();
        return View(new CalendarioMantenimientoViewModel { Mes=inicio,Dias=dias });
    }
}
