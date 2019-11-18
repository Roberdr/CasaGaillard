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
    public class compartimentoesController : Controller
    {
        private GaillardEntities db = new GaillardEntities();

        // GET: compartimentoes
        public async Task<ActionResult> Index()
        {
            var compartimentoes = db.compartimentoes.Include(c => c.cuba).Include(c => c.material).Include(c => c.producto);
            return View(await compartimentoes.ToListAsync());
        }

        // GET: compartimentoes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            compartimento compartimento = await db.compartimentoes.FindAsync(id);
            if (compartimento == null)
            {
                return HttpNotFound();
            }
            return View(compartimento);
        }

        // GET: compartimentoes/Create
        public ActionResult Create()
        {
            ViewBag.cuba_id = new SelectList(db.cubas, "id", "cuba1");
            ViewBag.material_interior_id = new SelectList(db.materials, "id", "material1");
            ViewBag.producto_id = new SelectList(db.productoes, "id", "iupac");
            return View();
        }

        // POST: compartimentoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,cuba_id,numero,capacidad,producto_id,material_interior_id")] compartimento compartimento)
        {
            if (ModelState.IsValid)
            {
                db.compartimentoes.Add(compartimento);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.cuba_id = new SelectList(db.cubas, "id", "cuba1", compartimento.cuba_id);
            ViewBag.material_interior_id = new SelectList(db.materials, "id", "material1", compartimento.material_interior_id);
            ViewBag.producto_id = new SelectList(db.productoes, "id", "iupac", compartimento.producto_id);
            return View(compartimento);
        }

        // GET: compartimentoes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            compartimento compartimento = await db.compartimentoes.FindAsync(id);
            if (compartimento == null)
            {
                return HttpNotFound();
            }
            ViewBag.cuba_id = new SelectList(db.cubas, "id", "cuba1", compartimento.cuba_id);
            ViewBag.material_interior_id = new SelectList(db.materials, "id", "material1", compartimento.material_interior_id);
            ViewBag.producto_id = new SelectList(db.productoes, "id", "iupac", compartimento.producto_id);
            return View(compartimento);
        }

        // POST: compartimentoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,cuba_id,numero,capacidad,producto_id,material_interior_id")] compartimento compartimento)
        {
            if (ModelState.IsValid)
            {
                db.Entry(compartimento).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.cuba_id = new SelectList(db.cubas, "id", "cuba1", compartimento.cuba_id);
            ViewBag.material_interior_id = new SelectList(db.materials, "id", "material1", compartimento.material_interior_id);
            ViewBag.producto_id = new SelectList(db.productoes, "id", "iupac", compartimento.producto_id);
            return View(compartimento);
        }

        // GET: compartimentoes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            compartimento compartimento = await db.compartimentoes.FindAsync(id);
            if (compartimento == null)
            {
                return HttpNotFound();
            }
            return View(compartimento);
        }

        // POST: compartimentoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            compartimento compartimento = await db.compartimentoes.FindAsync(id);
            db.compartimentoes.Remove(compartimento);
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
