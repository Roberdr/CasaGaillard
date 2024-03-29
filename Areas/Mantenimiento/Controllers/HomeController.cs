﻿//using Microsoft.AspNetCore.Mvc;
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

    [RouteArea("Mantenimiento")]
    public class HomeController : Controller
    {
        private readonly GaillardEntities db = new GaillardEntities();

        
        public class UltimasRevisiones                  // Objeto para pasar las últimas revisiones a la vista
        {
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

            var viewModel = from c in db.Cubas
                       join r in db.Revisiones on c.ID equals r.CubaID into gc
                       from grupo in gc
                       //let Valida = grupo.ValidaHasta
                       orderby grupo.ValidaHasta descending
                       where grupo.ValidaHasta == gc.Max(x => x.ValidaHasta)
                       select new UltimasRevisiones()
                       { 
                           MatriculaCuba = c.MatriculaCuba, 
                           ValidaHasta = grupo.ValidaHasta,
                           DescripcionProxima = grupo.DescripcionProxima
                       };

            var viewModel1 = from z in viewModel
                             orderby z.ValidaHasta
                             where (z.ValidaHasta > fechaInicio) && (z.ValidaHasta < fechaFinal)
                             select z;

            ViewBag.Revis = await viewModel1.ToListAsync();

            var viewModelV = db.RevisionesVehiculo
                .Include(i => i.Vehiculo)
                .GroupBy(r => new { r.Vehiculo.MatriculaVehiculo, r.TipoRevision})
                .Select(s => new UltimasRevisionesVehiculos()
                {
                    MatriculaVehiculo = s.Key.MatriculaVehiculo,
                    Caducidad = s.Max(m => m.Caducidad),
                    TipoRevision = s.Key.TipoRevision.Revision,
                 })
                .OrderBy(r => r.Caducidad)
                .Where(r => r.Caducidad > fechaInicio && r.Caducidad < fechaFinal);

            

            ViewBag.RevisV = await viewModelV.ToListAsync();

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