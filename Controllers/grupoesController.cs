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
    public class grupoesController : Controller
    {
        private GaillardEntities db = new GaillardEntities();

        // GET: grupoes
        public async Task<ActionResult> Index()
        {
            var grupoes = db.grupoes.Include(g => g.compartimento).Include(g => g.cuba).Include(g => g.situacion).Include(g => g.tipo_grupo);
            return View(await grupoes.ToListAsync());
        }

        // GET: grupoes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            grupo grupo = await db.grupoes.FindAsync(id);
            if (grupo == null)
            {
                return HttpNotFound();
            }
            return View(grupo);
        }

        // GET: grupoes/Create
        public ActionResult Create()
        {
            ViewBag.compartimento_id = new SelectList(db.compartimentoes, "id", "id");
            ViewBag.cuba_id = new SelectList(db.cubas, "id", "cuba1");
            ViewBag.situacion_id = new SelectList(db.situacions, "id", "lado_cuba_nombre");
            ViewBag.tipo_grupo_id = new SelectList(db.tipo_grupo, "id", "nombre_grupo");
            return View();
        }

        // POST: grupoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,tipo_grupo_id,cuba_id,compartimento_id,situacion_id")] grupo grupo)
        {
            if (ModelState.IsValid)
            {
                db.grupoes.Add(grupo);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.compartimento_id = new SelectList(db.compartimentoes, "id", "id", grupo.compartimento_id);
            ViewBag.cuba_id = new SelectList(db.cubas, "id", "cuba1", grupo.cuba_id);
            ViewBag.situacion_id = new SelectList(db.situacions, "id", "lado_cuba_nombre", grupo.situacion_id);
            ViewBag.tipo_grupo_id = new SelectList(db.tipo_grupo, "id", "nombre_grupo", grupo.tipo_grupo_id);
            return View(grupo);
        }

        // GET: grupoes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            grupo grupo = await db.grupoes.FindAsync(id);
            if (grupo == null)
            {
                return HttpNotFound();
            }
            ViewBag.compartimento_id = new SelectList(db.compartimentoes, "id", "id", grupo.compartimento_id);
            ViewBag.cuba_id = new SelectList(db.cubas, "id", "cuba1", grupo.cuba_id);
            ViewBag.situacion_id = new SelectList(db.situacions, "id", "lado_cuba_nombre", grupo.situacion_id);
            ViewBag.tipo_grupo_id = new SelectList(db.tipo_grupo, "id", "nombre_grupo", grupo.tipo_grupo_id);
            return View(grupo);
        }

        // POST: grupoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,tipo_grupo_id,cuba_id,compartimento_id,situacion_id")] grupo grupo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(grupo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.compartimento_id = new SelectList(db.compartimentoes, "id", "id", grupo.compartimento_id);
            ViewBag.cuba_id = new SelectList(db.cubas, "id", "cuba1", grupo.cuba_id);
            ViewBag.situacion_id = new SelectList(db.situacions, "id", "lado_cuba_nombre", grupo.situacion_id);
            ViewBag.tipo_grupo_id = new SelectList(db.tipo_grupo, "id", "nombre_grupo", grupo.tipo_grupo_id);
            return View(grupo);
        }

        // GET: grupoes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            grupo grupo = await db.grupoes.FindAsync(id);
            if (grupo == null)
            {
                return HttpNotFound();
            }
            return View(grupo);
        }

        // POST: grupoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            grupo grupo = await db.grupoes.FindAsync(id);
            db.grupoes.Remove(grupo);
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
