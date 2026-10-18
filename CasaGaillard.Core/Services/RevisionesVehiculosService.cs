using CasaGaillard.Core.Data;
using CasaGaillard.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CasaGaillard.Core.Services;

public sealed class RevisionesVehiculosService : IRevisionesVehiculosService
{
    private readonly AppDbContext _db;
    public RevisionesVehiculosService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<RevisionVehiculoFilaViewModel>> GetAllAsync(string? matricula, bool incluirBajas)
    {
        var result = await _db.Database.SqlQueryRaw<RevisionVehiculoFilaViewModel>(@"SELECT r.ID Id, r.VehiculoID VehiculoId,
            COALESCE(v.MatriculaVehiculo,'') Matricula, r.TipoRevisionID TipoRevisionId, COALESCE(t.Revision,'') TipoRevision,
            r.FechaRevision, r.Caducidad, r.Detalles, r.Ejecutor FROM gaillard.RevisionVehiculo r
            LEFT JOIN gaillard.Vehiculo v ON v.ID=r.VehiculoID LEFT JOIN gaillard.TipoRevision t ON t.ID=r.TipoRevisionID
            WHERE (@p0 IS NULL OR v.MatriculaVehiculo=@p0) AND (@p1=1 OR v.Baja IS NULL OR v.Baja=0)
            ORDER BY v.MatriculaVehiculo,t.Revision,r.FechaRevision DESC", (object?)matricula ?? DBNull.Value, incluirBajas ? 1 : 0).ToListAsync();
        return result;
    }

    public async Task<IReadOnlyList<RevisionVehiculoFilaViewModel>> GetProximasAsync() =>
        await _db.Database.SqlQueryRaw<RevisionVehiculoFilaViewModel>(@"SELECT r.ID Id,r.VehiculoID VehiculoId,COALESCE(v.MatriculaVehiculo,'') Matricula,
            r.TipoRevisionID TipoRevisionId,COALESCE(t.Revision,'') TipoRevision,r.FechaRevision,r.Caducidad,r.Detalles,r.Ejecutor
            FROM gaillard.RevisionVehiculo r LEFT JOIN gaillard.Vehiculo v ON v.ID=r.VehiculoID
            LEFT JOIN gaillard.TipoRevision t ON t.ID=r.TipoRevisionID WHERE r.Caducidad IS NOT NULL ORDER BY r.Caducidad").ToListAsync();

    public async Task<RevisionVehiculoFormViewModel?> GetFormAsync(int? id, int? vehiculoId, int? tipoId)
    {
        var model = new RevisionVehiculoFormViewModel { VehiculoId = vehiculoId, TipoRevisionId = tipoId, FechaRevision = DateTime.Today };
        if (id.HasValue)
        {
            var rows = await _db.Database.SqlQueryRaw<FormRow>(@"SELECT ID Id,VehiculoID VehiculoId,TipoRevisionID TipoRevisionId,
                FechaRevision,Caducidad,Detalles,Ejecutor,EjecutorPersonasEntidadID EjecutorPersonasEntidadId FROM gaillard.RevisionVehiculo WHERE ID=@p0", id.Value).ToListAsync();
            if (rows.Count == 0) return null;
            var row = rows[0];
            model = new RevisionVehiculoFormViewModel { Id = row.Id, VehiculoId = row.VehiculoId, TipoRevisionId = row.TipoRevisionId,
                FechaRevision = row.FechaRevision, Caducidad = row.Caducidad, Detalles = row.Detalles,
                Ejecutor = row.Ejecutor, EjecutorPersonasEntidadId = row.EjecutorPersonasEntidadId };
        }
        var vehicles = await _db.Database.SqlQueryRaw<OptionRow>("SELECT ID Id,MatriculaVehiculo Texto FROM gaillard.Vehiculo WHERE (Baja IS NULL OR Baja=0) OR ID=@p0 ORDER BY MatriculaVehiculo", (object?)model.VehiculoId ?? DBNull.Value).ToListAsync();
        var types = await _db.Database.SqlQueryRaw<OptionRow>("SELECT ID Id,Revision Texto FROM gaillard.TipoRevision ORDER BY Revision").ToListAsync();
        var people = await _db.Database.SqlQueryRaw<OptionRow>(@"SELECT pe.ID Id,LEFT(LTRIM(RTRIM(CONCAT(p.NombrePersona,' ',p.Apellido1,' ',p.Apellido2,' — ',e.NombreEntidad))),45) Texto
            FROM gaillard.PersonasEntidad pe JOIN gaillard.Persona p ON p.ID=pe.PersonaID JOIN gaillard.Entidad e ON e.ID=pe.EntidadID
            WHERE pe.FechaBaja IS NULL OR pe.ID=@p0 ORDER BY p.Apellido1,p.Apellido2,p.NombrePersona,e.NombreEntidad", (object?)model.EjecutorPersonasEntidadId ?? DBNull.Value).ToListAsync();
        model.Vehiculos = vehicles.Select(x => new SelectListItem(x.Texto ?? "", x.Id.ToString(), x.Id == model.VehiculoId)).ToList();
        model.Tipos = types.Select(x => new SelectListItem(x.Texto ?? "", x.Id.ToString(), x.Id == model.TipoRevisionId)).ToList();
        model.Ejecutores = people.Select(x => new SelectListItem(x.Texto ?? "", x.Id.ToString(), x.Id == model.EjecutorPersonasEntidadId)).ToList();
        return model;
    }

    public async Task<bool> SaveAsync(RevisionVehiculoFormViewModel m, bool create)
    {
        string? label = null;
        if (m.EjecutorPersonasEntidadId.HasValue)
        {
            var labels = await _db.Database.SqlQueryRaw<ScalarText>(@"SELECT LEFT(LTRIM(RTRIM(CONCAT(p.NombrePersona,' ',p.Apellido1,' ',p.Apellido2,' — ',e.NombreEntidad))),45) AS Value
                FROM gaillard.PersonasEntidad pe JOIN gaillard.Persona p ON p.ID=pe.PersonaID JOIN gaillard.Entidad e ON e.ID=pe.EntidadID
                WHERE pe.ID=@p0 AND (@p1=0 OR pe.FechaBaja IS NULL)", m.EjecutorPersonasEntidadId.Value, create ? 1 : 0).ToListAsync();
            label = labels.FirstOrDefault()?.Value;
            if (label is null) return false;
        }
        var expiry = m.Caducidad?.Date ?? m.FechaRevision?.Date.AddMonths(30);
        if (create)
            await _db.Database.ExecuteSqlInterpolatedAsync($@"INSERT INTO gaillard.RevisionVehiculo(VehiculoID,TipoRevisionID,FechaRevision,Caducidad,Detalles,Ejecutor,EjecutorPersonasEntidadID)
                VALUES({m.VehiculoId},{m.TipoRevisionId},{m.FechaRevision?.Date},{expiry},{m.Detalles},{label},{m.EjecutorPersonasEntidadId})");
        else
            await _db.Database.ExecuteSqlInterpolatedAsync($@"UPDATE gaillard.RevisionVehiculo SET VehiculoID={m.VehiculoId},TipoRevisionID={m.TipoRevisionId},FechaRevision={m.FechaRevision?.Date},
                Caducidad={expiry},Detalles={m.Detalles},Ejecutor=COALESCE({label},Ejecutor),EjecutorPersonasEntidadID={m.EjecutorPersonasEntidadId} WHERE ID={m.Id}");
        return true;
    }

    public async Task<bool> DeleteAsync(int id) => await _db.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM gaillard.RevisionVehiculo WHERE ID={id}") > 0;
    private sealed class OptionRow { public int Id { get; set; } public string? Texto { get; set; } }
    private sealed class ScalarText { public string? Value { get; set; } }
    private sealed class FormRow { public int Id { get; set; } public int? VehiculoId { get; set; } public int? TipoRevisionId { get; set; }
        public DateTime? FechaRevision { get; set; } public DateTime? Caducidad { get; set; } public string? Detalles { get; set; }
        public string? Ejecutor { get; set; } public int? EjecutorPersonasEntidadId { get; set; } }
}
