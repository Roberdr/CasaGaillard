using CasaGaillard.Core.Data;
using CasaGaillard.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CasaGaillard.Core.Services;

public sealed class CostesMantenimientoService : ICostesMantenimientoService
{
    private readonly AppDbContext _db;
    public CostesMantenimientoService(AppDbContext db)=>_db=db;

    private const string Cte=@"WITH Repuestos AS (
        SELECT IntervencionId,SUM(Cantidad*CosteUnitario) CosteRepuestos,SUM(CASE WHEN CosteUnitario IS NULL THEN 1 ELSE 0 END) RepuestosSinCoste
        FROM dbo.IntervencionRepuesto GROUP BY IntervencionId
    ), Costes AS (
        SELECT v.Id,v.TareaMantenimientoId,t.Codigo TareaCodigo,t.Titulo TareaTitulo,COALESCE(t.InstalacionId,e.InstalacionId) InstalacionId,
          i.Codigo InstalacionCodigo,i.Nombre InstalacionNombre,t.EquipoId,e.Codigo EquipoCodigo,e.Nombre EquipoNombre,v.Tecnico,v.FechaInicio,v.FechaFin,
          v.Descripcion,v.HorasEmpleadas,COALESCE(v.CosteManoObra,0) CosteManoObra,COALESCE(r.CosteRepuestos,0) CosteRepuestos,COALESCE(r.RepuestosSinCoste,0) RepuestosSinCoste
        FROM dbo.Intervencion v JOIN dbo.TareasMantenimiento t ON t.ID=v.TareaMantenimientoId
        LEFT JOIN gaillard.Equipo e ON e.Id=t.EquipoId LEFT JOIN gaillard.Instalacion i ON i.Id=COALESCE(t.InstalacionId,e.InstalacionId)
        LEFT JOIN Repuestos r ON r.IntervencionId=v.Id
        WHERE (@p0 IS NULL OR v.FechaInicio>=@p0) AND (@p1 IS NULL OR v.FechaInicio<DATEADD(day,1,CONVERT(date,@p1)))
          AND (@p2 IS NULL OR COALESCE(t.InstalacionId,e.InstalacionId)=@p2) AND (@p3 IS NULL OR t.EquipoId=@p3)
    ) ";

    public async Task<CostesMantenimientoViewModel> GetAsync(DateTime? desde,DateTime? hasta,int? instalacionId,int? equipoId)
    {
        var model=new CostesMantenimientoViewModel{FechaDesde=desde,FechaHasta=hasta,InstalacionId=instalacionId,EquipoId=equipoId};
        model.Instalaciones=(await _db.Database.SqlQueryRaw<Option>("SELECT Id Valor,CONCAT(Codigo,N' - ',Nombre) Texto FROM gaillard.Instalacion ORDER BY Nombre").ToListAsync()).Select(x=>new SelectListItem(x.Texto,x.Valor.ToString(),x.Valor==instalacionId)).ToList();
        model.Equipos=(await _db.Database.SqlQueryRaw<Option>("SELECT e.Id Valor,CONCAT(e.Codigo,N' - ',e.Nombre) Texto FROM gaillard.Equipo e WHERE (@p0 IS NULL OR e.InstalacionId=@p0) ORDER BY e.Nombre",(object?)instalacionId??DBNull.Value).ToListAsync()).Select(x=>new SelectListItem(x.Texto,x.Valor.ToString(),x.Valor==equipoId)).ToList();
        object[] args=[(object?)desde??DBNull.Value,(object?)hasta??DBNull.Value,(object?)instalacionId??DBNull.Value,(object?)equipoId??DBNull.Value];
        model.Resumen=await _db.Database.SqlQueryRaw<CosteActivoResumenViewModel>(Cte+@"SELECT InstalacionId,InstalacionCodigo,COALESCE(InstalacionNombre,N'(Sin instalación)') InstalacionNombre,
          EquipoId,EquipoCodigo,COALESCE(EquipoNombre,N'(Sin equipo)') EquipoNombre,COUNT(1) NumeroIntervenciones,SUM(COALESCE(HorasEmpleadas,0)) HorasEmpleadas,
          SUM(CosteManoObra) CosteManoObra,SUM(CosteRepuestos) CosteRepuestos,SUM(RepuestosSinCoste) RepuestosSinCoste,SUM(CosteManoObra+CosteRepuestos) CosteTotal
          FROM Costes GROUP BY InstalacionId,InstalacionCodigo,InstalacionNombre,EquipoId,EquipoCodigo,EquipoNombre ORDER BY InstalacionNombre,EquipoNombre",args).ToListAsync();
        model.Intervenciones=await _db.Database.SqlQueryRaw<CosteIntervencionViewModel>(Cte+@"SELECT Id,TareaMantenimientoId,TareaCodigo,TareaTitulo,COALESCE(InstalacionNombre,N'(Sin instalación)') InstalacionNombre,
          EquipoCodigo,EquipoNombre,Tecnico,FechaInicio,FechaFin,Descripcion,HorasEmpleadas,CosteManoObra,CosteRepuestos,RepuestosSinCoste,CosteManoObra+CosteRepuestos CosteTotal
          FROM Costes ORDER BY FechaInicio DESC,Id DESC",args).ToListAsync();
        return model;
    }
    private sealed class Option{public int Valor{get;set;}public string Texto{get;set;}=string.Empty;}
}
