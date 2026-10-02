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
    public class IntervencionesController : Controller
    {
        private readonly TareasMantenimientoContext tareasDb = new TareasMantenimientoContext();
        private readonly GaillardEntities catalogoDb = new GaillardEntities();

        public async Task<ActionResult> Index(int? tareaId)
        {
            if (tareaId.HasValue)
            {
                var tarea = await tareasDb.TareasMantenimiento.AsNoTracking().FirstOrDefaultAsync(x => x.ID == tareaId.Value);
                if (tarea == null) return HttpNotFound();
                ViewBag.Tarea = tarea;
            }
            var rows = await tareasDb.Database.SqlQuery<IntervencionFilaViewModel>(@"SELECT i.Id, i.TareaMantenimientoId,
                    t.Codigo AS TareaCodigo, t.Titulo AS TareaTitulo, COALESCE(NULLIF(i.Tecnico, N''), LTRIM(RTRIM(p.NombrePersona + N' ' + p.Apellido1 + N' ' + ISNULL(p.Apellido2,N'')))) AS Tecnico, i.FechaInicio, i.FechaFin,
                    i.Descripcion, i.HorasEmpleadas, i.CosteManoObra
                FROM dbo.Intervencion i INNER JOIN dbo.TareasMantenimiento t ON t.ID=i.TareaMantenimientoId LEFT JOIN gaillard.Persona p ON p.ID=i.TecnicoPersonaID
                WHERE (@p0 IS NULL OR i.TareaMantenimientoId=@p0) ORDER BY i.FechaInicio DESC", tareaId).ToListAsync();
            ViewBag.EsGlobal = !tareaId.HasValue;
            return View(rows);
        }

        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Create(int? tareaId)
        {
            if (!tareaId.HasValue) return new HttpStatusCodeResult(400);
            var tarea = await tareasDb.TareasMantenimiento.AsNoTracking().FirstOrDefaultAsync(x => x.ID == tareaId.Value);
            if (tarea == null) return HttpNotFound();
            var model = new IntervencionFormViewModel
            {
                TareaMantenimientoId = tarea.ID,
                TareaCodigo = tarea.Codigo,
                TareaTitulo = tarea.Titulo,
                Tecnico = User.Identity.Name,
                FechaInicio = DateTime.Now
            };
            CargarPersonas(model);
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Create(IntervencionFormViewModel model)
        {
            var tarea = await tareasDb.TareasMantenimiento.AsNoTracking().FirstOrDefaultAsync(x => x.ID == model.TareaMantenimientoId);
            if (tarea == null) return HttpNotFound();
            model.TareaCodigo = tarea.Codigo;
            model.TareaTitulo = tarea.Titulo;
            var tecnico = await catalogoDb.Database.SqlQuery<string>(@"SELECT LTRIM(RTRIM(NombrePersona + N' ' + Apellido1 + N' ' + ISNULL(Apellido2,N''))) FROM gaillard.Persona WHERE ID=@p0", model.TecnicoPersonaID).SingleOrDefaultAsync();
            if (string.IsNullOrWhiteSpace(tecnico)) ModelState.AddModelError("TecnicoPersonaID", "Selecciona una persona válida.");
            if (model.FechaFin.HasValue && model.FechaFin.Value < model.FechaInicio)
                ModelState.AddModelError("FechaFin", "La fecha de fin no puede ser anterior al inicio.");
            if (!ModelState.IsValid) { CargarPersonas(model); return View(model); }

            await tareasDb.Database.ExecuteSqlCommandAsync(@"INSERT INTO dbo.Intervencion
                (TareaMantenimientoId, Tecnico, TecnicoPersonaID, FechaInicio, FechaFin, Descripcion, HorasEmpleadas, CosteManoObra, CreadoPor)
                VALUES (@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8)", model.TareaMantenimientoId,
                tecnico, model.TecnicoPersonaID, model.FechaInicio, model.FechaFin, model.Descripcion.Trim(),
                model.HorasEmpleadas, model.CosteManoObra, User.Identity.Name);
            TempData["LoginMessage"] = "La intervención se ha registrado.";
            return RedirectToAction("Index", new { tareaId = model.TareaMantenimientoId });
        }

        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Edit(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(400);
            var model = await CargarFormularioEdicionAsync(id.Value);
            if (model == null) return HttpNotFound();
            CargarPersonas(model);
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Edit(IntervencionFormViewModel model)
        {
            var tarea = await tareasDb.TareasMantenimiento.AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == model.TareaMantenimientoId);
            if (tarea == null) return HttpNotFound();
            model.TareaCodigo = tarea.Codigo;
            model.TareaTitulo = tarea.Titulo;

            var tecnico = await catalogoDb.Database.SqlQuery<string>(@"SELECT LTRIM(RTRIM(NombrePersona + N' ' + Apellido1 + N' ' + ISNULL(Apellido2,N'')))
                FROM gaillard.Persona WHERE ID=@p0", model.TecnicoPersonaID).SingleOrDefaultAsync();
            if (string.IsNullOrWhiteSpace(tecnico)) ModelState.AddModelError("TecnicoPersonaID", "Selecciona una persona válida.");
            if (model.FechaFin.HasValue && model.FechaFin.Value < model.FechaInicio)
                ModelState.AddModelError("FechaFin", "La fecha de fin no puede ser anterior al inicio.");

            if (!ModelState.IsValid)
            {
                CargarPersonas(model);
                return View(model);
            }

            var filas = await tareasDb.Database.ExecuteSqlCommandAsync(@"UPDATE dbo.Intervencion SET
                    Tecnico=@p0, TecnicoPersonaID=@p1, FechaInicio=@p2, FechaFin=@p3, Descripcion=@p4,
                    HorasEmpleadas=@p5, CosteManoObra=@p6
                WHERE Id=@p7", tecnico, model.TecnicoPersonaID, model.FechaInicio, model.FechaFin,
                model.Descripcion.Trim(), model.HorasEmpleadas, model.CosteManoObra, model.Id);
            if (filas == 0) return HttpNotFound();
            TempData["LoginMessage"] = "La intervención se ha actualizado.";
            return RedirectToAction("Details", new { id = model.Id });
        }

        public async Task<ActionResult> Details(int? id, int? nuevoAccesorioId)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(400);
            var model = await tareasDb.Database.SqlQuery<IntervencionFilaViewModel>(@"SELECT i.Id, i.TareaMantenimientoId,
                    t.Codigo AS TareaCodigo, t.Titulo AS TareaTitulo, COALESCE(NULLIF(i.Tecnico, N''), LTRIM(RTRIM(p.NombrePersona + N' ' + p.Apellido1 + N' ' + ISNULL(p.Apellido2,N'')))) AS Tecnico, i.FechaInicio, i.FechaFin,
                    i.Descripcion, i.HorasEmpleadas, i.CosteManoObra
                FROM dbo.Intervencion i INNER JOIN dbo.TareasMantenimiento t ON t.ID=i.TareaMantenimientoId LEFT JOIN gaillard.Persona p ON p.ID=i.TecnicoPersonaID
                WHERE i.Id=@p0", id.Value).SingleOrDefaultAsync();
            if (model == null) return HttpNotFound();
            ViewBag.Repuestos = await tareasDb.Database.SqlQuery<IntervencionRepuestoViewModel>(@"SELECT r.Id, r.IntervencionId,
                    r.AccesorioId, a.Nombre AS AccesorioNombre, r.Cantidad, r.CosteUnitario, r.Observaciones
                FROM dbo.IntervencionRepuesto r INNER JOIN gaillard.Accesorio a ON a.ID=r.AccesorioId
                WHERE r.IntervencionId=@p0 ORDER BY a.Nombre", id.Value).ToListAsync();
            ViewBag.RepuestoForm = PrepararRepuesto(new IntervencionRepuestoFormViewModel { IntervencionId = id.Value, AccesorioId = nuevoAccesorioId ?? 0 });
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> AddRepuesto(IntervencionRepuestoFormViewModel model)
        {
            var existeIntervencion = await tareasDb.Database.SqlQuery<int>("SELECT COUNT(1) FROM dbo.Intervencion WHERE Id=@p0", model.IntervencionId).SingleAsync();
            if (existeIntervencion == 0) return HttpNotFound();
            var existeAccesorio = await catalogoDb.Database.SqlQuery<int>("SELECT COUNT(1) FROM gaillard.Accesorio WHERE ID=@p0", model.AccesorioId).SingleAsync();
            if (existeAccesorio == 0) ModelState.AddModelError("AccesorioId", "Selecciona un repuesto válido.");
            if (!ModelState.IsValid)
            {
                TempData["LoginMessage"] = "No se pudo añadir el repuesto. Revisa la cantidad, el coste y el accesorio.";
                return RedirectToAction("Details", new { id = model.IntervencionId });
            }
            await tareasDb.Database.ExecuteSqlCommandAsync(@"INSERT INTO dbo.IntervencionRepuesto
                (IntervencionId, AccesorioId, Cantidad, CosteUnitario, Observaciones) VALUES (@p0,@p1,@p2,@p3,@p4)",
                model.IntervencionId, model.AccesorioId, model.Cantidad, model.CosteUnitario, Texto(model.Observaciones));
            TempData["LoginMessage"] = "El repuesto utilizado se ha añadido.";
            return RedirectToAction("Details", new { id = model.IntervencionId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> RemoveRepuesto(int id)
        {
            var intervencionId = await tareasDb.Database.SqlQuery<int?>("SELECT IntervencionId FROM dbo.IntervencionRepuesto WHERE Id=@p0", id).SingleOrDefaultAsync();
            if (!intervencionId.HasValue) return HttpNotFound();
            await tareasDb.Database.ExecuteSqlCommandAsync("DELETE FROM dbo.IntervencionRepuesto WHERE Id=@p0", id);
            return RedirectToAction("Details", new { id = intervencionId.Value });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { tareasDb.Dispose(); catalogoDb.Dispose(); }
            base.Dispose(disposing);
        }

        private IntervencionRepuestoFormViewModel PrepararRepuesto(IntervencionRepuestoFormViewModel model)
        {
            model.Accesorios = catalogoDb.Database.SqlQuery<Opcion>(@"SELECT a.ID AS Valor,
                    COALESCE(NULLIF(a.Nombre, N''), NULLIF(t.TipoAccesorio, N''), N'Accesorio ' + CONVERT(nvarchar(20), a.ID))
                    + CASE WHEN a.Activo=1 THEN N'' ELSE N' (inactivo)' END AS Texto
                FROM gaillard.Accesorio a
                LEFT JOIN gaillard.TipoAccesorio t ON t.ID=a.TipoAccesorioID
                ORDER BY Texto, a.ID").ToList()
                .Select(x => new SelectListItem { Value = x.Valor.ToString(), Text = x.Texto, Selected = x.Valor == model.AccesorioId });
            return model;
        }

        private void CargarPersonas(IntervencionFormViewModel model)
        {
            var personas = (from p in catalogoDb.Personas.AsNoTracking()
                            orderby p.NombrePersona, p.Apellido1, p.Apellido2
                            select new { p.ID, p.NombrePersona, p.Apellido1, p.Apellido2 }).ToList()
                .Select(p => new SelectListItem
                {
                    Value = p.ID.ToString(),
                    Text = (p.NombrePersona + " " + p.Apellido1 + " " + (p.Apellido2 ?? "")).Trim(),
                    Selected = p.ID == model.TecnicoPersonaID
                });
            model.Personas = personas.ToList();
        }

        private async Task<IntervencionFormViewModel> CargarFormularioEdicionAsync(int id)
        {
            return await tareasDb.Database.SqlQuery<IntervencionFormViewModel>(@"SELECT i.Id, i.TareaMantenimientoId,
                    i.TecnicoPersonaID, i.FechaInicio, i.FechaFin, i.Descripcion, i.HorasEmpleadas, i.CosteManoObra,
                    t.Codigo AS TareaCodigo, t.Titulo AS TareaTitulo
                FROM dbo.Intervencion i INNER JOIN dbo.TareasMantenimiento t ON t.ID=i.TareaMantenimientoId
                WHERE i.Id=@p0", id).SingleOrDefaultAsync();
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
