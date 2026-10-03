using CasaGaillard.Models;
using CasaGaillard.Models.ViewModels;
using PagedList;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CasaGaillard.Areas.Mantenimiento.Controllers
{
    [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento + "," + AppRoles.Consulta)]
    public class VehiculosController : Controller
    {
        private readonly GaillardEntities db = new GaillardEntities();

        private void CargarListasVehiculo(Vehiculo vehiculo = null)
        {
            ViewBag.TipoVehiculoID = new SelectList(
                db.TiposVehiculo.OrderBy(tv => tv.Vehiculo),
                "ID",
                "Vehiculo",
                vehiculo?.TipoVehiculoID);

            ViewBag.TallerHabitualID = new SelectList(
                db.Entidads.OrderBy(e => e.NombreEntidad),
                "ID",
                "NombreEntidad",
                vehiculo?.TallerHabitualID);
        }

        // GET: Vehiculos
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento + "," + AppRoles.Consulta)]
        public ActionResult Index(string matricula, string marca, string modelo, string tipo, int page = 1, bool incluirBajas = false)
        {
            ViewBag.Matriculas = db.Vehiculos
                .Where(v => v.MatriculaVehiculo != null && v.MatriculaVehiculo != "")
                .Select(v => v.MatriculaVehiculo)
                .Distinct()
                .OrderBy(m => m)
                .ToList();
            ViewBag.Marcas = db.Vehiculos
                .Where(v => v.Marca != null && v.Marca != "")
                .Select(v => v.Marca)
                .Distinct()
                .OrderBy(m => m)
                .ToList();
            ViewBag.Modelos = db.Vehiculos
                .Where(v => v.Modelo != null && v.Modelo != "")
                .Select(v => v.Modelo)
                .Distinct()
                .OrderBy(m => m)
                .ToList();

            var vehiculos = db.Vehiculos
                .Include(v => v.TipoVehiculo)
                .Include(v => v.Taller)
                .OrderBy(v => v.MatriculaVehiculo)
                .AsQueryable();

            if (!incluirBajas)
            {
                vehiculos = vehiculos.Where(v => (v.Baja != true));
            }

            ViewBag.IncluirBajas = incluirBajas;

            if (!string.IsNullOrEmpty(matricula))
                vehiculos = vehiculos.Where(v => v.MatriculaVehiculo.Contains(matricula));

            if (!string.IsNullOrEmpty(marca))
                vehiculos = vehiculos.Where(v => v.Marca.Contains(marca));

            if (!string.IsNullOrEmpty(modelo))
                vehiculos = vehiculos.Where(v => v.Modelo.Contains(modelo));

            if (!string.IsNullOrEmpty(tipo))
                vehiculos = vehiculos.Where(v => v.TipoVehiculo.Vehiculo == tipo);

            var paged = vehiculos.OrderBy(v => v.MatriculaVehiculo)
                                 .ToPagedList(page, 8); // 10 por página

            var vm = new VehiculosIndexViewModel
            {
                PagedList = paged
            };

            return View(vm);
        }

        // GET: Vehiculos/AddOrEdit
        // GET: Vehiculos/AddOrEdit/1
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public ActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                CargarListasVehiculo();
                return View(new Vehiculo());
            }

            var vehiculo = db.Vehiculos.Find(id);
            if (vehiculo == null)
            {
                return HttpNotFound();
            }

            CargarListasVehiculo(vehiculo);
            return View(vehiculo);
        }

        // GET: Vehiculos/Details/5
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento + "," + AppRoles.Consulta)]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vehiculo vehiculo = await db.Vehiculos
                .Include(v => v.TipoVehiculo)
                .Include(v => v.Taller)
                .FirstOrDefaultAsync(v => v.ID == id);
            if (vehiculo == null)
            {
                return HttpNotFound();
            }
            return View(vehiculo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> AddOrEdit([Bind(Include = "ID,Marca,Modelo,MatriculaVehiculo,TipoVehiculoID,ModeloTacografo,Pma,Tara,FechaCompra,TallerHabitualID,Baja")] Vehiculo vehiculo)
        {
            if (ModelState.IsValid)
            {
                if (vehiculo.ID == 0)
                {
                    db.Vehiculos.Add(vehiculo);
                }
                else
                {
                    var vehiculoToUpdate = await db.Vehiculos.FindAsync(vehiculo.ID);
                    if (vehiculoToUpdate == null)
                    {
                        return HttpNotFound();
                    }

                    vehiculoToUpdate.Marca = vehiculo.Marca;
                    vehiculoToUpdate.Modelo = vehiculo.Modelo;
                    vehiculoToUpdate.MatriculaVehiculo = vehiculo.MatriculaVehiculo;
                    vehiculoToUpdate.TipoVehiculoID = vehiculo.TipoVehiculoID;
                    vehiculoToUpdate.ModeloTacografo = vehiculo.ModeloTacografo;
                    vehiculoToUpdate.Pma = vehiculo.Pma;
                    vehiculoToUpdate.Tara = vehiculo.Tara;
                    vehiculoToUpdate.FechaCompra = vehiculo.FechaCompra;
                    vehiculoToUpdate.TallerHabitualID = vehiculo.TallerHabitualID;
                    vehiculoToUpdate.Baja = vehiculo.Baja;
                }

                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            CargarListasVehiculo(vehiculo);
            return View(vehiculo);
        }

        // GET: Vehiculos/Delete/5
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
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
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
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
