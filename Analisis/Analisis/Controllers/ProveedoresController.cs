using System;
using System.Linq;
using System.Web.Mvc;
using Analisis.BD;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace Analisis.Controllers
{
    public class ProveedoresController : Controller
    {
        private QuiroFeetEntities1 db = new QuiroFeetEntities1();
        // GET: Proveedores (opcional, se puede usar como dashboard)
        public ActionResult Proveedores()
        {
            return View();
            
        }
        
        // LISTAR Proveedores
        public ActionResult DetallesProveedor()
        {
            var Proveedores = db.Proveedores.ToList();
            return View(Proveedores);
        }

        // CREAR Proveedor - GET
        [HttpGet]
        public ActionResult RegistrarProveedor()
        {
            return View();
        }

        // CREAR Proveedor - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarProveedor(Proveedores nuevoProveedor)
        {
            // Verifica si el correo ya existe
            bool correoExistente = db.Proveedores.Any(p => p.correo == nuevoProveedor.correo);

            if (correoExistente)
            {
                ModelState.AddModelError("Correo", "Ya existe un proveedor con este correo.");
            }

            if (ModelState.IsValid)
            {
                db.Proveedores.Add(nuevoProveedor);
                db.SaveChanges();
                return RedirectToAction("DetallesProveedor");
            }

            return View(nuevoProveedor);
        }

        // EDITAR Proveedor - GET
        [HttpGet]
        public ActionResult EditarProveedor(int id)
        {
            var proveedor = db.Proveedores.Find(id);
            if (proveedor == null)
                return HttpNotFound();

            return View(proveedor);
        }

        // EDITAR CLIENTE - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarProveedor(Proveedores proveedorEditado)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(proveedorEditado).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("DetallesProveedor");
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
            return View(proveedorEditado);
        }




        // ELIMINAR Proveedor - POST (directo)
        [HttpPost]
        public JsonResult EliminarProveedor(int id)
        {
            var proveedor = db.Proveedores.Find(id);
            if (proveedor == null)
            {
                return Json(new { success = false, message = "Proveedor no encontrado." });
            }

            db.Proveedores.Remove(proveedor);
            db.SaveChanges();

            return Json(new { success = true, message = "Proveedor eliminado exitosamente." });
        }


    }
}
