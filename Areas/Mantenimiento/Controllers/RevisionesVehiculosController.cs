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

        // GET: RevisionesVehiculos
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento + "," + AppRoles.Consulta)]
        public async Task<ActionResult> Index(string matriculaVehiculo = null)
        {
            var consulta = db.RevisionesVehiculo
                .AsNoTracking()
                .Include(r => r.Vehiculo)
                .Include(r => r.TipoRevision)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(matriculaVehiculo))
            {
                consulta = consulta.Where(r => r.Vehiculo != null && r.Vehiculo.MatriculaVehiculo == matriculaVehiculo);
            }

            var revisionesVehiculos = await consulta
                .OrderBy(r => r.Vehiculo.MatriculaVehiculo)
                .ThenByDescending(r => r.FechaRevision)
                .ToListAsync();

            ViewBag.MatriculaVehiculo = matriculaVehiculo;

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
            ViewBag.VehiculoID = new SelectList(db.Vehiculos.OrderBy(o => o.MatriculaVehiculo), "ID", "MatriculaVehiculo", vehiculoID);
            ViewBag.TipoRevisionID = new SelectList(db.TiposRevision.OrderBy(o => o.Revision), "ID", "Revision", tipoRevisionID);
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
        public async Task<ActionResult> Create([Bind(Include = "ID,VehiculoID,TipoRevisionID,FechaRevision,Detalles,Ejecutor,Caducidad")] RevisionVehiculo revisionVehiculo)
        {
            if (ModelState.IsValid)
            {
                revisionVehiculo.Caducidad = CalcularCaducidad(revisionVehiculo.FechaRevision);
                db.RevisionesVehiculo.Add(revisionVehiculo);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.VehiculoID = new SelectList(db.Vehiculos.OrderBy(o => o.MatriculaVehiculo), "ID", "MatriculaVehiculo", revisionVehiculo.VehiculoID);
            ViewBag.TipoRevisionID = new SelectList(db.TiposRevision.OrderBy(o => o.Revision), "ID", "Revision", revisionVehiculo.TipoRevisionID);
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
            ViewBag.VehiculoID = new SelectList(db.Vehiculos.OrderBy(o => o.MatriculaVehiculo), "ID", "MatriculaVehiculo", revisionVehiculo.VehiculoID);
            ViewBag.TipoRevisionID = new SelectList(db.TiposRevision.OrderBy(o => o.Revision), "ID", "Revision", revisionVehiculo.TipoRevisionID);

            return View(revisionVehiculo);
        }

        // POST: RevisionesVehiculos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Edit([Bind(Include = "ID,VehiculoID,TipoRevisionID,FechaRevision,Detalles,Ejecutor,Caducidad")] RevisionVehiculo revisionVehiculo)
        {
            if (ModelState.IsValid)
            {
                revisionVehiculo.Caducidad = CalcularCaducidad(revisionVehiculo.FechaRevision);
                db.Entry(revisionVehiculo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.VehiculoID = new SelectList(db.Vehiculos.OrderBy(o => o.MatriculaVehiculo), "ID", "MatriculaVehiculo", revisionVehiculo.VehiculoID);
            ViewBag.TipoRevisionID = new SelectList(db.TiposRevision.OrderBy(o => o.Revision), "ID", "Revision", revisionVehiculo.TipoRevisionID);

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
            //List<Revision> rev = new List<Revision>();

            var rev1 = db.RevisionesVehiculo.Include(r => r.Vehiculo)
                  .GroupBy(s => s.Vehiculo.MatriculaVehiculo);
            return View(await rev1.ToListAsync());

        }
    }
}
