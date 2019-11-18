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
    public class situacionsController : Controller
    {
        private GaillardEntities db = new GaillardEntities();

        // GET: situacions
        public async Task<ActionResult> Index()
        {
            return View(await db.situacions.ToListAsync());
        }

        // GET: situacions/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            situacion situacion = await db.situacions.FindAsync(id);
            if (situacion == null)
            {
                return HttpNotFound();
            }
            return View(situacion);
        }

        // GET: situacions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: situacions/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,lado_cuba_numero,lado_cuba_nombre,situacion_lado_numero,situacion_lado_letra")] situacion situacion)
        {
            if (ModelState.IsValid)
            {
                db.situacions.Add(situacion);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(situacion);
        }

        // GET: situacions/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            situacion situacion = await db.situacions.FindAsync(id);
            if (situacion == null)
            {
                return HttpNotFound();
            }
            return View(situacion);
        }

        // POST: situacions/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,lado_cuba_numero,lado_cuba_nombre,situacion_lado_numero,situacion_lado_letra")] situacion situacion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(situacion).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(situacion);
        }

        // GET: situacions/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            situacion situacion = await db.situacions.FindAsync(id);
            if (situacion == null)
            {
                return HttpNotFound();
            }
            return View(situacion);
        }

        // POST: situacions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            situacion situacion = await db.situacions.FindAsync(id);
            db.situacions.Remove(situacion);
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
