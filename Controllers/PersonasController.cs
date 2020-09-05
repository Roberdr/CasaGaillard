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
    public class PersonasController : Controller
    {
        private readonly GaillardEntities db = new GaillardEntities();

        // GET: Personas
        public async Task<ActionResult> Index()
        {
            var personas = db.Personas.Include(p => p.Direccion);
            return View(await personas.ToListAsync());
        }

        // GET: Personas/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Persona persona = await db.Personas.FindAsync(id);
            if (persona == null)
            {
                return HttpNotFound();
            }
            return View(persona);
        }

        // GET: Personas/Create
        public ActionResult Create()
        {
            ViewBag.DireccionID = new SelectList(db.Direcciones, "ID", "NombreVia");
            return View();
        }

        // POST: Personas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,NombrePersona,Apellido1,Apellido2,NIF,DireccionID")] Persona persona)
        {
            if (ModelState.IsValid)
            {
                db.Personas.Add(persona);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.DireccionID = new SelectList(db.Direcciones.Select(x => new {x.ID, DirComp = x.NombreVia + ", " + x.Numero}), "ID", "DirComp", persona.DireccionID);
            return View(persona);
        }

        // GET: Personas/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Persona persona = await db.Personas.FindAsync(id);
            if (persona == null)
            {
                return HttpNotFound();
            }
            ViewBag.DireccionID = new SelectList(db.Direcciones, "ID", "NombreVia", persona.DireccionID);
            return View(persona);
        }

        // POST: Personas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,NombrePersona,Apellido1,Apellido2,NIF,DireccionID")] Persona persona)
        {
            if (ModelState.IsValid)
            {
                db.Entry(persona).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.DireccionID = new SelectList(db.Direcciones, "ID", "NombreVia", persona.DireccionID);
            return View(persona);
        }

        // GET: Personas/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Persona persona = await db.Personas.FindAsync(id);
            if (persona == null)
            {
                return HttpNotFound();
            }
            return View(persona);
        }

        // POST: Personas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Persona persona = await db.Personas.FindAsync(id);
            db.Personas.Remove(persona);
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
