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
    public class revision_vehiculoController : Controller
    {
        private GaillardEntities db = new GaillardEntities();

        // GET: revision_vehiculo
        public async Task<ActionResult> Index()
        {
            var revision_vehiculo = db.revision_vehiculo.Include(r => r.vehiculo).Include(r => r.tipo_revision);
            return View(await revision_vehiculo.ToListAsync());
        }

        // GET: revision_vehiculo/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            revision_vehiculo revision_vehiculo = await db.revision_vehiculo.FindAsync(id);
            if (revision_vehiculo == null)
            {
                return HttpNotFound();
            }
            return View(revision_vehiculo);
        }

        // GET: revision_vehiculo/Create
        public ActionResult Create()
        {
            ViewBag.id_vehiculo = new SelectList(db.vehiculoes, "id", "marca");
            ViewBag.id_tipo_revision = new SelectList(db.tipo_revision, "id", "tipo_revision1");
            return View();
        }

        // POST: revision_vehiculo/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,id_vehiculo,id_tipo_revision,fecha_revision,detalles,ejecutor")] revision_vehiculo revision_vehiculo)
        {
            if (ModelState.IsValid)
            {
                db.revision_vehiculo.Add(revision_vehiculo);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.id_vehiculo = new SelectList(db.vehiculoes, "id", "marca", revision_vehiculo.id_vehiculo);
            ViewBag.id_tipo_revision = new SelectList(db.tipo_revision, "id", "tipo_revision1", revision_vehiculo.id_tipo_revision);
            return View(revision_vehiculo);
        }

        // GET: revision_vehiculo/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            revision_vehiculo revision_vehiculo = await db.revision_vehiculo.FindAsync(id);
            if (revision_vehiculo == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_vehiculo = new SelectList(db.vehiculoes, "id", "marca", revision_vehiculo.id_vehiculo);
            ViewBag.id_tipo_revision = new SelectList(db.tipo_revision, "id", "tipo_revision1", revision_vehiculo.id_tipo_revision);
            return View(revision_vehiculo);
        }

        // POST: revision_vehiculo/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,id_vehiculo,id_tipo_revision,fecha_revision,detalles,ejecutor")] revision_vehiculo revision_vehiculo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(revision_vehiculo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.id_vehiculo = new SelectList(db.vehiculoes, "id", "marca", revision_vehiculo.id_vehiculo);
            ViewBag.id_tipo_revision = new SelectList(db.tipo_revision, "id", "tipo_revision1", revision_vehiculo.id_tipo_revision);
            return View(revision_vehiculo);
        }

        // GET: revision_vehiculo/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            revision_vehiculo revision_vehiculo = await db.revision_vehiculo.FindAsync(id);
            if (revision_vehiculo == null)
            {
                return HttpNotFound();
            }
            return View(revision_vehiculo);
        }

        // POST: revision_vehiculo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            revision_vehiculo revision_vehiculo = await db.revision_vehiculo.FindAsync(id);
            db.revision_vehiculo.Remove(revision_vehiculo);
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
