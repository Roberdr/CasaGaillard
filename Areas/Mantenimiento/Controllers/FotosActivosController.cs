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
    public class FotosActivosController : Controller
    {
        private const int MaximoBytes = 5 * 1024 * 1024;
        private static readonly HashSet<string> ExtensionesPermitidas = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private readonly InstalacionEquipoFotosContext fotosDb = new InstalacionEquipoFotosContext();
        private readonly GaillardEntities db = new GaillardEntities();

        public async Task<ActionResult> Index(string tipo, int? id)
        {
            tipo = NormalizarTipo(tipo);
            if (tipo == null || !id.HasValue) return new HttpStatusCodeResult(400);
            var titulo = await ObtenerTituloAsync(tipo, id.Value);
            if (titulo == null) return HttpNotFound();
            var model = await CrearModeloAsync(tipo, id.Value, titulo);
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Subir(string tipo, int id)
        {
            tipo = NormalizarTipo(tipo);
            if (tipo == null) return new HttpStatusCodeResult(400);
            var titulo = await ObtenerTituloAsync(tipo, id);
            if (titulo == null) return HttpNotFound();

            var archivos = Request.Files;
            var seleccionados = Enumerable.Range(0, archivos.Count).Select(i => archivos[i])
                .Where(a => a != null && a.ContentLength > 0).ToList();
            if (seleccionados.Count == 0) return await MostrarErrorAsync(tipo, id, titulo, "Selecciona al menos una imagen.");
            if (seleccionados.Count > 10) return await MostrarErrorAsync(tipo, id, titulo, "Puedes subir un máximo de 10 imágenes cada vez.");

            foreach (var archivo in seleccionados)
            {
                var extension = Path.GetExtension(Path.GetFileName(archivo.FileName));
                if (archivo.ContentLength > MaximoBytes)
                    return await MostrarErrorAsync(tipo, id, titulo, "Cada imagen puede ocupar como máximo 5 MB.");
                if (string.IsNullOrWhiteSpace(extension) || !ExtensionesPermitidas.Contains(extension) ||
                    string.IsNullOrWhiteSpace(archivo.ContentType) || !archivo.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                    return await MostrarErrorAsync(tipo, id, titulo, "Formato no permitido. Usa JPG, PNG, GIF o WEBP.");
            }

            var carpetaRelativa = "~/App_Data/InstalacionEquipoFotos/" + tipo + "/" + id;
            var carpetaFisica = Server.MapPath(carpetaRelativa);
            Directory.CreateDirectory(carpetaFisica);
            foreach (var archivo in seleccionados)
            {
                var extension = Path.GetExtension(Path.GetFileName(archivo.FileName)).ToLowerInvariant();
                var nombreFisico = Guid.NewGuid().ToString("N") + extension;
                var rutaRelativa = carpetaRelativa + "/" + nombreFisico;
                archivo.SaveAs(Path.Combine(carpetaFisica, nombreFisico));
                fotosDb.Fotos.Add(new InstalacionEquipoFoto
                {
                    TipoActivo = tipo,
                    ActivoID = id,
                    RutaArchivo = rutaRelativa,
                    NombreOriginal = Path.GetFileName(archivo.FileName),
                    ContentType = archivo.ContentType,
                    FechaCreacion = DateTime.Now
                });
            }
            await fotosDb.SaveChangesAsync();
            TempData["LoginMessage"] = "Las fotos se han guardado.";
            return RedirectToAction("Index", new { tipo, id });
        }

        public async Task<ActionResult> Foto(int id)
        {
            var foto = await fotosDb.Fotos.AsNoTracking().SingleOrDefaultAsync(f => f.ID == id);
            if (foto == null) return HttpNotFound();
            var ruta = RutaSegura(foto.RutaArchivo, foto.TipoActivo, foto.ActivoID);
            if (ruta == null || !System.IO.File.Exists(ruta)) return HttpNotFound();
            return File(ruta, string.IsNullOrWhiteSpace(foto.ContentType) ? "application/octet-stream" : foto.ContentType);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Eliminar(int fotoId)
        {
            var foto = await fotosDb.Fotos.SingleOrDefaultAsync(f => f.ID == fotoId);
            if (foto == null) return HttpNotFound();
            var tipo = foto.TipoActivo;
            var id = foto.ActivoID;
            var ruta = RutaSegura(foto.RutaArchivo, tipo, id);
            fotosDb.Fotos.Remove(foto);
            await fotosDb.SaveChangesAsync();
            if (ruta != null && System.IO.File.Exists(ruta)) System.IO.File.Delete(ruta);
            TempData["LoginMessage"] = "La foto se ha eliminado.";
            return RedirectToAction("Index", new { tipo, id });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { fotosDb.Dispose(); db.Dispose(); }
            base.Dispose(disposing);
        }

        private async Task<ActionResult> MostrarErrorAsync(string tipo, int id, string titulo, string error)
        {
            ModelState.AddModelError("", error);
            return View("Index", await CrearModeloAsync(tipo, id, titulo));
        }

        private async Task<InstalacionEquipoFotosViewModel> CrearModeloAsync(string tipo, int id, string titulo)
        {
            var fotos = await fotosDb.Fotos.AsNoTracking().Where(f => f.TipoActivo == tipo && f.ActivoID == id)
                .OrderByDescending(f => f.FechaCreacion)
                .Select(f => new InstalacionEquipoFotoFilaViewModel { ID = f.ID, NombreOriginal = f.NombreOriginal, FechaCreacion = f.FechaCreacion })
                .ToListAsync();
            return new InstalacionEquipoFotosViewModel { Tipo = tipo, ActivoID = id, Titulo = titulo, Fotos = fotos };
        }

        private async Task<string> ObtenerTituloAsync(string tipo, int id)
        {
            if (tipo == "Instalacion")
                return await db.Database.SqlQuery<string>("SELECT Codigo + N' - ' + Nombre FROM gaillard.Instalacion WHERE Id=@p0", id).SingleOrDefaultAsync();
            return await db.Database.SqlQuery<string>("SELECT Codigo + N' - ' + Nombre FROM gaillard.Equipo WHERE Id=@p0", id).SingleOrDefaultAsync();
        }

        private string RutaSegura(string relativa, string tipo, int id)
        {
            var prefijo = "~/App_Data/InstalacionEquipoFotos/" + tipo + "/" + id + "/";
            if (tipo != "Instalacion" && tipo != "Equipo" || string.IsNullOrWhiteSpace(relativa) || !relativa.StartsWith(prefijo, StringComparison.OrdinalIgnoreCase)) return null;
            var raiz = Path.GetFullPath(Server.MapPath("~/App_Data/InstalacionEquipoFotos/" + tipo + "/" + id));
            var ruta = Path.GetFullPath(Server.MapPath(relativa));
            return ruta.StartsWith(raiz + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ? ruta : null;
        }

        private static string NormalizarTipo(string tipo)
        {
            if (string.Equals(tipo, "Instalacion", StringComparison.OrdinalIgnoreCase)) return "Instalacion";
            if (string.Equals(tipo, "Equipo", StringComparison.OrdinalIgnoreCase)) return "Equipo";
            return null;
        }
    }
}
