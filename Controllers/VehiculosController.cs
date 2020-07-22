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
    public class VehiculosController : Controller
    {
        private GaillardEntities db = new GaillardEntities();

        // GET: Vehiculos
        public async Task<ActionResult> Index()
        {
            var vehiculos = db.Vehiculos.Include(v => v.TipoVehiculo);
            return View(await vehiculos.ToListAsync());
        }

        // GET: Vehiculos/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vehiculo vehiculo = await db.Vehiculos.FindAsync(id);
            if (vehiculo == null)
            {
                return HttpNotFound();
            }
            return View(vehiculo);
        }

        // GET: Vehiculos/Create
        public ActionResult Create()
        {
            ViewBag.TipoVehiculoID = new SelectList(db.TiposVehiculo, "ID", "Vehiculo");
            return View();
        }

        // POST: Vehiculos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,Marca,Modelo,MatriculaVehiculo,TipoVehiculoID,ModeloTacografo,Pma,Tara,FechaCompra,TallerHabitual")] Vehiculo vehiculo)
        {
            if (ModelState.IsValid)
            {
                db.Vehiculos.Add(vehiculo);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.TipoVehiculoID = new SelectList(db.TiposVehiculo, "ID", "Vehiculo", vehiculo.TipoVehiculoID);
            return View(vehiculo);
        }

        // GET: Vehiculos/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vehiculo vehiculo = await db.Vehiculos.FindAsync(id);
            if (vehiculo == null)
            {
                return HttpNotFound();
            }
            ViewBag.TipoVehiculoID = new SelectList(db.TiposVehiculo, "ID", "Vehiculo", vehiculo.TipoVehiculoID);
            return View(vehiculo);
        }

        // POST: Vehiculos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,Marca,Modelo,MatriculaVehiculo,TipoVehiculoID,ModeloTacografo,Pma,Tara,FechaCompra,TallerHabitual")] Vehiculo vehiculo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vehiculo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.TipoVehiculoID = new SelectList(db.TiposVehiculo, "ID", "TipoVehiculo1", vehiculo.TipoVehiculoID);
            return View(vehiculo);
        }

        // GET: Vehiculos/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vehiculo vehiculo = await db.Vehiculos.FindAsync(id);
            if (vehiculo == null)
            {
                return HttpNotFound();
            }
            return View(vehiculo);
        }

        // POST: Vehiculos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Vehiculo vehiculo = await db.Vehiculos.FindAsync(id);
            db.Vehiculos.Remove(vehiculo);
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
