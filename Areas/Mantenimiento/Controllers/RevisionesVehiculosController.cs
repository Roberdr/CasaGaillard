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
using CasaGaillard.Models.ViewModels;
using Microsoft.Ajax.Utilities;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;

namespace CasaGaillard.Areas.Mantenimiento.Controllers
{
    [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento + "," + AppRoles.Consulta)]
    public class RevisionesVehiculosController : Controller
    {
        private readonly GaillardEntities db = new GaillardEntities();

        private static DateTime? CalcularCaducidad(DateTime? fechaRevision)
        {
            return fechaRevision.HasValue ? fechaRevision.Value.AddMonths(30) : (DateTime?)null;
        }

        private static DateTime? ObtenerCaducidad(DateTime? caducidad, DateTime? fechaRevision)
        {
            return caducidad ?? CalcularCaducidad(fechaRevision);
        }

        // GET: RevisionesVehiculos
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento + "," + AppRoles.Consulta)]
        public async Task<ActionResult> Index(string matriculaVehiculo = null, bool incluirBajas = false)
        {
            var consulta = db.RevisionesVehiculo
                .AsNoTracking()
                .Include(r => r.Vehiculo)
                .Include(r => r.TipoRevision)
                .AsQueryable();

            if (!incluirBajas)
            {
                consulta = consulta.Where(r => r.Vehiculo != null && (r.Vehiculo.Baja != true));
            }

            if (!string.IsNullOrWhiteSpace(matriculaVehiculo))
            {
                consulta = consulta.Where(r => r.Vehiculo != null && r.Vehiculo.MatriculaVehiculo == matriculaVehiculo);
            }

            var revisionesVehiculos = await consulta
                .OrderBy(r => r.Vehiculo.MatriculaVehiculo)
                .ThenByDescending(r => r.FechaRevision)
                .ToListAsync();

            ViewBag.MatriculaVehiculo = matriculaVehiculo;
            ViewBag.IncluirBajas = incluirBajas;

            var agrupadas = revisionesVehiculos
                .GroupBy(r => new
                {
                    VehiculoID = r.VehiculoID ?? 0,
                    Matricula = r.Vehiculo != null ? r.Vehiculo.MatriculaVehiculo : string.Empty
                })
                .OrderBy(g => g.Key.Matricula)
                .Select(g => new RevisionVehiculoGrupoViewModel
                {
                    VehiculoID = g.Key.VehiculoID,
                    MatriculaVehiculo = g.Key.Matricula,
                    TotalRevisiones = g.Count(),
                    Tipos = g.GroupBy(r => r.TipoRevision != null ? r.TipoRevision.Revision : string.Empty)
                        .OrderBy(tipo => tipo.Key)
                        .Select(tipo => new RevisionVehiculoTipoGrupoViewModel
                        {
                            TipoRevisionID = tipo.Select(r => r.TipoRevisionID).FirstOrDefault(),
                            TipoRevision = tipo.Key,
                            UltimaRevision = tipo
                                .OrderByDescending(r => r.FechaRevision ?? DateTime.MinValue)
                                .ThenByDescending(r => r.Caducidad ?? DateTime.MinValue)
                                .Select(r => new RevisionVehiculoHistorialItemViewModel
                                {
                                    ID = r.ID,
                                    TipoRevisionID = r.TipoRevisionID,
                                    TipoRevision = r.TipoRevision != null ? r.TipoRevision.Revision : string.Empty,
                                    FechaRevision = r.FechaRevision,
                                    Caducidad = r.Caducidad,
                                    Detalles = r.Detalles,
                                    Ejecutor = r.Ejecutor
                                })
                                .FirstOrDefault(),
                            Historial = tipo
                                .OrderByDescending(r => r.FechaRevision ?? DateTime.MinValue)
                                .ThenByDescending(r => r.Caducidad ?? DateTime.MinValue)
                                .Select(r => new RevisionVehiculoHistorialItemViewModel
                                {
                                    ID = r.ID,
                                    TipoRevisionID = r.TipoRevisionID,
                                    TipoRevision = r.TipoRevision != null ? r.TipoRevision.Revision : string.Empty,
                                    FechaRevision = r.FechaRevision,
                                    Caducidad = r.Caducidad,
                                    Detalles = r.Detalles,
                                    Ejecutor = r.Ejecutor
                                })
                                .ToList()
                        })
                        .ToList()
                })
                .OrderBy(x => x.MatriculaVehiculo)
                .ToList();

            return View(agrupadas);
        }

        // GET: RevisionesVehiculos/Details/5
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento + "," + AppRoles.Consulta)]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RevisionVehiculo revisionVehiculo = await db.RevisionesVehiculo.FindAsync(id);
            if (revisionVehiculo == null)
            {
                return HttpNotFound();
            }
            return View(revisionVehiculo);
        }

        // GET: RevisionesVehiculos/Create
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public ActionResult Create(int? vehiculoID = null, int? tipoRevisionID = null)
        {
            var vehiculos = db.Vehiculos.Where(v => (v.Baja != true) || (vehiculoID.HasValue && v.ID == vehiculoID.Value));
            ViewBag.VehiculoID = new SelectList(vehiculos.OrderBy(o => o.MatriculaVehiculo), "ID", "MatriculaVehiculo", vehiculoID);
            ViewBag.TipoRevisionID = new SelectList(db.TiposRevision.OrderBy(o => o.Revision), "ID", "Revision", tipoRevisionID);
            CargarPersonasEntidad(null);
            return View(new RevisionVehiculo
            {
                VehiculoID = vehiculoID,
                TipoRevisionID = tipoRevisionID,
                FechaRevision = DateTime.Now.Date
            });
        }

        // POST: RevisionesVehiculos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Create([Bind(Include = "ID,VehiculoID,TipoRevisionID,FechaRevision,Detalles,Ejecutor,Caducidad,EjecutorPersonasEntidadID")] RevisionVehiculo revisionVehiculo)
        {
            if (!revisionVehiculo.EjecutorPersonasEntidadID.HasValue || ObtenerEtiquetaPersonaEntidad(revisionVehiculo.EjecutorPersonasEntidadID.Value, true) == null)
                ModelState.AddModelError("EjecutorPersonasEntidadID", "Selecciona una persona perteneciente a una entidad.");
            if (ModelState.IsValid)
            {
                revisionVehiculo.Ejecutor = ObtenerEtiquetaPersonaEntidad(revisionVehiculo.EjecutorPersonasEntidadID.Value, true);
                revisionVehiculo.Caducidad = ObtenerCaducidad(revisionVehiculo.Caducidad, revisionVehiculo.FechaRevision);
                db.RevisionesVehiculo.Add(revisionVehiculo);
                try
                {
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var entityError in ex.EntityValidationErrors.SelectMany(e => e.ValidationErrors))
                    {
                        ModelState.AddModelError(entityError.PropertyName, entityError.ErrorMessage);
                    }
                }
            }

            var vehiculos = db.Vehiculos.Where(v => (v.Baja != true) || (revisionVehiculo.VehiculoID.HasValue && v.ID == revisionVehiculo.VehiculoID.Value));
            ViewBag.VehiculoID = new SelectList(vehiculos.OrderBy(o => o.MatriculaVehiculo), "ID", "MatriculaVehiculo", revisionVehiculo.VehiculoID);
            ViewBag.TipoRevisionID = new SelectList(db.TiposRevision.OrderBy(o => o.Revision), "ID", "Revision", revisionVehiculo.TipoRevisionID);
            CargarPersonasEntidad(revisionVehiculo.EjecutorPersonasEntidadID);
            return View(revisionVehiculo);
        }

        // GET: RevisionesVehiculos/Edit/5
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RevisionVehiculo revisionVehiculo = await db.RevisionesVehiculo.FindAsync(id);
            if (revisionVehiculo == null)
            {
                return HttpNotFound();
            }
            var vehiculos = db.Vehiculos.Where(v => (v.Baja != true) || (revisionVehiculo.VehiculoID.HasValue && v.ID == revisionVehiculo.VehiculoID.Value));
            ViewBag.VehiculoID = new SelectList(vehiculos.OrderBy(o => o.MatriculaVehiculo), "ID", "MatriculaVehiculo", revisionVehiculo.VehiculoID);
            ViewBag.TipoRevisionID = new SelectList(db.TiposRevision.OrderBy(o => o.Revision), "ID", "Revision", revisionVehiculo.TipoRevisionID);
            CargarPersonasEntidad(revisionVehiculo.EjecutorPersonasEntidadID);

            return View(revisionVehiculo);
        }

        // POST: RevisionesVehiculos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Edit([Bind(Include = "ID,VehiculoID,TipoRevisionID,FechaRevision,Detalles,Ejecutor,Caducidad,EjecutorPersonasEntidadID")] RevisionVehiculo revisionVehiculo)
        {
            if (revisionVehiculo.EjecutorPersonasEntidadID.HasValue)
            {
                var etiqueta = ObtenerEtiquetaPersonaEntidad(revisionVehiculo.EjecutorPersonasEntidadID.Value);
                if (etiqueta == null) ModelState.AddModelError("EjecutorPersonasEntidadID", "Selecciona una relación persona-entidad válida.");
                else revisionVehiculo.Ejecutor = etiqueta;
            }
            if (ModelState.IsValid)
            {
                revisionVehiculo.Caducidad = ObtenerCaducidad(revisionVehiculo.Caducidad, revisionVehiculo.FechaRevision);
                db.Entry(revisionVehiculo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            var vehiculos = db.Vehiculos.Where(v => (v.Baja != true) || (revisionVehiculo.VehiculoID.HasValue && v.ID == revisionVehiculo.VehiculoID.Value));
            ViewBag.VehiculoID = new SelectList(vehiculos.OrderBy(o => o.MatriculaVehiculo), "ID", "MatriculaVehiculo", revisionVehiculo.VehiculoID);
            ViewBag.TipoRevisionID = new SelectList(db.TiposRevision.OrderBy(o => o.Revision), "ID", "Revision", revisionVehiculo.TipoRevisionID);
            CargarPersonasEntidad(revisionVehiculo.EjecutorPersonasEntidadID);

            return View(revisionVehiculo);
        }

        // GET: RevisionesVehiculos/Delete/5
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RevisionVehiculo revisionVehiculo = await db.RevisionesVehiculo.FindAsync(id);
            if (revisionVehiculo == null)
            {
                return HttpNotFound();
            }
            return View(revisionVehiculo);
        }

        // POST: RevisionesVehiculos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            RevisionVehiculo revisionVehiculo = await db.RevisionesVehiculo.FindAsync(id);
            db.RevisionesVehiculo.Remove(revisionVehiculo);
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

        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Proximas()
        {
            var revisiones = await db.RevisionesVehiculo
                .AsNoTracking()
                .Include(r => r.Vehiculo)
                .Include(r => r.TipoRevision)
                .Where(r => r.Caducidad.HasValue)
                .OrderBy(r => r.Caducidad)
                .Select(r => new ProximaRevisionVehiculoViewModel
                {
                    MatriculaVehiculo = r.Vehiculo != null ? r.Vehiculo.MatriculaVehiculo : string.Empty,
                    TipoRevision = r.TipoRevision != null ? r.TipoRevision.Revision : string.Empty,
                    Caducidad = r.Caducidad
                })
                .ToListAsync();

            return View("ProximasRevisiones", revisiones);
        }

        private void CargarPersonasEntidad(int? selected)
        {
            var opciones = (from pe in db.PersonasEntidads
                            join p in db.Personas on pe.PersonaID equals p.ID
                            join e in db.Entidads on pe.EntidadID equals e.ID
                            where !pe.FechaBaja.HasValue || pe.ID == selected
                            orderby p.Apellido1, p.Apellido2, p.NombrePersona, e.NombreEntidad
                            select new { pe.ID, p.NombrePersona, p.Apellido1, p.Apellido2, e.NombreEntidad }).ToList()
                .Select(x => new { x.ID, Etiqueta = (x.NombrePersona + " " + x.Apellido1 + " " + (x.Apellido2 ?? "")).Trim() + " — " + x.NombreEntidad.Trim() });
            ViewBag.EjecutorPersonasEntidadID = new SelectList(opciones, "ID", "Etiqueta", selected);
        }

        private string ObtenerEtiquetaPersonaEntidad(int id, bool soloActiva = false)
        {
            var x = (from pe in db.PersonasEntidads
                     join p in db.Personas on pe.PersonaID equals p.ID
                     join e in db.Entidads on pe.EntidadID equals e.ID
                     where pe.ID == id && (!soloActiva || !pe.FechaBaja.HasValue)
                     select new { p.NombrePersona, p.Apellido1, p.Apellido2, e.NombreEntidad }).FirstOrDefault();
            if (x == null)
                return null;

            var etiqueta = (x.NombrePersona + " " + x.Apellido1 + " " + (x.Apellido2 ?? "")).Trim() + " — " + x.NombreEntidad.Trim();
            const int longitudMaximaEjecutor = 45;
            return etiqueta.Length <= longitudMaximaEjecutor
                ? etiqueta
                : etiqueta.Substring(0, longitudMaximaEjecutor - 3) + "...";
        }
    }
}
