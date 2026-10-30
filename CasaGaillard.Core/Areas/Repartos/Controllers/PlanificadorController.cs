using CasaGaillard.Core.Data;
using CasaGaillard.Core.Models;
using CasaGaillard.Core.Security;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CasaGaillard.Core.Areas.Repartos.Controllers;

[Area("Repartos"), Authorize(Roles = AppRoles.GestionMantenimiento)]
public sealed class PlanificadorController : Controller
{
    private readonly AppDbContext _db;
    private readonly IAntiforgery _antiforgery;
    private readonly IWebHostEnvironment _environment;

    public PlanificadorController(AppDbContext db, IAntiforgery antiforgery, IWebHostEnvironment environment)
    {
        _db = db;
        _antiforgery = antiforgery;
        _environment = environment;
    }

    [HttpGet]
    public IActionResult Index() => PhysicalFile(
        Path.Combine(_environment.WebRootPath, "repartos", "index.html"), "text/html; charset=utf-8");

    [HttpGet]
    public IActionResult Token()
    {
        var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
        return Json(new { requestToken = tokens.RequestToken });
    }

    [HttpGet]
    public async Task<IActionResult> Estado(CancellationToken cancellationToken)
    {
        var rows = await _db.PlanificadorEstados.AsNoTracking()
            .ToDictionaryAsync(x => x.Clave, x => x.Json, cancellationToken);
        return Json(rows);
    }

    [HttpPut]
    [RequestSizeLimit(5_000_000)]
    public async Task<IActionResult> Estado([FromBody] EstadoRequest? request, CancellationToken cancellationToken)
    {
        try { await _antiforgery.ValidateRequestAsync(HttpContext); }
        catch (AntiforgeryValidationException) { return BadRequest(); }
        if (request is null || string.IsNullOrWhiteSpace(request.Clave) || request.Clave.Length > 120 || request.Json is null)
            return BadRequest();

        var row = await _db.PlanificadorEstados.FindAsync([request.Clave], cancellationToken);
        if (row is null)
        {
            row = new PlanificadorEstado { Clave = request.Clave };
            _db.PlanificadorEstados.Add(row);
        }
        row.Json = request.Json;
        row.ActualizadoUtc = DateTime.UtcNow;
        row.ActualizadoPor = User.Identity?.Name;
        await _db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    public sealed record EstadoRequest(string Clave, string Json);
}
