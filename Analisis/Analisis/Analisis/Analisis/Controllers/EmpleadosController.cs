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

        // ===========================
        // 1. MENÚ PRINCIPAL
        // ===========================
        public ActionResult Empleados()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        // ===========================
        // 2. LISTAR EMPLEADOS
        // ===========================
        public ActionResult DetallesEmpleados()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var empleados = db.Empleados.ToList();
            ViewBag.SuccessMessage = TempData["SuccessMessage"];

            return View(empleados);
        }

        // ===========================
        // 3. REGISTRAR EMPLEADO
        // ===========================
        [HttpGet]
        public ActionResult RegistrarEmpleados()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarEmpleados(Empleados nuevoEmpleado)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            if (db.Empleados.Any(e => e.id == nuevoEmpleado.id))
                ModelState.AddModelError("id", "Ya existe un empleado registrado con esta cédula.");

            if (db.Empleados.Any(e => e.correo == nuevoEmpleado.correo))
                ModelState.AddModelError("correo", "Ya existe un empleado registrado con este correo.");

            if (ModelState.IsValid)
            {
                // *** Campo string => usar textos ***
                nuevoEmpleado.activo = true;

                db.Empleados.Add(nuevoEmpleado);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Empleado registrado exitosamente.";
                return RedirectToAction("DetallesEmpleados");
            }

            return View(nuevoEmpleado);
        }

        // ===========================
        // 4. EDITAR EMPLEADO
        // ===========================
        [HttpGet]
        public ActionResult EditarEmpleados(int id)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var empleado = db.Empleados.Find(id);
            if (empleado == null)
                return HttpNotFound();

            return View(empleado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarEmpleados(Empleados empleadoEditado)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            if (db.Empleados.Any(e => e.id != empleadoEditado.id &&
                                      e.correo == empleadoEditado.correo))
            {
                ModelState.AddModelError("correo", "Ya existe otro empleado registrado con este correo.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(empleadoEditado).State = EntityState.Modified;
                    db.SaveChanges();

                    TempData["SuccessMessage"] = "Empleado actualizado correctamente.";
                    return RedirectToAction("DetallesEmpleados");
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var error in ex.EntityValidationErrors
                                            .SelectMany(e => e.ValidationErrors))
                    {
                        ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                    }
                }
            }

            return View(empleadoEditado);
        }

        // ===========================
        // 5. DESACTIVAR EMPLEADO
        // ===========================
        [HttpGet]
        public ActionResult DesactivarEmpleados(int id)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var empleado = db.Empleados.Find(id);
            if (empleado == null)
                return HttpNotFound();

            return View(empleado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DesactivarEmpleados")]
        public ActionResult DesactivarEmpleadosPost(int id)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var empleado = db.Empleados.Find(id);
            if (empleado == null)
                return HttpNotFound();

            // *** Campo string => usar texto ***
            empleado.activo = false;

            db.Entry(empleado).State = EntityState.Modified;
            db.SaveChanges();

            TempData["SuccessMessage"] = "Empleado desactivado correctamente.";
            return RedirectToAction("DetallesEmpleados");
        }

        // ===========================
        // 6. ELIMINAR EMPLEADO
        // ===========================
        [HttpGet]
        public ActionResult DeleteEmpleados(int id)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var empleado = db.Empleados.Find(id);
            if (empleado == null)
                return HttpNotFound();

            return View(empleado);
        }

        [HttpPost]
        public ActionResult DeleteEmpleados(int id, FormCollection form)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var empleado = db.Empleados.Find(id);
            if (empleado == null)
                return HttpNotFound();

            db.Empleados.Remove(empleado);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Empleado eliminado correctamente.";
            return RedirectToAction("DetallesEmpleados");
        }
    }
}
