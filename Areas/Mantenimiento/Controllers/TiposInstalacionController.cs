using CasaGaillard.Models;
using CasaGaillard.Models.ViewModels;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CasaGaillard.Areas.Mantenimiento.Controllers
{
    [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento + "," + AppRoles.Consulta)]
    public class TiposInstalacionController : Controller
    {
        private readonly GaillardEntities db = new GaillardEntities();

        public async Task<ActionResult> Index()
        {
            var model = await db.Database.SqlQuery<TipoInstalacionCatalogoViewModel>(@"SELECT t.Id, t.Nombre, t.Descripcion,
                    (SELECT COUNT(1) FROM gaillard.Instalacion i WHERE i.TipoInstalacionId=t.Id) AS InstalacionesAsociadas
                FROM gaillard.TipoInstalacion t ORDER BY t.Nombre").ToListAsync();
            return View(model);
        }

        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public ActionResult Create() => View(new TipoInstalacionCatalogoViewModel());

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Create(TipoInstalacionCatalogoViewModel model)
        {
            await ValidarNombreAsync(model.Nombre, 0);
            if (!ModelState.IsValid) return View(model);

            await db.Database.ExecuteSqlCommandAsync(@"INSERT INTO gaillard.TipoInstalacion (Nombre, Descripcion)
                VALUES (@p0,@p1)", model.Nombre.Trim(), Texto(model.Descripcion));
            TempData["LoginMessage"] = "El tipo de instalación se ha creado.";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Edit(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(400);
            var model = await ObtenerAsync(id.Value);
            return model == null ? (ActionResult)HttpNotFound() : View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Edit(TipoInstalacionCatalogoViewModel model)
        {
            await ValidarNombreAsync(model.Nombre, model.Id);
            if (!ModelState.IsValid) return View(model);
            var filas = await db.Database.ExecuteSqlCommandAsync(@"UPDATE gaillard.TipoInstalacion
                SET Nombre=@p0, Descripcion=@p1 WHERE Id=@p2", model.Nombre.Trim(), Texto(model.Descripcion), model.Id);
            if (filas == 0) return HttpNotFound();
            TempData["LoginMessage"] = "El tipo de instalación se ha actualizado.";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Delete(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(400);
            var model = await ObtenerAsync(id.Value);
            return model == null ? (ActionResult)HttpNotFound() : View(model);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var filas = await db.Database.ExecuteSqlCommandAsync(@"DELETE FROM gaillard.TipoInstalacion
                WHERE Id=@p0 AND NOT EXISTS (SELECT 1 FROM gaillard.Instalacion WHERE TipoInstalacionId=@p0)", id);
            if (filas == 0)
            {
                var existe = await db.Database.SqlQuery<int>("SELECT COUNT(1) FROM gaillard.TipoInstalacion WHERE Id=@p0", id).SingleAsync();
                if (existe == 0) return HttpNotFound();
                TempData["CatalogoError"] = "No se puede eliminar: hay instalaciones que utilizan este tipo.";
                return RedirectToAction("Delete", new { id });
            }
            TempData["LoginMessage"] = "El tipo de instalación se ha eliminado.";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing) { if (disposing) db.Dispose(); base.Dispose(disposing); }

        private async Task<TipoInstalacionCatalogoViewModel> ObtenerAsync(int id) =>
            await db.Database.SqlQuery<TipoInstalacionCatalogoViewModel>(@"SELECT t.Id,t.Nombre,t.Descripcion,
                (SELECT COUNT(1) FROM gaillard.Instalacion i WHERE i.TipoInstalacionId=t.Id) AS InstalacionesAsociadas
                FROM gaillard.TipoInstalacion t WHERE t.Id=@p0", id).SingleOrDefaultAsync();

        private async Task ValidarNombreAsync(string nombre, int id)
        {
            if (!string.IsNullOrWhiteSpace(nombre) && await db.Database.SqlQuery<int>(@"SELECT COUNT(1)
                FROM gaillard.TipoInstalacion WHERE Nombre=@p0 AND Id<>@p1", nombre.Trim(), id).SingleAsync() > 0)
                ModelState.AddModelError("Nombre", "Ya existe un tipo de instalación con ese nombre.");
        }

        private static string Texto(string value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
