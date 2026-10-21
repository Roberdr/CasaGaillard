using CasaGaillard.Core.Data;
using CasaGaillard.Core.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CasaGaillard.Core.Areas.Mantenimiento.Controllers;

[Area("Mantenimiento"), Authorize(Roles = AppRoles.GestionMantenimiento)]
public sealed class MaterialesController : Controller
{
    private readonly AppDbContext _db;
    public MaterialesController(AppDbContext db) => _db = db;

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear([FromForm] string? nombre, CancellationToken cancellationToken)
    {
        var material = nombre?.Trim();
        if (string.IsNullOrWhiteSpace(material) || material.Length > 150)
            return BadRequest(new { mensaje = "Indica un nombre de hasta 150 caracteres." });

        var existentes = await _db.Database.SqlQueryRaw<MaterialCreado>(@"
            SELECT TOP (1) ID AS Id, Material AS Nombre
            FROM gaillard.Material
            WHERE LOWER(LTRIM(RTRIM(Material))) = LOWER(@p0)", material)
            .ToListAsync(cancellationToken);
        if (existentes.Count > 0) return Ok(existentes[0]);

        var creados = await _db.Database.SqlQueryRaw<MaterialCreado>(@"
            INSERT INTO gaillard.Material (Material) VALUES (@p0);
            SELECT CAST(SCOPE_IDENTITY() AS int) AS Id, @p0 AS Nombre", material)
            .ToListAsync(cancellationToken);
        return creados.Count == 0 ? StatusCode(500, new { mensaje = "No se pudo crear el material." }) : Ok(creados[0]);
    }

    private sealed class MaterialCreado
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}
