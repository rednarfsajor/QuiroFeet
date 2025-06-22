using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Analisis.BD;

namespace Analisis.Controllers
{
    public class CitasController : Controller
    {
        private QuiroFeetEntities1 db = new QuiroFeetEntities1();
        // GET: Citas
        public ActionResult Calendar()
        {
            return View();
        }

        
        public ActionResult Day(String fecha)
        {
            ViewBag.Fecha = DateTime.Parse(fecha);
            Console.WriteLine(fecha);
            DateTime day;
            day = DateTime.Parse(fecha);
            var daycitas = db.Citas.Where(c => c.fecha == day).ToList();

            return View(daycitas);
        }

        [HttpGet]
        public ActionResult CreateDate(String fecha)
        {
            ViewBag.Fecha = fecha;
            Citas cita = new Citas();
            cita.fecha=DateTime.Parse(fecha);
            ViewBag.Servicios = new SelectList(db.Servicios.ToList(), "Id", "Nombre");
            ViewBag.Clientes = new SelectList(db.Clientes.ToList(), "id", "nombre");
            ViewBag.Profesionales = new SelectList(db.Empleados.Where(p=>p.tipo=="Profesional"), "id", "nombre");
            return View(cita);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDate(Citas nuevacita)
        {
            if (ModelState.IsValid)
            {
                nuevacita.Estado = "Agendado"; // Marcar como agendado al crearse

                db.Citas.Add(nuevacita);
                db.SaveChanges();
                return RedirectToAction("Calendar");
            }
            ViewBag.Servicios = new SelectList(db.Servicios.ToList(), "Id", "Nombre");
            ViewBag.Clientes = new SelectList(db.Clientes.ToList(), "id", "nombre");
            ViewBag.Profesionales = new SelectList(db.Empleados.Where(p => p.tipo == "Profesional"), "id", "nombre");
            return View(nuevacita);
        }
    }
}