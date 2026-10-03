using CasaGaillard.Core.Models;
using CasaGaillard.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace CasaGaillard.Core.Areas.Mantenimiento.Controllers
{
    [Area("Mantenimiento")]
    public class VehiculosController : Controller
    {
        private readonly ICrudVehiculosService _service;

        public VehiculosController(ICrudVehiculosService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(string matricula, string marca, string modelo, string tipo, int page = 1, bool incluirBajas = false)
        {
            var items = await _service.GetAllAsync(incluirBajas, page);
            var vm = new VehiculosIndexViewModel { PagedList = items };
            ViewBag.IncluirBajas = incluirBajas;
            return View(vm);
        }

        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                return View(new Vehiculo());
            }

            var vehiculo = await _service.GetByIdAsync(id);
            if (vehiculo == null)
                return NotFound();

            return View(vehiculo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(Vehiculo vehiculo)
        {
            if (!ModelState.IsValid)
            {
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

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return BadRequest();
            var v = await _service.GetByIdAsync(id.Value);
            if (v == null) return NotFound();
            return View(v);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();
            var v = await _service.GetByIdAsync(id.Value);
            if (v == null) return NotFound();
            return View(v);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
