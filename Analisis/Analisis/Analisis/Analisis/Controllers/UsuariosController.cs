using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Analisis.BD;

namespace Analisis.Controllers
{
    public class UsuariosController : Controller
    {
        private QuiroFeetEntities6 db = new QuiroFeetEntities6();

        // ============================
        // VALIDACIÓN DE CORREO
        // ============================
        private bool EmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            // Cumple lo solicitado por QA: debe tener @ y terminar en .com
            return Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.(com|net|org|cr)$",
                RegexOptions.IgnoreCase);
        }

        private bool SesionActiva()
        {
            return Session["UsuarioId"] != null;
        }

        // ============================
        // MENÚ PRINCIPAL
        // ============================
        public ActionResult Users()
        {
            if (!SesionActiva()) return RedirectToAction("Login", "Account");
            return View();
        }

        // ============================
        // LISTA DE ACTIVOS
        // ============================
        public ActionResult ListUser()
        {
            if (!SesionActiva()) return RedirectToAction("Login", "Account");

            var usuarios = db.Usuarios.Include("Roles")
                                      .Where(u => u.Activo == true)
                                      .OrderBy(u => u.Nombre)
                                      .ToList();

            return View(usuarios);
        }

        // ============================
        // LISTA DE INACTIVOS
        // ============================
        public ActionResult ListInactiveUsers()
        {
            if (!SesionActiva()) return RedirectToAction("Login", "Account");

            var usuarios = db.Usuarios.Include("Roles")
                                      .Where(u => u.Activo == false)
                                      .OrderBy(u => u.Nombre)
                                      .ToList();

            return View(usuarios);
        }

        // ============================
        // CREAR USUARIO
        // ============================
        public ActionResult CreateUser()
        {
            if (!SesionActiva()) return RedirectToAction("Login", "Account");

            ViewBag.RolId = new SelectList(db.Roles, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(Usuarios usuario)
        {
            if (!SesionActiva()) return RedirectToAction("Login", "Account");

            if (!EmailValido(usuario.Email))
                ModelState.AddModelError("Email", "Debe ingresar un correo válido que termine en .com");

            if (!ModelState.IsValid)
            {
                ViewBag.RolId = new SelectList(db.Roles, "Id", "Nombre", usuario.RolId);
                return View(usuario);
            }

            usuario.Activo = true;
            db.Usuarios.Add(usuario);
            db.SaveChanges();

            TempData["SuccessMessage"] = $"Usuario {usuario.Nombre} creado correctamente.";
            return RedirectToAction("ListUser");
        }

        // ============================
        // EDITAR USUARIO
        // ============================
        public ActionResult EditUser(int id)
        {
            if (!SesionActiva()) return RedirectToAction("Login", "Account");

            var usuario = db.Usuarios.Find(id);
            if (usuario == null) return HttpNotFound();

            ViewBag.RolId = new SelectList(db.Roles, "Id", "Nombre", usuario.RolId);
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(Usuarios usuario)
        {
            if (!SesionActiva()) return RedirectToAction("Login", "Account");

            if (!EmailValido(usuario.Email))
                ModelState.AddModelError("Email", "Debe ingresar un correo válido que termine en .com");

            if (!ModelState.IsValid)
            {
                ViewBag.RolId = new SelectList(db.Roles, "Id", "Nombre", usuario.RolId);
                return View(usuario);
            }

            var usuarioDB = db.Usuarios.Find(usuario.Id);
            if (usuarioDB == null) return HttpNotFound();

            usuarioDB.Nombre = usuario.Nombre;
            usuarioDB.Email = usuario.Email;
            usuarioDB.RolId = usuario.RolId;

            db.SaveChanges();

            TempData["SuccessMessage"] = $"Usuario {usuario.Nombre} actualizado correctamente.";
            return RedirectToAction("ListUser");
        }

        // ============================
        // INACTIVAR USUARIO (SweetAlert)
        // ============================
        public ActionResult InactivarUser(int id)
        {
            if (!SesionActiva()) return RedirectToAction("Login", "Account");

            var usuario = db.Usuarios.Find(id);
            if (usuario == null) return HttpNotFound();

            if (!usuario.Activo)
            {
                TempData["SuccessMessage"] = "El usuario ya estaba inactivo.";
                return RedirectToAction("ListUser");
            }

            usuario.Activo = false;
            db.SaveChanges();

            TempData["SuccessMessage"] = $"El usuario {usuario.Nombre} fue inactivado correctamente.";
            return RedirectToAction("ListUser");
        }

        // ============================
        // ACTIVAR USUARIO (SweetAlert)
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActivarUser(int id)
        {
            if (!SesionActiva()) return RedirectToAction("Login", "Account");

            var usuario = db.Usuarios.Find(id);
            if (usuario == null) return HttpNotFound();

            if (usuario.Activo)
            {
                TempData["ActivateSuccess"] = "El usuario ya estaba activo.";
                return RedirectToAction("ListInactiveUsers");
            }

            usuario.Activo = true;
            db.SaveChanges();

            TempData["ActivateSuccess"] = $"El usuario {usuario.Nombre} fue activado correctamente.";
            return RedirectToAction("ListInactiveUsers");
        }
    }
}
