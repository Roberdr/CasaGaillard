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
    public class accesoriosController : Controller
    {
        private GaillardEntities db = new GaillardEntities();

        // GET: accesorios
        public async Task<ActionResult> Index()
        {
            var accesorios = db.accesorios.Include(a => a.material).Include(a => a.tipo_accesorio);
            return View(await accesorios.ToListAsync());
        }

        // GET: accesorios/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            accesorio accesorio = await db.accesorios.FindAsync(id);
            if (accesorio == null)
            {
                return HttpNotFound();
            }
            return View(accesorio);
        }

        // GET: accesorios/Create
        public ActionResult Create()
        {
            ViewBag.material_id = new SelectList(db.materials, "id", "material1");
            ViewBag.tipo_accesorio_id = new SelectList(db.tipo_accesorio, "id", "tipo_accesorio1");
            return View();
        }

        // POST: accesorios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,tipo_accesorio_id,material_id")] accesorio accesorio)
        {
            if (ModelState.IsValid)
            {
                db.accesorios.Add(accesorio);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.material_id = new SelectList(db.materials, "id", "material1", accesorio.material_id);
            ViewBag.tipo_accesorio_id = new SelectList(db.tipo_accesorio, "id", "tipo_accesorio1", accesorio.tipo_accesorio_id);
            return View(accesorio);
        }

        // GET: accesorios/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            accesorio accesorio = await db.accesorios.FindAsync(id);
            if (accesorio == null)
            {
                return HttpNotFound();
            }
            ViewBag.material_id = new SelectList(db.materials, "id", "material1", accesorio.material_id);
            ViewBag.tipo_accesorio_id = new SelectList(db.tipo_accesorio, "id", "tipo_accesorio1", accesorio.tipo_accesorio_id);
            return View(accesorio);
        }

        // POST: accesorios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,tipo_accesorio_id,material_id")] accesorio accesorio)
        {
            if (ModelState.IsValid)
            {
                db.Entry(accesorio).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.material_id = new SelectList(db.materials, "id", "material1", accesorio.material_id);
            ViewBag.tipo_accesorio_id = new SelectList(db.tipo_accesorio, "id", "tipo_accesorio1", accesorio.tipo_accesorio_id);
            return View(accesorio);
        }

        // GET: accesorios/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            accesorio accesorio = await db.accesorios.FindAsync(id);
            if (accesorio == null)
            {
                return HttpNotFound();
            }
            return View(accesorio);
        }

        // POST: accesorios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            accesorio accesorio = await db.accesorios.FindAsync(id);
            db.accesorios.Remove(accesorio);
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
