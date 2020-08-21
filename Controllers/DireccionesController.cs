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
    public class DireccionesController : Controller
    {
        private readonly GaillardEntities db = new GaillardEntities();

        // GET: Direcciones
        public async Task<ActionResult> Index()
        {
            var direccions = db.Direcciones.Include(d => d.Poblacion).Include(d => d.TipoVia);
            return View(await direccions.ToListAsync());
        }

        // GET: Direcciones/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Direccion direccion = await db.Direcciones.FindAsync(id);
            if (direccion == null)
            {
                return HttpNotFound();
            }
            return View(direccion);
        }

        // GET: Direcciones/Create
        public ActionResult Create()
        {
            ViewBag.PoblacionID = new SelectList(db.Poblaciones, "ID", "NombrePoblacion");
            ViewBag.TipoViaID = new SelectList(db.TiposVia, "ID", "TipoVia1");
            return View();
        }

        // POST: Direcciones/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,TipoViaID,NombreVia,Distrito,Numero,Escalera,Piso,Puerta,PoblacionID,CP,Pais")] Direccion direccion)
        {
            if (ModelState.IsValid)
            {
                db.Direcciones.Add(direccion);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.PoblacionID = new SelectList(db.Poblaciones, "ID", "NombrePoblacion", direccion.PoblacionID);
            ViewBag.TipoViaID = new SelectList(db.TiposVia, "ID", "TipoVia1", direccion.TipoViaID);
            return View(direccion);
        }

        // GET: Direcciones/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Direccion direccion = await db.Direcciones.FindAsync(id);
            if (direccion == null)
            {
                return HttpNotFound();
            }
            ViewBag.PoblacionID = new SelectList(db.Poblaciones, "ID", "NombrePoblacion", direccion.PoblacionID);
            ViewBag.TipoViaID = new SelectList(db.TiposVia, "ID", "TipoVia1", direccion.TipoViaID);
            return View(direccion);
        }

        // POST: Direcciones/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,TipoViaID,NombreVia,Distrito,Numero,Escalera,Piso,Puerta,PoblacionID,CP,Pais")] Direccion direccion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(direccion).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.PoblacionID = new SelectList(db.Poblaciones, "ID", "NombrePoblacion", direccion.PoblacionID);
            ViewBag.TipoViaID = new SelectList(db.TiposVia, "ID", "TipoVia1", direccion.TipoViaID);
            return View(direccion);
        }

        // GET: Direcciones/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Direccion direccion = await db.Direcciones.FindAsync(id);
            if (direccion == null)
            {
                return HttpNotFound();
            }
            return View(direccion);
        }

        // POST: Direcciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Direccion direccion = await db.Direcciones.FindAsync(id);
            db.Direcciones.Remove(direccion);
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
