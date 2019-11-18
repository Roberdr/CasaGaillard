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
    public class tipo_revisionController : Controller
    {
        private GaillardEntities db = new GaillardEntities();

        // GET: tipo_revision
        public async Task<ActionResult> Index()
        {
            return View(await db.tipo_revision.ToListAsync());
        }

        // GET: tipo_revision/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tipo_revision tipo_revision = await db.tipo_revision.FindAsync(id);
            if (tipo_revision == null)
            {
                return HttpNotFound();
            }
            return View(tipo_revision);
        }

        // GET: tipo_revision/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: tipo_revision/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,tipo_revision1")] tipo_revision tipo_revision)
        {
            if (ModelState.IsValid)
            {
                db.tipo_revision.Add(tipo_revision);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(tipo_revision);
        }

        // GET: tipo_revision/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tipo_revision tipo_revision = await db.tipo_revision.FindAsync(id);
            if (tipo_revision == null)
            {
                return HttpNotFound();
            }
            return View(tipo_revision);
        }

        // POST: tipo_revision/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,tipo_revision1")] tipo_revision tipo_revision)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipo_revision).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(tipo_revision);
        }

        // GET: tipo_revision/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tipo_revision tipo_revision = await db.tipo_revision.FindAsync(id);
            if (tipo_revision == null)
            {
                return HttpNotFound();
            }
            return View(tipo_revision);
        }

        // POST: tipo_revision/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            tipo_revision tipo_revision = await db.tipo_revision.FindAsync(id);
            db.tipo_revision.Remove(tipo_revision);
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
