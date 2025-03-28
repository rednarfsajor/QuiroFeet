using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QuiroFeet.Data;

namespace QuiroFeet.Controllers
{
    public class ProfesionalesController : Controller
    {
        private QuiroFeetEntities db = new QuiroFeetEntities();

        // GET: Profesionales
        public ActionResult Index()
        {
            return View(db.Profesionals.ToList());
        }

        // GET: Profesionales/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profesional profesional = db.Profesionals.Find(id);
            if (profesional == null)
            {
                return HttpNotFound();
            }
            return View(profesional);
        }

        // GET: Profesionales/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Profesionales/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,nombre,especialidad,correo,numero")] Profesional profesional)
        {
            if (ModelState.IsValid)
            {
                db.Profesionals.Add(profesional);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(profesional);
        }

        // GET: Profesionales/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profesional profesional = db.Profesionals.Find(id);
            if (profesional == null)
            {
                return HttpNotFound();
            }
            return View(profesional);
        }

        // POST: Profesionales/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,nombre,especialidad,correo,numero")] Profesional profesional)
        {
            if (ModelState.IsValid)
            {
                db.Entry(profesional).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(profesional);
        }

        // GET: Profesionales/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profesional profesional = db.Profesionals.Find(id);
            if (profesional == null)
            {
                return HttpNotFound();
            }
            return View(profesional);
        }

        // POST: Profesionales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Profesional profesional = db.Profesionals.Find(id);
            db.Profesionals.Remove(profesional);
            db.SaveChanges();
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
