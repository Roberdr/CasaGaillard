using CasaGaillard.Models;
using CasaGaillard.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CasaGaillard.Areas.Mantenimiento.Controllers
{
    [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento + "," + AppRoles.Consulta)]
    [RouteArea("Mantenimiento")]
    public class TareasMantenimientoController : Controller
    {
        private readonly TareasMantenimientoContext tareasDb = new TareasMantenimientoContext();
        private readonly GaillardEntities lookupDb = new GaillardEntities();

        public async Task<ActionResult> Index(string codigo = null)
        {
            var consulta = tareasDb.TareasMantenimiento
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(codigo))
            {
                consulta = consulta.Where(t => t.Codigo == codigo);
            }

            var tareas = await consulta
                .OrderByDescending(t => t.FechaCreacion)
                .Select(t => new TareaMantenimientoResumenViewModel
                {
                    ID = t.ID,
                    Codigo = t.Codigo,
                    Titulo = t.Titulo,
                    Ubicacion = t.Ubicacion,
                    Equipo = t.Equipo,
                    InstalacionId = t.InstalacionId,
                    EquipoId = t.EquipoId,
                    CubaId = t.CubaId,
                    GrupoId = t.GrupoId,
                    VehiculoId = t.VehiculoId,
                    PlanMantenimientoId = t.PlanMantenimientoId,
                    Prioridad = t.Prioridad,
                    Estado = t.Estado,
                    AsignadaA = t.AsignadaA,
                    FechaCreacion = t.FechaCreacion
                })
                .ToListAsync();

            ViewBag.Codigo = codigo;

            return View(tareas);
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var tarea = await tareasDb.TareasMantenimiento.FindAsync(id.Value);
            if (tarea == null)
            {
                return HttpNotFound();
            }

            ViewBag.Fotos = await tareasDb.TareaMantenimientoFotos
                .AsNoTracking()
                .Where(f => f.TareaMantenimientoID == tarea.ID)
                .OrderByDescending(f => f.FechaCreacion)
                .ToListAsync();
            ViewBag.AccesoriosTexto = ObtenerAccesoriosTexto(tarea.ID, tarea.AccesoriosNecesarios);
            return View(tarea);
        }

        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Gestion(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var tarea = await tareasDb.TareasMantenimiento.FindAsync(id.Value);
            if (tarea == null)
            {
                return HttpNotFound();
            }

            return View(CreateGestionModel(tarea));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Gestion(TareaMantenimientoGestionViewModel model)
        {
            await ValidarReferenciasPersonaEntidadAsync(model.DetectadoPorPersonaID, model.ContactoPersonaID, model.AsignadaAPersonaID, model.EmpresaExteriorEntidadID);
            if (!ModelState.IsValid)
            {
                return View(CreateGestionModel(model));
            }

            var tarea = await tareasDb.TareasMantenimiento.FindAsync(model.ID);
            if (tarea == null)
            {
                return HttpNotFound();
            }

            tarea.Prioridad = string.IsNullOrWhiteSpace(model.Prioridad) ? tarea.Prioridad : model.Prioridad;
            tarea.Estado = string.IsNullOrWhiteSpace(model.Estado) ? tarea.Estado : model.Estado;
            tarea.AsignadaA = model.AsignadaA?.Trim();
            tarea.EmpresaExterior = model.EmpresaExterior?.Trim();
            tarea.AsignadaAPersonaID = model.AsignadaAPersonaID;
            tarea.EmpresaExteriorEntidadID = model.EmpresaExteriorEntidadID;
            tarea.AsignadaA = ObtenerNombrePersona(model.AsignadaAPersonaID) ?? model.AsignadaA?.Trim();
            tarea.EmpresaExterior = ObtenerNombreEntidad(model.EmpresaExteriorEntidadID) ?? model.EmpresaExterior?.Trim();
            tarea.AccionesARealizar = model.AccionesARealizar?.Trim();
            tarea.AccesoriosNecesarios = NormalizarAccesorios(model.AccesoriosSeleccionados);
            tarea.Observaciones = model.Observaciones?.Trim();
            tarea.ActualizadoPor = User.Identity.Name;
            tarea.FechaActualizacion = DateTime.Now;

            await tareasDb.SaveChangesAsync();
            await GuardarAccesoriosTareaAsync(tarea.ID, model.AccesoriosSeleccionados);

            TempData["LoginMessage"] = "La tarea se ha actualizado.";
            return RedirectToAction("Details", new { id = tarea.ID });
        }

        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento + "," + AppRoles.Consulta)]
        public ActionResult Create()
        {
            var model = CreateFormModel();
            model.FechaDeteccion = DateTime.Now;
            model.Prioridad = "Media";
            model.Estado = "Nueva";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento + "," + AppRoles.Consulta)]
        public async Task<ActionResult> Create(TareaMantenimientoFormViewModel model)
        {
            await ValidarReferenciasActivoAsync(model);
            if (!ModelState.IsValid)
            {
                model = CreateFormModel(model);
                return View(model);
            }

            var tarea = new TareaMantenimiento
            {
                Codigo = GenerarCodigo(),
                Titulo = model.Titulo?.Trim(),
                Descripcion = model.Descripcion?.Trim(),
                Ubicacion = model.Ubicacion?.Trim(),
                Equipo = model.Equipo?.Trim(),
                InstalacionId = model.InstalacionId,
                EquipoId = model.EquipoId,
                CubaId = model.CubaId,
                GrupoId = model.GrupoId,
                VehiculoId = model.VehiculoId,
                PlanMantenimientoId = model.PlanMantenimientoId,
                DetectadoPor = ObtenerNombrePersona(model.DetectadoPorPersonaID) ?? model.DetectadoPor?.Trim(),
                Contacto = ObtenerNombrePersona(model.ContactoPersonaID) ?? model.Contacto?.Trim(),
                DetectadoPorPersonaID = model.DetectadoPorPersonaID,
                ContactoPersonaID = model.ContactoPersonaID,
                Prioridad = string.IsNullOrWhiteSpace(model.Prioridad) ? "Media" : model.Prioridad,
                Estado = string.IsNullOrWhiteSpace(model.Estado) ? "Nueva" : model.Estado,
                AsignadaA = ObtenerNombrePersona(model.AsignadaAPersonaID) ?? model.AsignadaA?.Trim(),
                EmpresaExterior = ObtenerNombreEntidad(model.EmpresaExteriorEntidadID) ?? model.EmpresaExterior?.Trim(),
                AsignadaAPersonaID = model.AsignadaAPersonaID,
                EmpresaExteriorEntidadID = model.EmpresaExteriorEntidadID,
                AccionesARealizar = model.AccionesARealizar?.Trim(),
                AccesoriosNecesarios = NormalizarAccesorios(model.AccesoriosSeleccionados),
                Observaciones = model.Observaciones?.Trim(),
                CreadoPor = User.Identity.Name,
                FechaDeteccion = model.FechaDeteccion == default(DateTime) ? DateTime.Now : model.FechaDeteccion,
                FechaCreacion = DateTime.Now
            };

            tareasDb.TareasMantenimiento.Add(tarea);
            await tareasDb.SaveChangesAsync();
            await GuardarAccesoriosTareaAsync(tarea.ID, model.AccesoriosSeleccionados);
            GuardarFotosAdjuntas(tarea.ID, Request.Files);

            TempData["LoginMessage"] = "La tarea de mantenimiento se ha creado correctamente.";
            return RedirectToAction("Details", new { id = tarea.ID });
        }

        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var tarea = await tareasDb.TareasMantenimiento.FindAsync(id.Value);
            if (tarea == null)
            {
                return HttpNotFound();
            }

            return View(MapToForm(tarea));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Edit(TareaMantenimientoFormViewModel model)
        {
            await ValidarReferenciasActivoAsync(model);
            if (!ModelState.IsValid)
            {
                model = CreateFormModel(model);
                return View(model);
            }

            var tarea = await tareasDb.TareasMantenimiento.FindAsync(model.ID);
            if (tarea == null)
            {
                return HttpNotFound();
            }

            tarea.Titulo = model.Titulo?.Trim();
            tarea.Descripcion = model.Descripcion?.Trim();
            tarea.Ubicacion = model.Ubicacion?.Trim();
            tarea.Equipo = model.Equipo?.Trim();
            tarea.InstalacionId = model.InstalacionId;
            tarea.EquipoId = model.EquipoId;
            tarea.CubaId = model.CubaId;
            tarea.GrupoId = model.GrupoId;
            tarea.VehiculoId = model.VehiculoId;
            tarea.PlanMantenimientoId = model.PlanMantenimientoId;
            tarea.DetectadoPor = model.DetectadoPor?.Trim();
            tarea.Contacto = model.Contacto?.Trim();
            tarea.Prioridad = string.IsNullOrWhiteSpace(model.Prioridad) ? "Media" : model.Prioridad;
            tarea.Estado = string.IsNullOrWhiteSpace(model.Estado) ? "Nueva" : model.Estado;
            tarea.AsignadaA = model.AsignadaA?.Trim();
            tarea.EmpresaExterior = model.EmpresaExterior?.Trim();
            tarea.DetectadoPorPersonaID = model.DetectadoPorPersonaID;
            tarea.ContactoPersonaID = model.ContactoPersonaID;
            tarea.AsignadaAPersonaID = model.AsignadaAPersonaID;
            tarea.EmpresaExteriorEntidadID = model.EmpresaExteriorEntidadID;
            tarea.DetectadoPor = ObtenerNombrePersona(model.DetectadoPorPersonaID) ?? model.DetectadoPor?.Trim();
            tarea.Contacto = ObtenerNombrePersona(model.ContactoPersonaID) ?? model.Contacto?.Trim();
            tarea.AsignadaA = ObtenerNombrePersona(model.AsignadaAPersonaID) ?? model.AsignadaA?.Trim();
            tarea.EmpresaExterior = ObtenerNombreEntidad(model.EmpresaExteriorEntidadID) ?? model.EmpresaExterior?.Trim();
            tarea.AccionesARealizar = model.AccionesARealizar?.Trim();
            tarea.AccesoriosNecesarios = NormalizarAccesorios(model.AccesoriosSeleccionados);
            tarea.Observaciones = model.Observaciones?.Trim();
            tarea.ActualizadoPor = User.Identity.Name;
            tarea.FechaActualizacion = DateTime.Now;

            await tareasDb.SaveChangesAsync();
            await GuardarAccesoriosTareaAsync(tarea.ID, model.AccesoriosSeleccionados);
            GuardarFotosAdjuntas(tarea.ID, Request.Files);

            TempData["LoginMessage"] = "La tarea se ha actualizado.";
            return RedirectToAction("Details", new { id = tarea.ID });
        }

        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento + "," + AppRoles.Consulta)]
        public async Task<ActionResult> Foto(int id)
        {
            var foto = await tareasDb.TareaMantenimientoFotos.AsNoTracking().FirstOrDefaultAsync(f => f.ID == id);
            if (foto == null)
            {
                return HttpNotFound();
            }

            var ruta = Server.MapPath(foto.RutaArchivo);
            if (string.IsNullOrWhiteSpace(ruta) || !System.IO.File.Exists(ruta))
            {
                return HttpNotFound();
            }

            return File(ruta, string.IsNullOrWhiteSpace(foto.ContentType) ? "application/octet-stream" : foto.ContentType);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                tareasDb.Dispose();
                lookupDb.Dispose();
            }

            base.Dispose(disposing);
        }

        private TareaMantenimientoFormViewModel CreateFormModel(TareaMantenimientoFormViewModel source = null)
        {
            source = source ?? new TareaMantenimientoFormViewModel();
            source.Prioridades = GetPrioridades(source.Prioridad);
            source.Estados = GetEstados(source.Estado);
            source.Empleados = GetEmpleados(source.AsignadaA);
            source.PersonasDetectadoPor = GetPersonas(source.DetectadoPorPersonaID);
            source.PersonasContacto = GetPersonas(source.ContactoPersonaID);
            source.PersonasAsignadaA = GetPersonas(source.AsignadaAPersonaID);
            source.Entidades = GetEntidades(source.EmpresaExteriorEntidadID);
            source.Accesorios = GetAccesorios(source.AccesoriosSeleccionados);
            source.Instalaciones = GetInstalaciones(source.InstalacionId);
            source.Equipos = GetEquipos(source.EquipoId);
            source.Cubas = GetCubas(source.CubaId);
            source.Grupos = GetGrupos(source.GrupoId);
            source.Vehiculos = GetVehiculos(source.VehiculoId);
            source.PlanesMantenimiento = GetPlanesMantenimiento(source.PlanMantenimientoId);
            return source;
        }

        private void GuardarFotosAdjuntas(int tareaId, HttpFileCollectionBase archivos)
        {
            if (archivos == null || archivos.Count == 0)
            {
                return;
            }

            var raiz = Server.MapPath("~/App_Data/TareasMantenimientoFotos");
            Directory.CreateDirectory(raiz);

            for (var i = 0; i < archivos.Count; i++)
            {
                var archivo = archivos[i];
                if (archivo == null || archivo.ContentLength <= 0)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(archivo.ContentType) ||
                    !archivo.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var extension = Path.GetExtension(archivo.FileName);
                if (string.IsNullOrWhiteSpace(extension))
                {
                    extension = ".jpg";
                }

                var carpetaTarea = Path.Combine(raiz, tareaId.ToString());
                Directory.CreateDirectory(carpetaTarea);

                var nombreFisico = Guid.NewGuid().ToString("N") + extension.ToLowerInvariant();
                var rutaFisica = Path.Combine(carpetaTarea, nombreFisico);
                archivo.SaveAs(rutaFisica);

                tareasDb.TareaMantenimientoFotos.Add(new TareaMantenimientoFoto
                {
                    TareaMantenimientoID = tareaId,
                    RutaArchivo = "~/App_Data/TareasMantenimientoFotos/" + tareaId + "/" + nombreFisico,
                    NombreOriginal = Path.GetFileName(archivo.FileName),
                    ContentType = archivo.ContentType,
                    FechaCreacion = DateTime.Now
                });
            }

            tareasDb.SaveChanges();
        }

        private TareaMantenimientoFormViewModel MapToForm(TareaMantenimiento tarea)
        {
            return CreateFormModel(new TareaMantenimientoFormViewModel
            {
                ID = tarea.ID,
                Codigo = tarea.Codigo,
                Titulo = tarea.Titulo,
                Descripcion = tarea.Descripcion,
                Ubicacion = tarea.Ubicacion,
                Equipo = tarea.Equipo,
                InstalacionId = tarea.InstalacionId,
                EquipoId = tarea.EquipoId,
                CubaId = tarea.CubaId,
                GrupoId = tarea.GrupoId,
                VehiculoId = tarea.VehiculoId,
                PlanMantenimientoId = tarea.PlanMantenimientoId,
                DetectadoPor = tarea.DetectadoPor,
                Contacto = tarea.Contacto,
                DetectadoPorPersonaID = tarea.DetectadoPorPersonaID,
                ContactoPersonaID = tarea.ContactoPersonaID,
                Prioridad = tarea.Prioridad,
                Estado = tarea.Estado,
                AsignadaA = tarea.AsignadaA,
                EmpresaExterior = tarea.EmpresaExterior,
                AsignadaAPersonaID = tarea.AsignadaAPersonaID,
                EmpresaExteriorEntidadID = tarea.EmpresaExteriorEntidadID,
                AccionesARealizar = tarea.AccionesARealizar,
                AccesoriosSeleccionados = ObtenerAccesoriosSeleccionados(tarea.ID, tarea.AccesoriosNecesarios),
                Observaciones = tarea.Observaciones,
                FechaDeteccion = tarea.FechaDeteccion,
                FechaCreacion = tarea.FechaCreacion,
                FechaActualizacion = tarea.FechaActualizacion,
                CreadoPor = tarea.CreadoPor,
                ActualizadoPor = tarea.ActualizadoPor
            });
        }

        private TareaMantenimientoGestionViewModel CreateGestionModel(TareaMantenimiento tarea)
        {
            return CreateGestionModel(new TareaMantenimientoGestionViewModel
            {
                ID = tarea.ID,
                Codigo = tarea.Codigo,
                Titulo = tarea.Titulo,
                Ubicacion = tarea.Ubicacion,
                Equipo = tarea.Equipo,
                InstalacionId = tarea.InstalacionId,
                EquipoId = tarea.EquipoId,
                CubaId = tarea.CubaId,
                GrupoId = tarea.GrupoId,
                VehiculoId = tarea.VehiculoId,
                PlanMantenimientoId = tarea.PlanMantenimientoId,
                DetectadoPor = tarea.DetectadoPor,
                Contacto = tarea.Contacto,
                DetectadoPorPersonaID = tarea.DetectadoPorPersonaID,
                ContactoPersonaID = tarea.ContactoPersonaID,
                Descripcion = tarea.Descripcion,
                Prioridad = tarea.Prioridad,
                Estado = tarea.Estado,
                AsignadaA = tarea.AsignadaA,
                EmpresaExterior = tarea.EmpresaExterior,
                AsignadaAPersonaID = tarea.AsignadaAPersonaID,
                EmpresaExteriorEntidadID = tarea.EmpresaExteriorEntidadID,
                AccionesARealizar = tarea.AccionesARealizar,
                AccesoriosSeleccionados = ObtenerAccesoriosSeleccionados(tarea.ID, tarea.AccesoriosNecesarios),
                Observaciones = tarea.Observaciones
            });
        }

        private TareaMantenimientoGestionViewModel CreateGestionModel(TareaMantenimientoGestionViewModel source)
        {
            source = source ?? new TareaMantenimientoGestionViewModel();
            source.Prioridades = GetPrioridades(source.Prioridad);
            source.Estados = GetEstados(source.Estado);
            source.Empleados = GetEmpleados(source.AsignadaA);
            source.PersonasDetectadoPor = GetPersonas(source.DetectadoPorPersonaID);
            source.PersonasContacto = GetPersonas(source.ContactoPersonaID);
            source.PersonasAsignadaA = GetPersonas(source.AsignadaAPersonaID);
            source.Entidades = GetEntidades(source.EmpresaExteriorEntidadID);
            source.Accesorios = GetAccesorios(source.AccesoriosSeleccionados);
            source.Instalaciones = GetInstalaciones(source.InstalacionId);
            source.Equipos = GetEquipos(source.EquipoId);
            source.PlanesMantenimiento = GetPlanesMantenimiento(source.PlanMantenimientoId);
            return source;
        }

        private async Task ValidarReferenciasActivoAsync(TareaMantenimientoFormViewModel model)
        {
            await ValidarReferenciasPersonaEntidadAsync(model.DetectadoPorPersonaID, model.ContactoPersonaID, model.AsignadaAPersonaID, model.EmpresaExteriorEntidadID);
            int? equipoInstalacionId = null;
            if (model.EquipoId.HasValue)
            {
                equipoInstalacionId = await lookupDb.Database.SqlQuery<int?>(
                    "SELECT InstalacionId FROM gaillard.Equipo WHERE Id = @p0 AND Activo = 1", model.EquipoId.Value).SingleOrDefaultAsync();
                if (!equipoInstalacionId.HasValue)
                {
                    ModelState.AddModelError("EquipoId", "Selecciona un equipo activo válido.");
                }
                else if (model.InstalacionId.HasValue && model.InstalacionId.Value != equipoInstalacionId.Value)
                {
                    ModelState.AddModelError("EquipoId", "El equipo no pertenece a la instalación seleccionada.");
                }
            }

            if (model.InstalacionId.HasValue)
            {
                var instalacionExiste = await lookupDb.Database.SqlQuery<int>(
                    "SELECT COUNT(1) FROM gaillard.Instalacion WHERE Id = @p0", model.InstalacionId.Value).SingleAsync() > 0;
                if (!instalacionExiste)
                {
                    ModelState.AddModelError("InstalacionId", "Selecciona una instalación válida.");
                }
            }

            if (model.PlanMantenimientoId.HasValue)
            {
                var plan = await lookupDb.Database.SqlQuery<PlanOpcion>(@"SELECT PlanId, EquipoId, CAST(NULL AS NVARCHAR(255)) AS Texto
                    FROM gaillard.PlanMantenimiento WHERE PlanId = @p0 AND Activo = 1", model.PlanMantenimientoId.Value).SingleOrDefaultAsync();
                if (plan == null)
                {
                    ModelState.AddModelError("PlanMantenimientoId", "Selecciona un plan activo válido.");
                }
                else if (plan.EquipoId.HasValue && model.EquipoId.HasValue && plan.EquipoId.Value != model.EquipoId.Value)
                {
                    ModelState.AddModelError("PlanMantenimientoId", "El plan seleccionado pertenece a otro equipo.");
                }
                else if (plan.EquipoId.HasValue && !model.EquipoId.HasValue)
                {
                    model.EquipoId = plan.EquipoId;
                    equipoInstalacionId = await lookupDb.Database.SqlQuery<int?>(
                        "SELECT InstalacionId FROM gaillard.Equipo WHERE Id = @p0", model.EquipoId.Value).SingleOrDefaultAsync();
                }
            }

            GrupoOpcion grupoSeleccionado = null;
            if (model.GrupoId.HasValue)
            {
                grupoSeleccionado = await lookupDb.Database.SqlQuery<GrupoOpcion>(@"SELECT g.ID AS Id, g.CubaID,
                        COALESCE(NULLIF(c.MatriculaCuba, N''), NULLIF(c.Codigo, N''), N'Cisterna') + N' / ' +
                        COALESCE(NULLIF(tg.NombreGrupo, N''), N'Grupo') +
                        CASE WHEN cp.Numero IS NULL THEN N'' ELSE N' - Comp. ' + CONVERT(NVARCHAR(10), cp.Numero) END +
                        CASE WHEN s.LadoCubaNombre IS NULL THEN N'' ELSE N' - ' + s.LadoCubaNombre END AS Texto
                    FROM gaillard.Grupo g
                    INNER JOIN gaillard.Cuba c ON c.ID = g.CubaID
                    LEFT JOIN gaillard.TipoGrupo tg ON tg.ID = g.TipoGrupoID
                    LEFT JOIN gaillard.Compartimento cp ON cp.ID = g.CompartimentoID
                    LEFT JOIN gaillard.Situacion s ON s.ID = g.SituacionID
                    WHERE g.ID = @p0 AND ISNULL(c.Baja, 0) = 0", model.GrupoId.Value).SingleOrDefaultAsync();
                if (grupoSeleccionado == null)
                    ModelState.AddModelError("GrupoId", "Selecciona un grupo válido de una cisterna activa.");
                else if (model.CubaId.HasValue && model.CubaId.Value != grupoSeleccionado.CubaID)
                    ModelState.AddModelError("GrupoId", "El grupo seleccionado pertenece a otra cisterna.");
                else
                    model.CubaId = grupoSeleccionado.CubaID;
            }

            CubaOpcion cubaSeleccionada = null;
            if (model.CubaId.HasValue)
            {
                cubaSeleccionada = await lookupDb.Database.SqlQuery<CubaOpcion>(@"SELECT ID AS Id, MatriculaCuba, Codigo
                    FROM gaillard.Cuba WHERE ID = @p0 AND ISNULL(Baja, 0) = 0", model.CubaId.Value).SingleOrDefaultAsync();
                if (cubaSeleccionada == null)
                    ModelState.AddModelError("CubaId", "Selecciona una cisterna activa válida.");
            }

            VehiculoOpcion vehiculoSeleccionado = null;
            if (model.VehiculoId.HasValue)
            {
                vehiculoSeleccionado = await lookupDb.Database.SqlQuery<VehiculoOpcion>(@"SELECT ID AS Id, MatriculaVehiculo, Marca, Modelo
                    FROM gaillard.Vehiculo WHERE ID = @p0 AND ISNULL(Baja, 0) = 0", model.VehiculoId.Value).SingleOrDefaultAsync();
                if (vehiculoSeleccionado == null)
                    ModelState.AddModelError("VehiculoId", "Selecciona un vehículo activo válido.");
            }

            var hayInstalacionEquipoPlan = model.InstalacionId.HasValue || model.EquipoId.HasValue || model.PlanMantenimientoId.HasValue;
            var hayCisternaGrupo = model.CubaId.HasValue || model.GrupoId.HasValue;
            if ((hayInstalacionEquipoPlan && hayCisternaGrupo) || (model.VehiculoId.HasValue && (hayInstalacionEquipoPlan || hayCisternaGrupo)))
                ModelState.AddModelError("CubaId", "Asocia cada tarea a una instalación/equipo, una cisterna/grupo o un vehículo.");

            if (!ModelState.IsValid)
            {
                return;
            }

            if (equipoInstalacionId.HasValue)
            {
                model.InstalacionId = equipoInstalacionId;
            }

            if (model.InstalacionId.HasValue)
            {
                var ubicacion = lookupDb.Database.SqlQuery<InstalacionOpcion>(@"SELECT Id, Codigo, Nombre
                    FROM gaillard.Instalacion WHERE Id = @p0", model.InstalacionId.Value).SingleOrDefault();
                if (ubicacion != null)
                {
                    model.Ubicacion = LimitarTexto(ubicacion.Texto, 150);
                }
            }

            if (model.EquipoId.HasValue)
            {
                var equipo = lookupDb.Database.SqlQuery<EquipoOpcion>(@"SELECT e.Id, e.InstalacionId, e.Codigo, e.Nombre, i.Nombre AS InstalacionNombre
                    FROM gaillard.Equipo e INNER JOIN gaillard.Instalacion i ON i.Id = e.InstalacionId
                    WHERE e.Id = @p0", model.EquipoId.Value).SingleOrDefault();
                if (equipo != null)
                {
                    model.Equipo = LimitarTexto(equipo.Texto, 150);
                }
            }

            if (cubaSeleccionada != null)
                model.Ubicacion = LimitarTexto(cubaSeleccionada.Texto, 150);
            if (grupoSeleccionado != null)
                model.Equipo = LimitarTexto(grupoSeleccionado.Texto, 150);
            if (vehiculoSeleccionado != null)
                model.Equipo = LimitarTexto(vehiculoSeleccionado.Texto, 150);
        }

        private async Task ValidarReferenciasPersonaEntidadAsync(int? detectadoPorId, int? contactoId, int? asignadaAId, int? entidadId)
        {
            foreach (var referencia in new[] { new { Id = detectadoPorId, Campo = "DetectadoPorPersonaID" }, new { Id = contactoId, Campo = "ContactoPersonaID" }, new { Id = asignadaAId, Campo = "AsignadaAPersonaID" } })
            {
                if (referencia.Id.HasValue && !await lookupDb.Personas.AnyAsync(p => p.ID == referencia.Id.Value))
                    ModelState.AddModelError(referencia.Campo, "Selecciona una persona válida.");
            }
            if (entidadId.HasValue && !await lookupDb.Entidads.AnyAsync(e => e.ID == entidadId.Value))
                ModelState.AddModelError("EmpresaExteriorEntidadID", "Selecciona una entidad válida.");
        }

        private IEnumerable<SelectListItem> GetInstalaciones(int? selected)
        {
            return lookupDb.Database.SqlQuery<InstalacionOpcion>(@"SELECT Id, Codigo, Nombre
                FROM gaillard.Instalacion WHERE Estado <> N'FUERA_SERVICIO' ORDER BY Nombre").ToList()
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Texto, Selected = selected == x.Id });
        }

        private IEnumerable<SelectListItem> GetEquipos(int? selected)
        {
            return lookupDb.Database.SqlQuery<EquipoOpcion>(@"SELECT e.Id, e.InstalacionId, e.Codigo, e.Nombre, i.Nombre AS InstalacionNombre
                FROM gaillard.Equipo e INNER JOIN gaillard.Instalacion i ON i.Id = e.InstalacionId
                WHERE e.Activo = 1 AND i.Estado <> N'FUERA_SERVICIO' ORDER BY i.Nombre, e.Nombre").ToList()
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Texto, Selected = selected == x.Id });
        }

        private IEnumerable<SelectListItem> GetCubas(int? selected)
        {
            return lookupDb.Database.SqlQuery<CubaOpcion>(@"SELECT ID AS Id, MatriculaCuba, Codigo FROM gaillard.Cuba
                WHERE ISNULL(Baja, 0) = 0 ORDER BY MatriculaCuba, Codigo").ToList()
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Texto, Selected = selected == x.Id });
        }

        private IEnumerable<SelectListItem> GetGrupos(int? selected)
        {
            return lookupDb.Database.SqlQuery<GrupoOpcion>(@"SELECT g.ID AS Id, g.CubaID,
                    COALESCE(NULLIF(c.MatriculaCuba, N''), NULLIF(c.Codigo, N''), N'Cisterna') + N' / ' +
                    COALESCE(NULLIF(tg.NombreGrupo, N''), N'Grupo') +
                    CASE WHEN cp.Numero IS NULL THEN N'' ELSE N' - Comp. ' + CONVERT(NVARCHAR(10), cp.Numero) END +
                    CASE WHEN s.LadoCubaNombre IS NULL THEN N'' ELSE N' - ' + s.LadoCubaNombre END AS Texto
                FROM gaillard.Grupo g
                INNER JOIN gaillard.Cuba c ON c.ID = g.CubaID
                LEFT JOIN gaillard.TipoGrupo tg ON tg.ID = g.TipoGrupoID
                LEFT JOIN gaillard.Compartimento cp ON cp.ID = g.CompartimentoID
                LEFT JOIN gaillard.Situacion s ON s.ID = g.SituacionID
                WHERE ISNULL(c.Baja, 0) = 0 ORDER BY c.MatriculaCuba, tg.NombreGrupo, cp.Numero").ToList()
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Texto, Selected = selected == x.Id });
        }

        private IEnumerable<SelectListItem> GetVehiculos(int? selected)
        {
            return lookupDb.Database.SqlQuery<VehiculoOpcion>(@"SELECT ID AS Id, MatriculaVehiculo, Marca, Modelo
                FROM gaillard.Vehiculo WHERE ISNULL(Baja, 0) = 0 ORDER BY MatriculaVehiculo").ToList()
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Texto, Selected = selected == x.Id });
        }

        private IEnumerable<SelectListItem> GetPlanesMantenimiento(int? selected)
        {
            return lookupDb.Database.SqlQuery<PlanOpcion>(@"SELECT p.PlanId, p.EquipoId,
                    COALESCE(NULLIF(p.Nombre, N''), e.Nombre, N'Plan ' + CONVERT(NVARCHAR(20), p.PlanId)) AS Texto
                FROM gaillard.PlanMantenimiento p
                LEFT JOIN gaillard.Equipo e ON e.Id = p.EquipoId
                WHERE p.Activo = 1 ORDER BY Texto").ToList()
                .Select(x => new SelectListItem { Value = x.PlanId.ToString(), Text = x.Texto, Selected = selected == x.PlanId });
        }

        private sealed class InstalacionOpcion
        {
            public InstalacionOpcion() { }

            public int Id { get; set; }
            public string Codigo { get; set; }
            public string Nombre { get; set; }
            public string Texto => string.IsNullOrWhiteSpace(Codigo) ? Nombre : Codigo + " - " + Nombre;
        }

        private sealed class EquipoOpcion
        {
            public EquipoOpcion() { }

            public int Id { get; set; }
            public int InstalacionId { get; set; }
            public string Codigo { get; set; }
            public string Nombre { get; set; }
            public string InstalacionNombre { get; set; }
            public string Texto => InstalacionNombre + " / " + Codigo + " - " + Nombre;
        }

        private sealed class CubaOpcion
        {
            public int Id { get; set; }
            public string MatriculaCuba { get; set; }
            public string Codigo { get; set; }
            public string Texto => string.IsNullOrWhiteSpace(MatriculaCuba) ? Codigo : MatriculaCuba;
        }

        private sealed class GrupoOpcion
        {
            public int Id { get; set; }
            public int CubaID { get; set; }
            public string Texto { get; set; }
        }

        private sealed class VehiculoOpcion
        {
            public int Id { get; set; }
            public string MatriculaVehiculo { get; set; }
            public string Marca { get; set; }
            public string Modelo { get; set; }
            public string Texto => MatriculaVehiculo + " - " + (Marca + " " + Modelo).Trim();
        }

        private sealed class PlanOpcion
        {
            public PlanOpcion() { }

            public int PlanId { get; set; }
            public int? EquipoId { get; set; }
            public string Texto { get; set; }
        }

        private static string LimitarTexto(string texto, int longitudMaxima)
        {
            if (string.IsNullOrWhiteSpace(texto) || texto.Length <= longitudMaxima)
            {
                return texto;
            }

            return texto.Substring(0, longitudMaxima);
        }

        private string GenerarCodigo()
        {
            return "TM-" + DateTime.Now.ToString("yyyyMMdd-HHmmss");
        }

        private string NormalizarAccesorios(string[] accesoriosSeleccionados)
        {
            if (accesoriosSeleccionados == null || accesoriosSeleccionados.Length == 0)
            {
                return null;
            }

            var ids = accesoriosSeleccionados
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x =>
                {
                    int id;
                    return int.TryParse(x, out id) ? (int?)id : null;
                })
                .Where(x => x.HasValue)
                .Select(x => x.Value)
                .Distinct()
                .ToArray();

            if (!ids.Any())
            {
                return null;
            }

            return string.Join(",", ids);
        }

        private string[] SepararAccesorios(string accesorios)
        {
            if (string.IsNullOrWhiteSpace(accesorios))
            {
                return new string[0];
            }

            return accesorios.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();
        }

        private string[] ObtenerAccesoriosSeleccionados(int tareaId, string accesoriosLegacy)
        {
            var idsNormalizados = tareasDb.TareaMantenimientoAccesorios
                .AsNoTracking()
                .Where(a => a.TareaMantenimientoID == tareaId)
                .OrderBy(a => a.ID)
                .Select(a => a.AccesorioID.ToString())
                .ToArray();

            return idsNormalizados.Any() ? idsNormalizados : SepararAccesorios(accesoriosLegacy);
        }

        private async Task GuardarAccesoriosTareaAsync(int tareaId, string[] accesoriosSeleccionados)
        {
            var ids = ObtenerIdsAccesorios(accesoriosSeleccionados);
            var accesoriosActuales = await tareasDb.TareaMantenimientoAccesorios
                .Where(a => a.TareaMantenimientoID == tareaId)
                .ToListAsync();

            if (accesoriosActuales.Any())
            {
                tareasDb.TareaMantenimientoAccesorios.RemoveRange(accesoriosActuales);
            }

            foreach (var id in ids)
            {
                tareasDb.TareaMantenimientoAccesorios.Add(new TareaMantenimientoAccesorio
                {
                    TareaMantenimientoID = tareaId,
                    AccesorioID = id,
                    Cantidad = 1
                });
            }

            await tareasDb.SaveChangesAsync();
        }

        private int[] ObtenerIdsAccesorios(string[] accesoriosSeleccionados)
        {
            if (accesoriosSeleccionados == null || accesoriosSeleccionados.Length == 0)
            {
                return new int[0];
            }

            return accesoriosSeleccionados
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x =>
                {
                    int id;
                    return int.TryParse(x, out id) ? (int?)id : null;
                })
                .Where(x => x.HasValue)
                .Select(x => x.Value)
                .Distinct()
                .ToArray();
        }

        private string ObtenerAccesoriosTexto(int tareaId, string accesoriosLegacy)
        {
            var idValues = ObtenerAccesoriosSeleccionados(tareaId, accesoriosLegacy)
                .Select(x =>
                {
                    int id;
                    return int.TryParse(x, out id) ? (int?)id : null;
                })
                .Where(x => x.HasValue)
                .Select(x => x.Value)
                .ToArray();

            if (!idValues.Any())
            {
                return "Sin accesorios indicados.";
            }

            var textos = lookupDb.Accesorios
                .AsNoTracking()
                .Include(a => a.TipoAccesorio)
                .Include(a => a.Material)
                .Where(a => idValues.Contains(a.ID))
                .OrderBy(a => a.TipoAccesorio.TipoAccesorio1)
                .ThenBy(a => a.Material.Material1)
                .ToList()
                .Select(a => (a.TipoAccesorio != null ? a.TipoAccesorio.TipoAccesorio1 : "Accesorio") +
                             " - " +
                             (a.Material != null ? a.Material.Material1 : "Material"));

            return textos.Any() ? string.Join(", ", textos) : "Sin accesorios indicados.";
        }

        private IEnumerable<SelectListItem> GetPrioridades(string selected = null)
        {
            var values = new[] { "Baja", "Media", "Alta", "Urgente" };
            return values.Select(v => new SelectListItem
            {
                Value = v,
                Text = v,
                Selected = string.Equals(v, selected, StringComparison.OrdinalIgnoreCase)
            });
        }

        private IEnumerable<SelectListItem> GetEstados(string selected = null)
        {
            var values = new[] { "Nueva", "En estudio", "Asignada", "En curso", "Pendiente repuestos", "Cerrada" };
            return values.Select(v => new SelectListItem
            {
                Value = v,
                Text = v,
                Selected = string.Equals(v, selected, StringComparison.OrdinalIgnoreCase)
            });
        }

        private IEnumerable<SelectListItem> GetEmpleados(string selected = null)
        {
            var personas = lookupDb.Personas
                .AsNoTracking()
                .OrderBy(p => p.NombrePersona)
                .ThenBy(p => p.Apellido1)
                .ThenBy(p => p.Apellido2)
                .ToList();

            return personas
                .Select(p => new SelectListItem
                {
                    Value = FormatearNombrePersona(p),
                    Text = FormatearNombrePersona(p),
                    Selected = string.Equals(FormatearNombrePersona(p), selected, StringComparison.OrdinalIgnoreCase)
                })
                .ToList();
        }

        private IEnumerable<SelectListItem> GetPersonas(int? selected = null)
        {
            return lookupDb.Personas.AsNoTracking()
                .OrderBy(p => p.NombrePersona).ThenBy(p => p.Apellido1).ThenBy(p => p.Apellido2)
                .ToList()
                .Select(p => new SelectListItem
                {
                    Value = p.ID.ToString(),
                    Text = FormatearNombrePersona(p),
                    Selected = selected == p.ID
                }).ToList();
        }

        private IEnumerable<SelectListItem> GetEntidades(int? selected = null)
        {
            return lookupDb.Entidads.AsNoTracking()
                .OrderBy(e => e.NombreEntidad)
                .ToList()
                .Select(e => new SelectListItem
                {
                    Value = e.ID.ToString(),
                    Text = e.NombreEntidad.Trim(),
                    Selected = selected == e.ID
                }).ToList();
        }

        private string ObtenerNombrePersona(int? id)
        {
            if (!id.HasValue) return null;
            var persona = lookupDb.Personas.AsNoTracking().FirstOrDefault(p => p.ID == id.Value);
            return persona == null ? null : FormatearNombrePersona(persona);
        }

        private string ObtenerNombreEntidad(int? id)
        {
            if (!id.HasValue) return null;
            var nombre = lookupDb.Entidads.AsNoTracking().Where(e => e.ID == id.Value)
                .Select(e => e.NombreEntidad).FirstOrDefault();
            return nombre == null ? null : nombre.Trim();
        }

        private string FormatearNombrePersona(Persona persona)
        {
            if (persona == null)
            {
                return string.Empty;
            }

            var partes = new List<string>();

            if (!string.IsNullOrWhiteSpace(persona.NombrePersona))
            {
                partes.Add(persona.NombrePersona.Trim());
            }

            if (!string.IsNullOrWhiteSpace(persona.Apellido1))
            {
                partes.Add(persona.Apellido1.Trim());
            }

            if (!string.IsNullOrWhiteSpace(persona.Apellido2))
            {
                partes.Add(persona.Apellido2.Trim());
            }

            return string.Join(" ", partes);
        }

        private IEnumerable<SelectListItem> GetAccesorios(string[] selected = null)
        {
            selected = selected ?? new string[0];

            var accesorios = lookupDb.Accesorios
                .AsNoTracking()
                .Include(a => a.TipoAccesorio)
                .Include(a => a.Material)
                .OrderBy(a => a.TipoAccesorio.TipoAccesorio1)
                .ThenBy(a => a.Material.Material1)
                .ToList()
                .Select(a =>
                {
                    var texto = (a.TipoAccesorio != null ? a.TipoAccesorio.TipoAccesorio1 : "Accesorio") +
                                " - " +
                                (a.Material != null ? a.Material.Material1 : "Material");

                    return new SelectListItem
                    {
                        Value = a.ID.ToString(),
                        Text = texto,
                        Selected = selected.Contains(a.ID.ToString())
                    };
                });

            return accesorios;
        }
    }
}
