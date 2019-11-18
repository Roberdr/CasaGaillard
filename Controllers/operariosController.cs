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
    public class operariosController : Controller
    {
        private GaillardEntities db = new GaillardEntities();

        // GET: operarios
        public async Task<ActionResult> Index()
        {
            return View(await db.operarios.ToListAsync());
        }

        // GET: operarios/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            operario operario = await db.operarios.FindAsync(id);
            if (operario == null)
            {
                return HttpNotFound();
            }
            return View(operario);
        }

        // GET: operarios/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: operarios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,nombre,apellidos")] operario operario)
        {
            if (ModelState.IsValid)
            {
                db.operarios.Add(operario);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(operario);
        }

        // GET: operarios/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            operario operario = await db.operarios.FindAsync(id);
            if (operario == null)
            {
                return HttpNotFound();
            }
            return View(operario);
        }

        // POST: operarios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,nombre,apellidos")] operario operario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(operario).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(operario);
        }

        // GET: operarios/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            operario operario = await db.operarios.FindAsync(id);
            if (operario == null)
            {
                return HttpNotFound();
            }
            return View(operario);
        }

        // POST: operarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            operario operario = await db.operarios.FindAsync(id);
            db.operarios.Remove(operario);
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
