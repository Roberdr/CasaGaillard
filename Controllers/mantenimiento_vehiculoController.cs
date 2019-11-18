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
    public class mantenimiento_vehiculoController : Controller
    {
        private GaillardEntities db = new GaillardEntities();

        // GET: mantenimiento_vehiculo
        public async Task<ActionResult> Index()
        {
            var mantenimiento_vehiculo = db.mantenimiento_vehiculo.Include(m => m.operario).Include(m => m.tipo_mantenimiento).Include(m => m.vehiculo);
            return View(await mantenimiento_vehiculo.ToListAsync());
        }

        // GET: mantenimiento_vehiculo/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            mantenimiento_vehiculo mantenimiento_vehiculo = await db.mantenimiento_vehiculo.FindAsync(id);
            if (mantenimiento_vehiculo == null)
            {
                return HttpNotFound();
            }
            return View(mantenimiento_vehiculo);
        }

        // GET: mantenimiento_vehiculo/Create
        public ActionResult Create()
        {
            ViewBag.id_operario = new SelectList(db.operarios, "id", "nombre");
            ViewBag.id_tipo_mantenimiento = new SelectList(db.tipo_mantenimiento, "id", "tipo_mantenimiento1");
            ViewBag.id_vehiculo = new SelectList(db.vehiculoes, "id", "marca");
            return View();
        }

        // POST: mantenimiento_vehiculo/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,id_vehiculo,fecha_mantenimiento,id_tipo_mantenimiento,detalle_mantenimiento,id_operario,kilometros")] mantenimiento_vehiculo mantenimiento_vehiculo)
        {
            if (ModelState.IsValid)
            {
                db.mantenimiento_vehiculo.Add(mantenimiento_vehiculo);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.id_operario = new SelectList(db.operarios, "id", "nombre", mantenimiento_vehiculo.id_operario);
            ViewBag.id_tipo_mantenimiento = new SelectList(db.tipo_mantenimiento, "id", "tipo_mantenimiento1", mantenimiento_vehiculo.id_tipo_mantenimiento);
            ViewBag.id_vehiculo = new SelectList(db.vehiculoes, "id", "marca", mantenimiento_vehiculo.id_vehiculo);
            return View(mantenimiento_vehiculo);
        }

        // GET: mantenimiento_vehiculo/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            mantenimiento_vehiculo mantenimiento_vehiculo = await db.mantenimiento_vehiculo.FindAsync(id);
            if (mantenimiento_vehiculo == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_operario = new SelectList(db.operarios, "id", "nombre", mantenimiento_vehiculo.id_operario);
            ViewBag.id_tipo_mantenimiento = new SelectList(db.tipo_mantenimiento, "id", "tipo_mantenimiento1", mantenimiento_vehiculo.id_tipo_mantenimiento);
            ViewBag.id_vehiculo = new SelectList(db.vehiculoes, "id", "marca", mantenimiento_vehiculo.id_vehiculo);
            return View(mantenimiento_vehiculo);
        }

        // POST: mantenimiento_vehiculo/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,id_vehiculo,fecha_mantenimiento,id_tipo_mantenimiento,detalle_mantenimiento,id_operario,kilometros")] mantenimiento_vehiculo mantenimiento_vehiculo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mantenimiento_vehiculo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.id_operario = new SelectList(db.operarios, "id", "nombre", mantenimiento_vehiculo.id_operario);
            ViewBag.id_tipo_mantenimiento = new SelectList(db.tipo_mantenimiento, "id", "tipo_mantenimiento1", mantenimiento_vehiculo.id_tipo_mantenimiento);
            ViewBag.id_vehiculo = new SelectList(db.vehiculoes, "id", "marca", mantenimiento_vehiculo.id_vehiculo);
            return View(mantenimiento_vehiculo);
        }

        // GET: mantenimiento_vehiculo/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            mantenimiento_vehiculo mantenimiento_vehiculo = await db.mantenimiento_vehiculo.FindAsync(id);
            if (mantenimiento_vehiculo == null)
            {
                return HttpNotFound();
            }
            return View(mantenimiento_vehiculo);
        }

        // POST: mantenimiento_vehiculo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            mantenimiento_vehiculo mantenimiento_vehiculo = await db.mantenimiento_vehiculo.FindAsync(id);
            db.mantenimiento_vehiculo.Remove(mantenimiento_vehiculo);
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
