using CasaGaillard.Models;
using CasaGaillard.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CasaGaillard.Areas.Mantenimiento.Controllers
{
    [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento + "," + AppRoles.Consulta)]
    public class InstalacionDocumentosController : Controller
    {
        private const int MaximoBytes = 20 * 1024 * 1024;
        private static readonly HashSet<string> ExtensionesPermitidas = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        { ".pdf", ".png", ".jpg", ".jpeg", ".doc", ".docx", ".xls", ".xlsx", ".dwg" };
        private readonly GaillardEntities db = new GaillardEntities();

        public async Task<ActionResult> Index(int? instalacionId)
        {
            if (!instalacionId.HasValue) return new HttpStatusCodeResult(400);
            var instalacion = await db.Database.SqlQuery<InstalacionOpcion>(@"SELECT Id, Codigo, Nombre
                FROM gaillard.Instalacion WHERE Id=@p0", instalacionId.Value).SingleOrDefaultAsync();
            if (instalacion == null) return HttpNotFound();

            var documentos = await db.Database.SqlQuery<InstalacionDocumentoFilaViewModel>(@"SELECT d.Id, d.InstalacionId,
                    i.Codigo AS InstalacionCodigo, i.Nombre AS InstalacionNombre, d.Nombre, d.TipoDocumento,
                    d.CodigoPlano, d.FechaDocumento, d.FechaAlta
                FROM gaillard.InstalacionDocumento d
                INNER JOIN gaillard.Instalacion i ON i.Id=d.InstalacionId
                WHERE d.InstalacionId=@p0 ORDER BY d.FechaAlta DESC, d.Nombre", instalacionId.Value).ToListAsync();

            return View(new InstalacionDocumentoFormViewModel
            {
                InstalacionId = instalacion.Id,
                InstalacionCodigo = instalacion.Codigo,
                InstalacionNombre = instalacion.Nombre,
                Documentos = documentos
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Subir(InstalacionDocumentoFormViewModel model)
        {
            var instalacion = await db.Database.SqlQuery<InstalacionOpcion>(@"SELECT Id, Codigo, Nombre
                FROM gaillard.Instalacion WHERE Id=@p0", model.InstalacionId).SingleOrDefaultAsync();
            if (instalacion == null) return HttpNotFound();
            model.InstalacionCodigo = instalacion.Codigo;
            model.InstalacionNombre = instalacion.Nombre;

            var archivo = model.Archivo;
            var extension = archivo == null ? null : Path.GetExtension(Path.GetFileName(archivo.FileName));
            if (archivo == null || archivo.ContentLength <= 0)
                ModelState.AddModelError("Archivo", "Selecciona un archivo.");
            else if (archivo.ContentLength > MaximoBytes)
                ModelState.AddModelError("Archivo", "El archivo no puede superar los 20 MB.");
            else if (string.IsNullOrWhiteSpace(Path.GetFileName(archivo.FileName)))
                ModelState.AddModelError("Archivo", "El archivo seleccionado no tiene un nombre válido.");
            else if (string.IsNullOrWhiteSpace(extension) || !ExtensionesPermitidas.Contains(extension))
                ModelState.AddModelError("Archivo", "Formato no permitido. Usa PDF, imagen, Office o DWG.");

            if (!ModelState.IsValid)
            {
                model.Documentos = await ObtenerDocumentosAsync(model.InstalacionId);
                return View("Index", model);
            }

            var carpetaRelativa = "~/App_Data/InstalacionesDocumentos/" + model.InstalacionId;
            var carpetaFisica = Server.MapPath(carpetaRelativa);
            Directory.CreateDirectory(carpetaFisica);
            var nombreFisico = Guid.NewGuid().ToString("N") + extension.ToLowerInvariant();
            var rutaFisica = Path.Combine(carpetaFisica, nombreFisico);
            var rutaRelativa = carpetaRelativa + "/" + nombreFisico;
            archivo.SaveAs(rutaFisica);

            try
            {
                await db.Database.SqlQuery<int>(@"INSERT INTO gaillard.InstalacionDocumento
                    (InstalacionId, Nombre, RutaArchivo, TipoDocumento, CodigoPlano, FechaDocumento)
                    OUTPUT INSERTED.Id
                    VALUES (@p0,@p1,@p2,@p3,@p4,@p5)", model.InstalacionId,
                    Path.GetFileName(archivo.FileName), rutaRelativa, Texto(model.TipoDocumento),
                    Texto(model.CodigoPlano), model.FechaDocumento).SingleAsync();
            }
            catch
            {
                if (System.IO.File.Exists(rutaFisica)) System.IO.File.Delete(rutaFisica);
                throw;
            }

            TempData["LoginMessage"] = "El documento se ha guardado.";
            return RedirectToAction("Index", new { instalacionId = model.InstalacionId });
        }

        public async Task<ActionResult> Archivo(int id)
        {
            var documento = await db.Database.SqlQuery<DocumentoArchivo>(@"SELECT Nombre, RutaArchivo
                FROM gaillard.InstalacionDocumento WHERE Id=@p0", id).SingleOrDefaultAsync();
            if (documento == null) return HttpNotFound();
            var ruta = RutaSegura(documento.RutaArchivo);
            if (ruta == null || !System.IO.File.Exists(ruta)) return HttpNotFound();
            return File(ruta, "application/octet-stream", Path.GetFileName(documento.Nombre));
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Eliminar(int id)
        {
            var documento = await db.Database.SqlQuery<DocumentoArchivo>(@"SELECT Nombre, RutaArchivo
                FROM gaillard.InstalacionDocumento WHERE Id=@p0", id).SingleOrDefaultAsync();
            if (documento == null) return HttpNotFound();

            var instalacionId = await db.Database.SqlQuery<int>("SELECT InstalacionId FROM gaillard.InstalacionDocumento WHERE Id=@p0", id).SingleAsync();
            await db.Database.ExecuteSqlCommandAsync("DELETE FROM gaillard.InstalacionDocumento WHERE Id=@p0", id);
            var ruta = RutaSegura(documento.RutaArchivo);
            if (ruta != null && System.IO.File.Exists(ruta)) System.IO.File.Delete(ruta);
            TempData["LoginMessage"] = "El documento se ha eliminado.";
            return RedirectToAction("Index", new { instalacionId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }

        private async Task<IEnumerable<InstalacionDocumentoFilaViewModel>> ObtenerDocumentosAsync(int instalacionId)
        {
            return await db.Database.SqlQuery<InstalacionDocumentoFilaViewModel>(@"SELECT d.Id, d.InstalacionId,
                    i.Codigo AS InstalacionCodigo, i.Nombre AS InstalacionNombre, d.Nombre, d.TipoDocumento,
                    d.CodigoPlano, d.FechaDocumento, d.FechaAlta
                FROM gaillard.InstalacionDocumento d INNER JOIN gaillard.Instalacion i ON i.Id=d.InstalacionId
                WHERE d.InstalacionId=@p0 ORDER BY d.FechaAlta DESC, d.Nombre", instalacionId).ToListAsync();
        }

        private string RutaSegura(string relativa)
        {
            if (string.IsNullOrWhiteSpace(relativa) || !relativa.StartsWith("~/App_Data/InstalacionesDocumentos/", StringComparison.OrdinalIgnoreCase))
                return null;
            var raiz = Path.GetFullPath(Server.MapPath("~/App_Data/InstalacionesDocumentos"));
            var ruta = Path.GetFullPath(Server.MapPath(relativa));
            return ruta.StartsWith(raiz + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ? ruta : null;
        }

        private static string Texto(string value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private sealed class InstalacionOpcion
        {
            public InstalacionOpcion() { }
            public int Id { get; set; }
            public string Codigo { get; set; }
            public string Nombre { get; set; }
        }

        private sealed class DocumentoArchivo
        {
            public DocumentoArchivo() { }
            public string Nombre { get; set; }
            public string RutaArchivo { get; set; }
        }
    }
}
