using CasaGaillard.Core.Data;
using CasaGaillard.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CasaGaillard.Core.Services;

public sealed class TiposInstalacionService : ITiposInstalacionService
{
    private readonly AppDbContext _db;
    public TiposInstalacionService(AppDbContext db) => _db = db;

    private const string SelectSql = @"SELECT t.Id,t.Nombre,t.Descripcion,
        (SELECT COUNT(1) FROM gaillard.Instalacion i WHERE i.TipoInstalacionId=t.Id) AS InstalacionesAsociadas
        FROM gaillard.TipoInstalacion t";

    public async Task<IReadOnlyList<TipoInstalacionCatalogoViewModel>> GetAllAsync() =>
        await _db.Database.SqlQueryRaw<TipoInstalacionCatalogoViewModel>(SelectSql + " ORDER BY t.Nombre").ToListAsync();

    public async Task<TipoInstalacionCatalogoViewModel?> GetAsync(int id) =>
        await _db.Database.SqlQueryRaw<TipoInstalacionCatalogoViewModel>(SelectSql + " WHERE t.Id=@p0", id).SingleOrDefaultAsync();

    public async Task<bool> NombreExisteAsync(string nombre, int id) =>
        await _db.Database.SqlQueryRaw<int>(@"SELECT COUNT(1) AS Value FROM gaillard.TipoInstalacion
            WHERE Nombre=@p0 AND Id<>@p1", nombre.Trim(), id).SingleAsync() > 0;

    public async Task SaveAsync(TipoInstalacionCatalogoViewModel model)
    {
        await _db.Database.ExecuteSqlRawAsync(@"INSERT INTO gaillard.TipoInstalacion (Nombre, Descripcion)
            VALUES (@p0,@p1)", model.Nombre.Trim(), SqlValue(model.Descripcion));
    }

    public async Task<bool> UpdateAsync(TipoInstalacionCatalogoViewModel model) =>
        await _db.Database.ExecuteSqlRawAsync(@"UPDATE gaillard.TipoInstalacion SET Nombre=@p0, Descripcion=@p1 WHERE Id=@p2",
            model.Nombre.Trim(), SqlValue(model.Descripcion), model.Id) > 0;

    public async Task<(bool Deleted, bool Exists)> DeleteAsync(int id)
    {
        var deleted = await _db.Database.ExecuteSqlRawAsync(@"DELETE FROM gaillard.TipoInstalacion
            WHERE Id=@p0 AND NOT EXISTS (SELECT 1 FROM gaillard.Instalacion WHERE TipoInstalacionId=@p0)", id) > 0;
        if (deleted) return (true, true);
        var exists = await _db.Database.SqlQueryRaw<int>("SELECT COUNT(1) AS Value FROM gaillard.TipoInstalacion WHERE Id=@p0", id).SingleAsync() > 0;
        return (false, exists);
    }

    private static object SqlValue(string? value) => string.IsNullOrWhiteSpace(value) ? DBNull.Value : value.Trim();
}
