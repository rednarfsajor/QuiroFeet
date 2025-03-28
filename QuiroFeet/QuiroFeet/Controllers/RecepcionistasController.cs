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
    public class RecepcionistasController : Controller
    {
        private QuiroFeetEntities db = new QuiroFeetEntities();

        // GET: Recepcionistas
        public ActionResult Index()
        {
            return View(db.Recepcionistas.ToList());
        }

        // GET: Recepcionistas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recepcionista recepcionista = db.Recepcionistas.Find(id);
            if (recepcionista == null)
            {
                return HttpNotFound();
            }
            return View(recepcionista);
        }

        // GET: Recepcionistas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Recepcionistas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,nombre,correo,numero")] Recepcionista recepcionista)
        {
            if (ModelState.IsValid)
            {
                db.Recepcionistas.Add(recepcionista);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(recepcionista);
        }

        // GET: Recepcionistas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recepcionista recepcionista = db.Recepcionistas.Find(id);
            if (recepcionista == null)
            {
                return HttpNotFound();
            }
            return View(recepcionista);
        }

        // POST: Recepcionistas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,nombre,correo,numero")] Recepcionista recepcionista)
        {
            if (ModelState.IsValid)
            {
                db.Entry(recepcionista).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(recepcionista);
        }

        // GET: Recepcionistas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recepcionista recepcionista = db.Recepcionistas.Find(id);
            if (recepcionista == null)
            {
                return HttpNotFound();
            }
            return View(recepcionista);
        }

        // POST: Recepcionistas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Recepcionista recepcionista = db.Recepcionistas.Find(id);
            db.Recepcionistas.Remove(recepcionista);
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
