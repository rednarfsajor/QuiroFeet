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
            return View();
        }

        // LISTAR Empleados
        public ActionResult DetallesEmpleado()
        {
            var Empleados = db.Empleados.ToList();
            return View(Empleados);
        }

        // CREAR Empleado - GET
        [HttpGet]
        public ActionResult RegistrarEmpleado()
        {
            return View();
        }

        // CREAR Empleado - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarEmpleado(Empleados nuevoEmpleado)
        {
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
                return RedirectToAction("DetallesEmpleado");
            }

            return View(nuevoEmpleado);
        }

        // EDITAR Empleado - GET
        [HttpGet]
        public ActionResult EditarEmpleado(int id)
        {
            var profesional = db.Empleados.Find(id);
            if (profesional == null)
                return HttpNotFound();

            return View(profesional);
        }

        // EDITAR CLIENTE - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarEmpleado(Empleados profesionalEditado)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(profesionalEditado).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("DetallesEmpleado");
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            // Esto imprime en la consola de salida de Visual Studio
                            System.Diagnostics.Debug.WriteLine($"Propiedad: {validationError.PropertyName} - Error: {validationError.ErrorMessage}");

                            // También puedes agregar el error al ModelState para mostrarlo en la vista
                            ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                }
            }

            // Si llega aquí, hubo un error
            return View(profesionalEditado);
        }


        // ELIMINAR Empleado - GET
        [HttpGet]
        public ActionResult DeleteEmpleado(int id)
        {
            var profesional = db.Empleados.Find(id);
            if (profesional == null)
                return HttpNotFound();

            return View(profesional);
        }

        // ELIMINAR Empleado - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteEmpleado(int id, FormCollection form)
        {
            var profesional = db.Empleados.Find(id);
            if (profesional == null)
                return HttpNotFound();

            db.Empleados.Remove(profesional);
            db.SaveChanges();
            return RedirectToAction("DetallesEmpleado");
        }
    }
}
