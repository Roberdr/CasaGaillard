using CasaGaillard.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CasaGaillard.Areas.Mantenimiento.Controllers
{
    [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento + "," + AppRoles.Consulta)]
    public class PlanMantenimientoController : Controller
    {
        private readonly GaillardEntities db = new GaillardEntities();

        public async Task<ActionResult> Index()
        {
            var planes = db.PlanMantenimiento
                .Include(p => p.Accesorio)
                .Include(p => p.Accesorio.TipoAccesorio)
                .OrderBy(p => p.Accesorio.Nombre)
                .ThenBy(p => p.PeriodicidadDias);

            var lista = await planes.ToListAsync();
            await CargarAccesoriosGrupoAsync(lista);
            var nombresEquipo = await db.Database.SqlQuery<EquipoPlanNombre>(@"SELECT Id,
                    COALESCE(NULLIF(LTRIM(RTRIM(Nombre)), N''), NULLIF(LTRIM(RTRIM(Codigo)), N''),
                        N'Equipo ' + CONVERT(NVARCHAR(20), Id)) AS Nombre
                FROM gaillard.Equipo").ToListAsync();
            ViewBag.NombresEquipo = nombresEquipo.ToDictionary(e => e.Id, e => e.Nombre);
            return View(lista);
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var plan = await db.PlanMantenimiento
                .Include(p => p.Accesorio)
                .Include(p => p.Accesorio.TipoAccesorio)
                .FirstOrDefaultAsync(p => p.PlanId == id.Value);

            if (plan == null)
            {
                return HttpNotFound();
            }

            await CargarAccesorioGrupoAsync(plan);
            ViewBag.NombreEquipo = await ObtenerNombreEquipoAsync(plan.EquipoId);
            ViewBag.AccesorioGrupoTexto = ObtenerTextoAccesorioGrupo(plan.AccesorioGrupo);
            return View(plan);
        }

        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public ActionResult Create()
        {
            var plan = new PlanMantenimiento
            {
                Activo = true,
                PeriodicidadDias = 365,
                UltimaEjecucion = DateTime.Today,
                ProximaEjecucion = DateTime.Today.AddDays(365)
            };

            PrepararListas(plan);
            return View(plan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Create([Bind(Include = "PlanId,AccesorioId,PeriodicidadDias,UltimaEjecucion,ProximaEjecucion,AccesorioGrupoID,EquipoId,Nombre,Descripcion,Tipo,FrecuenciaValor,FrecuenciaUnidad,DuracionEstimadaMinutos,Activo")] PlanMantenimiento plan)
        {
            await ValidarPlanAsync(plan);

            if (ModelState.IsValid)
            {
                db.PlanMantenimiento.Add(plan);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            PrepararListas(plan);
            return View(plan);
        }

        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var plan = await db.PlanMantenimiento.FindAsync(id.Value);
            if (plan == null)
            {
                return HttpNotFound();
            }

            PrepararListas(plan);
            return View(plan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Edit([Bind(Include = "PlanId,AccesorioId,PeriodicidadDias,UltimaEjecucion,ProximaEjecucion,AccesorioGrupoID,EquipoId,Nombre,Descripcion,Tipo,FrecuenciaValor,FrecuenciaUnidad,DuracionEstimadaMinutos,Activo")] PlanMantenimiento plan)
        {
            await ValidarPlanAsync(plan);

            if (ModelState.IsValid)
            {
                db.Entry(plan).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            PrepararListas(plan);
            return View(plan);
        }

        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var plan = await db.PlanMantenimiento
                .Include(p => p.Accesorio)
                .Include(p => p.Accesorio.TipoAccesorio)
                .FirstOrDefaultAsync(p => p.PlanId == id.Value);

            if (plan == null)
            {
                return HttpNotFound();
            }

            await CargarAccesorioGrupoAsync(plan);
            ViewBag.NombreEquipo = await ObtenerNombreEquipoAsync(plan.EquipoId);
            ViewBag.AccesorioGrupoTexto = ObtenerTextoAccesorioGrupo(plan.AccesorioGrupo);
            return View(plan);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var plan = await db.PlanMantenimiento.FindAsync(id);
            if (plan == null)
            {
                return HttpNotFound();
            }

            db.PlanMantenimiento.Remove(plan);
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

        private async Task ValidarPlanAsync(PlanMantenimiento plan)
        {
            if (plan.PeriodicidadDias <= 0)
            {
                ModelState.AddModelError("PeriodicidadDias", "La periodicidad debe ser mayor que cero.");
            }

            if (plan.FrecuenciaValor.HasValue && plan.FrecuenciaValor.Value <= 0)
            {
                ModelState.AddModelError("FrecuenciaValor", "La frecuencia debe ser mayor que cero.");
            }

            if (plan.FrecuenciaValor.HasValue != !string.IsNullOrWhiteSpace(plan.FrecuenciaUnidad))
            {
                ModelState.AddModelError("FrecuenciaUnidad", "Indica tanto el valor como la unidad de frecuencia.");
            }

            if (plan.UltimaEjecucion.HasValue && plan.ProximaEjecucion.HasValue && plan.ProximaEjecucion.Value < plan.UltimaEjecucion.Value)
            {
                ModelState.AddModelError("ProximaEjecucion", "La próxima ejecución no puede ser anterior a la última ejecución.");
            }

            var accesorioExiste = plan.AccesorioId.HasValue && await db.Accesorios.AnyAsync(a => a.ID == plan.AccesorioId.Value);
            var equipoExiste = plan.EquipoId.HasValue && await db.Database.SqlQuery<int>("SELECT COUNT(1) FROM gaillard.Equipo WHERE Id = @p0", plan.EquipoId.Value).SingleAsync() > 0;
            if (!accesorioExiste && !equipoExiste)
            {
                ModelState.AddModelError("EquipoId", "Selecciona un equipo o accesorio válido.");
            }
            else if (accesorioExiste && equipoExiste)
            {
                ModelState.AddModelError("EquipoId", "El plan debe estar asociado a un equipo o a un accesorio, no a ambos.");
            }

            if (plan.AccesorioGrupoID.HasValue)
            {
                var accesorioGrupo = await db.AccesoriosGrupo
                    .AsNoTracking()
                    .FirstOrDefaultAsync(g => g.ID == plan.AccesorioGrupoID.Value);

                if (accesorioGrupo == null)
                {
                    ModelState.AddModelError("AccesorioGrupoID", "Selecciona una ubicación de accesorio válida.");
                }
                else if (!plan.AccesorioId.HasValue || accesorioGrupo.AccesorioID != plan.AccesorioId.Value)
                {
                    ModelState.AddModelError("AccesorioGrupoID", "La ubicación seleccionada pertenece a otro accesorio.");
                }
            }
        }

        private async Task CargarAccesorioGrupoAsync(PlanMantenimiento plan)
        {
            if (plan == null)
            {
                return;
            }

            await CargarAccesoriosGrupoAsync(new[] { plan });
        }

        private async Task<string> ObtenerNombreEquipoAsync(int? equipoId)
        {
            if (!equipoId.HasValue) return null;
            return await db.Database.SqlQuery<string>(@"SELECT COALESCE(NULLIF(LTRIM(RTRIM(Nombre)), N''),
                    NULLIF(LTRIM(RTRIM(Codigo)), N''), N'Equipo ' + CONVERT(NVARCHAR(20), Id))
                FROM gaillard.Equipo WHERE Id=@p0", equipoId.Value).SingleOrDefaultAsync();
        }

        private sealed class EquipoPlanNombre
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
        }

        private async Task CargarAccesoriosGrupoAsync(IEnumerable<PlanMantenimiento> planes)
        {
            var lista = (planes ?? Enumerable.Empty<PlanMantenimiento>())
                .Where(p => p != null && p.AccesorioGrupoID.HasValue)
                .ToList();

            if (!lista.Any())
            {
                return;
            }

            var ids = lista
                .Select(p => p.AccesorioGrupoID.Value)
                .Distinct()
                .ToList();

            var accesoriosGrupo = await db.AccesoriosGrupo
                .AsNoTracking()
                .Include(ag => ag.Accesorio)
                .Include(ag => ag.Accesorio.TipoAccesorio)
                .Include(ag => ag.Grupos)
                .Include(ag => ag.Grupos.Cuba)
                .Include(ag => ag.Grupos.Compartimento)
                .Include(ag => ag.Grupos.Situacion)
                .Include(ag => ag.Grupos.TipoGrupo)
                .Where(ag => ids.Contains(ag.ID))
                .ToListAsync();

            var porId = accesoriosGrupo.ToDictionary(ag => ag.ID);
            foreach (var plan in lista)
            {
                if (plan.AccesorioGrupoID.HasValue && porId.TryGetValue(plan.AccesorioGrupoID.Value, out var accesorioGrupo))
                {
                    plan.AccesorioGrupo = accesorioGrupo;
                }
            }
        }

        private void PrepararListas(PlanMantenimiento plan)
        {
            var accesorios = db.Accesorios
                .AsNoTracking()
                .Include(a => a.TipoAccesorio)
                .OrderBy(a => a.Nombre)
                .ThenBy(a => a.ID)
                .ToList()
                .Select(a => new
                {
                    a.ID,
                    Texto = ObtenerTextoAccesorio(a)
                })
                .ToList();

            ViewBag.AccesorioId = new SelectList(accesorios, "ID", "Texto", plan?.AccesorioId);
            var equipos = db.Database.SqlQuery<EquipoOpcion>(@"SELECT e.Id, e.Codigo, e.Nombre, i.Nombre AS InstalacionNombre
                FROM gaillard.Equipo e INNER JOIN gaillard.Instalacion i ON i.Id = e.InstalacionId
                WHERE e.Activo = 1 ORDER BY i.Nombre, e.Nombre").ToList();
            ViewBag.EquipoId = new SelectList(equipos, "Id", "Texto", plan?.EquipoId);
            ViewBag.AccesorioGrupoID = ObtenerOpcionesAccesorioGrupo(plan?.AccesorioGrupoID);
        }

        private sealed class EquipoOpcion
        {
            public EquipoOpcion() { }

            public int Id { get; set; }
            public string Codigo { get; set; }
            public string Nombre { get; set; }
            public string InstalacionNombre { get; set; }
            public string Texto => InstalacionNombre + " / " + Codigo + " - " + Nombre;
        }

        private IEnumerable<SelectListItem> ObtenerOpcionesAccesorioGrupo(int? seleccionado)
        {
            return db.AccesoriosGrupo
                .AsNoTracking()
                .Include(ag => ag.Accesorio)
                .Include(ag => ag.Accesorio.TipoAccesorio)
                .Include(ag => ag.Grupos)
                .Include(ag => ag.Grupos.Cuba)
                .Include(ag => ag.Grupos.Compartimento)
                .Include(ag => ag.Grupos.Situacion)
                .Include(ag => ag.Grupos.TipoGrupo)
                .OrderBy(ag => ag.Accesorio.Nombre)
                .ThenBy(ag => ag.Grupos.Cuba.MatriculaCuba)
                .ThenBy(ag => ag.Grupos.Compartimento.Numero)
                .ToList()
                .Select(ag => new SelectListItem
                {
                    Value = ag.ID.ToString(),
                    Text = ObtenerTextoAccesorioGrupo(ag),
                    Selected = seleccionado.HasValue && ag.ID == seleccionado.Value
                })
                .ToList();
        }

        private static string ObtenerTextoAccesorio(Accesorio accesorio)
        {
            if (accesorio == null)
            {
                return string.Empty;
            }

            var nombre = string.IsNullOrWhiteSpace(accesorio.Nombre)
                ? "Accesorio " + accesorio.ID
                : accesorio.Nombre;
            var tipo = accesorio.TipoAccesorio != null ? accesorio.TipoAccesorio.TipoAccesorio1 : null;

            return string.IsNullOrWhiteSpace(tipo) ? nombre : nombre + " (" + tipo + ")";
        }

        private static string ObtenerTextoAccesorioGrupo(AccesorioGrupo accesorioGrupo)
        {
            if (accesorioGrupo == null)
            {
                return string.Empty;
            }

            var grupo = accesorioGrupo.Grupos;
            var partes = new List<string>
            {
                ObtenerTextoAccesorio(accesorioGrupo.Accesorio)
            };

            if (grupo != null)
            {
                if (grupo.Cuba != null && !string.IsNullOrWhiteSpace(grupo.Cuba.MatriculaCuba))
                {
                    partes.Add("Cuba " + grupo.Cuba.MatriculaCuba);
                }

                if (grupo.Compartimento != null && grupo.Compartimento.Numero.HasValue)
                {
                    partes.Add("Comp. " + grupo.Compartimento.Numero.Value);
                }

                if (grupo.Situacion != null && !string.IsNullOrWhiteSpace(grupo.Situacion.LadoCubaNombre))
                {
                    partes.Add(grupo.Situacion.LadoCubaNombre);
                }

                if (grupo.TipoGrupo != null && !string.IsNullOrWhiteSpace(grupo.TipoGrupo.NombreGrupo))
                {
                    partes.Add(grupo.TipoGrupo.NombreGrupo);
                }
            }

            partes.Add("Cantidad " + accesorioGrupo.Cantidad);
            return string.Join(" - ", partes.Where(p => !string.IsNullOrWhiteSpace(p)));
        }
    }
}
