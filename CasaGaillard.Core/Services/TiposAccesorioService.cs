using CasaGaillard.Core.Data;
using CasaGaillard.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CasaGaillard.Core.Services;

public sealed class TiposAccesorioService : ITiposAccesorioService
{
    private readonly AppDbContext _db;
    public TiposAccesorioService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<TipoAccesorioFilaViewModel>> GetAllAsync() =>
        await _db.Database.SqlQueryRaw<TipoAccesorioFilaViewModel>(@"SELECT t.ID AS Id,t.TipoAccesorio AS Nombre,s.Nombre AS SubfamiliaNombre,t.Descripcion,
            (SELECT COUNT(1) FROM gaillard.Accesorio a WHERE a.TipoAccesorioID=t.ID) AS AccesoriosAsociados
            FROM gaillard.TipoAccesorio t LEFT JOIN gaillard.Subfamilia s ON s.SubfamiliaId=t.SubfamiliaId ORDER BY t.TipoAccesorio").ToListAsync();

    public async Task<IReadOnlyList<SelectListItem>> GetSubfamiliasAsync(int? selectedId) =>
        (await _db.Database.SqlQueryRaw<SubfamiliaOption>(@"SELECT s.SubfamiliaId AS Id,CONCAT(f.Nombre,N' / ',s.Nombre) AS Nombre
            FROM gaillard.Subfamilia s INNER JOIN gaillard.Familia f ON f.FamiliaId=s.FamiliaId ORDER BY f.Nombre,s.Nombre").ToListAsync())
            .Select(s => new SelectListItem(s.Nombre, s.Id.ToString(), s.Id == selectedId)).ToList();

    public async Task<bool> SubfamiliaExisteAsync(int id) =>
        await _db.Database.SqlQueryRaw<int>("SELECT COUNT(1) AS Value FROM gaillard.Subfamilia WHERE SubfamiliaId=@p0", id).SingleAsync() > 0;

    public async Task<TipoAccesorioFilaViewModel?> FindByNameAsync(string nombre) =>
        (await _db.Database.SqlQueryRaw<TipoAccesorioFilaViewModel>(@"SELECT TOP (1) t.ID AS Id,t.TipoAccesorio AS Nombre,
            s.Nombre AS SubfamiliaNombre,t.Descripcion,0 AS AccesoriosAsociados FROM gaillard.TipoAccesorio t
            LEFT JOIN gaillard.Subfamilia s ON s.SubfamiliaId=t.SubfamiliaId WHERE LOWER(t.TipoAccesorio)=LOWER(@p0)", nombre.Trim()).ToListAsync()).FirstOrDefault();

    public async Task<TipoAccesorioFormViewModel?> GetByIdAsync(int id)
    {
        var row = (await _db.Database.SqlQueryRaw<TipoAccesorioFormRow>(@"SELECT ID AS Id,TipoAccesorio AS Nombre,Descripcion,SubfamiliaId
            FROM gaillard.TipoAccesorio WHERE ID=@p0", id).ToListAsync()).FirstOrDefault();
        return row is null ? null : new TipoAccesorioFormViewModel
        {
            Id = row.Id,
            Nombre = row.Nombre,
            Descripcion = row.Descripcion,
            SubfamiliaId = row.SubfamiliaId
        };
    }

    public async Task<bool> NameExistsAsync(string nombre, int excludingId) =>
        await _db.Database.SqlQueryRaw<int>("SELECT COUNT(1) AS Value FROM gaillard.TipoAccesorio WHERE LOWER(TipoAccesorio)=LOWER(@p0) AND ID<>@p1", nombre.Trim(), excludingId).SingleAsync() > 0;

    public async Task<int> CreateAsync(TipoAccesorioFormViewModel model)
    {
        var ids = await _db.Database.SqlQueryRaw<int>(@"INSERT INTO gaillard.TipoAccesorio (TipoAccesorio,Descripcion,SubfamiliaId)
            OUTPUT INSERTED.ID AS Value VALUES (@p0,@p1,@p2)", model.Nombre.Trim(), SqlValue(model.Descripcion), SqlValue(model.SubfamiliaId)).ToListAsync();
        return ids.Single();
    }

    public async Task<bool> UpdateAsync(TipoAccesorioFormViewModel model) =>
        await _db.Database.ExecuteSqlRawAsync(@"UPDATE gaillard.TipoAccesorio SET TipoAccesorio=@p0,Descripcion=@p1,SubfamiliaId=@p2 WHERE ID=@p3",
            model.Nombre.Trim(), SqlValue(model.Descripcion), SqlValue(model.SubfamiliaId), model.Id) > 0;

    private static object SqlValue(object? value) => value switch
    {
        null => DBNull.Value,
        string text when string.IsNullOrWhiteSpace(text) => DBNull.Value,
        string text => text.Trim(),
        _ => value
    };
    private sealed class SubfamiliaOption { public int Id { get; set; } public string Nombre { get; set; } = string.Empty; }
    private sealed class TipoAccesorioFormRow
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int? SubfamiliaId { get; set; }
    }
}
