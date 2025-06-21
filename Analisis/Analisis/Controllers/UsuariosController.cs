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
            return View();
        }

        // GET: Usuarios/ListUser
        public ActionResult ListUser()
        {
            var usuarios = db.Usuarios.Include("Roles").ToList();
            return View(usuarios);
        }

        // GET: Usuarios/CreateUser
        public ActionResult CreateUser()
        {
            ViewBag.RolId = new SelectList(db.Roles, "Id", "Nombre");
            return View();
        }

        // POST: Usuarios/CreateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(Usuarios usuario)
        {
            if (ModelState.IsValid)
            {
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
            if (ModelState.IsValid)
            {
                db.Entry(usuario).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ListUser");
            }

            ViewBag.RolId = new SelectList(db.Roles, "Id", "Nombre", usuario.RolId);
            return View(usuario);
        }

        // GET: Usuarios/DeleteUser/5
        public ActionResult DeleteUser(int id)
        {
            var usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/ConfirmDeleteUser
        [HttpPost, ActionName("ConfirmDeleteUser")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmDeleteUser(int id)
        {
            var usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            db.Usuarios.Remove(usuario);
            db.SaveChanges();
            return RedirectToAction("ListUser");
        }
    }
}
