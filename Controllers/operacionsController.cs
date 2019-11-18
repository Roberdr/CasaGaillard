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
    public class operacionsController : Controller
    {
        private GaillardEntities db = new GaillardEntities();

        // GET: operacions
        public async Task<ActionResult> Index()
        {
            var operacions = db.operacions.Include(o => o.cuba).Include(o => o.operario).Include(o => o.tipo_operacion);
            return View(await operacions.ToListAsync());
        }

        // GET: operacions/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            operacion operacion = await db.operacions.FindAsync(id);
            if (operacion == null)
            {
                return HttpNotFound();
            }
            return View(operacion);
        }

        // GET: operacions/Create
        public ActionResult Create()
        {
            ViewBag.cuba_id = new SelectList(db.cubas, "id", "cuba1");
            ViewBag.operario_id = new SelectList(db.operarios, "id", "nombre");
            ViewBag.operacion_id = new SelectList(db.tipo_operacion, "id", "operacion");
            return View();
        }

        // POST: operacions/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,cuba_id,operacion_id,fecha_operacion,descripcion,operario_id,duracion")] operacion operacion)
        {
            if (ModelState.IsValid)
            {
                db.operacions.Add(operacion);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.cuba_id = new SelectList(db.cubas, "id", "cuba1", operacion.cuba_id);
            ViewBag.operario_id = new SelectList(db.operarios, "id", "nombre", operacion.operario_id);
            ViewBag.operacion_id = new SelectList(db.tipo_operacion, "id", "operacion", operacion.operacion_id);
            return View(operacion);
        }

        // GET: operacions/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            operacion operacion = await db.operacions.FindAsync(id);
            if (operacion == null)
            {
                return HttpNotFound();
            }
            ViewBag.cuba_id = new SelectList(db.cubas, "id", "cuba1", operacion.cuba_id);
            ViewBag.operario_id = new SelectList(db.operarios, "id", "nombre", operacion.operario_id);
            ViewBag.operacion_id = new SelectList(db.tipo_operacion, "id", "operacion", operacion.operacion_id);
            return View(operacion);
        }

        // POST: operacions/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,cuba_id,operacion_id,fecha_operacion,descripcion,operario_id,duracion")] operacion operacion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(operacion).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.cuba_id = new SelectList(db.cubas, "id", "cuba1", operacion.cuba_id);
            ViewBag.operario_id = new SelectList(db.operarios, "id", "nombre", operacion.operario_id);
            ViewBag.operacion_id = new SelectList(db.tipo_operacion, "id", "operacion", operacion.operacion_id);
            return View(operacion);
        }

        // GET: operacions/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            operacion operacion = await db.operacions.FindAsync(id);
            if (operacion == null)
            {
                return HttpNotFound();
            }
            return View(operacion);
        }

        // POST: operacions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            operacion operacion = await db.operacions.FindAsync(id);
            db.operacions.Remove(operacion);
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
