using CasaGaillard.Models;
using CasaGaillard.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CasaGaillard.Areas.Mantenimiento.Controllers
{
    [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento + "," + AppRoles.Consulta)]
    public class CostesMantenimientoController : Controller
    {
        private readonly GaillardEntities db = new GaillardEntities();

        public async Task<ActionResult> Index(DateTime? fechaDesde, DateTime? fechaHasta, int? instalacionId, int? equipoId)
        {
            var model = new CostesMantenimientoViewModel
            {
                FechaDesde = fechaDesde,
                FechaHasta = fechaHasta,
                InstalacionId = instalacionId,
                EquipoId = equipoId
            };

            if (fechaDesde.HasValue && fechaHasta.HasValue && fechaHasta.Value.Date < fechaDesde.Value.Date)
                ModelState.AddModelError("FechaHasta", "La fecha hasta no puede ser anterior a la fecha desde.");

            model.Instalaciones = ObtenerInstalaciones(instalacionId);
            model.Equipos = ObtenerEquipos(instalacionId, equipoId);
            if (ModelState.IsValid)
            {
                var parametros = new object[] { fechaDesde, fechaHasta, instalacionId, equipoId };
                model.Resumen = await db.Database.SqlQuery<CosteActivoResumenViewModel>(SqlResumen, parametros).ToListAsync();
                model.Intervenciones = await db.Database.SqlQuery<CosteIntervencionViewModel>(SqlDetalle, parametros).ToListAsync();
            }

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }

        private IEnumerable<SelectListItem> ObtenerInstalaciones(int? seleccionada)
        {
            return db.Database.SqlQuery<Opcion>(@"SELECT Id AS Valor, Codigo + N' - ' + Nombre AS Texto
                FROM gaillard.Instalacion ORDER BY Nombre").ToList()
                .Select(x => new SelectListItem { Value = x.Valor.ToString(), Text = x.Texto, Selected = seleccionada == x.Valor });
        }

        private IEnumerable<SelectListItem> ObtenerEquipos(int? instalacionId, int? seleccionado)
        {
            return db.Database.SqlQuery<Opcion>(@"SELECT e.Id AS Valor, e.Codigo + N' - ' + e.Nombre AS Texto
                FROM gaillard.Equipo e
                WHERE (@p0 IS NULL OR e.InstalacionId=@p0)
                ORDER BY e.Nombre", instalacionId).ToList()
                .Select(x => new SelectListItem { Value = x.Valor.ToString(), Text = x.Texto, Selected = seleccionado == x.Valor });
        }

        private const string Cte = @"WITH Repuestos AS
            (
                SELECT IntervencionId, SUM(Cantidad * CosteUnitario) AS CosteRepuestos,
                    SUM(CASE WHEN CosteUnitario IS NULL THEN 1 ELSE 0 END) AS RepuestosSinCoste
                FROM dbo.IntervencionRepuesto GROUP BY IntervencionId
            ), Costes AS
            (
                SELECT v.Id, v.TareaMantenimientoId, t.Codigo AS TareaCodigo, t.Titulo AS TareaTitulo,
                    COALESCE(t.InstalacionId, e.InstalacionId) AS InstalacionId, i.Codigo AS InstalacionCodigo,
                    i.Nombre AS InstalacionNombre, t.EquipoId, e.Codigo AS EquipoCodigo, e.Nombre AS EquipoNombre,
                    v.Tecnico, v.FechaInicio, v.FechaFin, v.Descripcion, v.HorasEmpleadas,
                    COALESCE(v.CosteManoObra, 0) AS CosteManoObra, COALESCE(r.CosteRepuestos, 0) AS CosteRepuestos,
                    COALESCE(r.RepuestosSinCoste, 0) AS RepuestosSinCoste
                FROM dbo.Intervencion v
                INNER JOIN dbo.TareasMantenimiento t ON t.ID=v.TareaMantenimientoId
                LEFT JOIN gaillard.Equipo e ON e.Id=t.EquipoId
                LEFT JOIN gaillard.Instalacion i ON i.Id=COALESCE(t.InstalacionId, e.InstalacionId)
                LEFT JOIN Repuestos r ON r.IntervencionId=v.Id
                WHERE (@p0 IS NULL OR v.FechaInicio >= @p0)
                  AND (@p1 IS NULL OR v.FechaInicio < DATEADD(day, 1, CONVERT(date, @p1)))
                  AND (@p2 IS NULL OR COALESCE(t.InstalacionId, e.InstalacionId)=@p2)
                  AND (@p3 IS NULL OR t.EquipoId=@p3)
            ) ";

        private static readonly string SqlResumen = Cte + @"SELECT InstalacionId, InstalacionCodigo,
                COALESCE(InstalacionNombre, N'(Sin instalación)') AS InstalacionNombre,
                EquipoId, EquipoCodigo, COALESCE(EquipoNombre, N'(Sin equipo)') AS EquipoNombre,
                COUNT(1) AS NumeroIntervenciones, SUM(COALESCE(HorasEmpleadas, 0)) AS HorasEmpleadas,
                SUM(CosteManoObra) AS CosteManoObra, SUM(CosteRepuestos) AS CosteRepuestos, SUM(RepuestosSinCoste) AS RepuestosSinCoste,
                SUM(CosteManoObra + CosteRepuestos) AS CosteTotal
            FROM Costes
            GROUP BY InstalacionId, InstalacionCodigo, InstalacionNombre, EquipoId, EquipoCodigo, EquipoNombre
            ORDER BY InstalacionNombre, EquipoNombre";

        private static readonly string SqlDetalle = Cte + @"SELECT Id, TareaMantenimientoId, TareaCodigo, TareaTitulo,
                InstalacionNombre, EquipoCodigo, EquipoNombre, Tecnico, FechaInicio, FechaFin, Descripcion,
                HorasEmpleadas, CosteManoObra, CosteRepuestos, RepuestosSinCoste, CosteManoObra + CosteRepuestos AS CosteTotal
            FROM Costes ORDER BY FechaInicio DESC, Id DESC";

        private sealed class Opcion
        {
            public Opcion() { }
            public int Valor { get; set; }
            public string Texto { get; set; }
        }
    }
}
