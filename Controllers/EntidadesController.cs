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
    public class EntidadesController : Controller
    {
        private readonly GaillardEntities db = new GaillardEntities();

        // GET: Entidades
        public async Task<ActionResult> Index()
        {
            var entidads = db.Entidads
                .Include(e => e.Direccion)
                .AsNoTracking().OrderBy(e => e.NombreEntidad);
            return View(await entidads.ToListAsync());
        }

        // GET: Entidades/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entidad entidad = await db.Entidads.Include(e => e.Direccion.Poblacion)
                .Include(e => e.TelefonosEntidad)
                .Include(e => e.PersonasEntidad.Select(pe => pe.Persona))
                .Include(e => e.PersonasEntidad.Select(pe => pe.Cargo))
                .FirstOrDefaultAsync(e => e.ID == id.Value);
            if (entidad == null)
            {
                return HttpNotFound();
            }
            CargarPersonas();
            CargarCargos();
            return View(entidad);
        }

        // GET: Entidades/Create
        public ActionResult Create(int? direccionID = null)
        {
            CargarDirecciones(direccionID);
            return View();
        }

        // POST: Entidades/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "NombreEntidad,CIF,DireccionID")] Entidad entidad)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction(IsolationLevel.Serializable))
                {
                    entidad.ID = await db.Database.SqlQuery<int>("SELECT ISNULL(MAX(ID), 0) + 1 FROM gaillard.Entidad WITH (UPDLOCK, HOLDLOCK)").SingleAsync();
                    db.Entidads.Add(entidad);
                    await db.SaveChangesAsync();
                    transaction.Commit();
                }
                return RedirectToAction("Index");
            }
            CargarDirecciones(entidad.DireccionID);
            return View(entidad);
        }

        // GET: Entidades/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entidad entidad = await db.Entidads.Include(e => e.TelefonosEntidad).FirstOrDefaultAsync(e => e.ID == id.Value);
            if (entidad == null)
            {
                return HttpNotFound();
            }
            CargarDirecciones(entidad.DireccionID);
            return View(entidad);
        }

        // POST: Entidades/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,NombreEntidad,CIF,DireccionID")] Entidad entidad)
        {
            if (ModelState.IsValid)
            {
                var actual = await db.Entidads.FindAsync(entidad.ID);
                if (actual == null) return HttpNotFound();
                actual.NombreEntidad = entidad.NombreEntidad;
                actual.CIF = entidad.CIF;
                actual.DireccionID = entidad.DireccionID;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            var datosRelacionados = await db.Entidads.Include(e => e.TelefonosEntidad).FirstOrDefaultAsync(e => e.ID == entidad.ID);
            if (datosRelacionados != null) entidad.TelefonosEntidad = datosRelacionados.TelefonosEntidad;
            CargarDirecciones(entidad.DireccionID);
            return View(entidad);
        }

        // GET: Entidades/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entidad entidad = await db.Entidads.FindAsync(id);
            if (entidad == null)
            {
                return HttpNotFound();
            }
            return View(entidad);
        }

        // POST: Entidades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Entidad entidad = await db.Entidads.FindAsync(id);
            if (entidad == null) return HttpNotFound();
            try
            {
                db.Entidads.Remove(entidad);
                await db.SaveChangesAsync();
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                ModelState.AddModelError("", "No se puede eliminar esta entidad porque tiene teléfonos, personas vinculadas u otros registros asociados.");
                return View("Delete", entidad);
            }
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> AgregarTelefono(int entidadID, string telefono, string returnTo = "Details")
        {
            if (await db.Entidads.FindAsync(entidadID) == null) return HttpNotFound();
            telefono = (telefono ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(telefono) || telefono.Length > 10)
            {
                TempData["EntidadError"] = "Introduce un teléfono de hasta 10 caracteres.";
                return RedirectToAction(AccionRetorno(returnTo), new { id = entidadID });
            }
            await db.Database.ExecuteSqlCommandAsync(@"SET XACT_ABORT ON;
                BEGIN TRANSACTION;
                DECLARE @id INT;
                SELECT @id = ISNULL(MAX(ID), 0) + 1 FROM gaillard.TelefonoEntidad WITH (UPDLOCK, HOLDLOCK);
                INSERT INTO gaillard.TelefonoEntidad (ID, Telefono, EntidadID) VALUES (@id, @p0, @p1);
                COMMIT TRANSACTION;", telefono, entidadID);
            return RedirectToAction(AccionRetorno(returnTo), new { id = entidadID });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> ActualizarTelefono(int entidadID, int id, string telefono, string returnTo = "Details")
        {
            telefono = (telefono ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(telefono) || telefono.Length > 10)
            {
                TempData["EntidadError"] = "Introduce un teléfono de hasta 10 caracteres.";
                return RedirectToAction(AccionRetorno(returnTo), new { id = entidadID });
            }
            await db.Database.ExecuteSqlCommandAsync("UPDATE gaillard.TelefonoEntidad SET Telefono=@p0 WHERE ID=@p1 AND EntidadID=@p2", telefono, id, entidadID);
            return RedirectToAction(AccionRetorno(returnTo), new { id = entidadID });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> EliminarTelefono(int entidadID, int id, string returnTo = "Details")
        {
            await db.Database.ExecuteSqlCommandAsync("DELETE FROM gaillard.TelefonoEntidad WHERE ID=@p0 AND EntidadID=@p1", id, entidadID);
            return RedirectToAction(AccionRetorno(returnTo), new { id = entidadID });
        }

        private static string AccionRetorno(string returnTo)
        {
            return string.Equals(returnTo, "Edit", StringComparison.OrdinalIgnoreCase) ? "Edit" : "Details";
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> AgregarPersona(int entidadID, int personaID, int? cargoID, DateTime? fechaAlta)
        {
            if (await db.Entidads.FindAsync(entidadID) == null || await db.Personas.FindAsync(personaID) == null) return HttpNotFound();
            if (cargoID.HasValue && await db.Cargoes.FindAsync(cargoID.Value) == null) return HttpNotFound();
            var yaVinculada = await db.Database.SqlQuery<int>(@"SELECT COUNT(1) FROM gaillard.PersonasEntidad
                WHERE EntidadID=@p0 AND PersonaID=@p1 AND FechaBaja IS NULL", entidadID, personaID).SingleAsync();
            if (yaVinculada > 0)
            {
                TempData["EntidadError"] = "Esta persona ya tiene una relación activa con la entidad.";
                return RedirectToAction("Details", new { id = entidadID });
            }
            await db.Database.ExecuteSqlCommandAsync(@"SET XACT_ABORT ON;
                BEGIN TRANSACTION;
                DECLARE @id INT;
                SELECT @id = ISNULL(MAX(ID), 0) + 1 FROM gaillard.PersonasEntidad WITH (UPDLOCK, HOLDLOCK);
                INSERT INTO gaillard.PersonasEntidad (ID, EntidadID, PersonaID, CargoID, FechaAlta, FechaBaja)
                VALUES (@id, @p0, @p1, @p2, @p3, NULL);
                COMMIT TRANSACTION;", entidadID, personaID, cargoID, fechaAlta ?? DateTime.Today);
            return RedirectToAction("Details", new { id = entidadID });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> DarDeBajaPersona(int entidadID, int relacionID)
        {
            var relacion = await db.PersonasEntidads.FirstOrDefaultAsync(x => x.ID == relacionID && x.EntidadID == entidadID && x.FechaBaja == null);
            if (relacion != null && relacion.FechaAlta.HasValue && relacion.FechaAlta.Value.Date > DateTime.Today)
            {
                TempData["EntidadError"] = "No se puede dar de baja una relación antes de su fecha de alta.";
                return RedirectToAction("Details", new { id = entidadID });
            }
            if (relacion != null)
            {
                relacion.FechaBaja = DateTime.Today;
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Details", new { id = entidadID });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> EditarRelacion(int entidadID, int relacionID, int? cargoID, DateTime? fechaAlta, DateTime? fechaBaja)
        {
            var relacion = await db.PersonasEntidads.FirstOrDefaultAsync(x => x.ID == relacionID && x.EntidadID == entidadID);
            if (relacion == null) return HttpNotFound();
            if (cargoID.HasValue && await db.Cargoes.FindAsync(cargoID.Value) == null) return HttpNotFound();
            if (fechaAlta.HasValue && fechaBaja.HasValue && fechaBaja.Value < fechaAlta.Value)
            {
                TempData["EntidadError"] = "La fecha de baja no puede ser anterior a la fecha de alta.";
                return RedirectToAction("Details", new { id = entidadID });
            }
            if (!fechaBaja.HasValue && await db.PersonasEntidads.AnyAsync(x => x.ID != relacionID && x.EntidadID == entidadID && x.PersonaID == relacion.PersonaID && x.FechaBaja == null))
            {
                TempData["EntidadError"] = "Esta persona ya tiene otra relación activa con la entidad.";
                return RedirectToAction("Details", new { id = entidadID });
            }
            relacion.CargoID = cargoID;
            relacion.FechaAlta = fechaAlta;
            relacion.FechaBaja = fechaBaja;
            await db.SaveChangesAsync();
            return RedirectToAction("Details", new { id = entidadID });
        }

        private void CargarDirecciones(int? selected = null)
        {
            var direcciones = db.Direcciones.Include(d => d.Poblacion).OrderBy(d => d.NombreVia).ToList()
                .Select(d => new { d.ID, Texto = (d.NombreVia ?? "").Trim() + " " + d.Numero + " — " + d.Poblacion.NombrePoblacion.Trim() });
            ViewBag.DireccionID = new SelectList(direcciones, "ID", "Texto", selected);
        }

        private void CargarPersonas()
        {
            var personas = db.Personas.AsNoTracking().OrderBy(p => p.Apellido1).ThenBy(p => p.NombrePersona).ToList()
                .Select(p => new { p.ID, Texto = (p.NombrePersona + " " + p.Apellido1 + " " + (p.Apellido2 ?? "")).Trim() });
            ViewBag.PersonaID = new SelectList(personas, "ID", "Texto");
        }

        private void CargarCargos()
        {
            ViewBag.CargoID = new SelectList(db.Cargoes.AsNoTracking().OrderBy(c => c.Cargo1), "ID", "Cargo1");
            ViewBag.CargosLista = db.Cargoes.AsNoTracking().OrderBy(c => c.Cargo1)
                .Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.Cargo1 }).ToList();
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
