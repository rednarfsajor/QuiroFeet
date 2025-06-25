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
        private QuiroFeetEntities1 db = new QuiroFeetEntities1();

        // GET: Empleados (opcional, se puede usar como dashboard)
        public ActionResult Empleados()
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // LISTAR Empleados
        public ActionResult DetallesEmpleados()
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var Empleados = db.Empleados.ToList();
            return View(Empleados);
        }

        // CREAR Empleado - GET
        [HttpGet]
        public ActionResult RegistrarEmpleados()
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // CREAR Empleado - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarEmpleados(Empleados nuevoEmpleado)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Verifica si el correo ya existe
            bool correoExistente = db.Empleados.Any(p => p.correo == nuevoEmpleado.correo);

            if (correoExistente)
            {
                ModelState.AddModelError("Correo", "Ya existe un profesional con este correo.");
            }

            if (ModelState.IsValid)
            {
                db.Empleados.Add(nuevoEmpleado);
                db.SaveChanges();
                return RedirectToAction("DetallesEmpleados");
            }

            return View(nuevoEmpleado);
        }

        // EDITAR Empleado - GET
        [HttpGet]
        public ActionResult EditarEmpleados(int id)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var profesional = db.Empleados.Find(id);
            if (profesional == null)
                return HttpNotFound();

            return View(profesional);
        }

        // EDITAR Empleado - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarEmpleados(Empleados profesionalEditado)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(profesionalEditado).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("DetallesEmpleados");
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Diagnostics.Debug.WriteLine($"Propiedad: {validationError.PropertyName} - Error: {validationError.ErrorMessage}");
                            ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                }
            }

            return View(profesionalEditado);
        }

        // ELIMINAR Empleado - GET
        [HttpGet]
        public ActionResult DeleteEmpleados(int id)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var profesional = db.Empleados.Find(id);
            if (profesional == null)
                return HttpNotFound();

            return View(profesional);
        }

        // ELIMINAR Empleado - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteEmpleados(int id, FormCollection form)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var profesional = db.Empleados.Find(id);
            if (profesional == null)
                return HttpNotFound();

            db.Empleados.Remove(profesional);
            db.SaveChanges();
            return RedirectToAction("DetallesEmpleados");
        }
    }
}
