using CasaGaillard.Core.Data;
using CasaGaillard.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CasaGaillard.Core.Services;

public sealed class FamiliasService : IFamiliasService
{
    private readonly AppDbContext _db;
    public FamiliasService(AppDbContext db) => _db = db;

    public async Task<FamiliasSubfamiliasIndexViewModel> GetIndexAsync() => new()
    {
        Familias = await _db.Database.SqlQueryRaw<FamiliaFilaViewModel>(@"SELECT f.FamiliaId,f.Nombre,f.Descripcion,
            (SELECT COUNT(1) FROM gaillard.Subfamilia s WHERE s.FamiliaId=f.FamiliaId) AS NumeroSubfamilias,
            (SELECT COUNT(1) FROM gaillard.Accesorio a WHERE a.FamiliaId=f.FamiliaId) AS NumeroAccesorios
            FROM gaillard.Familia f ORDER BY f.Nombre").ToListAsync(),
        Subfamilias = await _db.Database.SqlQueryRaw<SubfamiliaFilaViewModel>(@"SELECT s.SubfamiliaId,s.FamiliaId,f.Nombre AS FamiliaNombre,s.Nombre,s.Descripcion,
            (SELECT COUNT(1) FROM gaillard.Accesorio a WHERE a.SubfamiliaId=s.SubfamiliaId) AS NumeroAccesorios,
            (SELECT COUNT(1) FROM gaillard.TipoAccesorio t WHERE t.SubfamiliaId=s.SubfamiliaId) AS NumeroTipos
            FROM gaillard.Subfamilia s INNER JOIN gaillard.Familia f ON f.FamiliaId=s.FamiliaId ORDER BY f.Nombre,s.Nombre").ToListAsync()
    };

    public async Task<FamiliaFormViewModel?> GetFamiliaAsync(int id) => (await _db.Database.SqlQueryRaw<FamiliaFormViewModel>(@"SELECT FamiliaId,Nombre,Descripcion
        FROM gaillard.Familia WHERE FamiliaId=@p0", id).ToListAsync()).FirstOrDefault();

    public async Task<bool> FamiliaNombreExisteAsync(string nombre, int id) => await _db.Database.SqlQueryRaw<int>(@"SELECT COUNT(1) AS Value
        FROM gaillard.Familia WHERE LOWER(Nombre)=LOWER(@p0) AND FamiliaId<>@p1", nombre.Trim(), id).SingleAsync() > 0;

    public async Task SaveFamiliaAsync(FamiliaFormViewModel model) => await _db.Database.ExecuteSqlRawAsync(@"INSERT INTO gaillard.Familia (Nombre,Descripcion) VALUES (@p0,@p1)", model.Nombre.Trim(), SqlValue(model.Descripcion));

    public async Task<bool> UpdateFamiliaAsync(FamiliaFormViewModel model) => await _db.Database.ExecuteSqlRawAsync(@"UPDATE gaillard.Familia SET Nombre=@p0,Descripcion=@p1 WHERE FamiliaId=@p2", model.Nombre.Trim(), SqlValue(model.Descripcion), model.FamiliaId) > 0;

    public async Task<(bool Deleted, bool Exists)> DeleteFamiliaAsync(int id)
    {
        var deleted = await _db.Database.ExecuteSqlRawAsync(@"DELETE FROM gaillard.Familia WHERE FamiliaId=@p0
            AND NOT EXISTS (SELECT 1 FROM gaillard.Subfamilia WHERE FamiliaId=@p0)
            AND NOT EXISTS (SELECT 1 FROM gaillard.Accesorio WHERE FamiliaId=@p0)", id) > 0;
        if (deleted) return (true, true);
        return (false, await _db.Database.SqlQueryRaw<int>("SELECT COUNT(1) AS Value FROM gaillard.Familia WHERE FamiliaId=@p0", id).SingleAsync() > 0);
    }

    public async Task<SubfamiliaFormViewModel?> GetSubfamiliaAsync(int id)
    {
        var row = (await _db.Database.SqlQueryRaw<SubfamiliaEditRow>(@"SELECT SubfamiliaId,FamiliaId,Nombre,Descripcion
            FROM gaillard.Subfamilia WHERE SubfamiliaId=@p0", id).ToListAsync()).FirstOrDefault();
        if (row is null) return null;
        return new SubfamiliaFormViewModel
        {
            SubfamiliaId = row.SubfamiliaId,
            FamiliaId = row.FamiliaId,
            Nombre = row.Nombre,
            Descripcion = row.Descripcion,
            Familias = await GetFamiliasAsync(row.FamiliaId)
        };
    }

    public async Task<IReadOnlyList<SelectListItem>> GetFamiliasAsync(int? selectedId) =>
        (await _db.Database.SqlQueryRaw<FamiliaOption>("SELECT FamiliaId AS Id,Nombre FROM gaillard.Familia ORDER BY Nombre").ToListAsync())
            .Select(f => new SelectListItem(f.Nombre, f.Id.ToString(), f.Id == selectedId)).ToList();

    public async Task<bool> FamiliaExisteAsync(int id) => await _db.Database.SqlQueryRaw<int>("SELECT COUNT(1) AS Value FROM gaillard.Familia WHERE FamiliaId=@p0", id).SingleAsync() > 0;

    public async Task<bool> SubfamiliaNombreExisteAsync(int familiaId, string nombre, int id) => await _db.Database.SqlQueryRaw<int>(@"SELECT COUNT(1) AS Value
        FROM gaillard.Subfamilia WHERE FamiliaId=@p0 AND LOWER(Nombre)=LOWER(@p1) AND SubfamiliaId<>@p2", familiaId, nombre.Trim(), id).SingleAsync() > 0;

    public async Task SaveSubfamiliaAsync(SubfamiliaFormViewModel model) => await _db.Database.ExecuteSqlRawAsync(@"INSERT INTO gaillard.Subfamilia (FamiliaId,Nombre,Descripcion) VALUES (@p0,@p1,@p2)", model.FamiliaId!.Value, model.Nombre.Trim(), SqlValue(model.Descripcion));

    public async Task<bool> UpdateSubfamiliaAsync(SubfamiliaFormViewModel model) => await _db.Database.ExecuteSqlRawAsync(@"UPDATE gaillard.Subfamilia SET FamiliaId=@p0,Nombre=@p1,Descripcion=@p2 WHERE SubfamiliaId=@p3", model.FamiliaId!.Value, model.Nombre.Trim(), SqlValue(model.Descripcion), model.SubfamiliaId) > 0;

    public async Task<(bool Deleted, bool Exists)> DeleteSubfamiliaAsync(int id)
    {
        var deleted = await _db.Database.ExecuteSqlRawAsync(@"DELETE FROM gaillard.Subfamilia WHERE SubfamiliaId=@p0
            AND NOT EXISTS (SELECT 1 FROM gaillard.Accesorio WHERE SubfamiliaId=@p0)
            AND NOT EXISTS (SELECT 1 FROM gaillard.TipoAccesorio WHERE SubfamiliaId=@p0)", id) > 0;
        if (deleted) return (true, true);
        return (false, await _db.Database.SqlQueryRaw<int>("SELECT COUNT(1) AS Value FROM gaillard.Subfamilia WHERE SubfamiliaId=@p0", id).SingleAsync() > 0);
    }

    private static object SqlValue(string? value) => string.IsNullOrWhiteSpace(value) ? DBNull.Value : value.Trim();
    private sealed class FamiliaOption { public int Id { get; set; } public string Nombre { get; set; } = string.Empty; }
    private sealed class SubfamiliaEditRow
    {
        public int SubfamiliaId { get; set; }
        public int FamiliaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }
}
