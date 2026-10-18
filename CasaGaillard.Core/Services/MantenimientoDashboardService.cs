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
            (SELECT c.ID CubaId,c.MatriculaCuba,r.ValidaHasta,r.DescripcionProxima,
            ROW_NUMBER() OVER(PARTITION BY c.ID ORDER BY r.ValidaHasta DESC,r.ID DESC) AS Orden
            FROM gaillard.Cuba c JOIN gaillard.Revision r ON r.CubaID=c.ID
            WHERE COALESCE(c.Baja,0)=0 AND r.ValidaHasta IS NOT NULL)
            SELECT CubaId,MatriculaCuba,ValidaHasta,DescripcionProxima FROM RevisionesOrdenadas
            WHERE Orden=1 AND ValidaHasta>DATEADD(month,-2,GETDATE()) AND ValidaHasta<DATEADD(month,2,GETDATE())
            ORDER BY ValidaHasta").ToListAsync();
        var vehiculos = await _db.Database.SqlQueryRaw<DashboardRevisionVehiculoViewModel>(@"WITH Ultimas AS
            (SELECT r.VehiculoID,r.TipoRevisionID,MAX(r.Caducidad) Caducidad FROM gaillard.RevisionVehiculo r
            JOIN gaillard.Vehiculo v ON v.ID=r.VehiculoID WHERE COALESCE(v.Baja,0)=0
            GROUP BY r.VehiculoID,r.TipoRevisionID)
            SELECT COALESCE(v.MatriculaVehiculo,'') MatriculaVehiculo,COALESCE(t.Revision,'') TipoRevision,u.Caducidad
            FROM Ultimas u JOIN gaillard.Vehiculo v ON v.ID=u.VehiculoID JOIN gaillard.TipoRevision t ON t.ID=u.TipoRevisionID
            WHERE u.Caducidad IS NOT NULL AND u.Caducidad<=DATEADD(month,2,CONVERT(date,GETDATE())) ORDER BY u.Caducidad").ToListAsync();
        var tareas = await _db.Database.SqlQueryRaw<TareaMantenimientoResumenViewModel>(@"SELECT TOP(5) ID,Codigo,Titulo,Ubicacion,Equipo,Prioridad,Estado,AsignadaA,FechaCreacion
            FROM dbo.TareasMantenimiento ORDER BY FechaCreacion DESC").ToListAsync();
        return new MantenimientoDashboardViewModel { RevisionesCubas=cubas, RevisionesVehiculos=vehiculos, Tareas=tareas };
    }
}
