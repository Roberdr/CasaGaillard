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
    public class tipo_grupoController : Controller
    {
        private GaillardEntities db = new GaillardEntities();

        // GET: tipo_grupo
        public async Task<ActionResult> Index()
        {
            return View(await db.tipo_grupo.ToListAsync());
        }

        // GET: tipo_grupo/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tipo_grupo tipo_grupo = await db.tipo_grupo.FindAsync(id);
            if (tipo_grupo == null)
            {
                return HttpNotFound();
            }
            return View(tipo_grupo);
        }

        // GET: tipo_grupo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: tipo_grupo/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,nombre_grupo")] tipo_grupo tipo_grupo)
        {
            if (ModelState.IsValid)
            {
                db.tipo_grupo.Add(tipo_grupo);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(tipo_grupo);
        }

        // GET: tipo_grupo/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tipo_grupo tipo_grupo = await db.tipo_grupo.FindAsync(id);
            if (tipo_grupo == null)
            {
                return HttpNotFound();
            }
            return View(tipo_grupo);
        }

        // POST: tipo_grupo/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,nombre_grupo")] tipo_grupo tipo_grupo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipo_grupo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(tipo_grupo);
        }

        // GET: tipo_grupo/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tipo_grupo tipo_grupo = await db.tipo_grupo.FindAsync(id);
            if (tipo_grupo == null)
            {
                return HttpNotFound();
            }
            return View(tipo_grupo);
        }

        // POST: tipo_grupo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            tipo_grupo tipo_grupo = await db.tipo_grupo.FindAsync(id);
            db.tipo_grupo.Remove(tipo_grupo);
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
