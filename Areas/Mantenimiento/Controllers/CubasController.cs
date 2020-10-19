using System;
using System.IO;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CasaGaillard.Models;
using System.Threading;
using System.Globalization;
<<<<<<<< HEAD:Areas/Mantenimiento/Controllers/CubasController.cs
using System.Collections.Generic;
========
>>>>>>>> Nueva:Controllers/cubasController.cs

namespace CasaGaillard.Areas.Mantenimiento.Controllers
{
    [Authorize]
    public class CubasController : Controller
    {
        private readonly GaillardEntities db = new GaillardEntities();

        // GET: Cubas
        public async Task<ActionResult> Index()
        {
            var cubas = db.Cubas.Include(c => c.Material).Include(c => c.Plataforma).OrderBy(c => c.MatriculaCuba);
            return View(await cubas.ToListAsync());
        }

        // GET: Cubas/Details/5
        public async Task<ActionResult> Details(int? id)
        {

           
            CultureInfo culture1 = CultureInfo.CurrentCulture;
            CultureInfo culture2 = Thread.CurrentThread.CurrentCulture;
            System.Diagnostics.Debug.WriteLine("The current culture is {0}", culture1.Name);
            System.Diagnostics.Debug.WriteLine("The two CultureInfo objects are equal: {0}",
                              culture1 == culture2);
            ViewBag.culture1 = culture1;
            ViewBag.culture2 = culture2;
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cuba cuba = await db.Cubas.FindAsync(id);
            if (cuba == null)
            {
                return HttpNotFound();
            }
            if (cuba.PesoMaxProducto == null || cuba.PesoMaxProducto == 0)
            {
                cuba.PesoMaxProducto = cuba.PesoBruto - cuba.Tara;
            }
<<<<<<<< HEAD:Areas/Mantenimiento/Controllers/CubasController.cs

            // Comprueba que haya un directorio con la matricula de la cuba a detallar
            // y crea una lista de los archivos existentes

            string imgPath = "C:/Users/magat/source/repos/CasaGaillard/Content/images/";
            string docPath;
            List<string> nameFiles = new List<string>();
            List<string> d = new List<string>(Directory.EnumerateDirectories(imgPath));
            if (d.Contains(imgPath + cuba.MatriculaCuba.ToString()))
            {
                docPath = imgPath + cuba.MatriculaCuba.ToString() + "/";
                List<string> files = new List<string>(Directory.EnumerateFiles(docPath));

                foreach (string f in files)
                {
                    var pos = f.LastIndexOf("/");
                    nameFiles.Add(f.Substring(pos));
                }
            }
            ViewBag.files = nameFiles;
========
>>>>>>>> Nueva:Controllers/cubasController.cs
            return View(cuba);
        }

        // GET: Cubas/Create
        public ActionResult Create()
        {
            ViewBag.MaterialExteriorID = new SelectList(db.Materiales, "ID", "Material1");
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
                cuba.CreatedAt = DateTime.Now;
                db.Cubas.Add(cuba);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.MaterialExteriorID = new SelectList(db.Materiales, "ID", "Material1", cuba.MaterialExteriorID);
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
            ViewBag.MaterialExteriorID = new SelectList(db.Materiales, "ID", "Material1", cuba.MaterialExteriorID);
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
                cuba.UpdatedAt = DateTime.Now;
                db.Entry(cuba).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.MaterialExteriorID = new SelectList(db.Materiales, "ID", "Material1", cuba.MaterialExteriorID);
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
