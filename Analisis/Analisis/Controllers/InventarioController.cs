using System.Web.Mvc;

namespace Analisis.Controllers
{
    public class InventarioController : Controller
    {
        // Página principal del inventario
        public ActionResult Index()
        {
            return View("~/Views/Inventario/Inventario.cshtml");
        }

        // Acción para registrar un nuevo producto
        public ActionResult RegistrarProducto()
        {
            return View();
        }

        // Acción para editar un producto existente
        public ActionResult EditarProducto(int id)
        {
            ViewBag.ProductoId = id;
            return View();
        }

        // Acción para registrar ingreso de stock
        public ActionResult RegistrarIngreso()
        {
            return View();
        }

        // Acción para registrar salida de stock
        public ActionResult RegistrarSalida()
        {
            return View();
        }

        // Acción para ajustar stock manualmente
        public ActionResult AjustarStock()
        {
            return View();
        }

        // Acción para ver el historial de movimientos
        public ActionResult HistorialMovimientos()
        {
            return View();
        }

        // Acción para mostrar alertas de stock mínimo
        public ActionResult AlertasStock()
        {
            return View();
        }
    }
}
