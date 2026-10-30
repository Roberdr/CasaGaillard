using CasaGaillard.Core.Models;
using CasaGaillard.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using CasaGaillard.Core.Security;
using System.Threading.Tasks;

namespace CasaGaillard.Core.Areas.Mantenimiento.Controllers
{
    [Area("Mantenimiento")]
    [Authorize(Roles = AppRoles.LecturaMantenimiento)]
    public class VehiculosController : Controller
    {
        private readonly ICrudVehiculosService _service;

        public VehiculosController(ICrudVehiculosService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(string matricula, string marca, string modelo, string tipo, int page = 1, bool incluirBajas = false)
        {
            var model = await _service.GetAllAsync(matricula, marca, modelo, tipo, incluirBajas, page);
            return View(model);
        }

        [Authorize(Roles = AppRoles.GestionMantenimiento)]
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            Vehiculo vehiculo;
            if (id == 0)
            {
                vehiculo = new Vehiculo();
            }
            else
            {
                var existingVehiculo = await _service.GetByIdAsync(id);
                if (existingVehiculo == null)
                    return NotFound();

                vehiculo = existingVehiculo;
            }

            await CargarListasVehiculoAsync(vehiculo);
            return View(vehiculo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.GestionMantenimiento)]
        public async Task<IActionResult> AddOrEdit(
            [Bind("ID,Marca,Modelo,MatriculaVehiculo,TipoVehiculoID,ModeloTacografo,Pma,Tara,FechaCompra,TallerHabitualID,Baja")] Vehiculo vehiculo)
        {
            if (!ModelState.IsValid)
            {
                await CargarListasVehiculoAsync(vehiculo);
                return View(vehiculo);
            }

            if (vehiculo.ID == 0)
            {
                await _service.AddAsync(vehiculo);
            }
            else
            {
                await _service.UpdateAsync(vehiculo);
            }

            return RedirectToAction("Index");
        }

        private async Task CargarListasVehiculoAsync(Vehiculo vehiculo)
        {
            ViewBag.TipoVehiculoID = new SelectList(
                await _service.GetTiposVehiculoAsync(),
                "ID",
                "Vehiculo",
                vehiculo.TipoVehiculoID);
            ViewBag.TallerHabitualID = new SelectList(
                await _service.GetTalleresAsync(),
                "ID",
                "NombreEntidad",
                vehiculo.TallerHabitualID);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return BadRequest();
            var v = await _service.GetByIdAsync(id.Value);
            if (v == null) return NotFound();
            return View(v);
        }

        [Authorize(Roles = AppRoles.GestionMantenimiento)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();
            var v = await _service.GetByIdAsync(id.Value);
            if (v == null) return NotFound();
            return View(v);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.GestionMantenimiento)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
