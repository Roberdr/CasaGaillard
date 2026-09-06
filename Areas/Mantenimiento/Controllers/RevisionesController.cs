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
    public class RevisionesController : Controller
    {
        private readonly GaillardEntities db = new GaillardEntities();

        private static DateTime? ObtenerValidaHasta(DateTime? validaHasta, DateTime fechaRevision)
        {
            return validaHasta ?? fechaRevision.AddMonths(30);
        }

        // GET: Revisiones
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento + "," + AppRoles.Consulta)]
        public async Task<ActionResult> Index(int? cubaID = null, string matriculaCuba = null, bool incluirBajas = false)
        {
            var consulta = db.Revisiones
                .AsNoTracking()
                .Include(r => r.Cuba)
                .AsQueryable();

            if (!incluirBajas)
            {
                consulta = consulta.Where(r => r.Cuba != null && (r.Cuba.Baja != true));
            }

            if (cubaID.HasValue)
            {
                consulta = consulta.Where(r => r.CubaID == cubaID.Value);
            }
            else if (!string.IsNullOrWhiteSpace(matriculaCuba))
            {
                consulta = consulta.Where(r => r.Cuba != null && r.Cuba.MatriculaCuba == matriculaCuba);
            }

            var revisiones = await consulta
                    .OrderBy(r => r.Cuba.MatriculaCuba)
                    .ThenByDescending(r => r.FechaRevision)
                    .ToListAsync();

            ViewBag.CubaID = cubaID;
            ViewBag.MatriculaCuba = matriculaCuba;
            ViewBag.IncluirBajas = incluirBajas;

            var modelo = revisiones
                .GroupBy(r => new { r.CubaID, Matricula = r.Cuba != null ? r.Cuba.MatriculaCuba : string.Empty })
                .Select(g => new RevisionCubaGrupoViewModel
                {
                    CubaID = g.Key.CubaID,
                    MatriculaCuba = g.Key.Matricula,
                    UltimaRevision = g
                        .OrderByDescending(r => r.FechaRevision)
                        .Select(r => new RevisionHistorialItemViewModel
                        {
                            ID = r.ID,
                            FechaRevision = r.FechaRevision,
                            Descripcion = r.Descripcion,
                            ValidaHasta = r.ValidaHasta,
                            DescripcionProxima = r.DescripcionProxima,
                            Autorizado = r.Autorizado
                        })
                        .FirstOrDefault(),
                    Historial = g
                        .OrderByDescending(r => r.FechaRevision)
                        .Select(r => new RevisionHistorialItemViewModel
                        {
                            ID = r.ID,
                            FechaRevision = r.FechaRevision,
                            Descripcion = r.Descripcion,
                            ValidaHasta = r.ValidaHasta,
                            DescripcionProxima = r.DescripcionProxima,
                            Autorizado = r.Autorizado
                        })
                        .ToList()
                })
                .OrderBy(x => x.MatriculaCuba)
                .ToList();

            return View(modelo);
        }

        // GET: Revisiones/Details/5
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento + "," + AppRoles.Consulta)]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Revision revision = await db.Revisiones.FindAsync(id);
            if (revision == null)
            {
                return HttpNotFound();
            }
            return View(revision);
        }

        // GET: Revisiones/Create
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public ActionResult Create(int? cubaID = null)
        {
            var cubas = db.Cubas.Where(c => (c.Baja != true) || (cubaID.HasValue && c.ID == cubaID.Value));
            ViewBag.CubaID = new SelectList(cubas.OrderBy(o => o.MatriculaCuba), "ID", "MatriculaCuba", cubaID);
            return View(new Revision
            {
                CubaID = cubaID ?? 0,
                FechaRevision = DateTime.Now.Date
            });
        }

        // POST: Revisiones/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Create([Bind(Include = "ID,CubaID,FechaRevision,Descripcion,ValidaHasta,DescripcionProxima,Autorizado")] Revision revision)
        {
            if (ModelState.IsValid)
            {
                revision.ValidaHasta = ObtenerValidaHasta(revision.ValidaHasta, revision.FechaRevision);
                db.Revisiones.Add(revision);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            var cubas = db.Cubas.Where(c => (c.Baja != true) || c.ID == revision.CubaID);
            ViewBag.CubaID = new SelectList(cubas.OrderBy(o => o.MatriculaCuba), "ID", "MatriculaCuba", revision.CubaID);
            return View(revision);
        }

        // GET: Revisiones/Edit/5
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Revision revision = await db.Revisiones.FindAsync(id);
            if (revision == null)
            {
                return HttpNotFound();
            }
            var cubas = db.Cubas.Where(c => (c.Baja != true) || c.ID == revision.CubaID);
            ViewBag.CubaID = new SelectList(cubas.OrderBy(o => o.MatriculaCuba), "ID", "MatriculaCuba", revision.CubaID);
            return View(revision);
        }

        // POST: Revisiones/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Edit([Bind(Include = "ID,CubaID,FechaRevision,Descripcion,ValidaHasta,DescripcionProxima,Autorizado")] Revision revision)
        {
            if (ModelState.IsValid)
            {
                db.Entry(revision).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            var cubas = db.Cubas.Where(c => (c.Baja != true) || c.ID == revision.CubaID);
            ViewBag.CubaID = new SelectList(cubas.OrderBy(o => o.MatriculaCuba), "ID", "MatriculaCuba", revision.CubaID);
            return View(revision);
        }

        // GET: Revisiones/Delete/5
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Revision revision = await db.Revisiones.FindAsync(id);
            if (revision == null)
            {
                return HttpNotFound();
            }
            return View(revision);
        }

        // POST: Revisiones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Revision revision = await db.Revisiones.FindAsync(id);
            db.Revisiones.Remove(revision);
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

            var rev1 = db.Revisiones.Include(r => r.Cuba)
                  .GroupBy(s => s.Cuba.MatriculaCuba);
            return View(await rev1.ToListAsync());

        }

        [Authorize(Roles = AppRoles.Administrador)]
        public async Task<ActionResult> CambiarValidaHasta()
        {
            var rev1 = await db.Revisiones.ToListAsync();

            foreach (var rev in rev1)
            {
                rev.ValidaHasta = rev.FechaRevision.AddMonths(30);
                await db.SaveChangesAsync();
             
            }

            return RedirectToAction("Index");
        }
    }
}
