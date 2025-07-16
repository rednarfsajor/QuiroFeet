using System.Linq;
using System.Web.Mvc;
using Analisis.BD;

namespace Analisis.Controllers
{
    public class AccountController : Controller
    {
        private QuiroFeetEntities5 db = new QuiroFeetEntities5();

        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Mensaje = "Debe ingresar correo y contraseña.";
                return View();
            }

            var user = db.Usuarios
                         .Where(u => u.Email == email && u.Password == password)
                         .FirstOrDefault();

            if (user != null)
            {
                if (user.Activo == false)
                {
                    ViewBag.Mensaje = "El usuario está inactivo. Contacte al administrador.";
                    return View();
                }

                // Usuario Activo
                Session["UsuarioId"] = user.Id;
                Session["UsuarioNombre"] = user.Nombre;
                Session["RolNombre"] = user.Roles.Nombre;

                TempData["LoginSuccess"] = $"¡Hola!, {user.Nombre}";

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Mensaje = "Email o contraseña incorrectos.";
                return View();
            }
        }

        // GET: Account/Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}
