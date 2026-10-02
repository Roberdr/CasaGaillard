using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CasaGaillard.Core.Services;
using CasaGaillard.Core.Models;

namespace CasaGaillard.Core.Controllers
{
    [Area("Mantenimiento")]
    public class IntervencionesController : Controller
    {
        private readonly IIntervencionesService _service;

        public IntervencionesController(IIntervencionesService service)
        {
            _service = service;
        }

        // GET: Mantenimiento/Intervenciones
        public async Task<IActionResult> Index()
        {
            var items = await _service.GetAllAsync();
            return View(items);
        }

        // GET: Mantenimiento/Intervenciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var model = await _service.GetByIdAsync(id.Value);
            if (model == null)
                return NotFound();

            return View(model);
        }

        // GET: Mantenimiento/Intervenciones/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Mantenimiento/Intervenciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IntervencionViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _service.CreateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        // GET: Mantenimiento/Intervenciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var model = await _service.GetByIdAsync(id.Value);
            if (model == null)
                return NotFound();

            return View(model);
        }

        // POST: Mantenimiento/Intervenciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IntervencionViewModel model)
        {
            if (id == 0)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            await _service.UpdateAsync(id, model);
            return RedirectToAction(nameof(Index));
        }

        // POST: Mantenimiento/Intervenciones/AddRepuesto
        [HttpPost]
        public async Task<IActionResult> AddRepuesto(int intervenId, int repuestoId)
        {
            await _service.AddRepuestoAsync(intervenId, repuestoId);
            return Ok();
        }

        // POST: Mantenimiento/Intervenciones/RemoveRepuesto/5
        [HttpPost]
        public async Task<IActionResult> RemoveRepuesto(int id)
        {
            await _service.RemoveRepuestoAsync(id);
            return Ok();
        }
    }
}
