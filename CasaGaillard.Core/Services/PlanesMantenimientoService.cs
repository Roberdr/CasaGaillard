using CasaGaillard.Core.Data;
using CasaGaillard.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CasaGaillard.Core.Services;

public sealed class PlanesMantenimientoService : IPlanesMantenimientoService
{
    private readonly AppDbContext _db;
    public PlanesMantenimientoService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<PlanMantenimientoFilaViewModel>> GetAllAsync() => await _db.Database.SqlQuery<PlanMantenimientoFilaViewModel>($@"
        SELECT p.PlanId,p.Nombre,p.Descripcion,p.Tipo,p.PeriodicidadDias,p.ProximaEjecucion,p.Activo,
          COALESCE(NULLIF(LTRIM(RTRIM(e.Nombre)),N''),e.Codigo,NULLIF(c.MatriculaCuba,N''),NULLIF(v.MatriculaVehiculo,N'')) AS EquipoNombre,
          COALESCE(NULLIF(LTRIM(RTRIM(a.Nombre)),N''),N'Accesorio '+CONVERT(nvarchar(20),a.ID)) AS AccesorioNombre,ta.TipoAccesorio AS AccesorioTipo
        FROM gaillard.PlanMantenimiento p LEFT JOIN gaillard.Equipo e ON e.Id=p.EquipoId LEFT JOIN gaillard.Cuba c ON c.ID=p.CubaId LEFT JOIN gaillard.Vehiculo v ON v.ID=p.VehiculoId
        LEFT JOIN gaillard.Accesorio a ON a.ID=p.AccesorioId LEFT JOIN gaillard.TipoAccesorio ta ON ta.ID=a.TipoAccesorioID
        ORDER BY p.Activo DESC,p.ProximaEjecucion,p.Nombre").ToListAsync();

    public async Task<PlanMantenimientoDetalleViewModel?> GetDetailsAsync(int id)
    {
        var rows = await _db.Database.SqlQuery<PlanRow>($@"SELECT p.PlanId,p.Nombre,p.Descripcion,p.Tipo,p.PeriodicidadDias,p.UltimaEjecucion,p.ProximaEjecucion,
            p.AccesorioId,p.AccesorioGrupoID,p.EquipoId,p.FrecuenciaValor,p.FrecuenciaUnidad,p.DuracionEstimadaMinutos,p.Activo,
            COALESCE(NULLIF(LTRIM(RTRIM(e.Nombre)),N''),e.Codigo,NULLIF(cPlan.MatriculaCuba,N''),NULLIF(vPlan.MatriculaVehiculo,N'')) EquipoNombre,
            COALESCE(NULLIF(LTRIM(RTRIM(a.Nombre)),N''),N'Accesorio '+CONVERT(nvarchar(20),a.ID)) AccesorioNombre,ta.TipoAccesorio AccesorioTipo,
            CASE WHEN ag.ID IS NULL THEN NULL ELSE CONCAT(COALESCE(NULLIF(LTRIM(RTRIM(a.Nombre)),N''),N'Accesorio '+CONVERT(nvarchar(20),a.ID)),N' - Cuba ',c.MatriculaCuba,N' - Comp. ',cp.Numero,N' - ',s.LadoCubaNombre,N' - ',tg.NombreGrupo,N' - Cantidad ',ag.Cantidad) END AccesorioGrupoTexto
            FROM gaillard.PlanMantenimiento p LEFT JOIN gaillard.Equipo e ON e.Id=p.EquipoId LEFT JOIN gaillard.Cuba cPlan ON cPlan.ID=p.CubaId LEFT JOIN gaillard.Vehiculo vPlan ON vPlan.ID=p.VehiculoId LEFT JOIN gaillard.Accesorio a ON a.ID=p.AccesorioId LEFT JOIN gaillard.TipoAccesorio ta ON ta.ID=a.TipoAccesorioID
            LEFT JOIN gaillard.AccesorioGrupo ag ON ag.ID=p.AccesorioGrupoID LEFT JOIN gaillard.Grupo g ON g.ID=ag.GrupoID
            LEFT JOIN gaillard.Cuba c ON c.ID=g.CubaID LEFT JOIN gaillard.Compartimento cp ON cp.ID=g.CompartimentoID
            LEFT JOIN gaillard.Situacion s ON s.ID=g.SituacionID LEFT JOIN gaillard.TipoGrupo tg ON tg.ID=g.TipoGrupoID WHERE p.PlanId={id}").ToListAsync();
        var x=rows.SingleOrDefault(); if(x is null)return null;
        return new PlanMantenimientoDetalleViewModel { PlanId=x.PlanId,Nombre=x.Nombre,Descripcion=x.Descripcion,Tipo=x.Tipo,PeriodicidadDias=x.PeriodicidadDias,
            UltimaEjecucion=x.UltimaEjecucion,ProximaEjecucion=x.ProximaEjecucion,AccesorioId=x.AccesorioId,AccesorioGrupoID=x.AccesorioGrupoID,EquipoId=x.EquipoId,
            FrecuenciaValor=x.FrecuenciaValor,FrecuenciaUnidad=x.FrecuenciaUnidad,DuracionEstimadaMinutos=x.DuracionEstimadaMinutos,Activo=x.Activo,
            EquipoNombre=x.EquipoNombre,AccesorioNombre=x.AccesorioNombre,AccesorioTipo=x.AccesorioTipo,AccesorioGrupoTexto=x.AccesorioGrupoTexto };
    }

    public async Task<PlanMantenimientoFormViewModel?> GetFormAsync(int? id)
    {
        var model=new PlanMantenimientoFormViewModel();
        if(id.HasValue)
        {
            var rows=await _db.Database.SqlQuery<PlanFormRow>($@"SELECT PlanId,Nombre,Descripcion,Tipo,PeriodicidadDias,UltimaEjecucion,ProximaEjecucion,AccesorioId,AccesorioGrupoID,EquipoId,CubaId,VehiculoId,FrecuenciaValor,FrecuenciaUnidad,DuracionEstimadaMinutos,Activo FROM gaillard.PlanMantenimiento WHERE PlanId={id.Value}").ToListAsync();
            var x=rows.SingleOrDefault();if(x is null)return null;
            model=new PlanMantenimientoFormViewModel { PlanId=x.PlanId,Nombre=x.Nombre,Descripcion=x.Descripcion,Tipo=x.Tipo,PeriodicidadDias=x.PeriodicidadDias,UltimaEjecucion=x.UltimaEjecucion,ProximaEjecucion=x.ProximaEjecucion,AccesorioId=x.AccesorioId,AccesorioGrupoID=x.AccesorioGrupoID,EquipoId=x.EquipoId,CubaId=x.CubaId,VehiculoId=x.VehiculoId,FrecuenciaValor=x.FrecuenciaValor,FrecuenciaUnidad=x.FrecuenciaUnidad,DuracionEstimadaMinutos=x.DuracionEstimadaMinutos,Activo=x.Activo };
        }
        return await PrepareFormAsync(model);
    }

    public async Task<PlanMantenimientoFormViewModel> PrepareFormAsync(PlanMantenimientoFormViewModel model)
    {
        model.Accesorios=await OptionsAsync("SELECT a.ID,CONCAT(COALESCE(NULLIF(a.Nombre,N''),N'Accesorio '+CONVERT(nvarchar(20),a.ID)),N' (',ta.TipoAccesorio,N')') Texto FROM gaillard.Accesorio a LEFT JOIN gaillard.TipoAccesorio ta ON ta.ID=a.TipoAccesorioID ORDER BY a.Nombre",model.AccesorioId);
        model.Equipos=await OptionsAsync("SELECT e.Id,CONCAT(COALESCE(i.Nombre,CONCAT(COALESCE(v.MatriculaVehiculo,N'Vehículo'),N' (vehículo)')),N' / ',e.Codigo,N' - ',e.Nombre) Texto FROM gaillard.Equipo e LEFT JOIN gaillard.Instalacion i ON i.Id=e.InstalacionId LEFT JOIN gaillard.Vehiculo v ON v.ID=e.VehiculoId WHERE e.Activo=1 ORDER BY COALESCE(i.Nombre,v.MatriculaVehiculo),e.Nombre",model.EquipoId);
        model.Cubas=await OptionsAsync("SELECT ID,CONCAT(N'Cisterna ',MatriculaCuba) Texto FROM gaillard.Cuba WHERE ISNULL(Baja,0)=0 ORDER BY MatriculaCuba",model.CubaId);
        model.Vehiculos=await OptionsAsync("SELECT ID,CONCAT(N'Vehículo ',MatriculaVehiculo) Texto FROM gaillard.Vehiculo WHERE ISNULL(Baja,0)=0 ORDER BY MatriculaVehiculo",model.VehiculoId);
        model.GruposAccesorio=await OptionsAsync("SELECT ag.ID,CONCAT(COALESCE(NULLIF(a.Nombre,N''),N'Accesorio '+CONVERT(nvarchar(20),a.ID)),N' - Cuba ',c.MatriculaCuba,N' - Comp. ',cp.Numero,N' - ',s.LadoCubaNombre,N' - ',tg.NombreGrupo,N' - Cantidad ',ag.Cantidad) Texto FROM gaillard.AccesorioGrupo ag JOIN gaillard.Accesorio a ON a.ID=ag.AccesorioID JOIN gaillard.Grupo g ON g.ID=ag.GrupoID LEFT JOIN gaillard.Cuba c ON c.ID=g.CubaID LEFT JOIN gaillard.Compartimento cp ON cp.ID=g.CompartimentoID LEFT JOIN gaillard.Situacion s ON s.ID=g.SituacionID LEFT JOIN gaillard.TipoGrupo tg ON tg.ID=g.TipoGrupoID WHERE (@p0 IS NULL OR ag.AccesorioID=@p0) ORDER BY a.Nombre,c.MatriculaCuba,cp.Numero",model.AccesorioGrupoID,model.AccesorioId);
        return model;
    }

    public async Task<IReadOnlyDictionary<string,string>> ValidateAsync(PlanMantenimientoFormViewModel m)
    {
        var errors=new Dictionary<string,string>();
        if(m.PeriodicidadDias<=0) errors["PeriodicidadDias"]="La periodicidad debe ser mayor que cero.";
        if(m.FrecuenciaValor.HasValue != !string.IsNullOrWhiteSpace(m.FrecuenciaUnidad)) errors["FrecuenciaUnidad"]="Indica tanto el valor como la unidad de frecuencia.";
        if(m.UltimaEjecucion.HasValue && m.ProximaEjecucion.HasValue && m.ProximaEjecucion<m.UltimaEjecucion) errors["ProximaEjecucion"]="La próxima ejecución no puede ser anterior a la última ejecución.";
        var selected=(m.AccesorioId.HasValue?1:0)+(m.EquipoId.HasValue?1:0)+(m.CubaId.HasValue?1:0)+(m.VehiculoId.HasValue?1:0); if(selected!=1) errors["EquipoId"]="El plan debe estar asociado a un único equipo, cisterna, vehículo o accesorio.";
        if(m.AccesorioId.HasValue && await CountAsync($"SELECT COUNT(1) Value FROM gaillard.Accesorio WHERE ID={m.AccesorioId}") == 0) errors["AccesorioId"]="Selecciona un accesorio válido.";
        if(m.EquipoId.HasValue && await CountAsync($"SELECT COUNT(1) Value FROM gaillard.Equipo WHERE Id={m.EquipoId} AND Activo=1") == 0) errors["EquipoId"]="Selecciona un equipo activo válido.";
        if(m.CubaId.HasValue && await CountAsync($"SELECT COUNT(1) Value FROM gaillard.Cuba WHERE ID={m.CubaId} AND ISNULL(Baja,0)=0") == 0) errors["CubaId"]="Selecciona una cisterna activa válida.";
        if(m.VehiculoId.HasValue && await CountAsync($"SELECT COUNT(1) Value FROM gaillard.Vehiculo WHERE ID={m.VehiculoId} AND ISNULL(Baja,0)=0") == 0) errors["VehiculoId"]="Selecciona un vehículo activo válido.";
        if(m.AccesorioGrupoID.HasValue && (!m.AccesorioId.HasValue || await CountAsync($"SELECT COUNT(1) Value FROM gaillard.AccesorioGrupo WHERE ID={m.AccesorioGrupoID} AND AccesorioID={m.AccesorioId}")==0)) errors["AccesorioGrupoID"]="La ubicación seleccionada debe pertenecer al accesorio elegido.";
        return errors;
    }

    public async Task<bool> SaveAsync(PlanMantenimientoFormViewModel m)
    {
        var name=m.Nombre.Trim();var description=Normalize(m.Descripcion);var type=Normalize(m.Tipo);var unit=Normalize(m.FrecuenciaUnidad);
        if(m.PlanId==0) { await _db.Database.ExecuteSqlInterpolatedAsync($@"INSERT INTO gaillard.PlanMantenimiento(AccesorioId,PeriodicidadDias,UltimaEjecucion,ProximaEjecucion,AccesorioGrupoID,EquipoId,CubaId,VehiculoId,Nombre,Descripcion,Tipo,FrecuenciaValor,FrecuenciaUnidad,DuracionEstimadaMinutos,Activo) VALUES({m.AccesorioId},{m.PeriodicidadDias},{m.UltimaEjecucion},{m.ProximaEjecucion},{m.AccesorioGrupoID},{m.EquipoId},{m.CubaId},{m.VehiculoId},{name},{description},{type},{m.FrecuenciaValor},{unit},{m.DuracionEstimadaMinutos},{m.Activo})");return true; }
        return await _db.Database.ExecuteSqlInterpolatedAsync($@"UPDATE gaillard.PlanMantenimiento SET AccesorioId={m.AccesorioId},PeriodicidadDias={m.PeriodicidadDias},UltimaEjecucion={m.UltimaEjecucion},ProximaEjecucion={m.ProximaEjecucion},AccesorioGrupoID={m.AccesorioGrupoID},EquipoId={m.EquipoId},CubaId={m.CubaId},VehiculoId={m.VehiculoId},Nombre={name},Descripcion={description},Tipo={type},FrecuenciaValor={m.FrecuenciaValor},FrecuenciaUnidad={unit},DuracionEstimadaMinutos={m.DuracionEstimadaMinutos},Activo={m.Activo} WHERE PlanId={m.PlanId}")>0;
    }
    public async Task<bool> DeleteAsync(int id)=>await _db.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM gaillard.PlanMantenimiento WHERE PlanId={id}")>0;
    private async Task<int> CountAsync(FormattableString sql)=>await _db.Database.SqlQuery<int>(sql).SingleAsync();
    private async Task<IReadOnlyList<SelectListItem>> OptionsAsync(string sql,int? selected,int? filter=null)
    {
        var rows=sql.Contains("@p0",StringComparison.Ordinal)
            ?await _db.Database.SqlQueryRaw<Option>(sql,(object?)filter??DBNull.Value).ToListAsync()
            :await _db.Database.SqlQueryRaw<Option>(sql).ToListAsync();
        return rows.Select(x=>new SelectListItem(x.Texto,x.Id.ToString(),x.Id==selected)).ToList();
    }
    private static string? Normalize(string? s)=>string.IsNullOrWhiteSpace(s)?null:s.Trim();
    private sealed class Option { public int Id{get;set;} public string Texto{get;set;}=string.Empty; }
    private sealed class PlanFormRow { public int PlanId{get;set;} public string Nombre{get;set;}=string.Empty;public string? Descripcion{get;set;}public string? Tipo{get;set;}public int PeriodicidadDias{get;set;}public DateTime? UltimaEjecucion{get;set;}public DateTime? ProximaEjecucion{get;set;}public int? AccesorioId{get;set;}public int? AccesorioGrupoID{get;set;}public int? EquipoId{get;set;}public int? CubaId{get;set;}public int? VehiculoId{get;set;}public int? FrecuenciaValor{get;set;}public string? FrecuenciaUnidad{get;set;}public int? DuracionEstimadaMinutos{get;set;}public bool Activo{get;set;} }
    private sealed class PlanRow { public int PlanId{get;set;} public string Nombre{get;set;}=string.Empty;public string? Descripcion{get;set;}public string? Tipo{get;set;}public int PeriodicidadDias{get;set;}public DateTime? UltimaEjecucion{get;set;}public DateTime? ProximaEjecucion{get;set;}public int? AccesorioId{get;set;}public int? AccesorioGrupoID{get;set;}public int? EquipoId{get;set;}public int? FrecuenciaValor{get;set;}public string? FrecuenciaUnidad{get;set;}public int? DuracionEstimadaMinutos{get;set;}public bool Activo{get;set;}public string? EquipoNombre{get;set;}public string? AccesorioNombre{get;set;}public string? AccesorioTipo{get;set;}public string? AccesorioGrupoTexto{get;set;} }
}
