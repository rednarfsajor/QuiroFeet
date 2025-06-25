using System.Web.Mvc;

namespace QuiroFeet.Controllers
{
    public class ReportesController : Controller
    {
        // Acción principal: Mostrar la vista de reportes
        public ActionResult Reportes()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        // Acción para generar el reporte de ventas
        public ActionResult ReporteVentas()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        // Acción para ver los productos más vendidos
        public ActionResult ProductosMasVendidos()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        // Acción para ver el estado del stock
        public ActionResult EstadoStock()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        // Acción para ver el historial de compras por cliente
        public ActionResult HistorialComprasCliente()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        // Acción para ver el historial de servicios por profesional
        public ActionResult HistorialServiciosProfesional()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }
    }
}
