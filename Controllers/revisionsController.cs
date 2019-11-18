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

namespace CasaGaillard.Controllers
{
    public class revisionsController : Controller
    {
        private GaillardEntities db = new GaillardEntities();

        // GET: revisions
        public async Task<ActionResult> Index()
        {
            var revisions = db.revisions.Include(r => r.cuba);
            return View(await revisions.ToListAsync());
        }

        // GET: revisions/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            revision revision = await db.revisions.FindAsync(id);
            if (revision == null)
            {
                return HttpNotFound();
            }
            return View(revision);
        }

        // GET: revisions/Create
        public ActionResult Create()
        {
            ViewBag.cuba_id = new SelectList(db.cubas, "id", "cuba1");
            return View();
        }

        // POST: revisions/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,cuba_id,fecha_revision,descripcion,valida_hasta,descripcion_proxima,autorizado")] revision revision)
        {
            if (ModelState.IsValid)
            {
                db.revisions.Add(revision);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.cuba_id = new SelectList(db.cubas, "id", "cuba1", revision.cuba_id);
            return View(revision);
        }

        // GET: revisions/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            revision revision = await db.revisions.FindAsync(id);
            if (revision == null)
            {
                return HttpNotFound();
            }
            ViewBag.cuba_id = new SelectList(db.cubas, "id", "cuba1", revision.cuba_id);
            return View(revision);
        }

        // POST: revisions/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,cuba_id,fecha_revision,descripcion,valida_hasta,descripcion_proxima,autorizado")] revision revision)
        {
            if (ModelState.IsValid)
            {
                db.Entry(revision).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.cuba_id = new SelectList(db.cubas, "id", "cuba1", revision.cuba_id);
            return View(revision);
        }

        // GET: revisions/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            revision revision = await db.revisions.FindAsync(id);
            if (revision == null)
            {
                return HttpNotFound();
            }
            return View(revision);
        }

        // POST: revisions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            revision revision = await db.revisions.FindAsync(id);
            db.revisions.Remove(revision);
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
    }
}
