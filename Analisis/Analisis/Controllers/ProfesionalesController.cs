using System.Web.Mvc;

namespace Analisis.Controllers
{
    public class ProfesionalesController : Controller
    {
        // Página principal de empleados
        public ActionResult Index()
        {
            return View("~/Views/Profesionales/Profesionales.cshtml");
        }

        // Acción registrar empleado
        public ActionResult RegistrarEmpleado()
        {
            return View();
        }

        // Acción para editar empleado
        public ActionResult EditarEmpleado()
        {
            return View();
        }

        // Acción para ver detalles de empleado
        public ActionResult DetallesEmpleado()
        {
            return View();
        }

        // Acción para ver disponibilidad de empleado
        public ActionResult GestionarDisponibilidad()
        {
            return View();
        }

        // Acción para ver agenda del empleado
        public ActionResult ConsultarAgenda()
        {
            return View();
        }
    }
}
