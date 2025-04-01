using System.Web.Mvc;

namespace Analisis.Controllers
{
    public class ReportesController : Controller
    {
        // Página principal de reportes
        public ActionResult Index()
        {
            return View("~/Views/Reportes/Reportes.cshtml");
        }

        // Acción para generar reportes de ventas
        public ActionResult ReporteVentas()
        {
            return View();
        }

        // Acción para ver productos más vendidos
        public ActionResult ProductosMasVendidos()
        {
            return View();
        }

        // Acción para ver el estado del stock
        public ActionResult EstadoStock()
        {
            return View();
        }

        // Acción para ver historial de compras por cliente
        public ActionResult HistorialComprasCliente()
        {
            return View();
        }

        // Acción para ver historial de servicios por profesional
        public ActionResult HistorialServiciosProfesional()
        {
            return View();
        }
    }
}
