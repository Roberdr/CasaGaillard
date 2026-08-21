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
            ViewBag.AccesoriosTexto = ObtenerAccesoriosTexto(tarea.AccesoriosNecesarios);
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
            tarea.AccionesARealizar = model.AccionesARealizar?.Trim();
            tarea.AccesoriosNecesarios = NormalizarAccesorios(model.AccesoriosSeleccionados);
            tarea.Observaciones = model.Observaciones?.Trim();
            tarea.ActualizadoPor = User.Identity.Name;
            tarea.FechaActualizacion = DateTime.Now;

            await tareasDb.SaveChangesAsync();

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
                DetectadoPor = model.DetectadoPor?.Trim(),
                Contacto = model.Contacto?.Trim(),
                Prioridad = string.IsNullOrWhiteSpace(model.Prioridad) ? "Media" : model.Prioridad,
                Estado = string.IsNullOrWhiteSpace(model.Estado) ? "Nueva" : model.Estado,
                AsignadaA = model.AsignadaA?.Trim(),
                EmpresaExterior = model.EmpresaExterior?.Trim(),
                AccionesARealizar = model.AccionesARealizar?.Trim(),
                AccesoriosNecesarios = NormalizarAccesorios(model.AccesoriosSeleccionados),
                Observaciones = model.Observaciones?.Trim(),
                CreadoPor = User.Identity.Name,
                FechaDeteccion = model.FechaDeteccion == default(DateTime) ? DateTime.Now : model.FechaDeteccion,
                FechaCreacion = DateTime.Now
            };

            tareasDb.TareasMantenimiento.Add(tarea);
            await tareasDb.SaveChangesAsync();
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
            tarea.DetectadoPor = model.DetectadoPor?.Trim();
            tarea.Contacto = model.Contacto?.Trim();
            tarea.Prioridad = string.IsNullOrWhiteSpace(model.Prioridad) ? "Media" : model.Prioridad;
            tarea.Estado = string.IsNullOrWhiteSpace(model.Estado) ? "Nueva" : model.Estado;
            tarea.AsignadaA = model.AsignadaA?.Trim();
            tarea.EmpresaExterior = model.EmpresaExterior?.Trim();
            tarea.AccionesARealizar = model.AccionesARealizar?.Trim();
            tarea.AccesoriosNecesarios = NormalizarAccesorios(model.AccesoriosSeleccionados);
            tarea.Observaciones = model.Observaciones?.Trim();
            tarea.ActualizadoPor = User.Identity.Name;
            tarea.FechaActualizacion = DateTime.Now;

            await tareasDb.SaveChangesAsync();
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
            source.Accesorios = GetAccesorios(source.AccesoriosSeleccionados);
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
                DetectadoPor = tarea.DetectadoPor,
                Contacto = tarea.Contacto,
                Prioridad = tarea.Prioridad,
                Estado = tarea.Estado,
                AsignadaA = tarea.AsignadaA,
                EmpresaExterior = tarea.EmpresaExterior,
                AccionesARealizar = tarea.AccionesARealizar,
                AccesoriosSeleccionados = SepararAccesorios(tarea.AccesoriosNecesarios),
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
                DetectadoPor = tarea.DetectadoPor,
                Contacto = tarea.Contacto,
                Descripcion = tarea.Descripcion,
                Prioridad = tarea.Prioridad,
                Estado = tarea.Estado,
                AsignadaA = tarea.AsignadaA,
                EmpresaExterior = tarea.EmpresaExterior,
                AccionesARealizar = tarea.AccionesARealizar,
                AccesoriosSeleccionados = SepararAccesorios(tarea.AccesoriosNecesarios),
                Observaciones = tarea.Observaciones
            });
        }

        private TareaMantenimientoGestionViewModel CreateGestionModel(TareaMantenimientoGestionViewModel source)
        {
            source = source ?? new TareaMantenimientoGestionViewModel();
            source.Prioridades = GetPrioridades(source.Prioridad);
            source.Estados = GetEstados(source.Estado);
            source.Empleados = GetEmpleados(source.AsignadaA);
            source.Accesorios = GetAccesorios(source.AccesoriosSeleccionados);
            return source;
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

        private string ObtenerAccesoriosTexto(string accesorios)
        {
            var ids = SepararAccesorios(accesorios);
            if (ids.Length == 0)
            {
                return "Sin accesorios indicados.";
            }

            var idValues = ids
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
