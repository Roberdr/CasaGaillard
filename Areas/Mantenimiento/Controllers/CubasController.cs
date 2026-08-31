using System;
using System.IO;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using CasaGaillard.Models;
using System.Collections.Generic;

namespace CasaGaillard.Areas.Mantenimiento.Controllers
{
    [Authorize]
    public class CubasController : Controller
    {
        private readonly GaillardEntities db = new GaillardEntities();

        private void CargarListasCuba(Cuba cuba = null)
        {
            var vehiculos = db.Vehiculos.AsQueryable();
            if (cuba == null || !cuba.PlataformaID.HasValue)
            {
                vehiculos = vehiculos.Where(v => !v.Baja);
            }
            else
            {
                vehiculos = vehiculos.Where(v => !v.Baja || v.ID == cuba.PlataformaID.Value);
            }

            ViewBag.MaterialExteriorID = new SelectList(
                db.Materiales.OrderBy(m => m.Material1),
                "ID",
                "Material1",
                cuba?.MaterialExteriorID);

            ViewBag.PlataformaID = new SelectList(
                vehiculos.OrderBy(v => v.MatriculaVehiculo),
                "ID",
                "MatriculaVehiculo",
                cuba?.PlataformaID);
        }

        // GET: Cubas
        public async Task<ActionResult> Index(bool incluirBajas = false)
        {
            var cubas = db.Cubas
                .Include(c => c.Material)
                .Include(c => c.Vehiculo)
                .Include(c => c.Revisions)
                .Where(c => incluirBajas || !c.Baja)
                .OrderBy(c => c.MatriculaCuba);

            ViewBag.IncluirBajas = incluirBajas;

            return View(await cubas.ToListAsync());
        }

        // GET: Cubas/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cuba cuba = await db.Cubas
                .Include(c => c.Material)
                .Include(c => c.Vehiculo)
                .Include(c => c.Revisions)
                .FirstOrDefaultAsync(c => c.ID == id);
            if (cuba == null)
            {
                return HttpNotFound();
            }
            if (cuba.PesoMaxProducto == null || cuba.PesoMaxProducto == 0)
            {
                cuba.PesoMaxProducto = cuba.PesoBruto - cuba.Tara;
            }

            // Comprueba que haya un directorio con la matricula de la cuba a detallar
            // y crea una lista de los archivos existentes

            string imgPath = Server.MapPath("~/Content/images/");
            List<string> nameFiles = new List<string>();
            string cubaImagePath = Path.Combine(imgPath, cuba.MatriculaCuba.ToString());
            if (Directory.Exists(cubaImagePath))
            {
                List<string> files = new List<string>(Directory.EnumerateFiles(cubaImagePath));

                foreach (string f in files)
                {
                    nameFiles.Add(Path.GetFileName(f));
                }
            }
/*            string imgPath = "C:/inetpub/wwwroot/CG/Content/images/";
            string docPath;
            List<string> nameFiles = new List<string>();
            List<string> d = new List<string>(Directory.EnumerateDirectories(imgPath));
            if (d.Contains(imgPath + cuba.MatriculaCuba.ToString()))
            {
                docPath = imgPath + cuba.MatriculaCuba.ToString() + '/';
                List<string> files = new List<string>(Directory.EnumerateFiles(docPath));

                foreach (string f in files)
                {
                    var pos = f.LastIndexOf("/");
                    nameFiles.Add(f.Substring(pos));
                }
            }*/
            ViewBag.files = nameFiles;
            return View(cuba);
        }

        // GET: Cubas/Create
        public ActionResult Create()
        {
            CargarListasCuba();
            return View();
        }

        // POST: Cubas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,MatriculaCuba,NumCuadro,Codigo,Constructor,NumFabricacion,NumHomologacion,FechaConstruccion,PaisFabricacion,NumTipoIMO,PaisAprobacion,Autoridad,CodigoDiseno,PruebaHidraulica,PresionServicioADR,PresionServicioIMO,PresionExterior,PresionTaradoValvulas,TemperaturaCalculoReferencia,PesoBruto,Tara,PesoMaxProducto,MaterialExteriorID,EspesorCuerpo,EspesorFondo,EspesorEquivalente,TipoForro,NumAprobacionCSC,Modelo,PesoMaxApilamiento,CargaRigidez,PresionPrueba,TemperaturaMinCarga,PlataformaID,Longitud,Ancho,Alto,UpdatedAt,CreatedAt,UpdatedBy,CreatedBy,NumAprobacionIMDG,NumAprobacionADR_RID,UNPortableTank,NumAprobacion,Baja")] Cuba cuba)
        {
            if (ModelState.IsValid)
            {
                cuba.CreatedAt = DateTime.Now;
                cuba.UpdatedAt = DateTime.Now;
                db.Cubas.Add(cuba);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            CargarListasCuba(cuba);
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
            CargarListasCuba(cuba);
            return View(cuba);
        }

        // POST: Cubas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,MatriculaCuba,NumCuadro,Codigo,Constructor,NumFabricacion,NumHomologacion,FechaConstruccion,PaisFabricacion,NumTipoIMO,PaisAprobacion,Autoridad,CodigoDiseno,PruebaHidraulica,PresionServicioADR,PresionServicioIMO,PresionExterior,PresionTaradoValvulas,TemperaturaCalculoReferencia,PesoBruto,Tara,PesoMaxProducto,MaterialExteriorID,EspesorCuerpo,EspesorFondo,EspesorEquivalente,TipoForro,NumAprobacionCSC,Modelo,PesoMaxApilamiento,CargaRigidez,PresionPrueba,TemperaturaMinCarga,PlataformaID,Longitud,Ancho,Alto,UpdatedAt,CreatedAt,UpdatedBy,CreatedBy,NumAprobacionIMDG,NumAprobacionADR_RID,UNPortableTank,NumAprobacion,Baja")] Cuba cuba)
        {
            if (ModelState.IsValid)
            {
                cuba.UpdatedAt = DateTime.Now;
                db.Entry(cuba).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            CargarListasCuba(cuba);
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
