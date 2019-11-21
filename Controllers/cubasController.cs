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
    public class CubasController : Controller
    {
        private GaillardEntities db = new GaillardEntities();

        // GET: Cubas
        public async Task<ActionResult> Index()
        {
            var cubas = db.Cubas.Include(c => c.Material).Include(c => c.Plataforma);
            return View(await cubas.ToListAsync());
        }

        // GET: Cubas/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cuba cuba = await db.Cubas.FindAsync(id);
            if (cuba == null)
            {
                return HttpNotFound();
            }
            return View(cuba);
        }

        // GET: Cubas/Create
        public ActionResult Create()
        {
            ViewBag.MaterialExteriorID = new SelectList(db.Materials, "ID", "Material1");
            ViewBag.PlataformaID = new SelectList(db.Plataformas, "ID", "MatriculaPlataforma");
            return View();
        }

        // POST: Cubas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,MatriculaCuba,NumCuadro,Codigo,Constructor,NumFabricacion,NumHomologacion,FechaConstruccion,PaisFabricacion,NumTipoIMO,PaisAprobacion,Autoridad,CodigoDiseno,PruebaHidraulica,PresionServicioADR,PresionServicioIMO,PresionExterior,PresionTaradoValvulas,TemperaturaCalculoReferencia,PesoBruto,Tara,PesoMaxProducto,MaterialExteriorID,EspesorCuerpo,EspesorFondo,EspesorEquivalente,TipoForro,NumAprobacionCSC,Modelo,PesoMaxApilamiento,CargaRigidez,PresionPrueba,TemperaturaMinCarga,PlataformaID,Longitud,Ancho,Alto,UpdatedAt,CreatedAt,UpdatedBy,CreatedBy,NumAprobacionIMDG,NumAprobacionADR_RID,UNPortableTank,NumAprobacion")] Cuba cuba)
        {
            if (ModelState.IsValid)
            {
                db.Cubas.Add(cuba);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.MaterialExteriorID = new SelectList(db.Materials, "ID", "Material1", cuba.MaterialExteriorID);
            ViewBag.PlataformaID = new SelectList(db.Plataformas, "ID", "MatriculaPlataforma", cuba.PlataformaID);
            return View(cuba);
        }

        // GET: Cubas/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cuba cuba = await db.Cubas.FindAsync(id);
            if (cuba == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaterialExteriorID = new SelectList(db.Materials, "ID", "Material1", cuba.MaterialExteriorID);
            ViewBag.PlataformaID = new SelectList(db.Plataformas, "ID", "MatriculaPlataforma", cuba.PlataformaID);
            return View(cuba);
        }

        // POST: Cubas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,MatriculaCuba,NumCuadro,Codigo,Constructor,NumFabricacion,NumHomologacion,FechaConstruccion,PaisFabricacion,NumTipoIMO,PaisAprobacion,Autoridad,CodigoDiseno,PruebaHidraulica,PresionServicioADR,PresionServicioIMO,PresionExterior,PresionTaradoValvulas,TemperaturaCalculoReferencia,PesoBruto,Tara,PesoMaxProducto,MaterialExteriorID,EspesorCuerpo,EspesorFondo,EspesorEquivalente,TipoForro,NumAprobacionCSC,Modelo,PesoMaxApilamiento,CargaRigidez,PresionPrueba,TemperaturaMinCarga,PlataformaID,Longitud,Ancho,Alto,UpdatedAt,CreatedAt,UpdatedBy,CreatedBy,NumAprobacionIMDG,NumAprobacionADR_RID,UNPortableTank,NumAprobacion")] Cuba cuba)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cuba).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.MaterialExteriorID = new SelectList(db.Materials, "ID", "Material1", cuba.MaterialExteriorID);
            ViewBag.PlataformaID = new SelectList(db.Plataformas, "ID", "MatriculaPlataforma", cuba.PlataformaID);
            return View(cuba);
        }

        // GET: Cubas/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cuba cuba = await db.Cubas.FindAsync(id);
            if (cuba == null)
            {
                return HttpNotFound();
            }
            return View(cuba);
        }

        // POST: Cubas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Cuba cuba = await db.Cubas.FindAsync(id);
            db.Cubas.Remove(cuba);
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
