using System.Linq;
using System.Web.Mvc;
using Analisis.BD;

namespace Analisis.Controllers
{
    public class AccountController : Controller
    {
        private QuiroFeetEntities1 db = new QuiroFeetEntities1();

        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            var user = db.Usuarios.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                Session["UsuarioId"] = user.Id;
                Session["UsuarioNombre"] = user.Nombre;
                Session["RolNombre"] = user.Roles.Nombre;

                // Redireccionar según el rol
                if (Session["RolNombre"].ToString() == "Administrador")
                {
                    return RedirectToAction("ServiceIndex", "Servicios");
                }
                else if (Session["RolNombre"].ToString() == "Recepcionista")
                {
                    return RedirectToAction("Calendar", "Citas");
                }
                else if (Session["RolNombre"].ToString() == "Profesional")
                {
                    return RedirectToAction("Calendar", "Citas");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
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
