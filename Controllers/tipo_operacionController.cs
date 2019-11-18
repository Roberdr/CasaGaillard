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
    public class tipo_operacionController : Controller
    {
        private GaillardEntities db = new GaillardEntities();

        // GET: tipo_operacion
        public async Task<ActionResult> Index()
        {
            return View(await db.tipo_operacion.ToListAsync());
        }

        // GET: tipo_operacion/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tipo_operacion tipo_operacion = await db.tipo_operacion.FindAsync(id);
            if (tipo_operacion == null)
            {
                return HttpNotFound();
            }
            return View(tipo_operacion);
        }

        // GET: tipo_operacion/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: tipo_operacion/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,operacion")] tipo_operacion tipo_operacion)
        {
            if (ModelState.IsValid)
            {
                db.tipo_operacion.Add(tipo_operacion);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(tipo_operacion);
        }

        // GET: tipo_operacion/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tipo_operacion tipo_operacion = await db.tipo_operacion.FindAsync(id);
            if (tipo_operacion == null)
            {
                return HttpNotFound();
            }
            return View(tipo_operacion);
        }

        // POST: tipo_operacion/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,operacion")] tipo_operacion tipo_operacion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipo_operacion).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(tipo_operacion);
        }

        // GET: tipo_operacion/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tipo_operacion tipo_operacion = await db.tipo_operacion.FindAsync(id);
            if (tipo_operacion == null)
            {
                return HttpNotFound();
            }
            return View(tipo_operacion);
        }

        // POST: tipo_operacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            tipo_operacion tipo_operacion = await db.tipo_operacion.FindAsync(id);
            db.tipo_operacion.Remove(tipo_operacion);
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
