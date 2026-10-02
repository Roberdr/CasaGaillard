using CasaGaillard.Models;
using CasaGaillard.Models.ViewModels;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CasaGaillard.Areas.Mantenimiento.Controllers
{
    [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento + "," + AppRoles.Consulta)]
    public class EquiposController : Controller
    {
        private readonly GaillardEntities db = new GaillardEntities();

        public async Task<ActionResult> Index(int? instalacionId, bool incluirBajas = false)
        {
            var equipos = await db.Database.SqlQuery<EquipoFilaViewModel>(@"WITH Arbol AS
                (
                    SELECT e.Id, e.InstalacionId, e.EquipoPadreId, e.Codigo, e.Nombre, e.TipoElemento,
                        e.Fabricante, e.Modelo, e.NumeroSerie, e.Activo, 0 AS Nivel,
                        CAST(RIGHT(REPLICATE('0',10)+CONVERT(varchar(10),e.Id),10) AS varchar(max)) AS Orden
                    FROM gaillard.Equipo e WHERE e.EquipoPadreId IS NULL
                    UNION ALL
                    SELECT e.Id, e.InstalacionId, e.EquipoPadreId, e.Codigo, e.Nombre, e.TipoElemento,
                        e.Fabricante, e.Modelo, e.NumeroSerie, e.Activo, a.Nivel+1,
                        CAST(a.Orden + '/' + RIGHT(REPLICATE('0',10)+CONVERT(varchar(10),e.Id),10) AS varchar(max))
                    FROM gaillard.Equipo e INNER JOIN Arbol a ON a.Id=e.EquipoPadreId
                )
                SELECT a.Id, a.InstalacionId, i.Nombre AS InstalacionNombre, a.Nivel, a.TipoElemento,
                    p.Nombre AS PadreNombre, a.Codigo, a.Nombre, a.Fabricante, a.Modelo, a.NumeroSerie, a.Activo
                FROM Arbol a INNER JOIN gaillard.Instalacion i ON i.Id=a.InstalacionId
                LEFT JOIN gaillard.Equipo p ON p.Id=a.EquipoPadreId
                WHERE (@p0 IS NULL OR a.InstalacionId=@p0) AND (@p1=1 OR a.Activo=1)
                ORDER BY i.Nombre, a.Orden OPTION (MAXRECURSION 100)", instalacionId, incluirBajas ? 1 : 0).ToListAsync();
            ViewBag.InstalacionId = instalacionId;
            ViewBag.IncluirBajas = incluirBajas;
            ViewBag.Instalaciones = OpcionesInstalacion(instalacionId);
            return View(equipos);
        }

        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public ActionResult Create(int? instalacionId)
        {
            return View(Preparar(new EquipoFormViewModel { InstalacionId = instalacionId ?? 0 }));
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Create(EquipoFormViewModel model)
        {
            await ValidarAsync(model);
            if (!ModelState.IsValid) return View(Preparar(model));

            await db.Database.ExecuteSqlCommandAsync(@"INSERT INTO gaillard.Equipo
                (InstalacionId, EquipoPadreId, TipoElemento, Codigo, Nombre, Descripcion, Fabricante, Modelo, NumeroSerie,
                 Potencia, Caudal, Presion, Voltaje, FechaInstalacion, FechaBaja, Activo, Observaciones)
                VALUES (@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,@p14,@p15,@p16)",
                model.InstalacionId, model.EquipoPadreId, model.TipoElemento, model.Codigo.Trim(), model.Nombre.Trim(), Texto(model.Descripcion),
                Texto(model.Fabricante), Texto(model.Modelo), Texto(model.NumeroSerie), model.Potencia, model.Caudal,
                model.Presion, model.Voltaje, model.FechaInstalacion, model.FechaBaja, model.Activo, Texto(model.Observaciones));
            TempData["LoginMessage"] = "El equipo se ha creado.";
            return RedirectToAction("Index", new { instalacionId = model.InstalacionId });
        }

        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Edit(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(400);
            var model = await db.Database.SqlQuery<EquipoFormViewModel>(@"SELECT Id, InstalacionId, EquipoPadreId, TipoElemento,
                    Codigo, Nombre, Descripcion, Fabricante, Modelo, NumeroSerie, Potencia, Caudal, Presion, Voltaje,
                    FechaInstalacion, FechaBaja, Activo, Observaciones
                FROM gaillard.Equipo WHERE Id=@p0", id.Value).SingleOrDefaultAsync();
            if (model == null) return HttpNotFound();
            return View(Preparar(model));
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Edit(EquipoFormViewModel model)
        {
            await ValidarAsync(model);
            if (!ModelState.IsValid) return View(Preparar(model));

            var filas = await db.Database.ExecuteSqlCommandAsync(@"UPDATE gaillard.Equipo SET
                    InstalacionId=@p0, EquipoPadreId=@p1, TipoElemento=@p2, Codigo=@p3, Nombre=@p4, Descripcion=@p5,
                    Fabricante=@p6, Modelo=@p7, NumeroSerie=@p8, Potencia=@p9, Caudal=@p10, Presion=@p11,
                    Voltaje=@p12, FechaInstalacion=@p13, FechaBaja=@p14, Activo=@p15, Observaciones=@p16,
                    UpdatedAt=SYSDATETIME() WHERE Id=@p17",
                model.InstalacionId, model.EquipoPadreId, model.TipoElemento, model.Codigo.Trim(), model.Nombre.Trim(), Texto(model.Descripcion),
                Texto(model.Fabricante), Texto(model.Modelo), Texto(model.NumeroSerie), model.Potencia, model.Caudal,
                model.Presion, model.Voltaje, model.FechaInstalacion, model.FechaBaja, model.Activo, Texto(model.Observaciones), model.Id);
            if (filas == 0) return HttpNotFound();
            TempData["LoginMessage"] = "El equipo se ha actualizado.";
            return RedirectToAction("Index", new { instalacionId = model.InstalacionId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> DarDeBaja(int id)
        {
            await db.Database.ExecuteSqlCommandAsync(@"UPDATE gaillard.Equipo SET Activo=0,
                FechaBaja=CASE WHEN FechaInstalacion IS NULL OR FechaInstalacion <= CONVERT(date, GETDATE())
                               THEN COALESCE(FechaBaja, CONVERT(date, GETDATE())) ELSE FechaBaja END,
                UpdatedAt=SYSDATETIME() WHERE Id=@p0", id);
            TempData["LoginMessage"] = "El equipo se ha marcado como inactivo.";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }

        private async Task ValidarAsync(EquipoFormViewModel model)
        {
            if (!new[] { "SUBSISTEMA", "EQUIPO", "COMPONENTE", "INSTALACION_AUXILIAR" }.Contains(model.TipoElemento ?? ""))
                ModelState.AddModelError("TipoElemento", "Selecciona un tipo de elemento válido.");

            var instalacionOk = await db.Database.SqlQuery<int>(@"SELECT COUNT(1) FROM gaillard.Instalacion
                WHERE Id=@p0 AND Estado<>N'FUERA_SERVICIO'", model.InstalacionId).SingleAsync();
            if (instalacionOk == 0) ModelState.AddModelError("InstalacionId", "Selecciona una instalación disponible.");

            if (model.FechaInstalacion.HasValue && model.FechaBaja.HasValue && model.FechaBaja.Value < model.FechaInstalacion.Value)
                ModelState.AddModelError("FechaBaja", "La fecha de baja no puede ser anterior a la instalación.");

            var codigoDuplicado = await db.Database.SqlQuery<int>(@"SELECT COUNT(1) FROM gaillard.Equipo
                WHERE InstalacionId=@p0 AND Codigo=@p1 AND Id<>@p2", model.InstalacionId, model.Codigo ?? "", model.Id).SingleAsync();
            if (codigoDuplicado > 0) ModelState.AddModelError("Codigo", "Ya existe un equipo con ese código en la instalación.");

            if (model.EquipoPadreId.HasValue)
            {
                if (model.EquipoPadreId.Value == model.Id)
                {
                    ModelState.AddModelError("EquipoPadreId", "Un equipo no puede ser su propio equipo padre.");
                }
                else
                {
                    var padreValido = await db.Database.SqlQuery<int>(@"SELECT COUNT(1) FROM gaillard.Equipo
                        WHERE Id=@p0 AND InstalacionId=@p1 AND Activo=1", model.EquipoPadreId.Value, model.InstalacionId).SingleAsync();
                    if (padreValido == 0) ModelState.AddModelError("EquipoPadreId", "El equipo padre debe estar activo y pertenecer a la misma instalación.");
                }
            }

            if (model.Id > 0)
            {
                var tieneHijos = await db.Database.SqlQuery<int>("SELECT COUNT(1) FROM gaillard.Equipo WHERE EquipoPadreId=@p0", model.Id).SingleAsync();
                var instalacionActual = await db.Database.SqlQuery<int>("SELECT InstalacionId FROM gaillard.Equipo WHERE Id=@p0", model.Id).SingleOrDefaultAsync();
                if (tieneHijos > 0 && instalacionActual != model.InstalacionId)
                    ModelState.AddModelError("InstalacionId", "No se puede cambiar la instalación mientras el equipo tenga subsistemas asociados.");

                if (model.EquipoPadreId.HasValue)
                {
                    var creaCiclo = await db.Database.SqlQuery<int>(@"WITH Antecesores AS
                        (SELECT Id, EquipoPadreId FROM gaillard.Equipo WHERE Id=@p0
                         UNION ALL SELECT e.Id, e.EquipoPadreId FROM gaillard.Equipo e
                         INNER JOIN Antecesores a ON e.Id=a.EquipoPadreId)
                        SELECT COUNT(1) FROM Antecesores WHERE Id=@p1 OPTION (MAXRECURSION 100)",
                        model.EquipoPadreId.Value, model.Id).SingleAsync();
                    if (creaCiclo > 0) ModelState.AddModelError("EquipoPadreId", "La jerarquía elegida crearía un ciclo.");
                }
            }
        }

        private EquipoFormViewModel Preparar(EquipoFormViewModel model)
        {
            model.Instalaciones = OpcionesInstalacion(model.InstalacionId);
            model.TiposElemento = new[]
            {
                new SelectListItem { Value = "SUBSISTEMA", Text = "Subsistema" },
                new SelectListItem { Value = "EQUIPO", Text = "Equipo" },
                new SelectListItem { Value = "COMPONENTE", Text = "Componente" },
                new SelectListItem { Value = "INSTALACION_AUXILIAR", Text = "Instalación auxiliar" }
            }.Select(x => { x.Selected = x.Value == model.TipoElemento; return x; });
            model.EquiposPadre = db.Database.SqlQuery<Opcion>(@"SELECT e.Id AS Valor,
                    i.Nombre + N' / ' + e.Codigo + N' - ' + e.Nombre AS Texto
                FROM gaillard.Equipo e INNER JOIN gaillard.Instalacion i ON i.Id=e.InstalacionId
                WHERE e.Activo=1 AND e.Id<>@p0 ORDER BY i.Nombre, e.Nombre", model.Id).ToList()
                .Select(x => new SelectListItem { Value = x.Valor.ToString(), Text = x.Texto, Selected = model.EquipoPadreId == x.Valor });
            return model;
        }

        private System.Collections.Generic.IEnumerable<SelectListItem> OpcionesInstalacion(int? selected)
        {
            return db.Database.SqlQuery<Opcion>(@"SELECT Id AS Valor, Codigo + N' - ' + Nombre AS Texto
                FROM gaillard.Instalacion WHERE Estado<>N'FUERA_SERVICIO' ORDER BY Nombre").ToList()
                .Select(x => new SelectListItem { Value = x.Valor.ToString(), Text = x.Texto, Selected = selected == x.Valor });
        }

        private static string Texto(string value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private sealed class Opcion
        {
            public Opcion() { }
            public int Valor { get; set; }
            public string Texto { get; set; }
        }
    }
}
