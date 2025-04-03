using System.Web.Mvc;

namespace QuiroFeet.Controllers
{
    public class ReportesController : Controller
    {
        // Acción principal: Mostrar la vista de reportes
        public ActionResult Reportes()
        {
            return View();
        }

        // Acción para generar el reporte de ventas
        public ActionResult ReporteVentas()
        {
            return View();
        }

        // Acción para ver los productos más vendidos
        public ActionResult ProductosMasVendidos()
        {
            return View();
        }

        // Acción para ver el estado del stock
        public ActionResult EstadoStock()
        {
            return View();
        }

        // Acción para ver el historial de compras por cliente
        public ActionResult HistorialComprasCliente()
        {
            return View();
        }

        // Acción para ver el historial de servicios por profesional
        public ActionResult HistorialServiciosProfesional()
        {
            return View();
        }
    }
}
