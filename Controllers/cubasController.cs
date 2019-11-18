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
    public class cubasController : Controller
    {
        private GaillardEntities db = new GaillardEntities();

        // GET: cubas
        public async Task<ActionResult> Index()
        {
            var cubas = db.cubas.Include(c => c.material).Include(c => c.plataforma);
            return View(await cubas.ToListAsync());
        }

        // GET: cubas/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cuba cuba = await db.cubas.FindAsync(id);
            if (cuba == null)
            {
                return HttpNotFound();
            }
            return View(cuba);
        }

        // GET: cubas/Create
        public ActionResult Create()
        {
            ViewBag.material_exterior_id = new SelectList(db.materials, "id", "material1");
            ViewBag.plataforma_id = new SelectList(db.plataformas, "id", "matricula");
            return View();
        }

        // POST: cubas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,cuba1,num_cuadro,codigo,constructor,num_fabricacion,num_homologacion,fecha_construccion,pais_fabricacion,num_tipo_imo,pais_aprobacion,autoridad,codigo_diseno,prueba_hidraulica,presion_servicio_ADR,presion_servicio_IMO,presion_exterior,presion_tarado_valvulas,temperatura_calculo_referencia,peso_bruto,tara,peso_max_producto,material_exterior_id,espesor_cuerpo,espesor_fondo,espesor_equivalente,tipo_forro,num_aprobacion_CSC,modelo,peso_max_apilamiento,carga_rigidez,presion_prueba,temperatura_min_carga,plataforma_id,longitud,ancho,alto,updated_at,created_at,updated_by,created_by,num_aprobacion_IMDG,num_aprobacion_ADR_RID,UN_portable_tank,num_aprobacion")] cuba cuba)
        {
            if (ModelState.IsValid)
            {
                db.cubas.Add(cuba);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.material_exterior_id = new SelectList(db.materials, "id", "material1", cuba.material_exterior_id);
            ViewBag.plataforma_id = new SelectList(db.plataformas, "id", "matricula", cuba.plataforma_id);
            return View(cuba);
        }

        // GET: cubas/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cuba cuba = await db.cubas.FindAsync(id);
            if (cuba == null)
            {
                return HttpNotFound();
            }
            ViewBag.material_exterior_id = new SelectList(db.materials, "id", "material1", cuba.material_exterior_id);
            ViewBag.plataforma_id = new SelectList(db.plataformas, "id", "matricula", cuba.plataforma_id);
            return View(cuba);
        }

        // POST: cubas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,cuba1,num_cuadro,codigo,constructor,num_fabricacion,num_homologacion,fecha_construccion,pais_fabricacion,num_tipo_imo,pais_aprobacion,autoridad,codigo_diseno,prueba_hidraulica,presion_servicio_ADR,presion_servicio_IMO,presion_exterior,presion_tarado_valvulas,temperatura_calculo_referencia,peso_bruto,tara,peso_max_producto,material_exterior_id,espesor_cuerpo,espesor_fondo,espesor_equivalente,tipo_forro,num_aprobacion_CSC,modelo,peso_max_apilamiento,carga_rigidez,presion_prueba,temperatura_min_carga,plataforma_id,longitud,ancho,alto,updated_at,created_at,updated_by,created_by,num_aprobacion_IMDG,num_aprobacion_ADR_RID,UN_portable_tank,num_aprobacion")] cuba cuba)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cuba).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.material_exterior_id = new SelectList(db.materials, "id", "material1", cuba.material_exterior_id);
            ViewBag.plataforma_id = new SelectList(db.plataformas, "id", "matricula", cuba.plataforma_id);
            return View(cuba);
        }

        // GET: cubas/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cuba cuba = await db.cubas.FindAsync(id);
            if (cuba == null)
            {
                return HttpNotFound();
            }
            return View(cuba);
        }

        // POST: cubas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            cuba cuba = await db.cubas.FindAsync(id);
            db.cubas.Remove(cuba);
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
