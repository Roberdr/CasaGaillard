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
        public async Task<ActionResult> Index()
        {
            var accesorios = db.Accesorios
                .Include(a => a.Material)
                .Include(a => a.TipoAccesorio)
                .Include("DetallesAccesorio.CaracteristicaAccesorio")
                .Include("DetallesAccesorio.Unidad");

            return View(await accesorios.ToListAsync());
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
        public ActionResult Create()
        {
            return View(CreateFormModel());
        }

        // POST: Accesorios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Create(AccesorioFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(CreateFormModel(model));
            }

            var accesorio = new Accesorio
            {
                TipoAccesorioID = model.TipoAccesorioID.Value,
                MaterialID = model.MaterialID.Value
            };

            db.Accesorios.Add(accesorio);
            await db.SaveChangesAsync();

            GuardarDetalles(accesorio.ID, model.Detalles);
            GuardarFotos(accesorio.ID, Request.Files);

            TempData["LoginMessage"] = "El accesorio se ha creado correctamente.";
            return RedirectToAction("Details", new { id = accesorio.ID });
        }

        // GET: Accesorios/Edit/5
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Edit(int? id)
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
                .FirstOrDefaultAsync(a => a.ID == id.Value);

            if (accesorio == null)
            {
                return HttpNotFound();
            }

            return View(MapToForm(accesorio));
        }

        // POST: Accesorios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Edit(AccesorioFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(CreateFormModel(model));
            }

            var accesorio = await db.Accesorios.FindAsync(model.ID);
            if (accesorio == null)
            {
                return HttpNotFound();
            }

            accesorio.TipoAccesorioID = model.TipoAccesorioID.Value;
            accesorio.MaterialID = model.MaterialID.Value;

            await db.SaveChangesAsync();

            GuardarDetalles(accesorio.ID, model.Detalles);
            GuardarFotos(accesorio.ID, Request.Files);

            TempData["LoginMessage"] = "El accesorio se ha actualizado.";
            return RedirectToAction("Details", new { id = accesorio.ID });
        }

        // GET: Accesorios/Delete/5
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Delete(int? id)
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

            return View(accesorio);
        }

        // POST: Accesorios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> DeleteConfirmed(int id)
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
            source.Caracteristicas = new SelectList(db.CaracteristicasAccesorio.OrderBy(c => c.CaracteristicaAccesorio1).ToList(), "ID", "CaracteristicaAccesorio1");
            source.Unidades = new SelectList(db.Unidades.OrderBy(u => u.Unidad1).ToList(), "ID", "Unidad1");

            source.Detalles = source.Detalles ?? new List<DetalleAccesorioFormViewModel>();
            if ((!source.Detalles.Any()) && source.ID > 0)
            {
                source.Detalles = db.DetallesAccesorio
                    .AsNoTracking()
                    .Include(d => d.CaracteristicaAccesorio)
                    .Include(d => d.Unidad)
                    .Where(d => d.AccesorioID == source.ID)
                    .OrderBy(d => d.ID)
                    .Select(d => new DetalleAccesorioFormViewModel
                    {
                        ID = d.ID,
                        CaracteristicaAccesorioID = d.CaracteristicaAccesorioID,
                        Cantidad = d.Cantidad,
                        Medida = d.Medida,
                        UnidadID = d.UnidadID,
                        Tipo = d.Tipo
                    })
                    .ToList();
            }

            if (!source.Detalles.Any())
            {
                source.Detalles = CrearFilasDetalleVacias(4);
            }

            source.FotosExistentes = source.FotosExistentes ?? new List<AccesorioFotoViewModel>();
            return source;
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
                FotosExistentes = fotos
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
            var lista = detalles ?? Enumerable.Empty<DetalleAccesorioFormViewModel>();

            foreach (var detalle in lista)
            {
                if (detalle == null)
                {
                    continue;
                }

                var vacio = !detalle.CaracteristicaAccesorioID.HasValue
                    && !detalle.Cantidad.HasValue
                    && !detalle.Medida.HasValue
                    && !detalle.UnidadID.HasValue
                    && string.IsNullOrWhiteSpace(detalle.Tipo);

                if (detalle.Eliminar)
                {
                    if (detalle.ID > 0)
                    {
                        var aEliminar = actuales.FirstOrDefault(d => d.ID == detalle.ID);
                        if (aEliminar != null)
                        {
                            db.DetallesAccesorio.Remove(aEliminar);
                        }
                    }

                    continue;
                }

                if (detalle.ID > 0)
                {
                    var existente = actuales.FirstOrDefault(d => d.ID == detalle.ID);
                    if (existente == null)
                    {
                        continue;
                    }

                    if (vacio)
                    {
                        db.DetallesAccesorio.Remove(existente);
                        continue;
                    }

                    existente.CaracteristicaAccesorioID = detalle.CaracteristicaAccesorioID;
                    existente.Cantidad = detalle.Cantidad;
                    existente.Medida = detalle.Medida;
                    existente.UnidadID = detalle.UnidadID;
                    existente.Tipo = detalle.Tipo?.Trim();
                }
                else if (!vacio)
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
