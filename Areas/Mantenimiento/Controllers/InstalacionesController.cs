using CasaGaillard.Models;
using CasaGaillard.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CasaGaillard.Areas.Mantenimiento.Controllers
{
    [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento + "," + AppRoles.Consulta)]
    public class InstalacionesController : Controller
    {
        private readonly GaillardEntities db = new GaillardEntities();

        public async Task<ActionResult> Index(bool incluirBajas = false)
        {
            var sql = @"SELECT i.Id, i.Codigo, i.Nombre, t.Nombre AS TipoNombre, u.Nombre AS UbicacionNombre, i.Estado, i.Criticidad,
                    (SELECT COUNT(1) FROM gaillard.Equipo e WHERE e.InstalacionId = i.Id) AS NumeroEquipos
                FROM gaillard.Instalacion i
                LEFT JOIN gaillard.TipoInstalacion t ON t.Id = i.TipoInstalacionId
                LEFT JOIN gaillard.Ubicacion u ON u.UbicacionId = i.UbicacionId
                WHERE (@p0 = 1 OR i.Estado <> N'FUERA_SERVICIO')
                ORDER BY i.Nombre";
            var instalaciones = await db.Database.SqlQuery<InstalacionFilaViewModel>(sql, incluirBajas ? 1 : 0).ToListAsync();
            ViewBag.IncluirBajas = incluirBajas;
            return View(instalaciones);
        }

        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public ActionResult Create()
        {
            return View(Preparar(new InstalacionFormViewModel()));
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Create(InstalacionFormViewModel model)
        {
            await ValidarAsync(model);
            if (!ModelState.IsValid)
            {
                return View(Preparar(model));
            }

            await db.Database.ExecuteSqlCommandAsync(@"INSERT INTO gaillard.Instalacion
                (Codigo, Nombre, Descripcion, TipoInstalacionId, UbicacionId, ResponsablePersonaId, FechaPuestaMarcha, FechaBaja,
                 Criticidad, Estado, CodigoPlano, CriticaProduccion, CriticaSeguridad, Observaciones)
                VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p13)",
                model.Codigo.Trim(), model.Nombre.Trim(), Texto(model.Descripcion), model.TipoInstalacionId, model.UbicacionId, model.ResponsablePersonaId,
                model.FechaPuestaMarcha, model.FechaBaja, model.Criticidad, model.Estado, Texto(model.CodigoPlano),
                model.CriticaProduccion, model.CriticaSeguridad, Texto(model.Observaciones));

            TempData["LoginMessage"] = "La instalación se ha creado.";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Edit(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(400);
            var model = await db.Database.SqlQuery<InstalacionFormViewModel>(@"SELECT Id, Codigo, Nombre, Descripcion,
                    TipoInstalacionId, UbicacionId, ResponsablePersonaId, FechaPuestaMarcha, FechaBaja, Criticidad, Estado,
                    CodigoPlano, CriticaProduccion, CriticaSeguridad, Observaciones
                FROM gaillard.Instalacion WHERE Id = @p0", id.Value).SingleOrDefaultAsync();
            if (model == null) return HttpNotFound();
            return View(Preparar(model));
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Edit(InstalacionFormViewModel model)
        {
            await ValidarAsync(model);
            if (!ModelState.IsValid) return View(Preparar(model));

            var filas = await db.Database.ExecuteSqlCommandAsync(@"UPDATE gaillard.Instalacion SET
                    Codigo=@p0, Nombre=@p1, Descripcion=@p2, TipoInstalacionId=@p3, UbicacionId=@p4, ResponsablePersonaId=@p5,
                    FechaPuestaMarcha=@p6, FechaBaja=@p7, Criticidad=@p8, Estado=@p9, CodigoPlano=@p10,
                    CriticaProduccion=@p11, CriticaSeguridad=@p12, Observaciones=@p13, UpdatedAt=SYSDATETIME()
                WHERE Id=@p14", model.Codigo.Trim(), model.Nombre.Trim(), Texto(model.Descripcion), model.TipoInstalacionId,
                model.UbicacionId, model.ResponsablePersonaId, model.FechaPuestaMarcha, model.FechaBaja, model.Criticidad, model.Estado,
                Texto(model.CodigoPlano), model.CriticaProduccion, model.CriticaSeguridad, Texto(model.Observaciones), model.Id);
            if (filas == 0) return HttpNotFound();
            TempData["LoginMessage"] = "La instalación se ha actualizado.";
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> DarDeBaja(int id)
        {
            await db.Database.ExecuteSqlCommandAsync(@"UPDATE gaillard.Instalacion SET Estado=N'FUERA_SERVICIO',
                FechaBaja=CASE WHEN FechaPuestaMarcha IS NULL OR FechaPuestaMarcha <= CONVERT(date, GETDATE())
                               THEN COALESCE(FechaBaja, CONVERT(date, GETDATE())) ELSE FechaBaja END,
                UpdatedAt=SYSDATETIME() WHERE Id=@p0", id);
            TempData["LoginMessage"] = "La instalación se ha marcado fuera de servicio.";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }

        private async Task ValidarAsync(InstalacionFormViewModel model)
        {
            if (model.Criticidad < 1 || model.Criticidad > 5)
                ModelState.AddModelError("Criticidad", "La criticidad debe estar entre 1 y 5.");
            if (!new[] { "ACTIVA", "PARADA", "MANTENIMIENTO", "FUERA_SERVICIO" }.Contains(model.Estado ?? ""))
                ModelState.AddModelError("Estado", "Selecciona un estado válido.");
            if (model.FechaPuestaMarcha.HasValue && model.FechaBaja.HasValue && model.FechaBaja.Value < model.FechaPuestaMarcha.Value)
                ModelState.AddModelError("FechaBaja", "La fecha de baja no puede ser anterior a la puesta en marcha.");

            var duplicado = await db.Database.SqlQuery<int>(@"SELECT COUNT(1) FROM gaillard.Instalacion
                WHERE Codigo=@p0 AND Id<>@p1", model.Codigo ?? "", model.Id).SingleAsync();
            if (duplicado > 0) ModelState.AddModelError("Codigo", "Ya existe una instalación con ese código.");

            if (model.TipoInstalacionId.HasValue && await db.Database.SqlQuery<int>(
                "SELECT COUNT(1) FROM gaillard.TipoInstalacion WHERE Id=@p0", model.TipoInstalacionId.Value).SingleAsync() == 0)
                ModelState.AddModelError("TipoInstalacionId", "Selecciona un tipo válido.");
            if (model.ResponsablePersonaId.HasValue && await db.Database.SqlQuery<int>(
                "SELECT COUNT(1) FROM gaillard.Persona WHERE ID=@p0", model.ResponsablePersonaId.Value).SingleAsync() == 0)
                ModelState.AddModelError("ResponsablePersonaId", "Selecciona una persona válida.");
            if (model.UbicacionId.HasValue && await db.Database.SqlQuery<int>(
                "SELECT COUNT(1) FROM gaillard.Ubicacion WHERE UbicacionId=@p0", model.UbicacionId.Value).SingleAsync() == 0)
                ModelState.AddModelError("UbicacionId", "Selecciona una ubicación válida.");
        }

        private InstalacionFormViewModel Preparar(InstalacionFormViewModel model)
        {
            model.Tipos = db.Database.SqlQuery<Opcion>("SELECT Id AS Valor, Nombre AS Texto FROM gaillard.TipoInstalacion ORDER BY Nombre")
                .ToList().Select(x => new SelectListItem { Value = x.Valor.ToString(), Text = x.Texto, Selected = model.TipoInstalacionId == x.Valor });
            model.Ubicaciones = db.Database.SqlQuery<Opcion>(@"SELECT UbicacionId AS Valor,
                    CAST(UbicacionId AS NVARCHAR(20)) + N' - ' + Nombre AS Texto
                FROM gaillard.Ubicacion ORDER BY Nombre")
                .ToList().Select(x => new SelectListItem { Value = x.Valor.ToString(), Text = x.Texto, Selected = model.UbicacionId == x.Valor });
            model.Responsables = db.Database.SqlQuery<Opcion>(@"SELECT ID AS Valor,
                    LTRIM(RTRIM(COALESCE(NombrePersona, N'') + N' ' + COALESCE(Apellido1, N'') + N' ' + COALESCE(Apellido2, N''))) AS Texto
                FROM gaillard.Persona ORDER BY NombrePersona, Apellido1, Apellido2")
                .ToList().Select(x => new SelectListItem { Value = x.Valor.ToString(), Text = x.Texto, Selected = model.ResponsablePersonaId == x.Valor });
            return model;
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
