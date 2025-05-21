using System.Web.Mvc;

namespace QuiroFeet.Controllers
{
    public class InventarioController : Controller
    {
        // Acción principal: Mostrar la vista de inventario
        public ActionResult Inventario()
        {
            return View();
        }

        // Acción para registrar un nuevo producto
        public ActionResult RegistrarProducto()
        {
            return View();
        }

        // Acción para editar un producto
        public ActionResult EditarProducto(int id)
        {
            // Puedes pasar un "id" a la vista si necesitas algo específico
            ViewBag.ProductoId = id;
            return View();
        }

        // Acción para inactivar un producto
        public ActionResult InactivarProducto(int id)
        {
            // Lógica para inactivar el producto (solo vista estática)
            ViewBag.ProductoId = id;
            return View();
        }

        // Acción para registrar ingreso de productos
        public ActionResult RegistrarIngreso()
        {
            return View();
        }

        // Acción para registrar salida de productos
        public ActionResult RegistrarSalida()
        {
            return View();
        }

        // Acción para ver el historial de movimientos
        public ActionResult HistorialMovimientos()
        {
            return View();
        }

        // Acción para ver alertas de stock mínimo
        public ActionResult AlertasStock()
        {
            return View();
        }

        // Acción para crear una orden de compra
        public ActionResult CrearOrdenCompra()
        {
            return View();
        }
    }
}
