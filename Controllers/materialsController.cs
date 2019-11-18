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
    public class materialsController : Controller
    {
        private GaillardEntities db = new GaillardEntities();

        // GET: materials
        public async Task<ActionResult> Index()
        {
            return View(await db.materials.ToListAsync());
        }

        // GET: materials/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            material material = await db.materials.FindAsync(id);
            if (material == null)
            {
                return HttpNotFound();
            }
            return View(material);
        }

        // GET: materials/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: materials/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,material1,descripcion")] material material)
        {
            if (ModelState.IsValid)
            {
                db.materials.Add(material);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(material);
        }

        // GET: materials/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            material material = await db.materials.FindAsync(id);
            if (material == null)
            {
                return HttpNotFound();
            }
            return View(material);
        }

        // POST: materials/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,material1,descripcion")] material material)
        {
            if (ModelState.IsValid)
            {
                db.Entry(material).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(material);
        }

        // GET: materials/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            material material = await db.materials.FindAsync(id);
            if (material == null)
            {
                return HttpNotFound();
            }
            return View(material);
        }

        // POST: materials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            material material = await db.materials.FindAsync(id);
            db.materials.Remove(material);
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
