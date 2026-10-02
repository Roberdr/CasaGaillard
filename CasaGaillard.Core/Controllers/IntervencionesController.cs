using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CasaGaillard.Core.Controllers
{
    [Area("Mantenimiento")]
    public class IntervencionesController : Controller
    {
        public IntervencionesController()
        {
            // TODO: inject services (DbContext, logger, etc.) via DI
        }

        // GET: Mantenimiento/Intervenciones
        public async Task<IActionResult> Index()
        {
            // TODO: implement data access
            return View();
        }

        // GET: Mantenimiento/Intervenciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            // TODO: load model
            return View();
        }

        // GET: Mantenimiento/Intervenciones/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Mantenimiento/Intervenciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(object model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // TODO: save model via DI service
            return RedirectToAction(nameof(Index));
        }

        // GET: Mantenimiento/Intervenciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            // TODO: load model for edit
            return View();
        }

        // POST: Mantenimiento/Intervenciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, object model)
        {
            if (id == 0)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            // TODO: update model via DI service
            return RedirectToAction(nameof(Index));
        }

        // POST: Mantenimiento/Intervenciones/AddRepuesto
        [HttpPost]
        public async Task<IActionResult> AddRepuesto(int intervenId, int repuestoId)
        {
            // TODO: implement
            return Ok();
        }

        // POST: Mantenimiento/Intervenciones/RemoveRepuesto/5
        [HttpPost]
        public async Task<IActionResult> RemoveRepuesto(int id)
        {
            // TODO: implement
            return Ok();
        }
    }
}
