using System.Web.Mvc;

namespace QuiroFeet.Controllers
{
    public class ReportesController : Controller
    {
        // Acción principal: Mostrar la vista de reportes
        public ActionResult Index()
        {
            return View("~/Views/Reportes/Reportes.cshtml");
        }

        // Acción para generar el reporte de ventas
        public ActionResult ReporteVentas()
        {
            return View("~/Views/Reportes/ReporteVentas.cshtml");
        }

        // Acción para ver los productos más vendidos
        public ActionResult ProductosMasVendidos()
        {
            return View("~/Views/Reportes/ProductosMasVendidos.cshtml");
        }

        // Acción para ver el estado del stock
        public ActionResult EstadoStock()
        {
            return View("~/Views/Reportes/EstadoStock.cshtml");
        }

        // Acción para ver el historial de compras por cliente
        public ActionResult HistorialComprasCliente()
        {
            return View("~/Views/Reportes/HistorialComprasCliente.cshtml");
        }

        // Acción para ver el historial de servicios por profesional
        public ActionResult HistorialServiciosProfesional()
        {
            return View("~/Views/Reportes/HistorialServiciosProfesional.cshtml");
        }
    }
}
