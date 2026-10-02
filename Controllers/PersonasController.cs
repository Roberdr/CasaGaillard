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
    public class PersonasController : Controller
    {
        private readonly GaillardEntities db = new GaillardEntities();

        // GET: Personas
        public async Task<ActionResult> Index()
        {
            var personas = db.Personas.Include(p => p.Direccion).OrderBy(p => p.NombrePersona).ThenBy(p => p.Apellido1);
            return View(await personas.ToListAsync());
        }

        // GET: Personas/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Persona persona = await db.Personas.Include(p => p.Direccion.Poblacion).Include(p => p.TelefonosPersona)
                .Include(p => p.PersonasEntidad.Select(pe => pe.Entidad)).Include(p => p.PersonasEntidad.Select(pe => pe.Cargo))
                .FirstOrDefaultAsync(p => p.ID == id.Value);
            if (persona == null)
            {
                return HttpNotFound();
            }
            CargarOpcionesRelacion();
            ViewBag.ReturnTo = "Details";
            return View(persona);
        }

        // GET: Personas/Create
        public ActionResult Create()
        {
            CargarDirecciones();
            return View();
        }

        // POST: Personas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "NombrePersona,Apellido1,Apellido2,NIF,DireccionID")] Persona persona)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction(IsolationLevel.Serializable))
                {
                    persona.ID = await db.Database.SqlQuery<int>("SELECT ISNULL(MAX(ID), 0) + 1 FROM gaillard.Persona WITH (UPDLOCK, HOLDLOCK)").SingleAsync();
                    db.Personas.Add(persona);
                    await db.SaveChangesAsync();
                    transaction.Commit();
                }
                return RedirectToAction("Index");
            }

            CargarDirecciones(persona.DireccionID);
            return View(persona);
        }

        // GET: Personas/Edit/5
        public async Task<ActionResult> Edit(int? id, int? direccionID = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Persona persona = await db.Personas.Include(p => p.TelefonosPersona)
                .Include(p => p.PersonasEntidad.Select(pe => pe.Entidad))
                .Include(p => p.PersonasEntidad.Select(pe => pe.Cargo))
                .FirstOrDefaultAsync(p => p.ID == id.Value);
            if (persona == null)
            {
                return HttpNotFound();
            }
            if (direccionID.HasValue && await db.Direcciones.AnyAsync(d => d.ID == direccionID.Value))
            {
                persona.DireccionID = direccionID.Value;
            }
            CargarDirecciones(persona.DireccionID);
            CargarOpcionesRelacion();
            ViewBag.ReturnTo = "Edit";
            return View(persona);
        }

        // POST: Personas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,NombrePersona,Apellido1,Apellido2,NIF,DireccionID")] Persona persona)
        {
            if (ModelState.IsValid)
            {
                var actual = await db.Personas.FindAsync(persona.ID);
                if (actual == null) return HttpNotFound();
                actual.NombrePersona = persona.NombrePersona;
                actual.Apellido1 = persona.Apellido1;
                actual.Apellido2 = persona.Apellido2;
                actual.NIF = persona.NIF;
                actual.DireccionID = persona.DireccionID;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            var datosRelacionados = await db.Personas.Include(p => p.TelefonosPersona)
                .Include(p => p.PersonasEntidad.Select(pe => pe.Entidad))
                .Include(p => p.PersonasEntidad.Select(pe => pe.Cargo))
                .FirstOrDefaultAsync(p => p.ID == persona.ID);
            if (datosRelacionados != null)
            {
                persona.TelefonosPersona = datosRelacionados.TelefonosPersona;
                persona.PersonasEntidad = datosRelacionados.PersonasEntidad;
            }
            CargarDirecciones(persona.DireccionID);
            CargarOpcionesRelacion();
            ViewBag.ReturnTo = "Edit";
            return View(persona);
        }

        // GET: Personas/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Persona persona = await db.Personas.FindAsync(id);
            if (persona == null)
            {
                return HttpNotFound();
            }
            return View(persona);
        }

        // POST: Personas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Persona persona = await db.Personas.FindAsync(id);
            if (persona == null) return HttpNotFound();
            try
            {
                db.Personas.Remove(persona);
                await db.SaveChangesAsync();
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                ModelState.AddModelError("", "No se puede eliminar esta persona porque tiene teléfonos, relaciones con entidades u otros registros asociados.");
                return View("Delete", persona);
            }
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> AgregarTelefono(int personaID, string telefono, string returnTo = "Details")
        {
            var persona = await db.Personas.FindAsync(personaID);
            if (persona == null) return HttpNotFound();
            telefono = (telefono ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(telefono) || telefono.Length > 10)
            {
                TempData["PersonaError"] = "Introduce un teléfono de hasta 10 caracteres.";
                return RedirectToAction(AccionRetorno(returnTo), new { id = personaID });
            }
            await db.Database.ExecuteSqlCommandAsync(@"SET XACT_ABORT ON;
                BEGIN TRANSACTION;
                DECLARE @id INT;
                SELECT @id = ISNULL(MAX(ID), 0) + 1 FROM gaillard.TelefonoPersona WITH (UPDLOCK, HOLDLOCK);
                INSERT INTO gaillard.TelefonoPersona (ID, TelefonoPersona, PersonaID) VALUES (@id, @p0, @p1);
                COMMIT TRANSACTION;", telefono, personaID);
            return RedirectToAction(AccionRetorno(returnTo), new { id = personaID });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> ActualizarTelefono(int personaID, int id, string telefono, string returnTo = "Details")
        {
            telefono = (telefono ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(telefono) || telefono.Length > 10)
            {
                TempData["PersonaError"] = "Introduce un teléfono de hasta 10 caracteres.";
                return RedirectToAction(AccionRetorno(returnTo), new { id = personaID });
            }
            await db.Database.ExecuteSqlCommandAsync("UPDATE gaillard.TelefonoPersona SET TelefonoPersona=@p0 WHERE ID=@p1 AND PersonaID=@p2", telefono, id, personaID);
            return RedirectToAction(AccionRetorno(returnTo), new { id = personaID });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> EliminarTelefono(int personaID, int id, string returnTo = "Details")
        {
            await db.Database.ExecuteSqlCommandAsync("DELETE FROM gaillard.TelefonoPersona WHERE ID=@p0 AND PersonaID=@p1", id, personaID);
            return RedirectToAction(AccionRetorno(returnTo), new { id = personaID });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> AgregarEntidad(int personaID, int entidadID, int? cargoID, DateTime? fechaAlta, string returnTo = "Details")
        {
            if (!await db.Personas.AnyAsync(p => p.ID == personaID) || !await db.Entidads.AnyAsync(e => e.ID == entidadID)) return HttpNotFound();
            if (cargoID.HasValue && !await db.Cargoes.AnyAsync(c => c.ID == cargoID.Value)) return HttpNotFound();
            if (await db.PersonasEntidads.AnyAsync(r => r.PersonaID == personaID && r.EntidadID == entidadID && r.FechaBaja == null))
            {
                TempData["PersonaError"] = "Esta persona ya tiene una relación activa con esa entidad.";
                return RedirectToAction(AccionRetorno(returnTo), new { id = personaID });
            }

            await db.Database.ExecuteSqlCommandAsync(@"SET XACT_ABORT ON;
                BEGIN TRANSACTION;
                DECLARE @id INT;
                SELECT @id = ISNULL(MAX(ID), 0) + 1 FROM gaillard.PersonasEntidad WITH (UPDLOCK, HOLDLOCK);
                INSERT INTO gaillard.PersonasEntidad (ID, EntidadID, PersonaID, CargoID, FechaAlta, FechaBaja)
                VALUES (@id, @p0, @p1, @p2, @p3, NULL);
                COMMIT TRANSACTION;", entidadID, personaID, cargoID, fechaAlta ?? DateTime.Today);
            return RedirectToAction(AccionRetorno(returnTo), new { id = personaID });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> EditarRelacion(int personaID, int relacionID, int? cargoID, DateTime? fechaAlta, DateTime? fechaBaja, string returnTo = "Details")
        {
            var relacion = await db.PersonasEntidads.FirstOrDefaultAsync(r => r.ID == relacionID && r.PersonaID == personaID);
            if (relacion == null) return HttpNotFound();
            if (cargoID.HasValue && !await db.Cargoes.AnyAsync(c => c.ID == cargoID.Value)) return HttpNotFound();
            if (fechaAlta.HasValue && fechaBaja.HasValue && fechaBaja.Value < fechaAlta.Value)
            {
                TempData["PersonaError"] = "La fecha de baja no puede ser anterior a la fecha de alta.";
                return RedirectToAction(AccionRetorno(returnTo), new { id = personaID });
            }
            if (!fechaBaja.HasValue && await db.PersonasEntidads.AnyAsync(r => r.ID != relacionID && r.PersonaID == personaID && r.EntidadID == relacion.EntidadID && r.FechaBaja == null))
            {
                TempData["PersonaError"] = "Ya existe otra relación activa con esa entidad.";
                return RedirectToAction(AccionRetorno(returnTo), new { id = personaID });
            }
            relacion.CargoID = cargoID;
            relacion.FechaAlta = fechaAlta;
            relacion.FechaBaja = fechaBaja;
            await db.SaveChangesAsync();
            return RedirectToAction(AccionRetorno(returnTo), new { id = personaID });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> DarDeBajaRelacion(int personaID, int relacionID, string returnTo = "Details")
        {
            var relacion = await db.PersonasEntidads.FirstOrDefaultAsync(r => r.ID == relacionID && r.PersonaID == personaID && r.FechaBaja == null);
            if (relacion != null && relacion.FechaAlta.HasValue && relacion.FechaAlta.Value.Date > DateTime.Today)
            {
                TempData["PersonaError"] = "No se puede dar de baja una relación antes de su fecha de alta.";
                return RedirectToAction(AccionRetorno(returnTo), new { id = personaID });
            }
            if (relacion != null)
            {
                relacion.FechaBaja = DateTime.Today;
                await db.SaveChangesAsync();
            }
            return RedirectToAction(AccionRetorno(returnTo), new { id = personaID });
        }

        private static string AccionRetorno(string returnTo)
        {
            return string.Equals(returnTo, "Edit", StringComparison.OrdinalIgnoreCase) ? "Edit" : "Details";
        }

        private void CargarDirecciones(int? selected = null)
        {
            var direcciones = db.Direcciones.Include(d => d.Poblacion).OrderBy(d => d.NombreVia).ToList()
                .Select(d => new { d.ID, Texto = (d.NombreVia ?? "").Trim() + " " + d.Numero + " — " + d.Poblacion.NombrePoblacion.Trim() });
            ViewBag.DireccionID = new SelectList(direcciones, "ID", "Texto", selected);
        }

        private void CargarOpcionesRelacion()
        {
            var entidades = db.Entidads.AsNoTracking().OrderBy(e => e.NombreEntidad)
                .Select(e => new { e.ID, e.NombreEntidad }).ToList()
                .Select(e => new { e.ID, Texto = e.NombreEntidad.Trim() });
            ViewBag.EntidadID = new SelectList(entidades, "ID", "Texto");
            ViewBag.CargosLista = db.Cargoes.AsNoTracking().OrderBy(c => c.Cargo1)
                .Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.Cargo1.Trim() }).ToList();
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
