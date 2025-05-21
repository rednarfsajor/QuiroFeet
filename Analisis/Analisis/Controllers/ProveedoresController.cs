using System.Web.Mvc;

namespace Analisis.Controllers
{
    public class ProveedoresController : Controller
    {
        // Página principal de empleados
        public ActionResult Index()
        {
            return View("~/Views/Proveedores/Proveedores.cshtml");
        }

        // Acción registrar empleado
        public ActionResult RegistrarProveedor()
        {
            return View();
        }

        // Acción para editar empleado
        public ActionResult EditarProveedor()
        {
            return View();
        }

        // Acción para ver detalles de empleado
        public ActionResult DetallesProveedor()
        {
            return View();
        }

    }
}
