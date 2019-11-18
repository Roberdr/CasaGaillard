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
    public class unidadsController : Controller
    {
        private GaillardEntities db = new GaillardEntities();

        // GET: unidads
        public async Task<ActionResult> Index()
        {
            return View(await db.unidads.ToListAsync());
        }

        // GET: unidads/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            unidad unidad = await db.unidads.FindAsync(id);
            if (unidad == null)
            {
                return HttpNotFound();
            }
            return View(unidad);
        }

        // GET: unidads/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: unidads/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,unidad1")] unidad unidad)
        {
            if (ModelState.IsValid)
            {
                db.unidads.Add(unidad);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(unidad);
        }

        // GET: unidads/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            unidad unidad = await db.unidads.FindAsync(id);
            if (unidad == null)
            {
                return HttpNotFound();
            }
            return View(unidad);
        }

        // POST: unidads/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,unidad1")] unidad unidad)
        {
            if (ModelState.IsValid)
            {
                db.Entry(unidad).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(unidad);
        }

        // GET: unidads/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            unidad unidad = await db.unidads.FindAsync(id);
            if (unidad == null)
            {
                return HttpNotFound();
            }
            return View(unidad);
        }

        // POST: unidads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            unidad unidad = await db.unidads.FindAsync(id);
            db.unidads.Remove(unidad);
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
