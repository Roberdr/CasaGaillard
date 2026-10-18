using CasaGaillard.Core.Data;
using CasaGaillard.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CasaGaillard.Core.Services;

public sealed class RevisionesService : IRevisionesService
{
    private readonly AppDbContext _db;
    public RevisionesService(AppDbContext db) => _db = db;

    public async Task<RevisionesIndexViewModel> GetIndexAsync(int? cubaId, string? matriculaCuba, bool incluirBajas)
    {
        var selectedRegistration=string.IsNullOrWhiteSpace(matriculaCuba)?null:matriculaCuba.Trim();
        var rows=await _db.Database.SqlQuery<RevisionIndexRow>($@"SELECT c.ID CubaId,c.MatriculaCuba,CAST(ISNULL(c.Baja,0) AS bit) CubaBaja,
            r.ID RevisionId,r.FechaRevision,r.Descripcion,r.ValidaHasta,r.DescripcionProxima,r.Autorizado
            FROM gaillard.Cuba c LEFT JOIN gaillard.Revision r ON r.CubaID=c.ID
            WHERE ({cubaId} IS NULL OR c.ID={cubaId}) AND ({selectedRegistration} IS NULL OR c.MatriculaCuba={selectedRegistration})
              AND ({incluirBajas}=1 OR COALESCE(c.Baja,0)=0)
            ORDER BY c.MatriculaCuba,r.FechaRevision DESC,r.ID DESC").ToListAsync();
        var grupos=rows.GroupBy(x=>new{x.CubaId,x.MatriculaCuba,x.CubaBaja}).Select(g=>
        {
            var history=g.Where(x=>x.RevisionId.HasValue).Select(x=>new RevisionHistorialItemViewModel{Id=x.RevisionId!.Value,CubaId=g.Key.CubaId,MatriculaCuba=g.Key.MatriculaCuba,
                FechaRevision=x.FechaRevision!.Value,Descripcion=x.Descripcion,ValidaHasta=x.ValidaHasta,DescripcionProxima=x.DescripcionProxima,Autorizado=x.Autorizado}).ToList();
            return new RevisionCubaFilaViewModel{CubaId=g.Key.CubaId,MatriculaCuba=g.Key.MatriculaCuba,CubaBaja=g.Key.CubaBaja,UltimaRevision=history.FirstOrDefault(),Historial=history};
        }).OrderBy(x=>x.MatriculaCuba).ToList();
        return new RevisionesIndexViewModel{CubaId=cubaId,MatriculaCuba=selectedRegistration,IncluirBajas=incluirBajas,Revisiones=grupos,
            Cubas=await GetCubaOptionsAsync(cubaId)};
    }

    public async Task<RevisionDetalleViewModel?> GetDetailsAsync(int id)
    {
        var rows=await _db.Database.SqlQuery<RevisionDetalleViewModel>($@"SELECT r.ID Id,r.CubaID CubaId,c.MatriculaCuba,r.FechaRevision,r.Descripcion,r.ValidaHasta,r.DescripcionProxima,r.Autorizado,r.AutorizadoPersonasEntidadID AutorizadoPersonasEntidadId
            FROM gaillard.Revision r JOIN gaillard.Cuba c ON c.ID=r.CubaID WHERE r.ID={id}").ToListAsync();
        return rows.SingleOrDefault();
    }

    public async Task<RevisionFormViewModel?> GetFormAsync(int? id,int? cubaId=null)
    {
        var model=new RevisionFormViewModel{CubaId=cubaId??0,FechaRevision=DateTime.Today};
        if(id.HasValue)
        {
            var rows=await _db.Database.SqlQuery<RevisionFormRow>($@"SELECT ID,CubaID CubaId,FechaRevision,Descripcion,ValidaHasta,DescripcionProxima,Autorizado,AutorizadoPersonasEntidadID AutorizadoPersonasEntidadId FROM gaillard.Revision WHERE ID={id.Value}").ToListAsync();
            var r=rows.SingleOrDefault();if(r is null)return null;
            model=new RevisionFormViewModel{Id=r.Id,CubaId=r.CubaId,FechaRevision=r.FechaRevision,Descripcion=r.Descripcion,ValidaHasta=r.ValidaHasta,
                DescripcionProxima=r.DescripcionProxima,Autorizado=r.Autorizado,AutorizadoPersonasEntidadId=r.AutorizadoPersonasEntidadId};
        }
        return await PrepareFormAsync(model);
    }

    public async Task<RevisionFormViewModel> PrepareFormAsync(RevisionFormViewModel model)
    {
        model.Cubas=await GetCubaOptionsAsync(model.CubaId==0?null:model.CubaId);
        var pe=await _db.Database.SqlQueryRaw<PersonOption>(@"SELECT pe.ID,CONCAT(LTRIM(RTRIM(p.NombrePersona)),N' ',LTRIM(RTRIM(p.Apellido1)),N' ',LTRIM(RTRIM(COALESCE(p.Apellido2,N''))),N' — ',LTRIM(RTRIM(e.NombreEntidad))) Texto
            FROM gaillard.PersonasEntidad pe JOIN gaillard.Persona p ON p.ID=pe.PersonaID JOIN gaillard.Entidad e ON e.ID=pe.EntidadID
            WHERE pe.FechaBaja IS NULL OR pe.ID=@p0 ORDER BY p.Apellido1,p.Apellido2,p.NombrePersona,e.NombreEntidad",(object?)model.AutorizadoPersonasEntidadId??DBNull.Value).ToListAsync();
        model.PersonasEntidad=pe.Select(x=>new SelectListItem(x.Texto,x.Id.ToString(),x.Id==model.AutorizadoPersonasEntidadId)).ToList();
        return model;
    }

    public async Task<IReadOnlyDictionary<string,string>> ValidateAsync(RevisionFormViewModel model,bool creating)
    {
        var errors=new Dictionary<string,string>();
        if(await CountAsync($"SELECT COUNT(1) AS Value FROM gaillard.Cuba WHERE ID={model.CubaId}")==0) errors[nameof(model.CubaId)]="Selecciona una cuba válida.";
        if(creating && !model.AutorizadoPersonasEntidadId.HasValue) errors[nameof(model.AutorizadoPersonasEntidadId)]="Selecciona una persona perteneciente a una entidad.";
        if(model.AutorizadoPersonasEntidadId.HasValue)
        {
            var activeOnly=creating;
            var label=await GetPersonLabelAsync(model.AutorizadoPersonasEntidadId.Value,activeOnly);
            if(label is null) errors[nameof(model.AutorizadoPersonasEntidadId)]=creating?"Selecciona una persona perteneciente a una entidad activa.":"Selecciona una relación persona-entidad válida.";
        }
        if(model.ValidaHasta.HasValue && model.ValidaHasta.Value.Date<model.FechaRevision.Date) errors[nameof(model.ValidaHasta)]="La fecha de validez no puede ser anterior a la revisión.";
        return errors;
    }

    public async Task<bool> SaveAsync(RevisionFormViewModel m)
    {
        var label=m.AutorizadoPersonasEntidadId.HasValue?await GetPersonLabelAsync(m.AutorizadoPersonasEntidadId.Value,false):null;
        var validUntil=m.ValidaHasta??m.FechaRevision.AddMonths(30);
        if(m.Id==0)
        {
            if(label is null)return false;
            await _db.Database.ExecuteSqlInterpolatedAsync($@"INSERT INTO gaillard.Revision(CubaID,FechaRevision,Descripcion,ValidaHasta,DescripcionProxima,Autorizado,AutorizadoPersonasEntidadID)
                VALUES({m.CubaId},{m.FechaRevision.Date},{Normalize(m.Descripcion)},{validUntil.Date},{Normalize(m.DescripcionProxima)},{label},{m.AutorizadoPersonasEntidadId})");return true;
        }
        var affected=await _db.Database.ExecuteSqlInterpolatedAsync($@"UPDATE gaillard.Revision SET CubaID={m.CubaId},FechaRevision={m.FechaRevision.Date},Descripcion={Normalize(m.Descripcion)},
            ValidaHasta={m.ValidaHasta?.Date},DescripcionProxima={Normalize(m.DescripcionProxima)},Autorizado=COALESCE({label},Autorizado),AutorizadoPersonasEntidadID={m.AutorizadoPersonasEntidadId} WHERE ID={m.Id}");
        return affected>0;
    }

    public async Task<bool> DeleteAsync(int id)=>await _db.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM gaillard.Revision WHERE ID={id}")>0;
    private async Task<IReadOnlyList<SelectListItem>> GetCubaOptionsAsync(int? selected)
    {
        var rows=await _db.Database.SqlQuery<CubaOption>($@"SELECT ID,MatriculaCuba Texto FROM gaillard.Cuba WHERE COALESCE(Baja,0)=0 OR ID={selected} ORDER BY MatriculaCuba").ToListAsync();
        return rows.Select(x=>new SelectListItem(x.Texto,x.Id.ToString(),x.Id==selected)).ToList();
    }
    private async Task<int> CountAsync(FormattableString sql)=>await _db.Database.SqlQuery<int>(sql).SingleAsync();
    private async Task<string?> GetPersonLabelAsync(int id,bool activeOnly)
    {
        var rows=await _db.Database.SqlQuery<PersonLabel>($@"SELECT CONCAT(LTRIM(RTRIM(p.NombrePersona)),N' ',LTRIM(RTRIM(p.Apellido1)),N' ',LTRIM(RTRIM(COALESCE(p.Apellido2,N''))),N' — ',LTRIM(RTRIM(e.NombreEntidad))) Texto
            FROM gaillard.PersonasEntidad pe JOIN gaillard.Persona p ON p.ID=pe.PersonaID JOIN gaillard.Entidad e ON e.ID=pe.EntidadID
            WHERE pe.ID={id} AND ({activeOnly}=0 OR pe.FechaBaja IS NULL)").ToListAsync();
        return rows.SingleOrDefault()?.Texto;
    }
    private static string? Normalize(string? value)=>string.IsNullOrWhiteSpace(value)?null:value.Trim();
    private sealed class RevisionIndexRow { public int CubaId{get;set;} public string MatriculaCuba{get;set;}=string.Empty;public bool CubaBaja{get;set;}public int? RevisionId{get;set;}public DateTime? FechaRevision{get;set;}public string? Descripcion{get;set;}public DateTime? ValidaHasta{get;set;}public string? DescripcionProxima{get;set;}public string? Autorizado{get;set;} }
    private sealed class RevisionFormRow { public int Id{get;set;}public int CubaId{get;set;}public DateTime FechaRevision{get;set;}public string? Descripcion{get;set;}public DateTime? ValidaHasta{get;set;}public string? DescripcionProxima{get;set;}public string? Autorizado{get;set;}public int? AutorizadoPersonasEntidadId{get;set;} }
    private sealed class CubaOption { public int Id{get;set;}public string Texto{get;set;}=string.Empty; }
    private sealed class PersonOption { public int Id{get;set;}public string Texto{get;set;}=string.Empty; }
    private sealed class PersonLabel { public string Texto{get;set;}=string.Empty; }
}
