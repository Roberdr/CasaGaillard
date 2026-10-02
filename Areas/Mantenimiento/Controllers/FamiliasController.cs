using CasaGaillard.Models;
using CasaGaillard.Models.ViewModels;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CasaGaillard.Areas.Mantenimiento.Controllers
{
    [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento + "," + AppRoles.Consulta)]
    public class FamiliasController : Controller
    {
        private readonly GaillardEntities db = new GaillardEntities();

        public async Task<ActionResult> Index()
        {
            var model = new FamiliasSubfamiliasIndexViewModel
            {
                Familias = await db.Database.SqlQuery<FamiliaFilaViewModel>(@"SELECT f.FamiliaId,f.Nombre,f.Descripcion,
                    (SELECT COUNT(1) FROM gaillard.Subfamilia s WHERE s.FamiliaId=f.FamiliaId) AS NumeroSubfamilias,
                    (SELECT COUNT(1) FROM gaillard.Accesorio a WHERE a.FamiliaId=f.FamiliaId) AS NumeroAccesorios
                    FROM gaillard.Familia f ORDER BY f.Nombre").ToListAsync(),
                Subfamilias = await db.Database.SqlQuery<SubfamiliaFilaViewModel>(@"SELECT s.SubfamiliaId,s.FamiliaId,f.Nombre AS FamiliaNombre,s.Nombre,s.Descripcion,
                    (SELECT COUNT(1) FROM gaillard.Accesorio a WHERE a.SubfamiliaId=s.SubfamiliaId) AS NumeroAccesorios,
                    (SELECT COUNT(1) FROM gaillard.TipoAccesorio t WHERE t.SubfamiliaId=s.SubfamiliaId) AS NumeroTipos
                    FROM gaillard.Subfamilia s INNER JOIN gaillard.Familia f ON f.FamiliaId=s.FamiliaId
                    ORDER BY f.Nombre,s.Nombre").ToListAsync()
            };
            return View(model);
        }

        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public ActionResult CreateFamilia() => View("FamiliaForm", new FamiliaFormViewModel());

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> CreateFamilia(FamiliaFormViewModel model)
        {
            await ValidarNombreFamilia(model);
            if (!ModelState.IsValid) return View("FamiliaForm", model);
            db.Familia.Add(new Familia { Nombre = model.Nombre.Trim(), Descripcion = Texto(model.Descripcion) });
            await db.SaveChangesAsync();
            TempData["LoginMessage"] = "La familia se ha creado.";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> EditFamilia(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(400);
            var item = await db.Familia.FindAsync(id.Value);
            return item == null ? (ActionResult)HttpNotFound() : View("FamiliaForm", new FamiliaFormViewModel { FamiliaId = item.FamiliaId, Nombre = item.Nombre, Descripcion = item.Descripcion });
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> EditFamilia(FamiliaFormViewModel model)
        {
            await ValidarNombreFamilia(model);
            if (!ModelState.IsValid) return View("FamiliaForm", model);
            var item = await db.Familia.FindAsync(model.FamiliaId);
            if (item == null) return HttpNotFound();
            item.Nombre = model.Nombre.Trim(); item.Descripcion = Texto(model.Descripcion);
            await db.SaveChangesAsync();
            TempData["LoginMessage"] = "La familia se ha actualizado.";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public ActionResult CreateSubfamilia() => View("SubfamiliaForm", PrepararSubfamilia(new SubfamiliaFormViewModel()));

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> CreateSubfamilia(SubfamiliaFormViewModel model)
        {
            await ValidarSubfamilia(model);
            if (!ModelState.IsValid) return View("SubfamiliaForm", PrepararSubfamilia(model));
            db.Subfamilia.Add(new Subfamilia { FamiliaId = model.FamiliaId.Value, Nombre = model.Nombre.Trim(), Descripcion = Texto(model.Descripcion) });
            await db.SaveChangesAsync();
            TempData["LoginMessage"] = "La subfamilia se ha creado y vinculado a su familia.";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> EditSubfamilia(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(400);
            var item = await db.Subfamilia.FindAsync(id.Value);
            if (item == null) return HttpNotFound();
            return View("SubfamiliaForm", PrepararSubfamilia(new SubfamiliaFormViewModel { SubfamiliaId = item.SubfamiliaId, FamiliaId = item.FamiliaId, Nombre = item.Nombre, Descripcion = item.Descripcion }));
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> EditSubfamilia(SubfamiliaFormViewModel model)
        {
            await ValidarSubfamilia(model);
            if (!ModelState.IsValid) return View("SubfamiliaForm", PrepararSubfamilia(model));
            var item = await db.Subfamilia.FindAsync(model.SubfamiliaId);
            if (item == null) return HttpNotFound();
            item.FamiliaId = model.FamiliaId.Value; item.Nombre = model.Nombre.Trim(); item.Descripcion = Texto(model.Descripcion);
            await db.SaveChangesAsync();
            TempData["LoginMessage"] = "La subfamilia se ha actualizado.";
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> DeleteFamilia(int id)
        {
            var item = await db.Familia.FindAsync(id);
            if (item == null) return HttpNotFound();
            if (await db.Subfamilia.AnyAsync(s => s.FamiliaId == id) || await db.Accesorios.AnyAsync(a => a.FamiliaId == id))
                TempData["CatalogoError"] = "No se puede eliminar: la familia tiene subfamilias o accesorios asociados.";
            else { db.Familia.Remove(item); await db.SaveChangesAsync(); TempData["LoginMessage"] = "La familia se ha eliminado."; }
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.Administrador + "," + AppRoles.Mantenimiento)]
        public async Task<ActionResult> DeleteSubfamilia(int id)
        {
            var item = await db.Subfamilia.FindAsync(id);
            if (item == null) return HttpNotFound();
            if (await db.Accesorios.AnyAsync(a => a.SubfamiliaId == id) || await db.TiposAccesorio.AnyAsync(t => t.SubfamiliaId == id))
                TempData["CatalogoError"] = "No se puede eliminar: la subfamilia tiene accesorios o tipos asociados.";
            else { db.Subfamilia.Remove(item); await db.SaveChangesAsync(); TempData["LoginMessage"] = "La subfamilia se ha eliminado."; }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing) { if (disposing) db.Dispose(); base.Dispose(disposing); }

        private SubfamiliaFormViewModel PrepararSubfamilia(SubfamiliaFormViewModel model)
        {
            model.Familias = db.Familia.OrderBy(f => f.Nombre).ToList().Select(f => new SelectListItem { Value = f.FamiliaId.ToString(), Text = f.Nombre, Selected = f.FamiliaId == model.FamiliaId });
            return model;
        }

        private async Task ValidarNombreFamilia(FamiliaFormViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Nombre) && await db.Familia.AnyAsync(f => f.Nombre.ToLower() == model.Nombre.Trim().ToLower() && f.FamiliaId != model.FamiliaId))
                ModelState.AddModelError("Nombre", "Ya existe una familia con ese nombre.");
        }

        private async Task ValidarSubfamilia(SubfamiliaFormViewModel model)
        {
            if (model.FamiliaId.HasValue && !await db.Familia.AnyAsync(f => f.FamiliaId == model.FamiliaId.Value))
                ModelState.AddModelError("FamiliaId", "Selecciona una familia válida.");
            if (!string.IsNullOrWhiteSpace(model.Nombre) && model.FamiliaId.HasValue && await db.Subfamilia.AnyAsync(s => s.FamiliaId == model.FamiliaId.Value && s.Nombre.ToLower() == model.Nombre.Trim().ToLower() && s.SubfamiliaId != model.SubfamiliaId))
                ModelState.AddModelError("Nombre", "Ya existe una subfamilia con ese nombre dentro de la familia seleccionada.");
        }

        private static string Texto(string value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
