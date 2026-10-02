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
    public class AccesoriosController : Controller
    {
        private readonly GaillardEntities db = new GaillardEntities();
        private readonly AccesoriosFotosContext fotosDb = new AccesoriosFotosContext();

        // GET: Accesorios
        public async Task<ActionResult> Index(int? tipoAccesorioId, int? materialId, int? familiaId, int? subfamiliaId, string q, int page = 1, int pageSize = 4)
        {
            var query = db.Accesorios
                .Include(a => a.Material)
                .Include(a => a.TipoAccesorio)
                .Include(a => a.Familia)
                .Include(a => a.Subfamilia)
                .Include("DetallesAccesorio.CaracteristicaAccesorio")
                .Include("DetallesAccesorio.Unidad")
                .AsQueryable();

            if (tipoAccesorioId.HasValue)
            {
                query = query.Where(a => a.TipoAccesorioID == tipoAccesorioId.Value);
            }

            if (materialId.HasValue)
            {
                query = query.Where(a => a.MaterialID == materialId.Value);
            }

            if (familiaId.HasValue)
            {
                query = query.Where(a => a.FamiliaId == familiaId.Value);
            }

            if (subfamiliaId.HasValue)
            {
                query = query.Where(a => a.SubfamiliaId == subfamiliaId.Value);
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(a => a.Nombre.Contains(q) || a.Descripcion.Contains(q));
            }

            var total = await query.CountAsync();

            var accesorios = await query
                .OrderBy(a => a.ID)
                .Skip((Math.Max(1, page) - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Obtener foto representativa (primera) para cada accesorio listado
            var accesorioIds = accesorios.Select(a => a.ID).ToList();
            var fotos = await fotosDb.AccesorioFotos
                .AsNoTracking()
                .Where(f => accesorioIds.Contains(f.AccesorioID))
                .GroupBy(f => f.AccesorioID)
                .Select(g => new { AccesorioID = g.Key, FotoID = g.OrderByDescending(x => x.FechaCreacion).FirstOrDefault().ID })
                .ToListAsync();

            var fotoDict = fotos.ToDictionary(f => f.AccesorioID, f => (int?)f.FotoID);

            var vm = new Models.ViewModels.AccesoriosIndexViewModel
            {
                Accesorios = accesorios,
                TiposAccesorio = new SelectList(db.TiposAccesorio.OrderBy(t => t.TipoAccesorio1).ToList(), "ID", "TipoAccesorio1", tipoAccesorioId),
                Materiales = new SelectList(db.Materiales.OrderBy(m => m.Material1).ToList(), "ID", "Material1", materialId),
                Familias = new SelectList(db.Familia.OrderBy(f => f.Nombre).ToList(), "FamiliaId", "Nombre", familiaId),
                Subfamilias = new SelectList(db.Subfamilia.OrderBy(s => s.Nombre).ToList(), "SubfamiliaId", "Nombre", subfamiliaId),
                TipoAccesorioId = tipoAccesorioId,
                MaterialId = materialId,
                FamiliaId = familiaId,
                SubfamiliaId = subfamiliaId,
                Query = q,
                Page = page,
                PageSize = pageSize,
                TotalItems = total
            };

            vm.FotoIdByAccesorio = fotoDict;

            return View(vm);
        }

        // GET: Accesorios/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var accesorio = await db.Accesorios
                .Include(a => a.Material)
                .Include(a => a.TipoAccesorio)
                .Include(a => a.Ubicacion)
                .Include(a => a.Familia)
                .Include(a => a.Subfamilia)
                .Include("DetallesAccesorio.CaracteristicaAccesorio")
                .Include("DetallesAccesorio.Unidad")
                .FirstOrDefaultAsync(a => a.ID == id.Value);

            if (accesorio == null)
            {
                return HttpNotFound();
            }

            ViewBag.Fotos = await fotosDb.AccesorioFotos
                .AsNoTracking()
                .Where(f => f.AccesorioID == accesorio.ID)
                .OrderByDescending(f => f.FechaCreacion)
                .Select(f => new AccesorioFotoViewModel
                {
                    ID = f.ID,
                    NombreOriginal = f.NombreOriginal,
                    FechaCreacion = f.FechaCreacion
                })
                .ToListAsync();

            return View(accesorio);
        }

        // GET: Accesorios/Create
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Create(int? intervencionId)
        {
            if (intervencionId.HasValue && !await ExisteIntervencionAsync(intervencionId.Value)) return HttpNotFound();
            return View(CreateFormModel(new AccesorioFormViewModel { IntervencionId = intervencionId }));
        }

        // POST: Accesorios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Create(AccesorioFormViewModel model)
        {
            if (model.IntervencionId.HasValue && !await ExisteIntervencionAsync(model.IntervencionId.Value)) return HttpNotFound();
            ValidarFamiliaSubfamilia(model);

            if (!ModelState.IsValid)
            {
                // Eliminar valores problemáticos de ModelState para que los helpers usen el valor del modelo
                if (model?.Detalles != null)
                {
                    for (int i = 0; i < model.Detalles.Count; i++)
                    {
                        ModelState.Remove($"Detalles[{i}].Medida");
                    }
                }

                // Recolectar errores de ModelState para diagnóstico y enviarlos a la vista
                var errors = ModelState
                    .Where(kv => kv.Value.Errors != null && kv.Value.Errors.Count > 0)
                    .Select(kv => new
                    {
                        Key = kv.Key,
                        AttemptedValue = kv.Value.Value?.AttemptedValue,
                        Errors = kv.Value.Errors.Select(e => new { e.ErrorMessage, Exception = e.Exception?.Message }).ToArray()
                    })
                    .ToList();
                ViewBag.ModelStateErrors = errors;

                // También escribir en Debug para revisar en output
                foreach (var e in errors)
                {
                    System.Diagnostics.Debug.WriteLine($"ModelState error on '{e.Key}' attempted='{e.AttemptedValue}'");
                    foreach (var sub in e.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($" - {sub.ErrorMessage} {sub.Exception}");
                    }
                }

                return View(CreateFormModel(model));
            }

            var accesorio = new Accesorio
            {
                TipoAccesorioID = model.TipoAccesorioID.Value,
                MaterialID = model.MaterialID.Value,
                Nombre = Texto(model.Nombre),
                Descripcion = Texto(model.Descripcion),
                FamiliaId = model.FamiliaID,
                SubfamiliaId = model.SubfamiliaID,
                UbicacionId = model.UbicacionId,
                FechaAlta = model.FechaAlta,
                Activo = model.Activo
            };

            db.Accesorios.Add(accesorio);
            await db.SaveChangesAsync();

            GuardarDetalles(accesorio.ID, model.Detalles);
            GuardarFotos(accesorio.ID, Request.Files);

            if (model.IntervencionId.HasValue)
            {
                TempData["LoginMessage"] = "Repuesto creado. Completa cantidad y coste para registrarlo en la intervención.";
                return RedirectToAction("Details", "Intervenciones", new { area = "Mantenimiento", id = model.IntervencionId.Value, nuevoAccesorioId = accesorio.ID });
            }

            TempData["LoginMessage"] = "El accesorio se ha creado correctamente.";
            return RedirectToAction("Details", new { id = accesorio.ID });
        }

        // GET: Accesorios/Edit/5
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Edit(int? id, string returnUrl)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var accesorio = await db.Accesorios
                .Include(a => a.Material)
                .Include(a => a.TipoAccesorio)
                .Include("DetallesAccesorio.CaracteristicaAccesorio")
                .Include("DetallesAccesorio.Unidad")
                .Include(a => a.Familia)
                .Include(a => a.Subfamilia)
                .FirstOrDefaultAsync(a => a.ID == id.Value);

            if (accesorio == null)
            {
                return HttpNotFound();
            }
            var vm = MapToForm(accesorio);
            ViewBag.ReturnUrl = returnUrl;
            return View(vm);
        }

        // POST: Accesorios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Edit(AccesorioFormViewModel model, string returnUrl)
        {
            ValidarFamiliaSubfamilia(model);

            if (!ModelState.IsValid)
            {
                // Eliminar valores problemáticos de ModelState para que los helpers usen el valor del modelo
                if (model?.Detalles != null)
                {
                    for (int i = 0; i < model.Detalles.Count; i++)
                    {
                        ModelState.Remove($"Detalles[{i}].Medida");
                    }
                }

                // Recolectar errores de ModelState para diagnóstico y enviarlos a la vista
                var errors = ModelState
                    .Where(kv => kv.Value.Errors != null && kv.Value.Errors.Count > 0)
                    .Select(kv => new
                    {
                        Key = kv.Key,
                        AttemptedValue = kv.Value.Value?.AttemptedValue,
                        Errors = kv.Value.Errors.Select(e => new { e.ErrorMessage, Exception = e.Exception?.Message }).ToArray()
                    })
                    .ToList();
                ViewBag.ModelStateErrors = errors;

                foreach (var e in errors)
                {
                    System.Diagnostics.Debug.WriteLine($"ModelState error on '{e.Key}' attempted='{e.AttemptedValue}'");
                    foreach (var sub in e.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($" - {sub.ErrorMessage} {sub.Exception}");
                    }
                }

                ViewBag.ReturnUrl = returnUrl;
                return View(CreateFormModel(model));
            }

            var accesorio = await db.Accesorios.FindAsync(model.ID);
            if (accesorio == null)
            {
                return HttpNotFound();
            }

            accesorio.TipoAccesorioID = model.TipoAccesorioID.Value;
            accesorio.MaterialID = model.MaterialID.Value;
            accesorio.Nombre = Texto(model.Nombre);
            accesorio.Descripcion = Texto(model.Descripcion);
            accesorio.FamiliaId = model.FamiliaID;
            accesorio.SubfamiliaId = model.SubfamiliaID;
            accesorio.UbicacionId = model.UbicacionId;
            accesorio.FechaAlta = model.FechaAlta;
            accesorio.Activo = model.Activo;

            await db.SaveChangesAsync();

            GuardarDetalles(accesorio.ID, model.Detalles);
            GuardarFotos(accesorio.ID, Request.Files);

            TempData["LoginMessage"] = "El accesorio se ha actualizado.";
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Details", new { id = accesorio.ID });
        }

        // GET: Accesorios/Delete/5
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Delete(int? id, string returnUrl)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var accesorio = await db.Accesorios
                .Include(a => a.Material)
                .Include(a => a.TipoAccesorio)
                .FirstOrDefaultAsync(a => a.ID == id.Value);

            if (accesorio == null)
            {
                return HttpNotFound();
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(accesorio);
        }

        // POST: Accesorios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> DeleteConfirmed(int id, string returnUrl)
        {
            var accesorio = await db.Accesorios.FindAsync(id);
            if (accesorio == null)
            {
                return HttpNotFound();
            }

            var detalles = db.DetallesAccesorio.Where(d => d.AccesorioID == id).ToList();
            if (detalles.Any())
            {
                db.DetallesAccesorio.RemoveRange(detalles);
            }

            var grupos = db.AccesoriosGrupo.Where(g => g.AccesorioID == id).ToList();
            if (grupos.Any())
            {
                db.AccesoriosGrupo.RemoveRange(grupos);
            }

            EliminarFotosFisicasYRegistro(id);

            db.Accesorios.Remove(accesorio);
            await db.SaveChangesAsync();
            TempData["LoginMessage"] = "El accesorio se ha eliminado.";
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento + "," + AppRoles.Consulta)]
        public async Task<ActionResult> Foto(int id)
        {
            var foto = await fotosDb.AccesorioFotos.AsNoTracking().FirstOrDefaultAsync(f => f.ID == id);
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
                db.Dispose();
                fotosDb.Dispose();
            }

            base.Dispose(disposing);
        }

        private AccesorioFormViewModel CreateFormModel(AccesorioFormViewModel source = null)
        {
            source = source ?? new AccesorioFormViewModel();
            source.TiposAccesorio = new SelectList(db.TiposAccesorio.OrderBy(t => t.TipoAccesorio1).ToList(), "ID", "TipoAccesorio1", source.TipoAccesorioID);
            source.Materiales = new SelectList(db.Materiales.OrderBy(m => m.Material1).ToList(), "ID", "Material1", source.MaterialID);
            source.Familias = new SelectList(db.Familia.OrderBy(f => f.Nombre).ToList(), "FamiliaId", "Nombre", source.FamiliaID);
            source.Ubicaciones = new SelectList(db.Ubicacion.OrderBy(u => u.Nombre).ToList(), "UbicacionId", "Nombre", source.UbicacionId);
            var subfamilias = db.Subfamilia.AsQueryable();
            if (source.FamiliaID.HasValue)
            {
                subfamilias = subfamilias.Where(s => s.FamiliaId == source.FamiliaID.Value);
            }
            else
            {
                subfamilias = subfamilias.Where(s => false);
            }

            source.Subfamilias = new SelectList(subfamilias.OrderBy(s => s.Nombre).ToList(), "SubfamiliaId", "Nombre", source.SubfamiliaID);
            source.Caracteristicas = new SelectList(db.CaracteristicasAccesorio.OrderBy(c => c.CaracteristicaAccesorio1).ToList(), "ID", "CaracteristicaAccesorio1");
            source.Unidades = new SelectList(db.Unidades.OrderBy(u => u.Unidad1).ToList(), "ID", "Unidad1");

            source.Detalles = source.Detalles ?? new List<DetalleAccesorioFormViewModel>();
            // Si existe un accesorio (edit), asegurarse de traer siempre los detalles guardados desde la BD
            if (source.ID > 0)
            {
                var detallesDb = db.DetallesAccesorio
                    .AsNoTracking()
                    .Include(d => d.CaracteristicaAccesorio)
                    .Include(d => d.Unidad)
                    .Where(d => d.AccesorioID == source.ID)
                    .OrderBy(d => d.ID)
                    .Select(d => new DetalleAccesorioFormViewModel
                    {
                        ID = d.ID,
                        CaracteristicaAccesorioID = d.CaracteristicaAccesorioID,
                        CaracteristicaNombre = d.CaracteristicaAccesorio != null ? d.CaracteristicaAccesorio.CaracteristicaAccesorio1 : null,
                        Cantidad = d.Cantidad,
                        Medida = d.Medida,
                        UnidadID = d.UnidadID,
                        UnidadNombre = d.Unidad != null ? d.Unidad.Unidad1 : null,
                        Tipo = d.Tipo
                    })
                    .ToList();

                // Conservar las filas nuevas que el usuario ya haya introducido en el modelo (ID == 0)
                var nuevosDesdeModelo = source.Detalles.Where(d => d != null && d.ID == 0
                    && (d.CaracteristicaAccesorioID.HasValue || d.Cantidad.HasValue || d.Medida.HasValue || d.UnidadID.HasValue || !string.IsNullOrWhiteSpace(d.Tipo)))
                    .ToList();

                source.Detalles = detallesDb.Concat(nuevosDesdeModelo).ToList();
            }

            if (!source.Detalles.Any())
            {
                source.Detalles = CrearFilasDetalleVacias(4);
            }

            source.FotosExistentes = source.FotosExistentes ?? new List<AccesorioFotoViewModel>();
            return source;
        }

        private static async Task<bool> ExisteIntervencionAsync(int id)
        {
            using (var tareasDb = new TareasMantenimientoContext())
            {
                return await tareasDb.Database.SqlQuery<int>("SELECT COUNT(1) FROM dbo.Intervencion WHERE Id=@p0", id).SingleAsync() > 0;
            }
        }

        private static string Texto(string value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private void ValidarFamiliaSubfamilia(AccesorioFormViewModel model)
        {
            if (model == null || !model.SubfamiliaID.HasValue)
            {
                return;
            }

            if (!model.FamiliaID.HasValue)
            {
                ModelState.AddModelError("SubfamiliaID", "Selecciona primero una familia.");
                return;
            }

            var pertenece = db.Subfamilia.Any(s => s.SubfamiliaId == model.SubfamiliaID.Value && s.FamiliaId == model.FamiliaID.Value);
            if (!pertenece)
            {
                ModelState.AddModelError("SubfamiliaID", "La subfamilia seleccionada no pertenece a la familia indicada.");
            }
        }

        // GET: Accesorios/SubfamiliasPorFamilia/5
        [HttpGet]
        public ActionResult SubfamiliasPorFamilia(int familiaId)
        {
            var lista = db.Subfamilia
                .Where(s => s.FamiliaId == familiaId)
                .OrderBy(s => s.Nombre)
                .Select(s => new { id = s.SubfamiliaId, text = s.Nombre })
                .ToList();

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> CrearTipoAccesorioRapido(string nombre, string descripcion, int? subfamiliaId)
        {
            nombre = (nombre ?? string.Empty).Trim();
            descripcion = (descripcion ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                return Json(new { ok = false, message = "Introduce el nombre del tipo de accesorio." });
            }

            var nombreNormalizado = nombre.ToLower();
            var existente = await db.TiposAccesorio
                .FirstOrDefaultAsync(t => t.TipoAccesorio1.ToLower() == nombreNormalizado);

            if (existente != null)
            {
                return Json(new
                {
                    ok = true,
                    id = existente.ID,
                    text = existente.TipoAccesorio1,
                    existing = true
                });
            }

            if (subfamiliaId.HasValue)
            {
                var subfamiliaExiste = await db.Subfamilia.AnyAsync(s => s.SubfamiliaId == subfamiliaId.Value);
                if (!subfamiliaExiste)
                {
                    subfamiliaId = null;
                }
            }

            var tipo = new TipoAccesorio
            {
                TipoAccesorio1 = nombre,
                Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion,
                SubfamiliaId = subfamiliaId
            };

            db.TiposAccesorio.Add(tipo);
            await db.SaveChangesAsync();

            return Json(new
            {
                ok = true,
                id = tipo.ID,
                text = tipo.TipoAccesorio1,
                existing = false
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> CrearSubfamiliaRapida(string nombre, int? familiaId)
        {
            nombre = (nombre ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(nombre)) return Json(new { ok = false, message = "Introduce el nombre de la subfamilia." });
            if (!familiaId.HasValue || !await db.Familia.AnyAsync(f => f.FamiliaId == familiaId.Value))
                return Json(new { ok = false, message = "Selecciona una familia válida." });

            var existente = await db.Subfamilia.FirstOrDefaultAsync(s => s.FamiliaId == familiaId.Value && s.Nombre.ToLower() == nombre.ToLower());
            if (existente != null) return Json(new { ok = true, id = existente.SubfamiliaId, text = existente.Nombre, existing = true });

            var subfamilia = new Subfamilia { FamiliaId = familiaId.Value, Nombre = nombre };
            db.Subfamilia.Add(subfamilia);
            await db.SaveChangesAsync();
            return Json(new { ok = true, id = subfamilia.SubfamiliaId, text = subfamilia.Nombre, existing = false });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> CrearCaracteristicaRapida(string nombre)
        {
            nombre = (nombre ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(nombre)) return Json(new { ok = false, message = "Introduce el nombre de la característica." });
            var existente = await db.CaracteristicasAccesorio.FirstOrDefaultAsync(c => c.CaracteristicaAccesorio1.ToLower() == nombre.ToLower());
            if (existente != null) return Json(new { ok = true, id = existente.ID, text = existente.CaracteristicaAccesorio1, existing = true });
            var caracteristica = new CaracteristicaAccesorio { CaracteristicaAccesorio1 = nombre };
            db.CaracteristicasAccesorio.Add(caracteristica);
            await db.SaveChangesAsync();
            return Json(new { ok = true, id = caracteristica.ID, text = caracteristica.CaracteristicaAccesorio1, existing = false });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> CrearUnidadRapida(string nombre)
        {
            nombre = (nombre ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(nombre)) return Json(new { ok = false, message = "Introduce el nombre de la unidad." });
            var existente = await db.Unidades.FirstOrDefaultAsync(u => u.Unidad1.ToLower() == nombre.ToLower());
            if (existente != null) return Json(new { ok = true, id = existente.ID, text = existente.Unidad1, existing = true });
            var unidad = new Unidad { Unidad1 = nombre };
            db.Unidades.Add(unidad);
            await db.SaveChangesAsync();
            return Json(new { ok = true, id = unidad.ID, text = unidad.Unidad1, existing = false });
        }

        private AccesorioFormViewModel MapToForm(Accesorio accesorio)
        {
            var fotos = fotosDb.AccesorioFotos
                .AsNoTracking()
                .Where(f => f.AccesorioID == accesorio.ID)
                .OrderByDescending(f => f.FechaCreacion)
                .ToList()
                .Select(f => new AccesorioFotoViewModel
                {
                    ID = f.ID,
                    NombreOriginal = f.NombreOriginal,
                    FechaCreacion = f.FechaCreacion
                })
                .ToList();

            var detalles = (accesorio.DetallesAccesorio ?? Enumerable.Empty<DetalleAccesorio>())
                .OrderBy(d => d.ID)
                .Select(d => new DetalleAccesorioFormViewModel
                {
                    ID = d.ID,
                    CaracteristicaAccesorioID = d.CaracteristicaAccesorioID,
                    CaracteristicaNombre = d.CaracteristicaAccesorio != null ? d.CaracteristicaAccesorio.CaracteristicaAccesorio1 : null,
                    Cantidad = d.Cantidad,
                    Medida = d.Medida,
                    UnidadID = d.UnidadID,
                    UnidadNombre = d.Unidad != null ? d.Unidad.Unidad1 : null,
                    Tipo = d.Tipo
                })
                .ToList();

            // Si por alguna razón las referencias de navegación no contienen los nombres, rellenarlos desde la BD
            foreach (var d in detalles)
            {
                if (string.IsNullOrWhiteSpace(d.CaracteristicaNombre) && d.CaracteristicaAccesorioID.HasValue)
                {
                    var c = db.CaracteristicasAccesorio.Find(d.CaracteristicaAccesorioID.Value);
                    if (c != null) d.CaracteristicaNombre = c.CaracteristicaAccesorio1;
                }

                if (string.IsNullOrWhiteSpace(d.UnidadNombre) && d.UnidadID.HasValue)
                {
                    var u = db.Unidades.Find(d.UnidadID.Value);
                    if (u != null) d.UnidadNombre = u.Unidad1;
                }
            }

            if (!detalles.Any())
            {
                detalles = db.DetallesAccesorio
                    .AsNoTracking()
                    .Include(d => d.CaracteristicaAccesorio)
                    .Include(d => d.Unidad)
                    .Where(d => d.AccesorioID == accesorio.ID)
                    .OrderBy(d => d.ID)
                    .Select(d => new DetalleAccesorioFormViewModel
                    {
                        ID = d.ID,
                        CaracteristicaAccesorioID = d.CaracteristicaAccesorioID,
                        CaracteristicaNombre = d.CaracteristicaAccesorio != null ? d.CaracteristicaAccesorio.CaracteristicaAccesorio1 : null,
                        Cantidad = d.Cantidad,
                        Medida = d.Medida,
                        UnidadID = d.UnidadID,
                        UnidadNombre = d.Unidad != null ? d.Unidad.Unidad1 : null,
                        Tipo = d.Tipo
                    })
                    .ToList();
            }

            detalles.AddRange(CrearFilasDetalleVacias(3));

            return CreateFormModel(new AccesorioFormViewModel
            {
                ID = accesorio.ID,
                TipoAccesorioID = accesorio.TipoAccesorioID,
                MaterialID = accesorio.MaterialID,
                Detalles = detalles,
                FotosExistentes = fotos,
                FamiliaID = accesorio.FamiliaId,
                SubfamiliaID = accesorio.SubfamiliaId,
                Nombre = accesorio.Nombre,
                Descripcion = accesorio.Descripcion,
                UbicacionId = accesorio.UbicacionId,
                FechaAlta = accesorio.FechaAlta,
                Activo = accesorio.Activo
            });
        }

        private List<DetalleAccesorioFormViewModel> CrearFilasDetalleVacias(int numeroFilas)
        {
            var filas = new List<DetalleAccesorioFormViewModel>();
            for (var i = 0; i < numeroFilas; i++)
            {
                filas.Add(new DetalleAccesorioFormViewModel());
            }

            return filas;
        }

        private void GuardarDetalles(int accesorioId, IEnumerable<DetalleAccesorioFormViewModel> detalles)
        {
            var actuales = db.DetallesAccesorio.Where(d => d.AccesorioID == accesorioId).ToList();
            var lista = (detalles ?? Enumerable.Empty<DetalleAccesorioFormViewModel>()).ToList();


            // Crear diccionario para búsqueda rápida
            var actualesById = actuales.ToDictionary(d => d.ID);

            foreach (var detalle in lista)
            {
                if (detalle == null) continue;

                var vacio = !detalle.CaracteristicaAccesorioID.HasValue
                    && !detalle.Cantidad.HasValue
                    && !detalle.Medida.HasValue
                    && !detalle.UnidadID.HasValue
                    && string.IsNullOrWhiteSpace(detalle.Tipo);

                if (detalle.ID > 0)
                {
                    // detalle existente: actualizar o eliminar sólo si se marca Eliminar
                    if (detalle.Eliminar)
                    {
                        if (actualesById.TryGetValue(detalle.ID, out var aEliminar))
                        {
                            db.DetallesAccesorio.Remove(aEliminar);
                        }
                        continue;
                    }

                    if (vacio)
                    {
                        // No borrar automáticamente detalles existentes si vienen vacíos en el POST.
                        // Simplemente ignorar y mantener el registro tal cual.
                        continue;
                    }

                    if (actualesById.TryGetValue(detalle.ID, out var existente))
                    {
                        existente.CaracteristicaAccesorioID = detalle.CaracteristicaAccesorioID;
                        existente.Cantidad = detalle.Cantidad;
                        existente.Medida = detalle.Medida;
                        existente.UnidadID = detalle.UnidadID;
                        existente.Tipo = detalle.Tipo?.Trim();
                    }

                    continue;
                }

                // Nuevo detalle
                if (!vacio)
                {
                    db.DetallesAccesorio.Add(new DetalleAccesorio
                    {
                        AccesorioID = accesorioId,
                        CaracteristicaAccesorioID = detalle.CaracteristicaAccesorioID,
                        Cantidad = detalle.Cantidad,
                        Medida = detalle.Medida,
                        UnidadID = detalle.UnidadID,
                        Tipo = detalle.Tipo?.Trim()
                    });
                }
            }

            db.SaveChanges();
        }

        private void GuardarFotos(int accesorioId, HttpFileCollectionBase archivos)
        {
            if (archivos == null || archivos.Count == 0)
            {
                return;
            }

            var raiz = Server.MapPath("~/App_Data/AccesoriosFotos");
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

                var carpetaAccesorio = Path.Combine(raiz, accesorioId.ToString());
                Directory.CreateDirectory(carpetaAccesorio);

                var nombreFisico = Guid.NewGuid().ToString("N") + extension.ToLowerInvariant();
                var rutaFisica = Path.Combine(carpetaAccesorio, nombreFisico);
                archivo.SaveAs(rutaFisica);

                fotosDb.AccesorioFotos.Add(new AccesorioFoto
                {
                    AccesorioID = accesorioId,
                    RutaArchivo = "~/App_Data/AccesoriosFotos/" + accesorioId + "/" + nombreFisico,
                    NombreOriginal = Path.GetFileName(archivo.FileName),
                    ContentType = archivo.ContentType,
                    FechaCreacion = DateTime.Now
                });
            }

            fotosDb.SaveChanges();
        }

        private void EliminarFotosFisicasYRegistro(int accesorioId)
        {
            var fotos = fotosDb.AccesorioFotos.Where(f => f.AccesorioID == accesorioId).ToList();
            if (!fotos.Any())
            {
                return;
            }

            foreach (var foto in fotos)
            {
                var ruta = Server.MapPath(foto.RutaArchivo);
                if (!string.IsNullOrWhiteSpace(ruta) && System.IO.File.Exists(ruta))
                {
                    System.IO.File.Delete(ruta);
                }
            }

            fotosDb.AccesorioFotos.RemoveRange(fotos);
            fotosDb.SaveChanges();
        }
    }
}
