using CasaGaillard.Core.Data;
using CasaGaillard.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CasaGaillard.Core.Services;

public sealed class MantenimientoDashboardService : IMantenimientoDashboardService
{
    private readonly AppDbContext _db;
    public MantenimientoDashboardService(AppDbContext db) => _db = db;

    public async Task<MantenimientoDashboardViewModel> GetAsync()
    {
        var cubas = await _db.Database.SqlQueryRaw<DashboardRevisionCubaViewModel>(@"WITH RevisionesOrdenadas AS
            (SELECT r.ID RevisionId,c.ID CubaId,c.MatriculaCuba,r.ValidaHasta,r.DescripcionProxima,
            r.FechaReservada,
            ROW_NUMBER() OVER(PARTITION BY c.ID ORDER BY r.ValidaHasta DESC,r.ID DESC) AS Orden
            FROM gaillard.Cuba c JOIN gaillard.Revision r ON r.CubaID=c.ID
            WHERE COALESCE(c.Baja,0)=0 AND r.ValidaHasta IS NOT NULL)
            SELECT RevisionId,CubaId,MatriculaCuba,ValidaHasta,DescripcionProxima,FechaReservada FROM RevisionesOrdenadas
            WHERE Orden=1 AND ValidaHasta>DATEADD(month,-2,GETDATE()) AND ValidaHasta<DATEADD(month,2,GETDATE())
            ORDER BY ValidaHasta").ToListAsync();
        var vehiculos = await _db.Database.SqlQueryRaw<DashboardRevisionVehiculoViewModel>(@"WITH Ultimas AS
            (SELECT r.ID RevisionId,r.VehiculoID,r.TipoRevisionID,r.Caducidad,r.FechaReservada,
            ROW_NUMBER() OVER(PARTITION BY r.VehiculoID,r.TipoRevisionID ORDER BY r.Caducidad DESC,r.FechaRevision DESC,r.ID DESC) AS Orden
            FROM gaillard.RevisionVehiculo r JOIN gaillard.Vehiculo v ON v.ID=r.VehiculoID WHERE COALESCE(v.Baja,0)=0)
            SELECT u.RevisionId,COALESCE(v.MatriculaVehiculo,'') MatriculaVehiculo,COALESCE(t.Revision,'') TipoRevision,u.Caducidad,u.FechaReservada
            FROM Ultimas u JOIN gaillard.Vehiculo v ON v.ID=u.VehiculoID JOIN gaillard.TipoRevision t ON t.ID=u.TipoRevisionID
            WHERE u.Orden=1 AND u.Caducidad IS NOT NULL AND u.Caducidad>=DATEADD(month,-2,CONVERT(date,GETDATE()))
              AND u.Caducidad<=DATEADD(month,2,CONVERT(date,GETDATE())) ORDER BY u.Caducidad").ToListAsync();
        var tareas = await _db.Database.SqlQueryRaw<TareaMantenimientoResumenViewModel>(@"SELECT TOP(8) ID,Codigo,Titulo,Ubicacion,Equipo,Prioridad,Estado,AsignadaA,FechaCreacion,FechaProgramada
            FROM dbo.TareasMantenimiento WHERE Estado IS NULL OR Estado<>N'Cerrada'
            ORDER BY CASE WHEN FechaProgramada IS NULL THEN 1 ELSE 0 END,FechaProgramada,FechaCreacion DESC").ToListAsync();
        var metricas = await _db.Database.SqlQueryRaw<DashboardMetricasViewModel>(@"SELECT
            (SELECT COUNT(1) FROM dbo.TareasMantenimiento WHERE ISNULL(Estado,N'')<>N'Cerrada') AS TareasPendientes,
            (SELECT COUNT(1) FROM dbo.TareasMantenimiento WHERE ISNULL(Estado,N'')<>N'Cerrada' AND FechaProgramada<CONVERT(date,GETDATE())) AS TareasVencidas,
            (SELECT COUNT(1) FROM dbo.TareasMantenimiento WHERE Estado=N'Cerrada' AND FechaActualizacion>=DATEFROMPARTS(YEAR(GETDATE()),MONTH(GETDATE()),1)) AS TareasCompletadasMes,
            COALESCE((SELECT SUM(COALESCE(v.CosteManoObra,0)+COALESCE(r.CosteRepuestos,0)) FROM dbo.Intervencion v LEFT JOIN
                (SELECT IntervencionId,SUM(Cantidad*CosteUnitario) CosteRepuestos FROM dbo.IntervencionRepuesto GROUP BY IntervencionId) r ON r.IntervencionId=v.Id
                WHERE v.FechaInicio>=DATEFROMPARTS(YEAR(GETDATE()),MONTH(GETDATE()),1) AND v.FechaInicio<DATEADD(month,1,DATEFROMPARTS(YEAR(GETDATE()),MONTH(GETDATE()),1))),0) AS CosteMes,
            COALESCE((SELECT SUM(COALESCE(v.CosteManoObra,0)+COALESCE(r.CosteRepuestos,0)) FROM dbo.Intervencion v LEFT JOIN
                (SELECT IntervencionId,SUM(Cantidad*CosteUnitario) CosteRepuestos FROM dbo.IntervencionRepuesto GROUP BY IntervencionId) r ON r.IntervencionId=v.Id
                WHERE v.FechaInicio>=DATEFROMPARTS(YEAR(GETDATE()),1,1) AND v.FechaInicio<DATEFROMPARTS(YEAR(GETDATE())+1,1,1)),0) AS CosteAnio").ToListAsync();
        var resumen = metricas.Single();
        return new MantenimientoDashboardViewModel { RevisionesCubas=cubas, RevisionesVehiculos=vehiculos, Tareas=tareas,
            TareasPendientes=resumen.TareasPendientes,TareasVencidas=resumen.TareasVencidas,TareasCompletadasMes=resumen.TareasCompletadasMes,
            CosteMes=resumen.CosteMes,CosteAnio=resumen.CosteAnio };
    }
}

public sealed class DashboardMetricasViewModel
{
    public int TareasPendientes { get; set; }
    public int TareasVencidas { get; set; }
    public int TareasCompletadasMes { get; set; }
    public decimal CosteMes { get; set; }
    public decimal CosteAnio { get; set; }
}
