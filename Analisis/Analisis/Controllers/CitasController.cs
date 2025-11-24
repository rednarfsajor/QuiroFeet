using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Analisis.BD;
using Analisis.ViewModel;

namespace Analisis.Controllers
{
    public class CitasController : Controller
    {
        private QuiroFeetEntities6 db = new QuiroFeetEntities6();

        // Cultura y formatos aceptados (incluye HTML5 date: yyyy-MM-dd)
        private static readonly CultureInfo CulturaEs = CultureInfo.GetCultureInfo("es-CR"); // o "es-ES"
        private static readonly string[] FormatosFecha =
        {
            "dd/MM/yyyy",
            "dd-MM-yyyy",
            "yyyy-MM-dd",               // <input type="date">
            "dd/MM/yyyy HH:mm",
            "dd/MM/yyyy H:mm",
            "yyyy-MM-ddTHH:mm",
            "yyyy-MM-ddTHH:mm:ss"
        };

        private static bool TryParseFecha(string s, out DateTime fecha)
        {
            // 1) Intenta exacto con los formatos más comunes
            if (DateTime.TryParseExact(s, FormatosFecha, CulturaEs, DateTimeStyles.None, out fecha))
                return true;

            // 2) Fallback: intenta parseo por cultura (por si viene con otros separadores)
            return DateTime.TryParse(s, CulturaEs, DateTimeStyles.None, out fecha);
        }

        // GET: Citas/Calendar
        public ActionResult Calendar()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        // GET: Citas/Day?fecha=14/11/2025  (o 2025-11-14)
        public ActionResult Day(string fecha)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(fecha) || !TryParseFecha(fecha, out var parsed))
            {
                // Si no se puede parsear, vuelve al calendario con aviso
                TempData["Error"] = "La fecha indicada no es válida.";
                return RedirectToAction("Calendar");
            }

            var dayOnly = parsed.Date;
            ViewBag.Fecha = dayOnly.ToString("dd/MM/yyyy", CulturaEs);

            var citas = db.Citas
                .Include(c => c.Clientes)
                .Include(c => c.Empleados)
                .Include(c => c.Servicios)
                .Where(c => DbFunctions.TruncateTime(c.fecha) == dayOnly)
                .Select(c => new DateViewModel
                {
                    id = c.id,
                    fecha = c.fecha,
                    hora = c.hora,
                    Estado = c.Estado,
                    NombreCliente = c.Clientes.nombre,
                    NombreProfesional = c.Empleados.nombre,
                    NombreServicio = c.Servicios.Nombre
                })
                .ToList();

            return View(citas);
        }

        [HttpGet]
        public ActionResult CreateDate(string fecha)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var cita = new Citas();

            if (string.IsNullOrWhiteSpace(fecha) || !TryParseFecha(fecha, out var parsed))
            {
                // Si la fecha no llega bien, usa hoy para no romper flujo
                parsed = DateTime.Today;
            }

            cita.fecha = parsed.Date;
            ViewBag.Fecha = parsed.ToString("dd/MM/yyyy", CulturaEs);

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
                return RedirectToAction("Login", "Account");

            // Normaliza a solo fecha (evita que una hora oculta rompa la igualdad)
            if (nuevacita != null)
            {
                nuevacita.fecha = nuevacita.fecha.Date;
            }

            if (ModelState.IsValid)
            {
                // Chequeo de choque: misma fecha (solo día) y misma hora
                bool citaExistente = db.Citas.Any(c =>
                    DbFunctions.TruncateTime(c.fecha) == nuevacita.fecha &&
                    c.hora == nuevacita.hora
                );

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
                return RedirectToAction("Login", "Account");

            var cita = db.Citas.Find(id);
            if (cita == null)
                return HttpNotFound();

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
                return RedirectToAction("Login", "Account");

            if (cita != null)
            {
                cita.fecha = cita.fecha.Date; // normaliza
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
                return RedirectToAction("Login", "Account");

            var cita = db.Citas.Find(id);
            if (cita == null)
                return HttpNotFound();

            cita.Estado = "Atendida";
            db.Citas.AddOrUpdate(cita);
            db.SaveChanges();

            return RedirectToAction("Day", new { fecha = cita.fecha.ToString("dd/MM/yyyy", CulturaEs) });
        }

        public ActionResult CancelDate(int id)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var cita = db.Citas.Find(id);
            if (cita == null)
                return HttpNotFound();

            cita.Estado = "Cancelada";
            db.Citas.AddOrUpdate(cita);
            db.SaveChanges();

            return RedirectToAction("Day", new { fecha = cita.fecha.ToString("dd/MM/yyyy", CulturaEs) });
        }
    }
}
