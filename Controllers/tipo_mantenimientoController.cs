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
    public class tipo_mantenimientoController : Controller
    {
        private GaillardEntities db = new GaillardEntities();

        // GET: tipo_mantenimiento
        public async Task<ActionResult> Index()
        {
            return View(await db.tipo_mantenimiento.ToListAsync());
        }

        // GET: tipo_mantenimiento/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tipo_mantenimiento tipo_mantenimiento = await db.tipo_mantenimiento.FindAsync(id);
            if (tipo_mantenimiento == null)
            {
                return HttpNotFound();
            }
            return View(tipo_mantenimiento);
        }

        // GET: tipo_mantenimiento/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: tipo_mantenimiento/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,tipo_mantenimiento1")] tipo_mantenimiento tipo_mantenimiento)
        {
            if (ModelState.IsValid)
            {
                db.tipo_mantenimiento.Add(tipo_mantenimiento);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(tipo_mantenimiento);
        }

        // GET: tipo_mantenimiento/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tipo_mantenimiento tipo_mantenimiento = await db.tipo_mantenimiento.FindAsync(id);
            if (tipo_mantenimiento == null)
            {
                return HttpNotFound();
            }
            return View(tipo_mantenimiento);
        }

        // POST: tipo_mantenimiento/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,tipo_mantenimiento1")] tipo_mantenimiento tipo_mantenimiento)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipo_mantenimiento).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(tipo_mantenimiento);
        }

        // GET: tipo_mantenimiento/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tipo_mantenimiento tipo_mantenimiento = await db.tipo_mantenimiento.FindAsync(id);
            if (tipo_mantenimiento == null)
            {
                return HttpNotFound();
            }
            return View(tipo_mantenimiento);
        }

        // POST: tipo_mantenimiento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            tipo_mantenimiento tipo_mantenimiento = await db.tipo_mantenimiento.FindAsync(id);
            db.tipo_mantenimiento.Remove(tipo_mantenimiento);
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
