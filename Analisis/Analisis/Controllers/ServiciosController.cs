using System;
using System.Linq;
using System.Web.Mvc;
using Analisis.BD; // Tu contexto de base de datos
using System.Data.Entity;


namespace Analisis.Controllers
{
    public class ServiciosController : Controller
    {
        private QuiroFeetEntities1 db = new QuiroFeetEntities1();


        public ActionResult ServiceIndex()
        {
            return View();
        }


        // GET: Servicios/ListServices
        public ActionResult ListServices()
        {
            var servicios = db.Servicios.ToList();
            return View(servicios);
        }

        // GET: Servicios/CreateService
        public ActionResult CreateService()
        {
            return View();
        }

        // POST: Servicios/CreateService
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateService(Servicios servicio)
        {
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
            var servicio = db.Servicios.Find(id);
            if (servicio == null) return HttpNotFound();
            return View(servicio);
        }

        // POST: Servicios/EditService/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditService(Servicios servicio)
        {
            if (ModelState.IsValid)
            {
                db.Entry(servicio).State = EntityState.Modified; // 💡 Requiere el using
                db.SaveChanges();
                return RedirectToAction("ListServices");
            }
            return View(servicio);
        }

        // GET: Servicios/DeleteService/5
        public ActionResult DeleteService(int id)
        {
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
