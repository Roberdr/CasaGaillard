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

namespace CasaGaillard.Areas.Mantenimiento.Controllers
{
    [Authorize]
    public class VehiculosController : Controller
    {
        private readonly GaillardEntities db = new GaillardEntities();

        // GET: Vehiculos
        public ActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> GetVehiculos()
        {
            var vehiculos = db.Vehiculos.Include(v => v.TipoVehiculo);
            return Json(await vehiculos.ToListAsync(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PoblarSelects()
        {
            var selTipoVehiculo = new SelectList(db.TiposVehiculo, "ID", "Vehiculo");
            var selCombustible = new SelectList(db.Combustibles, "ID", "Combustible1");
            var selSeguro = new SelectList(db.Seguroes, "ID", "Compania");

            var selects = new { selTipoVehiculo, selCombustible, selSeguro };

            return Json(new { selTipoVehiculo, selCombustible, selSeguro }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetVehiculoByID(int? VehiculoID)
        {
            try
            {
                var vehiculo =  db.Vehiculos.Include(v => v.TipoVehiculo).Where(v => v.ID == VehiculoID).FirstOrDefault();
                var selTipoVehiculo = new SelectList(db.TiposVehiculo, "ID", "Vehiculo");
                var selCombustible = new SelectList(db.Combustibles, "ID", "Combustible1");
                var selSeguro = new SelectList(db.Seguroes, "ID", "Compania");

                return Json(new { vehiculo, selTipoVehiculo, selCombustible, selSeguro }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return (Json(null, JsonRequestBehavior.AllowGet));
            }
        }

        public JsonResult UpdateVehiculo(Vehiculo vehiculo)
        {

            string status = "success";
            try
            {
                db.Entry(vehiculo).State = EntityState.Modified;
                db.SaveChanges();

            }
            catch (Exception ex)
            {
                status = ex.Message;

            }
            return Json(vehiculo, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteVehiculo(int vehiculoId)
        {
            string status = "success";
            try
            {

                var vehiculo = db.Vehiculos.Find(vehiculoId);
                db.Vehiculos.Remove(vehiculo);
                db.SaveChanges();

            }
            catch (Exception ex)
            {
                status = ex.Message;

            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddVehiculo(Vehiculo vehiculo)
        {
            string status = "success";
            try
            {
                db.Vehiculos.Add(vehiculo);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                status = ex.Message;
            }
            return Json(status, JsonRequestBehavior.AllowGet);

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

        // GET: Vehiculos/AddOrEdit
        // GET: Vehiculos/AddOrEdit/1
        public async Task<ActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                ViewBag.TipoVehiculoID = new SelectList(db.TiposVehiculo, "ID", "Vehiculo");
                ViewBag.TallerHabitualID = new SelectList(db.Entidades, "ID", "NombreEntidad");
                return View(new Vehiculo());
            }
            else
            {
                Vehiculo vehiculo = await db.Vehiculos.FindAsync(id);
                if (vehiculo == null)
                {
                    return HttpNotFound();
                }
                ViewBag.TipoVehiculoID = new SelectList(db.TiposVehiculo, "ID", "Vehiculo", vehiculo.TipoVehiculoID);
                ViewBag.TallerHabitualID = new SelectList(db.Entidades, "ID", "NombreEntidad", vehiculo.TallerHabitualID);
                return View(vehiculo);
            }
            
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
            ViewBag.TallerHabitualID = new SelectList(db.Entidades, "ID", "NombreEntidad", vehiculo.TallerHabitualID);
            return View(vehiculo);
        }

        // POST: Vehiculos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AddOrEdit(int id, [Bind(Include = "ID,Marca,Modelo,MatriculaVehiculo,TipoVehiculoID,ModeloTacografo,Pma,Tara,FechaCompra,TallerHabitual")] Vehiculo vehiculo)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    db.Vehiculos.Add(vehiculo);
                    await db.SaveChangesAsync();
                }
                else
                {                  
                    db.Entry(vehiculo).State = EntityState.Modified;
                    await db.SaveChangesAsync();                    
                }
                return Json(new { isValid = "true", html = "11" });
            }
            ViewBag.TipoVehiculoID = new SelectList(db.TiposVehiculo, "ID", "TipoVehiculo1", vehiculo.TipoVehiculoID);
            return Json(new { isValid = "false", html = ""});
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
