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
    public class tipo_accesorioController : Controller
    {
        private GaillardEntities db = new GaillardEntities();

        // GET: tipo_accesorio
        public async Task<ActionResult> Index()
        {
            return View(await db.tipo_accesorio.ToListAsync());
        }

        // GET: tipo_accesorio/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tipo_accesorio tipo_accesorio = await db.tipo_accesorio.FindAsync(id);
            if (tipo_accesorio == null)
            {
                return HttpNotFound();
            }
            return View(tipo_accesorio);
        }

        // GET: tipo_accesorio/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: tipo_accesorio/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,tipo_accesorio1")] tipo_accesorio tipo_accesorio)
        {
            if (ModelState.IsValid)
            {
                db.tipo_accesorio.Add(tipo_accesorio);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(tipo_accesorio);
        }

        // GET: tipo_accesorio/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tipo_accesorio tipo_accesorio = await db.tipo_accesorio.FindAsync(id);
            if (tipo_accesorio == null)
            {
                return HttpNotFound();
            }
            return View(tipo_accesorio);
        }

        // POST: tipo_accesorio/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,tipo_accesorio1")] tipo_accesorio tipo_accesorio)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipo_accesorio).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(tipo_accesorio);
        }

        // GET: tipo_accesorio/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tipo_accesorio tipo_accesorio = await db.tipo_accesorio.FindAsync(id);
            if (tipo_accesorio == null)
            {
                return HttpNotFound();
            }
            return View(tipo_accesorio);
        }

        // POST: tipo_accesorio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            tipo_accesorio tipo_accesorio = await db.tipo_accesorio.FindAsync(id);
            db.tipo_accesorio.Remove(tipo_accesorio);
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
