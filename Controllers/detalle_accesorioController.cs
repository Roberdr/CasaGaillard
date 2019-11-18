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
    public class detalle_accesorioController : Controller
    {
        private GaillardEntities db = new GaillardEntities();

        // GET: detalle_accesorio
        public async Task<ActionResult> Index()
        {
            var detalle_accesorio = db.detalle_accesorio.Include(d => d.accesorio).Include(d => d.caracteristica_accesorio).Include(d => d.unidad);
            return View(await detalle_accesorio.ToListAsync());
        }

        // GET: detalle_accesorio/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            detalle_accesorio detalle_accesorio = await db.detalle_accesorio.FindAsync(id);
            if (detalle_accesorio == null)
            {
                return HttpNotFound();
            }
            return View(detalle_accesorio);
        }

        // GET: detalle_accesorio/Create
        public ActionResult Create()
        {
            ViewBag.accesorio_id = new SelectList(db.accesorios, "id", "id");
            ViewBag.caracteristica_accesorio_id = new SelectList(db.caracteristica_accesorio, "id", "caracteristica_accesorio1");
            ViewBag.unidad_id = new SelectList(db.unidads, "id", "unidad1");
            return View();
        }

        // POST: detalle_accesorio/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,accesorio_id,cantidad,caracteristica_accesorio_id,medida,unidad_id,tipo")] detalle_accesorio detalle_accesorio)
        {
            if (ModelState.IsValid)
            {
                db.detalle_accesorio.Add(detalle_accesorio);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.accesorio_id = new SelectList(db.accesorios, "id", "id", detalle_accesorio.accesorio_id);
            ViewBag.caracteristica_accesorio_id = new SelectList(db.caracteristica_accesorio, "id", "caracteristica_accesorio1", detalle_accesorio.caracteristica_accesorio_id);
            ViewBag.unidad_id = new SelectList(db.unidads, "id", "unidad1", detalle_accesorio.unidad_id);
            return View(detalle_accesorio);
        }

        // GET: detalle_accesorio/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            detalle_accesorio detalle_accesorio = await db.detalle_accesorio.FindAsync(id);
            if (detalle_accesorio == null)
            {
                return HttpNotFound();
            }
            ViewBag.accesorio_id = new SelectList(db.accesorios, "id", "id", detalle_accesorio.accesorio_id);
            ViewBag.caracteristica_accesorio_id = new SelectList(db.caracteristica_accesorio, "id", "caracteristica_accesorio1", detalle_accesorio.caracteristica_accesorio_id);
            ViewBag.unidad_id = new SelectList(db.unidads, "id", "unidad1", detalle_accesorio.unidad_id);
            return View(detalle_accesorio);
        }

        // POST: detalle_accesorio/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,accesorio_id,cantidad,caracteristica_accesorio_id,medida,unidad_id,tipo")] detalle_accesorio detalle_accesorio)
        {
            if (ModelState.IsValid)
            {
                db.Entry(detalle_accesorio).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.accesorio_id = new SelectList(db.accesorios, "id", "id", detalle_accesorio.accesorio_id);
            ViewBag.caracteristica_accesorio_id = new SelectList(db.caracteristica_accesorio, "id", "caracteristica_accesorio1", detalle_accesorio.caracteristica_accesorio_id);
            ViewBag.unidad_id = new SelectList(db.unidads, "id", "unidad1", detalle_accesorio.unidad_id);
            return View(detalle_accesorio);
        }

        // GET: detalle_accesorio/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            detalle_accesorio detalle_accesorio = await db.detalle_accesorio.FindAsync(id);
            if (detalle_accesorio == null)
            {
                return HttpNotFound();
            }
            return View(detalle_accesorio);
        }

        // POST: detalle_accesorio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            detalle_accesorio detalle_accesorio = await db.detalle_accesorio.FindAsync(id);
            db.detalle_accesorio.Remove(detalle_accesorio);
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
