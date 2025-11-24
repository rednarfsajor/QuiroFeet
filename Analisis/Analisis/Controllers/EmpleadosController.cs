using System;
using System.Linq;
using System.Web.Mvc;
using Analisis.BD;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace Analisis.Controllers
{
    public class EmpleadosController : Controller
    {
        private QuiroFeetEntities6 db = new QuiroFeetEntities6();

        // --- Helpers de sesión ---
        private bool IsLogged() => Session["UsuarioId"] != null;
        private ActionResult GoLogin() => RedirectToAction("Login", "Account");

        // GET: Empleados (dashboard simple)
        public ActionResult Empleados()
        {
            if (!IsLogged()) return GoLogin();
            return View();
        }

        // LISTAR Empleados (por defecto: solo activos)
        // Para ver también inactivos: /Empleados/DetallesEmpleados?incluirInactivos=true
        public ActionResult DetallesEmpleados(bool incluirInactivos = false)
        {
            if (!IsLogged()) return GoLogin();

            IQueryable<Empleados> query = db.Empleados;

            if (!incluirInactivos)
                query = query.Where(e => e.activo == true);

            var empleados = query.ToList();

            ViewBag.ToastMessage = TempData["ToastMessage"];
            ViewBag.ToastError = TempData["ToastError"];
            ViewBag.IncluirInactivos = incluirInactivos;

            return View(empleados);
        }

        // CREAR Empleado - GET
        [HttpGet]
        public ActionResult RegistrarEmpleados()
        {
            if (!IsLogged()) return GoLogin();
            return View();
        }

        // CREAR Empleado - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarEmpleados(Empleados nuevoEmpleado)
        {
            if (!IsLogged()) return GoLogin();

            // Ejemplo: correo único entre activos
            if (db.Empleados.Any(p => p.correo == nuevoEmpleado.correo && p.activo == true))
                ModelState.AddModelError("correo", "Ya existe un profesional activo con este correo.");

            if (!ModelState.IsValid) return View(nuevoEmpleado);

            try
            {
                // Siempre crear como ACTIVO
                nuevoEmpleado.activo = true;

                db.Empleados.Add(nuevoEmpleado);
                db.SaveChanges();

                TempData["ToastMessage"] = "Empleado registrado correctamente.";
                return RedirectToAction("DetallesEmpleados");
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var set in ex.EntityValidationErrors)
                    foreach (var err in set.ValidationErrors)
                        ModelState.AddModelError(err.PropertyName, err.ErrorMessage);

                return View(nuevoEmpleado);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Ocurrió un error al guardar el empleado: " + e.Message);
                return View(nuevoEmpleado);
            }
        }

        // EDITAR Empleado - GET
        [HttpGet]
        public ActionResult EditarEmpleados(int id)
        {
            if (!IsLogged()) return GoLogin();

            var profesional = db.Empleados.Find(id);
            if (profesional == null) return HttpNotFound();

            return View(profesional);
        }

        // EDITAR Empleado - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarEmpleados(Empleados profesionalEditado)
        {
            if (!IsLogged()) return GoLogin();

            if (!ModelState.IsValid) return View(profesionalEditado);

            try
            {
                db.Entry(profesionalEditado).State = EntityState.Modified;
                db.SaveChanges();

                TempData["ToastMessage"] = "Empleado actualizado correctamente.";
                return RedirectToAction("DetallesEmpleados");
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var set in ex.EntityValidationErrors)
                    foreach (var err in set.ValidationErrors)
                        ModelState.AddModelError(err.PropertyName, err.ErrorMessage);

                return View(profesionalEditado);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Ocurrió un error al actualizar: " + e.Message);
                return View(profesionalEditado);
            }
        }

        // CONFIRMAR (muestra tarjeta; el botón "Inactivar usuario" postea aquí)
        public ActionResult ConfirmarEliminarEmpleados(int id)
        {
            if (!IsLogged()) return GoLogin();

            var empleado = db.Empleados.Find(id);
            if (empleado == null) return HttpNotFound();

            ViewBag.ToastError = TempData["ToastError"];
            ViewBag.ToastMessage = TempData["ToastMessage"];

            return View(empleado);
        }

        // "Eliminar" -> INACTIVAR (soft-delete)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteEmpleados(int id)
        {
            if (!IsLogged()) return GoLogin();

            var empleado = db.Empleados.Find(id);
            if (empleado == null) return HttpNotFound();

            try
            {
                if (empleado.activo == false)
                {
                    TempData["ToastMessage"] = "Este empleado ya estaba inactivo.";
                }
                else
                {
                    empleado.activo = false;
                    db.Entry(empleado).State = EntityState.Modified;
                    db.SaveChanges();

                    TempData["ToastMessage"] = "Empleado inactivado correctamente.";
                }

                // Vuelve al listado filtrando solo activos: el inactivado ya no se mostrará
                return RedirectToAction("DetallesEmpleados", new { incluirInactivos = false });
            }
            catch (Exception ex)
            {
                TempData["ToastError"] = "No fue posible inactivar: " + ex.Message;
                return RedirectToAction("ConfirmarEliminarEmpleados", new { id });
            }
        }

        // ACTIVAR (para reingresarlo al staff)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActivarEmpleado(int id)
        {
            if (!IsLogged()) return GoLogin();

            var empleado = db.Empleados.Find(id);
            if (empleado == null) return HttpNotFound();

            try
            {
                if (empleado.activo == true)
                {
                    TempData["ToastMessage"] = "Este empleado ya estaba activo.";
                }
                else
                {
                    empleado.activo = true;
                    db.Entry(empleado).State = EntityState.Modified;
                    db.SaveChanges();

                    TempData["ToastMessage"] = "Empleado activado correctamente.";
                }

                // Muestra inactivos para que lo veas reactivado si lo necesitas
                return RedirectToAction("DetallesEmpleados", new { incluirInactivos = true });
            }
            catch (Exception ex)
            {
                TempData["ToastError"] = "No fue posible activar: " + ex.Message;
                return RedirectToAction("DetallesEmpleados", new { incluirInactivos = true });
            }
        }

        // Limpia recursos
        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
