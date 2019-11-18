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
    public class plataformasController : Controller
    {
        private GaillardEntities db = new GaillardEntities();

        // GET: plataformas
        public async Task<ActionResult> Index()
        {
            return View(await db.plataformas.ToListAsync());
        }

        // GET: plataformas/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            plataforma plataforma = await db.plataformas.FindAsync(id);
            if (plataforma == null)
            {
                return HttpNotFound();
            }
            return View(plataforma);
        }

        // GET: plataformas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: plataformas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,matricula,propiedad,pma")] plataforma plataforma)
        {
            if (ModelState.IsValid)
            {
                db.plataformas.Add(plataforma);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(plataforma);
        }

        // GET: plataformas/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            plataforma plataforma = await db.plataformas.FindAsync(id);
            if (plataforma == null)
            {
                return HttpNotFound();
            }
            return View(plataforma);
        }

        // POST: plataformas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,matricula,propiedad,pma")] plataforma plataforma)
        {
            if (ModelState.IsValid)
            {
                db.Entry(plataforma).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(plataforma);
        }

        // GET: plataformas/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            plataforma plataforma = await db.plataformas.FindAsync(id);
            if (plataforma == null)
            {
                return HttpNotFound();
            }
            return View(plataforma);
        }

        // POST: plataformas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            plataforma plataforma = await db.plataformas.FindAsync(id);
            db.plataformas.Remove(plataforma);
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
