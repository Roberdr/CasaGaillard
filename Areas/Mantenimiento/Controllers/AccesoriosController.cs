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

namespace CasaGaillard.Areas.Mantenimiento.Controllers
{

    [Authorize]
    public class AccesoriosController : Controller
    {
        private readonly GaillardEntities db = new GaillardEntities();

        // GET: Accesorios
        
        public async Task<ActionResult> Index()
        {
            var accesorios = db.Accesorios.Include(a => a.Material).Include(a => a.TipoAccesorio);
            return View(await accesorios.ToListAsync());
        }

        // GET: Accesorios/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Accesorio accesorio = await db.Accesorios.FindAsync(id);
            if (accesorio == null)
            {
                return HttpNotFound();
            }
            return View(accesorio);
        }

        // GET: Accesorios/Create
        public ActionResult Create()
        {
            ViewBag.MaterialID = new SelectList(db.Materiales, "ID", "Material1");
            ViewBag.TipoAccesorioID = new SelectList(db.TiposAccesorio, "ID", "TipoAccesorio1");
            return View();
        }

        // POST: Accesorios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,TipoAccesorioID,MaterialID")] Accesorio accesorio)
        {
            if (ModelState.IsValid)
            {
                db.Accesorios.Add(accesorio);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.MaterialID = new SelectList(db.Materiales, "ID", "Material1", accesorio.MaterialID);
            ViewBag.TipoAccesorioID = new SelectList(db.TiposAccesorio, "ID", "TipoAccesorio1", accesorio.TipoAccesorioID);
            return View(accesorio);
        }

        // GET: Accesorios/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Accesorio accesorio = await db.Accesorios.FindAsync(id);
            if (accesorio == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaterialID = new SelectList(db.Materiales, "ID", "Material1", accesorio.MaterialID);
            ViewBag.TipoAccesorioID = new SelectList(db.TiposAccesorio, "ID", "TipoAccesorio1", accesorio.TipoAccesorioID);
            return View(accesorio);
        }

        // POST: Accesorios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,TipoAccesorioID,MaterialID")] Accesorio accesorio)
        {
            if (ModelState.IsValid)
            {
                db.Entry(accesorio).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.MaterialID = new SelectList(db.Materiales, "ID", "Material1", accesorio.MaterialID);
            ViewBag.TipoAccesorioID = new SelectList(db.TiposAccesorio, "ID", "TipoAccesorio1", accesorio.TipoAccesorioID);
            return View(accesorio);
        }

        // GET: Accesorios/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Accesorio accesorio = await db.Accesorios.FindAsync(id);
            if (accesorio == null)
            {
                return HttpNotFound();
            }
            return View(accesorio);
        }

        // POST: Accesorios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Accesorio accesorio = await db.Accesorios.FindAsync(id);
            db.Accesorios.Remove(accesorio);
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
