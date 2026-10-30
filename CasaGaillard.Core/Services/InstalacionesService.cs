using CasaGaillard.Core.Data;
using CasaGaillard.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CasaGaillard.Core.Services;

public sealed class InstalacionesService : IInstalacionesService
{
    private readonly AppDbContext _db;
    public InstalacionesService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<InstalacionFilaViewModel>> GetAllAsync(bool incluirBajas) =>
        await _db.Database.SqlQuery<InstalacionFilaViewModel>($@"
            SELECT i.Id, i.Codigo, i.Nombre, t.Nombre AS TipoNombre, u.Nombre AS UbicacionNombre,
                i.Estado, i.Criticidad,
                (SELECT COUNT(1) FROM gaillard.Equipo e WHERE e.InstalacionId=i.Id) AS NumeroEquipos
            FROM gaillard.Instalacion i
            LEFT JOIN gaillard.TipoInstalacion t ON t.Id=i.TipoInstalacionId
            LEFT JOIN gaillard.Ubicacion u ON u.UbicacionId=i.UbicacionId
            WHERE ({incluirBajas}=1 OR i.Estado<>N'FUERA_SERVICIO')
            ORDER BY i.Nombre").ToListAsync();

    public async Task<InstalacionFormViewModel?> GetFormAsync(int? id)
    {
        var model = new InstalacionFormViewModel();
        if (id.HasValue)
        {
            var rows = await _db.Database.SqlQuery<FormData>($@"
                SELECT Id, Codigo, Nombre, Descripcion, TipoInstalacionId, UbicacionId, ResponsablePersonaId,
                    FechaPuestaMarcha, FechaBaja, Criticidad, Estado, CodigoPlano, CriticaProduccion,
                    CriticaSeguridad, Observaciones
                FROM gaillard.Instalacion WHERE Id={id.Value}").ToListAsync();
            var row = rows.SingleOrDefault();
            if (row is null) return null;
            model = new InstalacionFormViewModel
            {
                Id=row.Id, Codigo=row.Codigo, Nombre=row.Nombre, Descripcion=row.Descripcion,
                TipoInstalacionId=row.TipoInstalacionId, UbicacionId=row.UbicacionId, ResponsablePersonaId=row.ResponsablePersonaId,
                FechaPuestaMarcha=row.FechaPuestaMarcha, FechaBaja=row.FechaBaja, Criticidad=row.Criticidad,
                Estado=row.Estado, CodigoPlano=row.CodigoPlano, CriticaProduccion=row.CriticaProduccion,
                CriticaSeguridad=row.CriticaSeguridad, Observaciones=row.Observaciones
            };
        }
        return await PrepareFormAsync(model);
    }

    public async Task<InstalacionFormViewModel> PrepareFormAsync(InstalacionFormViewModel model)
    {
        model.Tipos = await GetOptionsAsync("SELECT Id AS Id, Nombre AS Nombre FROM gaillard.TipoInstalacion ORDER BY Nombre", model.TipoInstalacionId);
        model.Ubicaciones = await GetOptionsAsync("SELECT UbicacionId AS Id, CONCAT(UbicacionId,N' - ',Nombre) AS Nombre FROM gaillard.Ubicacion ORDER BY Nombre", model.UbicacionId);
        model.Responsables = await GetOptionsAsync("SELECT ID AS Id,LTRIM(RTRIM(COALESCE(NombrePersona,N'')+N' '+COALESCE(Apellido1,N'')+N' '+COALESCE(Apellido2,N''))) AS Nombre FROM gaillard.Persona ORDER BY NombrePersona,Apellido1,Apellido2", model.ResponsablePersonaId);
        return model;
    }

    public async Task<IReadOnlyDictionary<string, string>> ValidateAsync(InstalacionFormViewModel model)
    {
        var errors = new Dictionary<string, string>();
        if (!new[] { "ACTIVA", "PARADA", "MANTENIMIENTO", "FUERA_SERVICIO" }.Contains(model.Estado ?? string.Empty))
            errors["Estado"] = "Selecciona un estado válido.";
        if (model.FechaPuestaMarcha.HasValue && model.FechaBaja.HasValue && model.FechaBaja < model.FechaPuestaMarcha)
            errors["FechaBaja"] = "La fecha de baja no puede ser anterior a la puesta en marcha.";
        var codigo = (model.Codigo ?? string.Empty).Trim();
        var duplicates = await _db.Database.SqlQuery<CountRow>($"SELECT COUNT(1) AS Value FROM gaillard.Instalacion WHERE Codigo={codigo} AND Id<>{model.Id}").SingleAsync();
        if (duplicates.Value > 0) errors["Codigo"] = "Ya existe una instalación con ese código.";
        if (model.TipoInstalacionId.HasValue && !await ExistsAsync($"SELECT COUNT(1) AS Value FROM gaillard.TipoInstalacion WHERE Id={model.TipoInstalacionId.Value}"))
            errors["TipoInstalacionId"] = "Selecciona un tipo válido.";
        if (model.UbicacionId.HasValue && !await ExistsAsync($"SELECT COUNT(1) AS Value FROM gaillard.Ubicacion WHERE UbicacionId={model.UbicacionId.Value}"))
            errors["UbicacionId"] = "Selecciona una ubicación válida.";
        if (model.ResponsablePersonaId.HasValue && !await ExistsAsync($"SELECT COUNT(1) AS Value FROM gaillard.Persona WHERE ID={model.ResponsablePersonaId.Value}"))
            errors["ResponsablePersonaId"] = "Selecciona una persona válida.";
        return errors;
    }

    public async Task<bool> SaveAsync(InstalacionFormViewModel model)
    {
        var description = Normalize(model.Descripcion); var plano = Normalize(model.CodigoPlano); var notes = Normalize(model.Observaciones);
        if (model.Id == 0)
        {
            await _db.Database.ExecuteSqlInterpolatedAsync($@"INSERT INTO gaillard.Instalacion
                (Codigo,Nombre,Descripcion,TipoInstalacionId,UbicacionId,ResponsablePersonaId,FechaPuestaMarcha,FechaBaja,
                 Criticidad,Estado,CodigoPlano,CriticaProduccion,CriticaSeguridad,Observaciones)
                VALUES ({model.Codigo.Trim()},{model.Nombre.Trim()},{description},{model.TipoInstalacionId},{model.UbicacionId},
                 {model.ResponsablePersonaId},{model.FechaPuestaMarcha},{model.FechaBaja},{model.Criticidad},{model.Estado},
                 {plano},{model.CriticaProduccion},{model.CriticaSeguridad},{notes})");
            return true;
        }
        var affected = await _db.Database.ExecuteSqlInterpolatedAsync($@"UPDATE gaillard.Instalacion SET
            Codigo={model.Codigo.Trim()},Nombre={model.Nombre.Trim()},Descripcion={description},TipoInstalacionId={model.TipoInstalacionId},
            UbicacionId={model.UbicacionId},ResponsablePersonaId={model.ResponsablePersonaId},FechaPuestaMarcha={model.FechaPuestaMarcha},
            FechaBaja={model.FechaBaja},Criticidad={model.Criticidad},Estado={model.Estado},CodigoPlano={plano},
            CriticaProduccion={model.CriticaProduccion},CriticaSeguridad={model.CriticaSeguridad},Observaciones={notes},UpdatedAt=SYSDATETIME()
            WHERE Id={model.Id}");
        return affected > 0;
    }

    public async Task DarDeBajaAsync(int id) => await _db.Database.ExecuteSqlInterpolatedAsync($@"UPDATE gaillard.Instalacion SET Estado=N'FUERA_SERVICIO',
        FechaBaja=CASE WHEN FechaPuestaMarcha IS NULL OR FechaPuestaMarcha<=CONVERT(date,GETDATE()) THEN COALESCE(FechaBaja,CONVERT(date,GETDATE())) ELSE FechaBaja END,
        UpdatedAt=SYSDATETIME() WHERE Id={id}");

    private async Task<bool> ExistsAsync(FormattableString sql)
    {
        var row = await _db.Database.SqlQuery<CountRow>(sql).SingleAsync();
        return row.Value > 0;
    }

    private async Task<IReadOnlyList<SelectListItem>> GetOptionsAsync(string sql, int? selected) =>
        (await _db.Database.SqlQueryRaw<LookupOption>(sql).ToListAsync())
            .Select(x => new SelectListItem(x.Nombre, x.Id.ToString(), x.Id == selected)).ToList();

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private sealed class CountRow { public int Value { get; set; } }
    private sealed class LookupOption { public int Id { get; set; } public string Nombre { get; set; } = string.Empty; }
    private sealed class FormData
    {
        public int Id { get; set; } public string Codigo { get; set; } = string.Empty; public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; } public int? TipoInstalacionId { get; set; } public int? UbicacionId { get; set; }
        public int? ResponsablePersonaId { get; set; } public DateTime? FechaPuestaMarcha { get; set; } public DateTime? FechaBaja { get; set; }
        public byte Criticidad { get; set; } public string Estado { get; set; } = string.Empty; public string? CodigoPlano { get; set; }
        public bool CriticaProduccion { get; set; } public bool CriticaSeguridad { get; set; } public string? Observaciones { get; set; }
    }
}
