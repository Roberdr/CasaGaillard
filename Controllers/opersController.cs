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
    public class opersController : Controller
    {
        private GaillardEntities db = new GaillardEntities();

        // GET: opers
        public async Task<ActionResult> Index()
        {
            return View(await db.opers.ToListAsync());
        }

        // GET: opers/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            oper oper = await db.opers.FindAsync(id);
            if (oper == null)
            {
                return HttpNotFound();
            }
            return View(oper);
        }

        // GET: opers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: opers/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,nombre")] oper oper)
        {
            if (ModelState.IsValid)
            {
                db.opers.Add(oper);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(oper);
        }

        // GET: opers/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            oper oper = await db.opers.FindAsync(id);
            if (oper == null)
            {
                return HttpNotFound();
            }
            return View(oper);
        }

        // POST: opers/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,nombre")] oper oper)
        {
            if (ModelState.IsValid)
            {
                db.Entry(oper).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(oper);
        }

        // GET: opers/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            oper oper = await db.opers.FindAsync(id);
            if (oper == null)
            {
                return HttpNotFound();
            }
            return View(oper);
        }

        // POST: opers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            oper oper = await db.opers.FindAsync(id);
            db.opers.Remove(oper);
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
