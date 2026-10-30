using CasaGaillard.Core.Data;
using CasaGaillard.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CasaGaillard.Core.Services;

public sealed class UbicacionesService : IUbicacionesService
{
    private readonly AppDbContext _db;
    public UbicacionesService(AppDbContext db) => _db = db;

    private const string SelectSql = @"SELECT u.UbicacionId,u.Codigo,u.Nombre,u.Descripcion,
        (SELECT COUNT(1) FROM gaillard.Instalacion i WHERE i.UbicacionId=u.UbicacionId) AS InstalacionesAsociadas,
        (SELECT COUNT(1) FROM gaillard.Accesorio a WHERE a.UbicacionId=u.UbicacionId) AS AccesoriosAsociados
        FROM gaillard.Ubicacion u";

    public async Task<IReadOnlyList<UbicacionCatalogoViewModel>> GetAllAsync() =>
        await _db.Database.SqlQueryRaw<UbicacionCatalogoViewModel>(SelectSql + " ORDER BY u.Nombre").ToListAsync();

    public async Task<UbicacionCatalogoViewModel?> GetAsync(int id) =>
        await _db.Database.SqlQueryRaw<UbicacionCatalogoViewModel>(SelectSql + " WHERE u.UbicacionId=@p0", id).SingleOrDefaultAsync();

    public async Task<bool> CodigoExisteAsync(string? codigo, int id)
    {
        if (string.IsNullOrWhiteSpace(codigo)) return false;
        return await _db.Database.SqlQueryRaw<int>(@"SELECT COUNT(1) AS Value FROM gaillard.Ubicacion
            WHERE Codigo=@p0 AND UbicacionId<>@p1", codigo.Trim(), id).SingleAsync() > 0;
    }

    public async Task SaveAsync(UbicacionCatalogoViewModel model) =>
        await _db.Database.ExecuteSqlRawAsync(@"INSERT INTO gaillard.Ubicacion (Codigo, Nombre, Descripcion)
            VALUES (@p0,@p1,@p2)", SqlValue(model.Codigo), model.Nombre.Trim(), SqlValue(model.Descripcion));

    public async Task<bool> UpdateAsync(UbicacionCatalogoViewModel model) =>
        await _db.Database.ExecuteSqlRawAsync(@"UPDATE gaillard.Ubicacion SET Codigo=@p0, Nombre=@p1, Descripcion=@p2
            WHERE UbicacionId=@p3", SqlValue(model.Codigo), model.Nombre.Trim(), SqlValue(model.Descripcion), model.UbicacionId) > 0;

    public async Task<(bool Deleted, bool Exists)> DeleteAsync(int id)
    {
        var deleted = await _db.Database.ExecuteSqlRawAsync(@"DELETE FROM gaillard.Ubicacion
            WHERE UbicacionId=@p0
              AND NOT EXISTS (SELECT 1 FROM gaillard.Instalacion WHERE UbicacionId=@p0)
              AND NOT EXISTS (SELECT 1 FROM gaillard.Accesorio WHERE UbicacionId=@p0)", id) > 0;
        if (deleted) return (true, true);
        var exists = await _db.Database.SqlQueryRaw<int>("SELECT COUNT(1) AS Value FROM gaillard.Ubicacion WHERE UbicacionId=@p0", id).SingleAsync() > 0;
        return (false, exists);
    }

    private static object SqlValue(string? value) => string.IsNullOrWhiteSpace(value) ? DBNull.Value : value.Trim();
}
