using CasaGaillard.Core.Data;
using CasaGaillard.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CasaGaillard.Core.Services;

public sealed class IntervencionesService : IIntervencionesService
{
    private readonly AppDbContext _db;

    public IntervencionesService(AppDbContext db) => _db = db;

    public async Task<IntervencionesIndexViewModel?> GetIndexAsync(int? tareaId)
    {
        var task = tareaId.HasValue ? await GetTareaAsync(tareaId.Value) : null;
        if (tareaId.HasValue && task is null) return null;

        var rows = await _db.Database.SqlQuery<IntervencionFilaViewModel>($@"
            SELECT i.Id, i.TareaMantenimientoId, t.Codigo AS TareaCodigo, t.Titulo AS TareaTitulo,
                COALESCE(NULLIF(i.Tecnico, N''), LTRIM(RTRIM(p.NombrePersona + N' ' + p.Apellido1 + N' ' + ISNULL(p.Apellido2,N'')))) AS Tecnico,
                i.FechaInicio, i.FechaFin, i.Descripcion, i.HorasEmpleadas, i.CosteManoObra
            FROM dbo.Intervencion i
            INNER JOIN dbo.TareasMantenimiento t ON t.ID=i.TareaMantenimientoId
            LEFT JOIN gaillard.Persona p ON p.ID=i.TecnicoPersonaID
            WHERE ({tareaId} IS NULL OR i.TareaMantenimientoId={tareaId})
            ORDER BY i.FechaInicio DESC").ToListAsync();

        return new IntervencionesIndexViewModel { Tarea = task, Intervenciones = rows };
    }

    public Task<TareaMantenimientoResumen?> GetTareaSummaryAsync(int id) => GetTareaAsync(id);

    public async Task<IntervencionFormViewModel?> GetCreateAsync(int tareaId)
    {
        var task = await GetTareaAsync(tareaId);
        if (task is null) return null;
        return new IntervencionFormViewModel
        {
            TareaMantenimientoId = task.Id,
            TareaCodigo = task.Codigo,
            TareaTitulo = task.Titulo,
            FechaInicio = DateTime.Now,
            Personas = await GetPersonasAsync(null)
        };
    }

    public async Task<IntervencionFormViewModel?> GetForEditAsync(int id)
    {
        var models = await _db.Database.SqlQuery<IntervencionFormViewModel>($@"
            SELECT i.Id, i.TareaMantenimientoId, i.TecnicoPersonaID, i.FechaInicio, i.FechaFin,
                i.Descripcion, i.HorasEmpleadas, i.CosteManoObra,
                t.Codigo AS TareaCodigo, t.Titulo AS TareaTitulo
            FROM dbo.Intervencion i
            INNER JOIN dbo.TareasMantenimiento t ON t.ID=i.TareaMantenimientoId
            WHERE i.Id={id}").ToListAsync();
        var model = models.SingleOrDefault();
        if (model is not null) model.Personas = await GetPersonasAsync(model.TecnicoPersonaID);
        return model;
    }

    public async Task<IntervencionDetailsViewModel?> GetDetailsAsync(int id, int? nuevoAccesorioId = null)
    {
        var rows = await _db.Database.SqlQuery<IntervencionFilaViewModel>($@"
            SELECT i.Id, i.TareaMantenimientoId, t.Codigo AS TareaCodigo, t.Titulo AS TareaTitulo,
                COALESCE(NULLIF(i.Tecnico, N''), LTRIM(RTRIM(p.NombrePersona + N' ' + p.Apellido1 + N' ' + ISNULL(p.Apellido2,N'')))) AS Tecnico,
                i.FechaInicio, i.FechaFin, i.Descripcion, i.HorasEmpleadas, i.CosteManoObra
            FROM dbo.Intervencion i
            INNER JOIN dbo.TareasMantenimiento t ON t.ID=i.TareaMantenimientoId
            LEFT JOIN gaillard.Persona p ON p.ID=i.TecnicoPersonaID
            WHERE i.Id={id}").ToListAsync();
        var row = rows.SingleOrDefault();
        if (row is null) return null;

        var parts = await _db.Database.SqlQuery<IntervencionRepuestoViewModel>($@"
            SELECT r.Id, r.IntervencionId, r.AccesorioId, a.Nombre AS AccesorioNombre,
                r.Cantidad, r.CosteUnitario, r.Observaciones
            FROM dbo.IntervencionRepuesto r
            INNER JOIN gaillard.Accesorio a ON a.ID=r.AccesorioId
            WHERE r.IntervencionId={id}
            ORDER BY a.Nombre").ToListAsync();

        return new IntervencionDetailsViewModel
        {
            Id = row.Id,
            TareaMantenimientoId = row.TareaMantenimientoId,
            TareaCodigo = row.TareaCodigo,
            TareaTitulo = row.TareaTitulo,
            Tecnico = row.Tecnico,
            FechaInicio = row.FechaInicio,
            FechaFin = row.FechaFin,
            Descripcion = row.Descripcion,
            HorasEmpleadas = row.HorasEmpleadas,
            CosteManoObra = row.CosteManoObra,
            Repuestos = parts,
            RepuestoForm = new IntervencionRepuestoFormViewModel
            {
                IntervencionId = id,
                AccesorioId = nuevoAccesorioId ?? 0,
                Accesorios = await GetAccesoriosAsync(nuevoAccesorioId ?? 0)
            }
        };
    }

    public async Task<IReadOnlyList<SelectListItem>> GetPersonasAsync(int? selectedId)
    {
        var people = await _db.Database.SqlQuery<PersonaOption>($@"
            SELECT ID, LTRIM(RTRIM(NombrePersona + N' ' + Apellido1 + N' ' + ISNULL(Apellido2,N''))) AS Nombre
            FROM gaillard.Persona
            ORDER BY NombrePersona, Apellido1, Apellido2").ToListAsync();
        return people.Select(p => new SelectListItem
        {
            Value = p.Id.ToString(), Text = p.Nombre, Selected = p.Id == selectedId
        }).ToList();
    }

    public async Task<IReadOnlyList<SelectListItem>> GetAccesoriosAsync(int selectedId)
    {
        var accessories = await _db.Database.SqlQuery<AccesorioOption>($@"
            SELECT a.ID, COALESCE(NULLIF(a.Nombre, N''), NULLIF(t.TipoAccesorio, N''),
                N'Accesorio ' + CONVERT(nvarchar(20), a.ID)) +
                CASE WHEN a.Activo=1 THEN N'' ELSE N' (inactivo)' END AS Texto
            FROM gaillard.Accesorio a
            LEFT JOIN gaillard.TipoAccesorio t ON t.ID=a.TipoAccesorioID
            ORDER BY Texto, a.ID").ToListAsync();
        return accessories.Select(a => new SelectListItem
        {
            Value = a.Id.ToString(), Text = a.Texto, Selected = a.Id == selectedId
        }).ToList();
    }

    public async Task<bool> SaveCreateAsync(IntervencionFormViewModel model, string? creadoPor)
    {
        if (await GetTareaAsync(model.TareaMantenimientoId) is null) return false;
        var technician = await GetTechnicianNameAsync(model.TecnicoPersonaID);
        if (technician is null) return false;
        var affected = await _db.Database.ExecuteSqlInterpolatedAsync($@"
            INSERT INTO dbo.Intervencion
                (TareaMantenimientoId, Tecnico, TecnicoPersonaID, FechaInicio, FechaFin, Descripcion,
                 HorasEmpleadas, CosteManoObra, CreadoPor)
            VALUES ({model.TareaMantenimientoId}, {technician}, {model.TecnicoPersonaID},
                {model.FechaInicio}, {model.FechaFin}, {model.Descripcion.Trim()},
                {model.HorasEmpleadas}, {model.CosteManoObra}, {creadoPor})");
        return affected > 0;
    }

    public async Task<bool> SaveUpdateAsync(IntervencionFormViewModel model)
    {
        if (await GetTareaAsync(model.TareaMantenimientoId) is null) return false;
        var technician = await GetTechnicianNameAsync(model.TecnicoPersonaID);
        if (technician is null) return false;
        var affected = await _db.Database.ExecuteSqlInterpolatedAsync($@"
            UPDATE dbo.Intervencion SET
                Tecnico={technician}, TecnicoPersonaID={model.TecnicoPersonaID},
                FechaInicio={model.FechaInicio}, FechaFin={model.FechaFin},
                Descripcion={model.Descripcion.Trim()}, HorasEmpleadas={model.HorasEmpleadas},
                CosteManoObra={model.CosteManoObra}
            WHERE Id={model.Id}");
        return affected > 0;
    }

    public async Task<bool> AddRepuestoAsync(IntervencionRepuestoFormViewModel model)
    {
        var exists = await _db.Database.SqlQuery<CountRow>($@"SELECT COUNT(1) AS Value FROM dbo.Intervencion WHERE Id={model.IntervencionId}").SingleAsync();
        if (exists.Value == 0) return false;
        var accessory = await _db.Database.SqlQuery<CountRow>($@"SELECT COUNT(1) AS Value FROM gaillard.Accesorio WHERE ID={model.AccesorioId}").SingleAsync();
        if (accessory.Value == 0) return false;

        await _db.Database.ExecuteSqlInterpolatedAsync($@"
            INSERT INTO dbo.IntervencionRepuesto
                (IntervencionId, AccesorioId, Cantidad, CosteUnitario, Observaciones)
            VALUES ({model.IntervencionId}, {model.AccesorioId}, {model.Cantidad},
                {model.CosteUnitario}, {NormalizeText(model.Observaciones)})");
        return true;
    }

    public async Task<int?> RemoveRepuestoAsync(int id)
    {
        var rows = await _db.Database.SqlQuery<IntervencionRepuestoId>($@"SELECT IntervencionId FROM dbo.IntervencionRepuesto WHERE Id={id}").ToListAsync();
        var taskId = rows.SingleOrDefault()?.IntervencionId;
        if (!taskId.HasValue) return null;
        await _db.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM dbo.IntervencionRepuesto WHERE Id={id}");
        return taskId;
    }

    private async Task<TareaMantenimientoResumen?> GetTareaAsync(int id)
    {
        var rows = await _db.Database.SqlQuery<TareaMantenimientoResumen>($@"SELECT ID AS Id, Codigo, Titulo FROM dbo.TareasMantenimiento WHERE ID={id}").ToListAsync();
        return rows.SingleOrDefault();
    }

    private async Task<string?> GetTechnicianNameAsync(int? personaId)
    {
        if (!personaId.HasValue) return null;
        var people = await _db.Database.SqlQuery<TechnicianName>($@"
            SELECT LTRIM(RTRIM(NombrePersona + N' ' + Apellido1 + N' ' + ISNULL(Apellido2,N''))) AS Nombre
            FROM gaillard.Persona WHERE ID={personaId.Value}").ToListAsync();
        var name = people.SingleOrDefault()?.Nombre;
        return string.IsNullOrWhiteSpace(name) ? null : name;
    }

    private static string? NormalizeText(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private sealed class PersonaOption { public int Id { get; set; } public string Nombre { get; set; } = string.Empty; }
    private sealed class AccesorioOption { public int Id { get; set; } public string Texto { get; set; } = string.Empty; }
    private sealed class TechnicianName { public string Nombre { get; set; } = string.Empty; }
    private sealed class CountRow { public int Value { get; set; } }
    private sealed class IntervencionRepuestoId { public int IntervencionId { get; set; } }
}
