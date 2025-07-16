using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Web.WebPages;
using Analisis.BD;
using Analisis.ViewModel;

namespace Analisis.Controllers
{
    public class CitasController : Controller
    {
        private QuiroFeetEntities5 db = new QuiroFeetEntities5();

        // GET: Citas/Calendar
        public ActionResult Calendar()
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        public ActionResult Day(string fecha)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new DateViewModel();

            ViewBag.Fecha = fecha;
            Console.WriteLine(fecha);
            DateTime day = DateTime.Parse(fecha);
            var citas = db.Citas
                .Include("Clientes")
                .Include("Empleados")
                .Include("Servicios")
                .Select(c => new DateViewModel
                {
                    id = c.id,
                    fecha = c.fecha,
                    hora = c.hora,
                    Estado = c.Estado,
                    NombreCliente = c.Clientes.nombre,
                    NombreProfesional = c.Empleados.nombre,
                    NombreServicio = c.Servicios.Nombre
                }).Where(c => c.fecha == day).ToList();

            return View(citas);
        }

        [HttpGet]
        public ActionResult CreateDate(string fecha)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Fecha = fecha;
            Citas cita = new Citas();
            cita.fecha = DateTime.Parse(fecha);
            ViewBag.Servicios = new SelectList(db.Servicios.ToList(), "Id", "Nombre");
            ViewBag.Clientes = new SelectList(db.Clientes.ToList(), "id", "nombre");
            ViewBag.Profesionales = new SelectList(db.Empleados.Where(p => p.tipo == "Profesional"), "id", "nombre");

            return View(cita);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDate(Citas nuevacita)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                bool citaExistente = db.Citas.Any(c =>
                    c.fecha == nuevacita.fecha &&
                    c.hora == nuevacita.hora);

                if (citaExistente)
                {
                    ModelState.AddModelError("hora", "Ya existe una cita en esa fecha y hora.");
                }
                else
                {
                    nuevacita.Estado = "Agendado"; // Marcar como agendado al crearse

                    db.Citas.Add(nuevacita);
                    db.SaveChanges();

                    return RedirectToAction("Calendar");
                }
            }

            ViewBag.Servicios = new SelectList(db.Servicios.ToList(), "Id", "Nombre");
            ViewBag.Clientes = new SelectList(db.Clientes.ToList(), "id", "nombre");
            ViewBag.Profesionales = new SelectList(db.Empleados.Where(p => p.tipo == "Profesional"), "id", "nombre");

            return View(nuevacita);
        }

        public ActionResult ModifyDate(int id)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            Citas cita = db.Citas.Find(id);

            ViewBag.Servicios = new SelectList(db.Servicios.ToList(), "Id", "Nombre");
            ViewBag.Clientes = new SelectList(db.Clientes.ToList(), "id", "nombre");
            ViewBag.Profesionales = new SelectList(db.Empleados.Where(p => p.tipo == "Profesional"), "id", "nombre");

            return View(cita);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ModifyDate(Citas cita)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                db.Citas.AddOrUpdate(cita);
                db.SaveChanges();
                return RedirectToAction("Calendar");
            }

            return View(cita);
        }

        public ActionResult ProcessDate(int id)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cita = db.Citas.Find(id);
            if (cita == null)
            {
                return HttpNotFound();
            }

            cita.Estado = "Atendida";
            db.Citas.AddOrUpdate(cita);
            db.SaveChanges();

            return RedirectToAction("Day", new { fecha = cita.fecha.ToString("dd/MM/yyyy") });
        }

        public ActionResult CancelDate(int id)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cita = db.Citas.Find(id);
            if (cita == null)
            {
                return HttpNotFound();
            }

            cita.Estado = "Cancelada";
            db.Citas.AddOrUpdate(cita);
            db.SaveChanges();

            return RedirectToAction("Day", new { fecha = cita.fecha.ToString("dd/MM/yyyy") });
        }
    }
}
