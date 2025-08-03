using System;
using System.Linq;
using System.Web.Mvc;
using Analisis.BD;
using System.Data.Entity;

namespace Analisis.Controllers
{
    public class ServiciosController : Controller
    {
        private QuiroFeetEntities6 db = new QuiroFeetEntities6();

        public ActionResult ServiceIndex()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        // GET: Servicios/ListServices
        public ActionResult ListServices()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var servicios = db.Servicios.ToList();
            return View(servicios);
        }

        // GET: Servicios/CreateService
        public ActionResult CreateService()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        // POST: Servicios/CreateService
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateService(Servicios servicio)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                db.Servicios.Add(servicio);
                db.SaveChanges();
                return RedirectToAction("ListServices");
            }
            return View(servicio);
        }

        // GET: Servicios/EditService/5
        public ActionResult EditService(int id)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var servicio = db.Servicios.Find(id);
            if (servicio == null) return HttpNotFound();
            return View(servicio);
        }

        // POST: Servicios/EditService/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditService(Servicios servicio)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                db.Entry(servicio).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ListServices");
            }
            return View(servicio);
        }

        // GET: Servicios/DeleteService/5
        public ActionResult DeleteService(int id)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var servicio = db.Servicios.Find(id);
            if (servicio == null) return HttpNotFound();

            db.Servicios.Remove(servicio);
            db.SaveChanges();
            return RedirectToAction("ListServices");
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
