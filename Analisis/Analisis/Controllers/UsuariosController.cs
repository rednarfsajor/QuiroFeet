using System.Linq;
using System.Net;
using System.Web.Mvc;
using Analisis.BD;

namespace Analisis.Controllers
{
    public class UsuariosController : Controller
    {
        private QuiroFeetEntities1 db = new QuiroFeetEntities1();

        // GET: Usuarios/Users (MENÚ)
        public ActionResult Users()
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // GET: Usuarios/ListUser (Activos)
        public ActionResult ListUser()
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var usuarios = db.Usuarios.Include("Roles")
                                      .Where(u => u.Activo == true)
                                      .ToList();

            return View(usuarios);
        }

        // GET: Usuarios/ListInactiveUsers
        public ActionResult ListInactiveUsers()
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var inactivos = db.Usuarios.Include("Roles")
                                       .Where(u => u.Activo == false)
                                       .ToList();

            return View(inactivos);
        }

        // GET: Usuarios/CreateUser
        public ActionResult CreateUser()
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.RolId = new SelectList(db.Roles, "Id", "Nombre");
            return View();
        }

        // POST: Usuarios/CreateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(Usuarios usuario)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                usuario.Activo = true; // Siempre se crean activos
                db.Usuarios.Add(usuario);
                db.SaveChanges();
                return RedirectToAction("ListUser");
            }

            ViewBag.RolId = new SelectList(db.Roles, "Id", "Nombre", usuario.RolId);
            return View(usuario);
        }

        // GET: Usuarios/EditUser/5
        public ActionResult EditUser(int id)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            ViewBag.RolId = new SelectList(db.Roles, "Id", "Nombre", usuario.RolId);
            return View(usuario);
        }

        // POST: Usuarios/EditUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(Usuarios usuario)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                var usuarioDB = db.Usuarios.Find(usuario.Id);
                if (usuarioDB == null)
                {
                    return HttpNotFound();
                }

                // Solo actualizamos los campos que queremos modificar
                usuarioDB.Nombre = usuario.Nombre;
                usuarioDB.Email = usuario.Email;
                usuarioDB.RolId = usuario.RolId;

                // No se toca Password ni Activo
                db.SaveChanges();

                TempData["SuccessMessage"] = $"El usuario {usuario.Nombre} ha sido actualizado correctamente.";
                return RedirectToAction("ListUser");
            }

            ViewBag.RolId = new SelectList(db.Roles, "Id", "Nombre", usuario.RolId);
            return View(usuario);
        }

        // GET: Usuarios/InactivarUser/5
        public ActionResult InactivarUser(int id)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            usuario.Activo = false;
            db.SaveChanges();

            TempData["SuccessMessage"] = $"El usuario {usuario.Nombre} ha sido inactivado correctamente.";
            return RedirectToAction("ListUser");
        }

        // POST: Usuarios/ActivarUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActivarUser(int id)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            usuario.Activo = true;
            db.SaveChanges();

            TempData["ActivateSuccess"] = $"El usuario {usuario.Nombre} ha sido activado correctamente.";
            return RedirectToAction("ListInactiveUsers");
        }
    }
}
