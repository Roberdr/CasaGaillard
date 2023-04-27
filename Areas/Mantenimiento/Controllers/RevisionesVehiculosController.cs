using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CasaGaillard.Models;
using CasaGaillard.Models.ViewModels;
using Microsoft.Ajax.Utilities;
using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Areas.Mantenimiento.Controllers
{
    [Authorize]
    public class RevisionesVehiculosController : Controller
    {
        private readonly GaillardEntities db = new GaillardEntities();

        public class Revisiones                  // Objeto para pasar las últimas revisiones a la vista
        {
            public string MatriculaVehiculo { get; set; }

            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:dd-mm-yyyy}")]
            public DateTime? Caducidad { get; set; }
            public string TipoRevision { get; set; }
        }

        // GET: RevisionesVehiculos
        public async Task<ActionResult> Index()
        {
            var revisionesVehiculos = db.RevisionesVehiculo
                .Include(r => r.Vehiculo)
                .OrderBy(r => r.Vehiculo.MatriculaVehiculo)
                .GroupBy(r => r.Vehiculo.MatriculaVehiculo);

           
            return View(await revisionesVehiculos.ToListAsync());
        }

        // GET: RevisionesVehiculos/ProximasRevisiones
        public ActionResult ProximasRevisiones()
        {
            var result = from r in db.RevisionesVehiculo
                         join v in db.Vehiculos on r.VehiculoID equals v.ID
                         join t in db.TiposRevision on r.TipoRevisionID equals t.ID
                         group new { v.MatriculaVehiculo, t.Revision, r.Caducidad } by new { v.MatriculaVehiculo, t.Revision } into g
                         orderby g.Max(x => x.Caducidad)
                         select new Revisiones()
                         { 
                             MatriculaVehiculo = g.Key.MatriculaVehiculo, 
                             TipoRevision = g.Key.Revision, 
                             Caducidad = g.Max(x => x.Caducidad) 
                         };

            ViewBag.res = result.ToList();
            return View();
        }

        // GET: RevisionesVehiculos/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RevisionVehiculo revisionVehiculo = await db.RevisionesVehiculo.FindAsync(id);
            if (revisionVehiculo == null)
            {
                return HttpNotFound();
            }
            return View(revisionVehiculo);
        }

        // GET: RevisionesVehiculos/Create
        public ActionResult Create()
        {
            ViewBag.VehiculoID = new SelectList(db.Vehiculos.OrderBy(o => o.MatriculaVehiculo), "ID", "MatriculaVehiculo");
            ViewBag.TipoRevisionID = new SelectList(db.TiposRevision.OrderBy(o => o.Revision), "ID", "Revision");
            return View();
        }

        // POST: RevisionesVehiculos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,VehiculoID,TipoRevisionID,FechaRevision,Detalles,Ejecutor,Caducidad")] RevisionVehiculo revisionVehiculo)
        {
            if (ModelState.IsValid)
            {
                //revisionVehiculo.FechaRevision .AddMonths(30);
                db.RevisionesVehiculo.Add(revisionVehiculo);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.VehiculoID = new SelectList(db.Vehiculos.OrderBy(o => o.MatriculaVehiculo), "ID", "MatriculaVehiculo", revisionVehiculo.VehiculoID);
            ViewBag.TipoRevisionID = new SelectList(db.TiposRevision.OrderBy(o => o.Revision), "ID", "Revision", revisionVehiculo.TipoRevisionID);
            return View(revisionVehiculo);
        }

        // GET: RevisionesVehiculos/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RevisionVehiculo revisionVehiculo = await db.RevisionesVehiculo.FindAsync(id);
            if (revisionVehiculo == null)
            {
                return HttpNotFound();
            }
            ViewBag.VehiculoID = new SelectList(db.Vehiculos.OrderBy(o => o.MatriculaVehiculo), "ID", "MatriculaVehiculo", revisionVehiculo.VehiculoID);
            ViewBag.TipoRevisionID = new SelectList(db.TiposRevision.OrderBy(o => o.Revision), "ID", "Revision", revisionVehiculo.TipoRevisionID);

            return View(revisionVehiculo);
        }

        // POST: RevisionesVehiculos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,VehiculoID,TipoRevisionID,FechaRevision,Detalles,Ejecutor,Caducidad")] RevisionVehiculo revisionVehiculo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(revisionVehiculo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.VehiculoID = new SelectList(db.Vehiculos.OrderBy(o => o.MatriculaVehiculo), "ID", "MatriculaVehiculo", revisionVehiculo.VehiculoID);
            ViewBag.TipoRevisionID = new SelectList(db.TiposRevision.OrderBy(o => o.Revision), "ID", "Revision", revisionVehiculo.TipoRevisionID);

            return View(revisionVehiculo);
        }

        // GET: RevisionesVehiculos/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RevisionVehiculo revisionVehiculo = await db.RevisionesVehiculo.FindAsync(id);
            if (revisionVehiculo == null)
            {
                return HttpNotFound();
            }
            return View(revisionVehiculo);
        }

        // POST: RevisionesVehiculos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            RevisionVehiculo revisionVehiculo = await db.RevisionesVehiculo.FindAsync(id);
            db.RevisionesVehiculo.Remove(revisionVehiculo);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public async Task<ActionResult> Proximas()
        {
            //List<Revision> rev = new List<Revision>();

            var rev1 = db.RevisionesVehiculo.Include(r => r.Vehiculo)
                  .GroupBy(s => s.Vehiculo.MatriculaVehiculo);
            return View(await rev1.ToListAsync());

        }
    }
}
