using CasaGaillard.Models;
using CasaGaillard.Models.ViewModels;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CasaGaillard.Areas.Mantenimiento.Controllers
{
    [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento + "," + AppRoles.Consulta)]
    public class UbicacionesController : Controller
    {
        private readonly GaillardEntities db = new GaillardEntities();

        public async Task<ActionResult> Index()
        {
            var model = await db.Database.SqlQuery<UbicacionCatalogoViewModel>(@"SELECT u.UbicacionId,u.Codigo,u.Nombre,u.Descripcion,
                    (SELECT COUNT(1) FROM gaillard.Instalacion i WHERE i.UbicacionId=u.UbicacionId) AS InstalacionesAsociadas,
                    (SELECT COUNT(1) FROM gaillard.Accesorio a WHERE a.UbicacionId=u.UbicacionId) AS AccesoriosAsociados
                FROM gaillard.Ubicacion u ORDER BY u.Nombre").ToListAsync();
            return View(model);
        }

        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public ActionResult Create() => View(new UbicacionCatalogoViewModel());

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> Create(UbicacionCatalogoViewModel model)
        {
            await ValidarAsync(model, 0);
            if (!ModelState.IsValid) return View(model);
            await db.Database.ExecuteSqlCommandAsync(@"INSERT INTO gaillard.Ubicacion (Codigo, Nombre, Descripcion)
                VALUES (@p0,@p1,@p2)", Texto(model.Codigo), model.Nombre.Trim(), Texto(model.Descripcion));
            TempData["LoginMessage"] = "La ubicación se ha creado.";
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
        public async Task<ActionResult> Edit(UbicacionCatalogoViewModel model)
        {
            await ValidarAsync(model, model.UbicacionId);
            if (!ModelState.IsValid) return View(model);
            var filas = await db.Database.ExecuteSqlCommandAsync(@"UPDATE gaillard.Ubicacion SET
                Codigo=@p0, Nombre=@p1, Descripcion=@p2 WHERE UbicacionId=@p3",
                Texto(model.Codigo), model.Nombre.Trim(), Texto(model.Descripcion), model.UbicacionId);
            if (filas == 0) return HttpNotFound();
            TempData["LoginMessage"] = "La ubicación se ha actualizado.";
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
            var filas = await db.Database.ExecuteSqlCommandAsync(@"DELETE FROM gaillard.Ubicacion
                WHERE UbicacionId=@p0
                  AND NOT EXISTS (SELECT 1 FROM gaillard.Instalacion WHERE UbicacionId=@p0)
                  AND NOT EXISTS (SELECT 1 FROM gaillard.Accesorio WHERE UbicacionId=@p0)", id);
            if (filas == 0)
            {
                var existe = await db.Database.SqlQuery<int>("SELECT COUNT(1) FROM gaillard.Ubicacion WHERE UbicacionId=@p0", id).SingleAsync();
                if (existe == 0) return HttpNotFound();
                TempData["CatalogoError"] = "No se puede eliminar: hay instalaciones o accesorios que utilizan esta ubicación.";
                return RedirectToAction("Delete", new { id });
            }
            TempData["LoginMessage"] = "La ubicación se ha eliminado.";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing) { if (disposing) db.Dispose(); base.Dispose(disposing); }

        private async Task<UbicacionCatalogoViewModel> ObtenerAsync(int id) =>
            await db.Database.SqlQuery<UbicacionCatalogoViewModel>(@"SELECT u.UbicacionId,u.Codigo,u.Nombre,u.Descripcion,
                (SELECT COUNT(1) FROM gaillard.Instalacion i WHERE i.UbicacionId=u.UbicacionId) AS InstalacionesAsociadas,
                (SELECT COUNT(1) FROM gaillard.Accesorio a WHERE a.UbicacionId=u.UbicacionId) AS AccesoriosAsociados
                FROM gaillard.Ubicacion u WHERE u.UbicacionId=@p0", id).SingleOrDefaultAsync();

        private async Task ValidarAsync(UbicacionCatalogoViewModel model, int id)
        {
            if (!string.IsNullOrWhiteSpace(model.Codigo) && await db.Database.SqlQuery<int>(@"SELECT COUNT(1)
                FROM gaillard.Ubicacion WHERE Codigo=@p0 AND UbicacionId<>@p1", model.Codigo.Trim(), id).SingleAsync() > 0)
                ModelState.AddModelError("Codigo", "Ya existe una ubicación con ese código.");
        }

        private static string Texto(string value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
