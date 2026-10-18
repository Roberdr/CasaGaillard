using CasaGaillard.Core.Data;
using CasaGaillard.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CasaGaillard.Core.Services;

public sealed class GruposService : IGruposService
{
    private readonly AppDbContext _db;
    public GruposService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<GrupoFilaViewModel>> GetAllAsync()
    {
        var groups = await _db.Database.SqlQueryRaw<GrupoFilaViewModel>(@"SELECT g.ID Id,c.MatriculaCuba Cuba,coalesce(co.Numero,0) Compartimento,
        coalesce(t.NombreGrupo,'') Tipo,coalesce(s.LadoCubaNombre,'') Situacion FROM gaillard.Grupo g
        LEFT JOIN gaillard.Cuba c ON c.ID=g.CubaID LEFT JOIN gaillard.Compartimento co ON co.ID=g.CompartimentoID
        LEFT JOIN gaillard.TipoGrupo t ON t.ID=g.TipoGrupoID LEFT JOIN gaillard.Situacion s ON s.ID=g.SituacionID
        ORDER BY c.MatriculaCuba,co.Numero,t.NombreGrupo").ToListAsync();
        var accessories = await _db.Database.SqlQueryRaw<GrupoAccesorioViewModel>(@"SELECT ag.GrupoID GrupoId,ag.Cantidad,ta.TipoAccesorio,m.Material
            FROM gaillard.AccesorioGrupo ag JOIN gaillard.Accesorio a ON a.ID=ag.AccesorioID
            JOIN gaillard.TipoAccesorio ta ON ta.ID=a.TipoAccesorioID JOIN gaillard.Material m ON m.ID=a.MaterialID
            ORDER BY ta.TipoAccesorio,m.Material").ToListAsync();
        var byGroup = accessories.GroupBy(x => x.GrupoId).ToDictionary(g => g.Key, g => (IReadOnlyList<GrupoAccesorioViewModel>)g.ToList());
        foreach (var group in groups) if (byGroup.TryGetValue(group.Id, out var rows)) group.Accesorios = rows;
        return groups;
    }

    public async Task<GrupoFormViewModel?> GetFormAsync(int? id)
    {
        var m = new GrupoFormViewModel();
        if (id.HasValue)
        {
            var rows = await _db.Database.SqlQueryRaw<FormRow>("SELECT ID,TipoGrupoID TipoGrupoId,CubaID CubaId,CompartimentoID CompartimentoId,SituacionID SituacionId FROM gaillard.Grupo WHERE ID=@p0", id.Value).ToListAsync();
            if (rows.Count == 0) return null;
            var r = rows[0]; m = new GrupoFormViewModel { Id = r.Id, TipoGrupoId = r.TipoGrupoId, CubaId = r.CubaId, CompartimentoId = r.CompartimentoId, SituacionId = r.SituacionId };
        }
        var types = await _db.Database.SqlQueryRaw<OptionRow>("SELECT ID,NombreGrupo Texto FROM gaillard.TipoGrupo ORDER BY NombreGrupo").ToListAsync();
        var cubas = await _db.Database.SqlQueryRaw<OptionRow>("SELECT ID,MatriculaCuba Texto FROM gaillard.Cuba ORDER BY MatriculaCuba").ToListAsync();
        var compartments = await _db.Database.SqlQueryRaw<OptionRow>("SELECT ID,CONCAT('Cuba ',CubaID,' · Compartimento ',Numero) Texto FROM gaillard.Compartimento ORDER BY CubaID,Numero").ToListAsync();
        var situations = await _db.Database.SqlQueryRaw<OptionRow>("SELECT ID,CONCAT(LadoCubaNombre,' · ',SituacionLadoLetra) Texto FROM gaillard.Situacion ORDER BY LadoCubaNumero,SituacionLadoNumero").ToListAsync();
        m.Tipos = Items(types, m.TipoGrupoId); m.Cubas = Items(cubas, m.CubaId); m.Compartimentos = Items(compartments, m.CompartimentoId); m.Situaciones = Items(situations, m.SituacionId);
        return m;
    }

    public async Task SaveAsync(GrupoFormViewModel m, bool create)
    {
        if (create)
            await _db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO gaillard.Grupo(TipoGrupoID,CubaID,CompartimentoID,SituacionID) VALUES({m.TipoGrupoId},{m.CubaId},{m.CompartimentoId},{m.SituacionId})");
        else
            await _db.Database.ExecuteSqlInterpolatedAsync($"UPDATE gaillard.Grupo SET TipoGrupoID={m.TipoGrupoId},CubaID={m.CubaId},CompartimentoID={m.CompartimentoId},SituacionID={m.SituacionId} WHERE ID={m.Id}");
    }

    public async Task<string?> ValidateAsync(GrupoFormViewModel m)
    {
        var count = await _db.Database.SqlQueryRaw<int>(@"SELECT COUNT(*) AS Value FROM gaillard.TipoGrupo t
            CROSS JOIN gaillard.Cuba c CROSS JOIN gaillard.Compartimento co CROSS JOIN gaillard.Situacion s
            WHERE t.ID=@p0 AND c.ID=@p1 AND co.ID=@p2 AND co.CubaID=c.ID AND s.ID=@p3",
            (object?)m.TipoGrupoId ?? DBNull.Value, (object?)m.CubaId ?? DBNull.Value,
            (object?)m.CompartimentoId ?? DBNull.Value, (object?)m.SituacionId ?? DBNull.Value).SingleAsync();
        return count == 1 ? null : "Selecciona un tipo, cuba, compartimento y situación válidos. El compartimento debe pertenecer a la cuba seleccionada.";
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var tx = await _db.Database.BeginTransactionAsync();
        await _db.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM gaillard.AccesorioGrupo WHERE GrupoID={id}");
        var count = await _db.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM gaillard.Grupo WHERE ID={id}");
        await tx.CommitAsync(); return count > 0;
    }

    private static IReadOnlyList<SelectListItem> Items(IEnumerable<OptionRow> rows, int? selected) => rows.Select(x => new SelectListItem(x.Texto ?? "", x.Id.ToString(), x.Id == selected)).ToList();
    private sealed class OptionRow { public int Id { get; set; } public string? Texto { get; set; } }
    private sealed class FormRow { public int Id { get; set; } public int TipoGrupoId { get; set; } public int CubaId { get; set; } public int CompartimentoId { get; set; } public int SituacionId { get; set; } }
}
