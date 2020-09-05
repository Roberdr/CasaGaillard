using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CasaGaillard.Models;

namespace CasaGaillard.Controllers
{
    [Authorize]
    public class GruposController : Controller
    {
        private readonly GaillardEntities db = new GaillardEntities();

        // GET: Grupos
        public async Task<ActionResult> Index()
        {
            var grupos = db.Grupos
                .Include(g => g.TipoGrupo)
                .Include(g => g.Cuba)
                .Include(g => g.Situacion)
                .Include(g => g.Compartimento)
                .Include(g => g.AccesoriosGrupo)
                .OrderBy(g => g.Cuba.MatriculaCuba)
                    .ThenBy(g => g.Compartimento.Numero)
                    .ThenBy(g => g.TipoGrupo.NombreGrupo);
            return View(await grupos.ToListAsync());
        }

        public async Task<ActionResult> Index2()
        {
            var grupos = db.Grupos
                //.Include(g => g.TipoGrupo)
                //.Include(g => g.Cuba)
                //.Include(g => g.Situacion)
                //.Include(g => g.Compartimento)
                .OrderBy(g => g.Cuba.MatriculaCuba)
                    .ThenBy(g => g.Compartimento.Numero)
                    .ThenBy(g => g.TipoGrupo.NombreGrupo);
                //.Include(g => g.AccesoriosGrupo.SelectMany(a => a.Accesorio.TipoAccesorio, a => a.))
                //.Include(a => a.);
            return View(await grupos.ToListAsync());
        }

        public async Task<ActionResult> Index3()
        {
            var grupos = from g in db.Grupos
                         orderby g.Cuba.MatriculaCuba, g.Compartimento.Numero
                         //join ag in db.AccesoriosGrupo on g.ID equals ag.GrupoID
                         select g;
            ViewBag.Grups = grupos;

            return View(await grupos.ToListAsync());
        }

        // GET: Grupos/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grupo grupo = await db.Grupos.FindAsync(id);

            if (grupo == null)
            {
                return HttpNotFound();
            }
            return View(grupo);
        }

        // GET: Grupos/Create
        public ActionResult Create()
        {
            ViewBag.TipoGrupoID = new SelectList(db.TiposGrupo, "ID", "NombreGrupo");
            ViewBag.CubaID = new SelectList(db.Cubas, "ID", "MatriculaCuba");
            ViewBag.SituacionID = new SelectList(db.Situaciones, "ID", "LadoCubaNombre");

            return View();
        }

        // POST: Grupos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,TipoGrupoID,CubaID,CompartimentoID,SituacionID")] Grupo grupo)
        {

            if (grupo.Compartimento.Numero == null)
            {
                
            } 
            if (ModelState.IsValid)
            {
                db.Grupos.Add(grupo);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.TipoGrupoID = new SelectList(db.TiposGrupo, "ID", "NombreGrupo", grupo.TipoGrupoID);
            ViewBag.CubaID = new SelectList(db.Cubas, "ID", "MatriculaCuba", grupo.CubaID);
            ViewBag.SituacionID = new SelectList(db.Situaciones, "ID", "LadoCubaNombre", grupo.SituacionID);
            return View(grupo);
        }

        // GET: Grupos/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grupo grupo = await db.Grupos.FindAsync(id);
            if (grupo == null)
            {
                return HttpNotFound();
            }
            ViewBag.TipoGrupoID = new SelectList(db.TiposGrupo, "ID", "NombreGrupo", grupo.TipoGrupoID);
            ViewBag.CubaID = new SelectList(db.Cubas, "ID", "MatriculaCuba", grupo.CubaID);
            ViewBag.SituacionID = new SelectList(db.Situaciones, "ID", "LadoCubaNombre", grupo.SituacionID);
            return View(grupo);
        }

        // POST: Grupos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,TipoGrupoID,CubaID,CompartimentoID,SituacionID")] Grupo grupo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(grupo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.TipoGrupoID = new SelectList(db.TiposGrupo, "ID", "NombreGrupo", grupo.TipoGrupoID);
            ViewBag.CubaID = new SelectList(db.Cubas, "ID", "MatriculaCuba", grupo.CubaID);
            ViewBag.SituacionID = new SelectList(db.Situaciones, "ID", "LadoCubaNombre", grupo.SituacionID);
            return View(grupo);
        }

        // GET: Grupos/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grupo grupo = await db.Grupos.FindAsync(id);
            if (grupo == null)
            {
                return HttpNotFound();
            }
            return View(grupo);
        }

        // POST: Grupos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Grupo grupo = await db.Grupos.FindAsync(id);
            db.Grupos.Remove(grupo);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
