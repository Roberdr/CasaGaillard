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
    public class accesorio_grupoController : Controller
    {
        private GaillardEntities db = new GaillardEntities();

        // GET: accesorio_grupo
        public async Task<ActionResult> Index()
        {
            var accesorio_grupo = db.accesorio_grupo.Include(a => a.accesorio).Include(a => a.grupo);
            return View(await accesorio_grupo.ToListAsync());
        }

        // GET: accesorio_grupo/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            accesorio_grupo accesorio_grupo = await db.accesorio_grupo.FindAsync(id);
            if (accesorio_grupo == null)
            {
                return HttpNotFound();
            }
            return View(accesorio_grupo);
        }

        // GET: accesorio_grupo/Create
        public ActionResult Create()
        {
            ViewBag.accesorio_id = new SelectList(db.accesorios, "id", "id");
            ViewBag.grupo_id = new SelectList(db.grupoes, "id", "id");
            return View();
        }

        // POST: accesorio_grupo/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,grupo_id,accesorio_id,cantidad")] accesorio_grupo accesorio_grupo)
        {
            if (ModelState.IsValid)
            {
                db.accesorio_grupo.Add(accesorio_grupo);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.accesorio_id = new SelectList(db.accesorios, "id", "id", accesorio_grupo.accesorio_id);
            ViewBag.grupo_id = new SelectList(db.grupoes, "id", "id", accesorio_grupo.grupo_id);
            return View(accesorio_grupo);
        }

        // GET: accesorio_grupo/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            accesorio_grupo accesorio_grupo = await db.accesorio_grupo.FindAsync(id);
            if (accesorio_grupo == null)
            {
                return HttpNotFound();
            }
            ViewBag.accesorio_id = new SelectList(db.accesorios, "id", "id", accesorio_grupo.accesorio_id);
            ViewBag.grupo_id = new SelectList(db.grupoes, "id", "id", accesorio_grupo.grupo_id);
            return View(accesorio_grupo);
        }

        // POST: accesorio_grupo/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,grupo_id,accesorio_id,cantidad")] accesorio_grupo accesorio_grupo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(accesorio_grupo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.accesorio_id = new SelectList(db.accesorios, "id", "id", accesorio_grupo.accesorio_id);
            ViewBag.grupo_id = new SelectList(db.grupoes, "id", "id", accesorio_grupo.grupo_id);
            return View(accesorio_grupo);
        }

        // GET: accesorio_grupo/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            accesorio_grupo accesorio_grupo = await db.accesorio_grupo.FindAsync(id);
            if (accesorio_grupo == null)
            {
                return HttpNotFound();
            }
            return View(accesorio_grupo);
        }

        // POST: accesorio_grupo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            accesorio_grupo accesorio_grupo = await db.accesorio_grupo.FindAsync(id);
            db.accesorio_grupo.Remove(accesorio_grupo);
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
