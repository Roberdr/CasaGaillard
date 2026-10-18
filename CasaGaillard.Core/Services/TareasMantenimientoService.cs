using CasaGaillard.Core.Data;
using CasaGaillard.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CasaGaillard.Core.Services;

public sealed class TareasMantenimientoService : ITareasMantenimientoService
{
    private readonly AppDbContext _db;

    public TareasMantenimientoService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<TareaMantenimientoResumenViewModel>> GetAllAsync(string? codigo)
    {
        var query = _db.TareasMantenimiento.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(codigo))
            query = query.Where(tarea => tarea.Codigo == codigo.Trim());

        return await query.OrderByDescending(tarea => tarea.FechaCreacion)
            .Select(tarea => new TareaMantenimientoResumenViewModel
            {
                ID = tarea.ID,
                Codigo = tarea.Codigo,
                Titulo = tarea.Titulo,
                Ubicacion = tarea.Ubicacion,
                Equipo = tarea.Equipo,
                Prioridad = tarea.Prioridad,
                Estado = tarea.Estado,
                AsignadaA = tarea.AsignadaA,
                FechaCreacion = tarea.FechaCreacion
            }).ToListAsync();
    }

    public async Task<TareaMantenimientoDetailsViewModel?> GetDetailsAsync(int id)
    {
        var tarea = await _db.TareasMantenimiento.AsNoTracking().SingleOrDefaultAsync(t => t.ID == id);
        if (tarea is null) return null;

        var fotos = await _db.TareaMantenimientoFotos.AsNoTracking()
            .Where(foto => foto.TareaMantenimientoID == id)
            .OrderByDescending(foto => foto.FechaCreacion)
            .ToListAsync();

        var accessoryIds = await _db.TareaMantenimientoAccesorios.AsNoTracking()
            .Where(item => item.TareaMantenimientoID == id).OrderBy(item => item.ID)
            .Select(item => item.AccesorioID).ToArrayAsync();
        if (accessoryIds.Length == 0)
            accessoryIds = ParseAccessoryIds(tarea.AccesoriosNecesarios);
        var accessoriesText = "Sin accesorios indicados.";
        if (accessoryIds.Length > 0)
        {
            var accessories = await _db.Database.SqlQuery<AccessoryName>($@"
                SELECT a.ID AS Id,
                    COALESCE(NULLIF(a.Nombre, N''), NULLIF(t.TipoAccesorio, N''), N'Accesorio') AS Nombre
                FROM gaillard.Accesorio a
                LEFT JOIN gaillard.TipoAccesorio t ON t.ID=a.TipoAccesorioID")
                .Where(item => accessoryIds.Contains(item.Id)).OrderBy(item => item.Nombre).ToListAsync();
            if (accessories.Count > 0) accessoriesText = string.Join(", ", accessories.Select(item => item.Nombre));
        }

        return new TareaMantenimientoDetailsViewModel
        {
            Tarea = tarea,
            Fotos = fotos,
            AccesoriosTexto = accessoriesText
        };
    }

    public Task<TareaMantenimientoFoto?> GetFotoAsync(int id) => _db.TareaMantenimientoFotos
        .AsNoTracking().SingleOrDefaultAsync(foto => foto.ID == id);

    public async Task<TareaMantenimientoFormViewModel?> GetFormAsync(int? id)
    {
        if (!id.HasValue)
            return await PopulateLookupsAsync(new TareaMantenimientoFormViewModel { FechaDeteccion = DateTime.Now, Prioridad = "Media", Estado = "Nueva" });
        var tarea = await _db.TareasMantenimiento.AsNoTracking().SingleOrDefaultAsync(t => t.ID == id.Value);
        return tarea is null ? null : await PopulateLookupsAsync(new TareaMantenimientoFormViewModel
        {
            ID = tarea.ID,
            Codigo = tarea.Codigo,
            Titulo = tarea.Titulo,
            Descripcion = tarea.Descripcion,
            Ubicacion = tarea.Ubicacion,
            Equipo = tarea.Equipo,
            InstalacionId = tarea.InstalacionId, EquipoId = tarea.EquipoId, CubaId = tarea.CubaId,
            GrupoId = tarea.GrupoId, VehiculoId = tarea.VehiculoId, PlanMantenimientoId = tarea.PlanMantenimientoId,
            DetectadoPorPersonaID = tarea.DetectadoPorPersonaID, ContactoPersonaID = tarea.ContactoPersonaID,
            AsignadaAPersonaID = tarea.AsignadaAPersonaID, EmpresaExteriorEntidadID = tarea.EmpresaExteriorEntidadID,
            DetectadoPor = tarea.DetectadoPor,
            Contacto = tarea.Contacto,
            Prioridad = tarea.Prioridad,
            Estado = tarea.Estado,
            AsignadaA = tarea.AsignadaA,
            EmpresaExterior = tarea.EmpresaExterior,
            AccionesARealizar = tarea.AccionesARealizar,
            AccesoriosNecesarios = tarea.AccesoriosNecesarios,
            Observaciones = tarea.Observaciones,
            FechaDeteccion = tarea.FechaDeteccion,
            FechaCreacion = tarea.FechaCreacion,
            FechaActualizacion = tarea.FechaActualizacion,
            CreadoPor = tarea.CreadoPor,
            ActualizadoPor = tarea.ActualizadoPor
        });
    }

    public async Task<int> CreateAsync(TareaMantenimientoFormViewModel model, string? userName)
    {
        await PopulateLookupsAsync(model);
        SyncLegacyLabels(model);
        var now = DateTime.Now;
        var tarea = new TareaMantenimiento
        {
            Codigo = "TM-" + now.ToString("yyyyMMdd-HHmmssfff"),
            Titulo = model.Titulo.Trim(),
            Descripcion = model.Descripcion.Trim(),
            Ubicacion = NormalizeText(model.Ubicacion),
            Equipo = NormalizeText(model.Equipo),
            InstalacionId = model.InstalacionId, EquipoId = model.EquipoId, CubaId = model.CubaId,
            GrupoId = model.GrupoId, VehiculoId = model.VehiculoId, PlanMantenimientoId = model.PlanMantenimientoId,
            DetectadoPorPersonaID = model.DetectadoPorPersonaID, ContactoPersonaID = model.ContactoPersonaID,
            AsignadaAPersonaID = model.AsignadaAPersonaID, EmpresaExteriorEntidadID = model.EmpresaExteriorEntidadID,
            DetectadoPor = NormalizeText(model.DetectadoPor),
            Contacto = NormalizeText(model.Contacto),
            Prioridad = string.IsNullOrWhiteSpace(model.Prioridad) ? "Media" : model.Prioridad.Trim(),
            Estado = string.IsNullOrWhiteSpace(model.Estado) ? "Nueva" : model.Estado.Trim(),
            AsignadaA = NormalizeText(model.AsignadaA),
            EmpresaExterior = NormalizeText(model.EmpresaExterior),
            AccionesARealizar = NormalizeText(model.AccionesARealizar),
            AccesoriosNecesarios = NormalizeAccessoryIds(model.AccesoriosNecesarios),
            Observaciones = NormalizeText(model.Observaciones),
            CreadoPor = NormalizeText(userName),
            FechaDeteccion = model.FechaDeteccion == default ? now : model.FechaDeteccion,
            FechaCreacion = now
        };
        _db.TareasMantenimiento.Add(tarea);
        await _db.SaveChangesAsync();
        await SaveAccessoryLinksAsync(tarea.ID, model.AccesoriosNecesarios);
        return tarea.ID;
    }

    public Task<TareaMantenimientoFormViewModel> PrepareFormAsync(TareaMantenimientoFormViewModel model) => PopulateLookupsAsync(model);

    public async Task<bool> UpdateAsync(TareaMantenimientoFormViewModel model, string? userName)
    {
        await PopulateLookupsAsync(model);
        SyncLegacyLabels(model);
        var tarea = await _db.TareasMantenimiento.SingleOrDefaultAsync(t => t.ID == model.ID);
        if (tarea is null) return false;
        tarea.Titulo = model.Titulo.Trim();
        tarea.Descripcion = model.Descripcion.Trim();
        tarea.Ubicacion = NormalizeText(model.Ubicacion);
        tarea.Equipo = NormalizeText(model.Equipo);
        tarea.InstalacionId = model.InstalacionId; tarea.EquipoId = model.EquipoId; tarea.CubaId = model.CubaId;
        tarea.GrupoId = model.GrupoId; tarea.VehiculoId = model.VehiculoId; tarea.PlanMantenimientoId = model.PlanMantenimientoId;
        tarea.DetectadoPorPersonaID = model.DetectadoPorPersonaID; tarea.ContactoPersonaID = model.ContactoPersonaID;
        tarea.AsignadaAPersonaID = model.AsignadaAPersonaID; tarea.EmpresaExteriorEntidadID = model.EmpresaExteriorEntidadID;
        tarea.DetectadoPor = NormalizeText(model.DetectadoPor);
        tarea.Contacto = NormalizeText(model.Contacto);
        tarea.Prioridad = string.IsNullOrWhiteSpace(model.Prioridad) ? "Media" : model.Prioridad.Trim();
        tarea.Estado = string.IsNullOrWhiteSpace(model.Estado) ? "Nueva" : model.Estado.Trim();
        tarea.AsignadaA = NormalizeText(model.AsignadaA);
        tarea.EmpresaExterior = NormalizeText(model.EmpresaExterior);
        tarea.AccionesARealizar = NormalizeText(model.AccionesARealizar);
        tarea.AccesoriosNecesarios = NormalizeAccessoryIds(model.AccesoriosNecesarios);
        tarea.Observaciones = NormalizeText(model.Observaciones);
        tarea.FechaDeteccion = model.FechaDeteccion;
        tarea.ActualizadoPor = NormalizeText(userName);
        tarea.FechaActualizacion = DateTime.Now;
        await _db.SaveChangesAsync();
        await SaveAccessoryLinksAsync(tarea.ID, model.AccesoriosNecesarios);
        return true;
    }

    public async Task<TareaMantenimientoGestionViewModel?> GetGestionAsync(int id)
    {
        var tarea = await _db.TareasMantenimiento.AsNoTracking().SingleOrDefaultAsync(t => t.ID == id);
        return tarea is null ? null : await PopulateLookupsAsync(new TareaMantenimientoGestionViewModel
        {
            ID = tarea.ID,
            Codigo = tarea.Codigo,
            Titulo = tarea.Titulo,
            Descripcion = tarea.Descripcion,
            Ubicacion = tarea.Ubicacion,
            Equipo = tarea.Equipo,
            AsignadaAPersonaID = tarea.AsignadaAPersonaID,
            EmpresaExteriorEntidadID = tarea.EmpresaExteriorEntidadID,
            Prioridad = tarea.Prioridad,
            Estado = tarea.Estado,
            AsignadaA = tarea.AsignadaA,
            EmpresaExterior = tarea.EmpresaExterior,
            AccionesARealizar = tarea.AccionesARealizar,
            AccesoriosNecesarios = tarea.AccesoriosNecesarios,
            Observaciones = tarea.Observaciones
        });
    }

    public async Task<bool> UpdateGestionAsync(TareaMantenimientoGestionViewModel model, string? userName)
    {
        await PopulateLookupsAsync(model);
        model.AsignadaA = SelectedText(model.Personas, model.AsignadaAPersonaID) ?? model.AsignadaA;
        model.EmpresaExterior = SelectedText(model.Entidades, model.EmpresaExteriorEntidadID) ?? model.EmpresaExterior;
        var tarea = await _db.TareasMantenimiento.SingleOrDefaultAsync(t => t.ID == model.ID);
        if (tarea is null) return false;
        tarea.Prioridad = NormalizeText(model.Prioridad);
        tarea.Estado = NormalizeText(model.Estado);
        tarea.AsignadaA = NormalizeText(model.AsignadaA);
        tarea.EmpresaExterior = NormalizeText(model.EmpresaExterior);
        tarea.AsignadaAPersonaID = model.AsignadaAPersonaID;
        tarea.EmpresaExteriorEntidadID = model.EmpresaExteriorEntidadID;
        tarea.AccionesARealizar = NormalizeText(model.AccionesARealizar);
        tarea.AccesoriosNecesarios = NormalizeAccessoryIds(model.AccesoriosNecesarios);
        tarea.Observaciones = NormalizeText(model.Observaciones);
        tarea.ActualizadoPor = NormalizeText(userName);
        tarea.FechaActualizacion = DateTime.Now;
        await _db.SaveChangesAsync();
        return true;
    }

    public Task<TareaMantenimientoGestionViewModel> PrepareGestionAsync(TareaMantenimientoGestionViewModel model) => PopulateLookupsAsync(model);

    public async Task AddFotoAsync(int tareaId, string fileName, string originalName, string contentType, DateTime uploadedAt)
    {
        _db.TareaMantenimientoFotos.Add(new TareaMantenimientoFoto
        {
            TareaMantenimientoID = tareaId,
            RutaArchivo = "~/App_Data/TareasMantenimientoFotos/" + tareaId + "/" + fileName,
            NombreOriginal = Path.GetFileName(originalName),
            ContentType = contentType,
            FechaCreacion = uploadedAt
        });
        await _db.SaveChangesAsync();
    }

    private static string? NormalizeText(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private async Task<TareaMantenimientoFormViewModel> PopulateLookupsAsync(TareaMantenimientoFormViewModel model)
    {
        model.Personas = await GetOptionsAsync("SELECT ID AS Id, LTRIM(RTRIM(CONCAT(COALESCE(NombrePersona,N''), N' ', COALESCE(Apellido1,N''), N' ', COALESCE(Apellido2,N'')))) AS Nombre FROM gaillard.Persona ORDER BY Nombre", model.DetectadoPorPersonaID);
        model.Entidades = await GetOptionsAsync("SELECT ID AS Id, NombreEntidad AS Nombre FROM gaillard.Entidad ORDER BY NombreEntidad", model.EmpresaExteriorEntidadID);
        model.Instalaciones = await GetOptionsAsync("SELECT Id, CONCAT(Codigo, N' - ', Nombre) AS Nombre FROM gaillard.Instalacion WHERE Estado <> N'FUERA_SERVICIO' ORDER BY Nombre", model.InstalacionId);
        model.Equipos = await GetOptionsAsync("SELECT e.Id, CONCAT(i.Nombre, N' - ', e.Nombre) AS Nombre FROM gaillard.Equipo e JOIN gaillard.Instalacion i ON i.Id=e.InstalacionId WHERE e.Activo=1 AND i.Estado <> N'FUERA_SERVICIO' ORDER BY Nombre", model.EquipoId);
        model.Cubas = await GetOptionsAsync("SELECT ID AS Id, COALESCE(NULLIF(MatriculaCuba,N''),Codigo) AS Nombre FROM gaillard.Cuba WHERE ISNULL(Baja,0)=0 ORDER BY Nombre", model.CubaId);
        model.Grupos = await GetOptionsAsync("SELECT g.ID AS Id, CONCAT(COALESCE(NULLIF(c.MatriculaCuba,N''),c.Codigo), N' / ', COALESCE(tg.NombreGrupo,N'Grupo')) AS Nombre FROM gaillard.Grupo g JOIN gaillard.Cuba c ON c.ID=g.CubaID LEFT JOIN gaillard.TipoGrupo tg ON tg.ID=g.TipoGrupoID WHERE ISNULL(c.Baja,0)=0 ORDER BY Nombre", model.GrupoId);
        model.Vehiculos = await GetOptionsAsync("SELECT ID AS Id, CONCAT(MatriculaVehiculo, N' - ', Marca, N' ', Modelo) AS Nombre FROM gaillard.Vehiculo WHERE ISNULL(Baja,0)=0 ORDER BY MatriculaVehiculo", model.VehiculoId);
        model.PlanesMantenimiento = await GetOptionsAsync("SELECT PlanId AS Id, COALESCE(NULLIF(Nombre,N''), CONCAT(N'Plan ',PlanId)) AS Nombre FROM gaillard.PlanMantenimiento WHERE Activo=1 ORDER BY Nombre", model.PlanMantenimientoId);
        model.Contactos = model.Personas;
        model.Asignados = model.Personas;
        return model;
    }

    private async Task<TareaMantenimientoGestionViewModel> PopulateLookupsAsync(TareaMantenimientoGestionViewModel model)
    {
        model.Personas = await GetOptionsAsync("SELECT ID AS Id, LTRIM(RTRIM(CONCAT(COALESCE(NombrePersona,N''), N' ', COALESCE(Apellido1,N''), N' ', COALESCE(Apellido2,N'')))) AS Nombre FROM gaillard.Persona ORDER BY Nombre", model.AsignadaAPersonaID);
        model.Entidades = await GetOptionsAsync("SELECT ID AS Id, NombreEntidad AS Nombre FROM gaillard.Entidad ORDER BY NombreEntidad", model.EmpresaExteriorEntidadID);
        return model;
    }

    private async Task<IReadOnlyList<SelectListItem>> GetOptionsAsync(string sql, int? selected) =>
        (await _db.Database.SqlQueryRaw<LookupOption>(sql).ToListAsync())
        .Select(option => new SelectListItem(option.Nombre, option.Id.ToString(), option.Id == selected)).ToList();

    private static void SyncLegacyLabels(TareaMantenimientoFormViewModel model)
    {
        model.DetectadoPor = SelectedText(model.Personas, model.DetectadoPorPersonaID) ?? model.DetectadoPor;
        model.Contacto = SelectedText(model.Contactos, model.ContactoPersonaID) ?? model.Contacto;
        model.AsignadaA = SelectedText(model.Asignados, model.AsignadaAPersonaID) ?? model.AsignadaA;
        model.EmpresaExterior = SelectedText(model.Entidades, model.EmpresaExteriorEntidadID) ?? model.EmpresaExterior;
        model.Ubicacion = SelectedText(model.Instalaciones, model.InstalacionId) ?? SelectedText(model.Cubas, model.CubaId) ?? model.Ubicacion;
        model.Equipo = SelectedText(model.Equipos, model.EquipoId) ?? SelectedText(model.Grupos, model.GrupoId) ?? SelectedText(model.Vehiculos, model.VehiculoId) ?? model.Equipo;
    }

    private static string? SelectedText(IReadOnlyList<SelectListItem> options, int? selectedId) => selectedId.HasValue
        ? options.FirstOrDefault(option => option.Value == selectedId.Value.ToString())?.Text
        : null;

    private static string? NormalizeAccessoryIds(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var normalized = string.Join(",", ParseAccessoryIds(value));
        return normalized.Length == 0 ? null : normalized;
    }

    private async Task SaveAccessoryLinksAsync(int tareaId, string? accessoryValues)
    {
        var existing = await _db.TareaMantenimientoAccesorios
            .Where(item => item.TareaMantenimientoID == tareaId).ToListAsync();
        _db.TareaMantenimientoAccesorios.RemoveRange(existing);
        foreach (var accessoryId in ParseAccessoryIds(accessoryValues))
        {
            _db.TareaMantenimientoAccesorios.Add(new TareaMantenimientoAccesorio
            {
                TareaMantenimientoID = tareaId,
                AccesorioID = accessoryId,
                Cantidad = 1
            });
        }
        await _db.SaveChangesAsync();
    }

    private static int[] ParseAccessoryIds(string? value) => (value ?? string.Empty)
        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Select(item => int.TryParse(item, out var id) ? (int?)id : null)
        .Where(id => id.HasValue && id.Value > 0).Select(id => id!.Value).Distinct().ToArray();

    private sealed class AccessoryName
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
    private sealed class LookupOption { public int Id { get; set; } public string Nombre { get; set; } = string.Empty; }
}
