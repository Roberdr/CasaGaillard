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
    public class vehiculoesController : Controller
    {
        private GaillardEntities db = new GaillardEntities();

        // GET: vehiculoes
        public async Task<ActionResult> Index()
        {
            var vehiculoes = db.vehiculoes.Include(v => v.tipo_vehiculo);
            return View(await vehiculoes.ToListAsync());
        }

        // GET: vehiculoes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            vehiculo vehiculo = await db.vehiculoes.FindAsync(id);
            if (vehiculo == null)
            {
                return HttpNotFound();
            }
            return View(vehiculo);
        }

        // GET: vehiculoes/Create
        public ActionResult Create()
        {
            ViewBag.id_tipo_vehiculo = new SelectList(db.tipo_vehiculo, "id", "tipo_vehiculo1");
            return View();
        }

        // POST: vehiculoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,marca,modelo,matricula_vehiculo,id_tipo_vehiculo,modelo_tacografo,pma,tara,fecha_compra,taller_habitual")] vehiculo vehiculo)
        {
            if (ModelState.IsValid)
            {
                db.vehiculoes.Add(vehiculo);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.id_tipo_vehiculo = new SelectList(db.tipo_vehiculo, "id", "tipo_vehiculo1", vehiculo.id_tipo_vehiculo);
            return View(vehiculo);
        }

        // GET: vehiculoes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            vehiculo vehiculo = await db.vehiculoes.FindAsync(id);
            if (vehiculo == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_tipo_vehiculo = new SelectList(db.tipo_vehiculo, "id", "tipo_vehiculo1", vehiculo.id_tipo_vehiculo);
            return View(vehiculo);
        }

        // POST: vehiculoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,marca,modelo,matricula_vehiculo,id_tipo_vehiculo,modelo_tacografo,pma,tara,fecha_compra,taller_habitual")] vehiculo vehiculo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vehiculo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.id_tipo_vehiculo = new SelectList(db.tipo_vehiculo, "id", "tipo_vehiculo1", vehiculo.id_tipo_vehiculo);
            return View(vehiculo);
        }

        // GET: vehiculoes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            vehiculo vehiculo = await db.vehiculoes.FindAsync(id);
            if (vehiculo == null)
            {
                return HttpNotFound();
            }
            return View(vehiculo);
        }

        // POST: vehiculoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            vehiculo vehiculo = await db.vehiculoes.FindAsync(id);
            db.vehiculoes.Remove(vehiculo);
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
