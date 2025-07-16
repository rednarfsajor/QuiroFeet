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
        private QuiroFeetEntities2 db = new QuiroFeetEntities2();

        // GET: Proveedores (opcional, se puede usar como dashboard)
        public ActionResult Proveedores()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        // LISTAR Proveedores
        public ActionResult DetallesProveedor()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var Proveedores = db.Proveedores.ToList();
            return View(Proveedores);
        }

        // CREAR Proveedor - GET
        [HttpGet]
        public ActionResult RegistrarProveedor()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        // CREAR Proveedor - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarProveedor(Proveedores nuevoProveedor)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

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
        public ActionResult EditarProveedor(int? id)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            if (id == null)
                return RedirectToAction("DetallesProveedor");

            var proveedor = db.Proveedores.Find(id);
            if (proveedor == null)
                return HttpNotFound();

            return View(proveedor);
        }

        // EDITAR Proveedor - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarProveedor(Proveedores proveedorEditado)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

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
                            System.Diagnostics.Debug.WriteLine($"Propiedad: {validationError.PropertyName} - Error: {validationError.ErrorMessage}");
                            ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                }
            }

            return View(proveedorEditado);
        }

        // ELIMINAR Proveedor - POST (directo)
        [HttpPost]
        public JsonResult EliminarProveedor(int id)
        {
            if (Session["UsuarioId"] == null)
            {
                return Json(new { success = false, message = "Sesión no válida. Inicie sesión nuevamente." });
            }

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
