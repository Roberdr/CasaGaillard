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

namespace CasaGaillard.Areas.Mantenimiento.Controllers
{
    [Authorize]
    public class RevisionesController : Controller
    {
        private readonly GaillardEntities db = new GaillardEntities();

        // GET: Revisiones
        public async Task<ActionResult> Index()
        {
            var revisiones = db.Revisiones
                .Include(r => r.Cuba)
                .OrderBy(r => r.Cuba.MatriculaCuba)
                .ThenByDescending(r => r.FechaRevision)
                .GroupBy(r => r.Cuba.MatriculaCuba);

            return View(await revisiones.ToListAsync());
        }

        // GET: Revisiones/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Revision revision = await db.Revisiones.FindAsync(id);
            if (revision == null)
            {
                return HttpNotFound();
            }
            return View(revision);
        }

        // GET: Revisiones/Create
        public ActionResult Create()
        {
            ViewBag.CubaID = new SelectList(db.Cubas, "ID", "MatriculaCuba");
            return View();
        }

        // POST: Revisiones/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,CubaID,FechaRevision,Descripcion,ValidaHasta,DescripcionProxima,Autorizado")] Revision revision)
        {
            if (ModelState.IsValid)
            {
                revision.ValidaHasta = revision.FechaRevision.AddMonths(30);
                db.Revisiones.Add(revision);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CubaID = new SelectList(db.Cubas, "ID", "MatriculaCuba", revision.CubaID);
            return View(revision);
        }

        // GET: Revisiones/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Revision revision = await db.Revisiones.FindAsync(id);
            if (revision == null)
            {
                return HttpNotFound();
            }
            ViewBag.CubaID = new SelectList(db.Cubas, "ID", "MatriculaCuba", revision.CubaID);
            return View(revision);
        }

        // POST: Revisiones/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,CubaID,FechaRevision,Descripcion,ValidaHasta,DescripcionProxima,Autorizado")] Revision revision)
        {
            if (ModelState.IsValid)
            {
                db.Entry(revision).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CubaID = new SelectList(db.Cubas, "ID", "MatriculaCuba", revision.CubaID);
            return View(revision);
        }

        // GET: Revisiones/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Revision revision = await db.Revisiones.FindAsync(id);
            if (revision == null)
            {
                return HttpNotFound();
            }
            return View(revision);
        }

        // POST: Revisiones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Revision revision = await db.Revisiones.FindAsync(id);
            db.Revisiones.Remove(revision);
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

            var rev1 = db.Revisiones.Include(r => r.Cuba)
                  .GroupBy(s => s.Cuba.MatriculaCuba);
            return View(await rev1.ToListAsync());

        }

        public async Task<ActionResult> CambiarValidaHasta()
        {
            var rev1 = await db.Revisiones.ToListAsync();

            foreach (var rev in rev1)
            {
                rev.ValidaHasta = rev.FechaRevision.AddMonths(30);
                await db.SaveChangesAsync();
             
            }

            return RedirectToAction("Index");
        }
    }
}
