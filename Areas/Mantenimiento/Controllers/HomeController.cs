//using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;
using System.Runtime;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using CasaGaillard.Models;
using System;
using CasaGaillard.Models.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Globalization;
using System.Data;

namespace CasaGaillard.Areas.Mantenimiento.Controllers
{

    [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento + "," + AppRoles.Consulta)]
    [RouteArea("Mantenimiento")]
    public class HomeController : Controller
    {
        private readonly GaillardEntities db = new GaillardEntities();

        
        public class UltimasRevisiones                  // Objeto para pasar las últimas revisiones a la vista
        {
            public int CubaID { get; set; }
            public string MatriculaCuba { get; set; }

            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:dd-mm-yyyy}")]
            public DateTime? ValidaHasta { get; set; }
            public string DescripcionProxima { get; set; }
        }

        public class UltimasRevisionesVehiculos
        {
            public string MatriculaVehiculo { get; set; }

            public string TipoRevision { get; set; }

            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:dd-mm-yyyy}")]
            public DateTime? Caducidad { get; set; }

            //public string Cuba { get; set; }
        }

        public async Task<ActionResult> Index()
        {
            var fechaInicio = DateTime.Now.AddMonths(-2);
            var fechaFinal = DateTime.Now.AddMonths(2);
            var revisionesCubas = new List<UltimasRevisiones>();
            var revisionesVehiculos = new List<UltimasRevisionesVehiculos>();
            var tareasRecientes = new List<TareaMantenimientoResumenViewModel>();

            var viewModel = from c in db.Cubas
                            join r in db.Revisiones on c.ID equals r.CubaID into gc
                            from grupo in gc.DefaultIfEmpty()
                            where (c.Baja != true) && grupo != null && grupo.ValidaHasta.HasValue
                            group new { c, grupo } by new { c.ID, c.MatriculaCuba } into g
                            let ultimo = g.OrderByDescending(x => x.grupo.ValidaHasta).FirstOrDefault()
                            where ultimo != null
                            select new UltimasRevisiones()
                            {
                                CubaID = g.Key.ID,
                                MatriculaCuba = g.Key.MatriculaCuba,
                                ValidaHasta = ultimo.grupo.ValidaHasta,
                                DescripcionProxima = ultimo.grupo.DescripcionProxima
                            };

            var viewModel1 = from z in viewModel
                             where z.ValidaHasta.HasValue
                                && z.ValidaHasta > fechaInicio
                                && z.ValidaHasta < fechaFinal
                             orderby z.ValidaHasta
                             select z;

            revisionesCubas = await viewModel1.ToListAsync();

            // Obtener la última caducidad por vehículo y tipo, sin filtrar por fecha antes del GroupBy
            // (si filtramos antes podemos perder la última revisión si queda fuera del rango)
            var viewModelV = db.RevisionesVehiculo
                .Include(i => i.Vehiculo)
                .Include(i => i.TipoRevision)
                .Where(r => r.Vehiculo != null && (r.Vehiculo.Baja != true) && r.TipoRevision != null)
                .GroupBy(r => new { r.Vehiculo.MatriculaVehiculo, r.TipoRevisionID })
                .Select(s => new UltimasRevisionesVehiculos()
                {
                    MatriculaVehiculo = s.Key.MatriculaVehiculo,
                    Caducidad = s.Max(m => m.Caducidad),
                    TipoRevision = s.Select(m => m.TipoRevision != null ? m.TipoRevision.Revision : string.Empty).FirstOrDefault(),
                 })
                .OrderBy(r => r.Caducidad)
                .Where(r => r.Caducidad > fechaInicio && r.Caducidad < fechaFinal);

            

            revisionesVehiculos = await viewModelV.ToListAsync();

            using (var tareasDb = new TareasMantenimientoContext())
            {
                tareasRecientes = await tareasDb.TareasMantenimiento
                    .AsNoTracking()
                    .OrderByDescending(t => t.FechaCreacion)
                    .Take(5)
                    .Select(t => new TareaMantenimientoResumenViewModel
                    {
                        ID = t.ID,
                        Codigo = t.Codigo,
                        Titulo = t.Titulo,
                        Ubicacion = t.Ubicacion,
                        Equipo = t.Equipo,
                        Prioridad = t.Prioridad,
                        Estado = t.Estado,
                        AsignadaA = t.AsignadaA,
                        FechaCreacion = t.FechaCreacion
                    })
                    .ToListAsync();
            }

            ViewBag.Revis = revisionesCubas;
            ViewBag.RevisV = revisionesVehiculos;
            ViewBag.Tareas = tareasRecientes;

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
