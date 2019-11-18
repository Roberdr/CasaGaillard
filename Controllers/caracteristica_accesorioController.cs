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
    public class caracteristica_accesorioController : Controller
    {
        private GaillardEntities db = new GaillardEntities();

        // GET: caracteristica_accesorio
        public async Task<ActionResult> Index()
        {
            return View(await db.caracteristica_accesorio.ToListAsync());
        }

        // GET: caracteristica_accesorio/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            caracteristica_accesorio caracteristica_accesorio = await db.caracteristica_accesorio.FindAsync(id);
            if (caracteristica_accesorio == null)
            {
                return HttpNotFound();
            }
            return View(caracteristica_accesorio);
        }

        // GET: caracteristica_accesorio/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: caracteristica_accesorio/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,caracteristica_accesorio1")] caracteristica_accesorio caracteristica_accesorio)
        {
            if (ModelState.IsValid)
            {
                db.caracteristica_accesorio.Add(caracteristica_accesorio);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(caracteristica_accesorio);
        }

        // GET: caracteristica_accesorio/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            caracteristica_accesorio caracteristica_accesorio = await db.caracteristica_accesorio.FindAsync(id);
            if (caracteristica_accesorio == null)
            {
                return HttpNotFound();
            }
            return View(caracteristica_accesorio);
        }

        // POST: caracteristica_accesorio/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,caracteristica_accesorio1")] caracteristica_accesorio caracteristica_accesorio)
        {
            if (ModelState.IsValid)
            {
                db.Entry(caracteristica_accesorio).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(caracteristica_accesorio);
        }

        // GET: caracteristica_accesorio/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            caracteristica_accesorio caracteristica_accesorio = await db.caracteristica_accesorio.FindAsync(id);
            if (caracteristica_accesorio == null)
            {
                return HttpNotFound();
            }
            return View(caracteristica_accesorio);
        }

        // POST: caracteristica_accesorio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            caracteristica_accesorio caracteristica_accesorio = await db.caracteristica_accesorio.FindAsync(id);
            db.caracteristica_accesorio.Remove(caracteristica_accesorio);
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
