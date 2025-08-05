using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Analisis.BD;
using System.Globalization; // Asegúrate que este namespace es correcto para tu EDMX

namespace Analisis.Controllers
{
    public class ReportesController : Controller
    {
        private QuiroFeetEntities6 db = new QuiroFeetEntities6();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Estadisticas()
        {
            var ventas = db.Ventas.ToList();
            var citas = db.Citas.ToList();

            // Total ingresos
            var totalIngresos = ventas.Sum(v => v.monto);

            // Venta más alta y más baja
            var ventaMasAlta = ventas.OrderByDescending(v => v.monto).FirstOrDefault();
            var ventaMasBaja = ventas.OrderBy(v => v.monto).FirstOrDefault();

            // Total de ventas por mes
            var ventasPorMes = ventas
            .Where(v => v.fecha.HasValue)
            .GroupBy(v => new { v.fecha.Value.Year, v.fecha.Value.Month })
            .Select(g => new
            {
                Mes = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy", new CultureInfo("es-ES")),
                Total = g.Sum(v => v.monto)
            })
            .ToList(); // <- Muy importante

            ViewBag.VentasPorMes = ventasPorMes;



            // Citas por profesional
            var citasPorProfesional = citas
                .GroupBy(c => c.id_profesional)
                .Select(g => new
                {
                    Profesional = g.Key,
                    Total = g.Count()
                })
                .ToList();

            // Citas por servicio
            var citasPorServicio = citas
                .GroupBy(c => c.id_servicio)
                .Select(g => new
                {
                    Servicio = g.Key,
                    Total = g.Count()
                })
                .ToList();

            // Clientes con más citas
            var clientesConMasCitas = citas
                .GroupBy(c => c.id_cliente)
                .Select(g => new
                {
                    Cliente = g.Key,
                    Total = g.Count()
                })
                .OrderByDescending(x => x.Total)
                .ToList();

            // Citas por día
            var citasPorDia = citas
                .GroupBy(c => c.fecha.Date)
                .Select(g => new
                {
                    Dia = g.Key.ToString("dd/MM/yyyy"),
                    Total = g.Count()
                })
                .ToList();

            // Citas por mes
            var citasPorMes = citas
                .GroupBy(c => new { c.fecha.Year, c.fecha.Month })
                .Select(g => new
                {
                    Mes = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy", new CultureInfo("es-ES")),
                    Total = g.Count()
                })
                .ToList();

            // Pasar a la vista usando ViewBag o un ViewModel
            ViewBag.TotalIngresos = totalIngresos;
            ViewBag.VentaMasAlta = ventaMasAlta;
            ViewBag.VentaMasBaja = ventaMasBaja;
            ViewBag.VentasPorMes = ventasPorMes;
            ViewBag.CitasPorProfesional = citasPorProfesional;
            ViewBag.CitasPorServicio = citasPorServicio;
            ViewBag.ClientesConMasCitas = clientesConMasCitas;
            ViewBag.CitasPorDia = citasPorDia;
            ViewBag.CitasPorMes = citasPorMes;

            return View();
        }


        // ---------------------
        // Órdenes de Compra PDF
        // ---------------------
        public ActionResult ExportarOrdenesPdf()
        {
            var ordenes = db.Ordenes.ToList();

            MemoryStream ms = new MemoryStream();
            Document doc = new Document(PageSize.A4, 25, 25, 30, 30);
            PdfWriter.GetInstance(doc, ms);
            doc.Open();

            var titulo = new Paragraph("Reporte Total de Órdenes de Compra", new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD));
            titulo.Alignment = Element.ALIGN_CENTER;
            titulo.SpacingAfter = 20f;
            doc.Add(titulo);

            PdfPTable tabla = new PdfPTable(4) { WidthPercentage = 100 };
            tabla.SetWidths(new float[] { 10f, 40f, 25f, 25f });

            BaseColor bgColor = new BaseColor(35, 68, 73);
            Font fontHeader = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, BaseColor.WHITE);
            Font fontBody = new Font(Font.FontFamily.HELVETICA, 11, Font.NORMAL);

            string[] headers = { "ID", "Productos", "Fecha Creación", "Fecha Recepción" };
            foreach (var header in headers)
            {
                PdfPCell cell = new PdfPCell(new Phrase(header, fontHeader))
                {
                    BackgroundColor = bgColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 5
                };
                tabla.AddCell(cell);
            }

            foreach (var o in ordenes)
            {
                tabla.AddCell(new Phrase(o.id.ToString(), fontBody));
                tabla.AddCell(new Phrase(o.productos ?? "", fontBody));
                tabla.AddCell(new Phrase(o.fecha_creacion.ToShortDateString(), fontBody));
            }

            doc.Add(tabla);
            doc.Close();

            byte[] byteInfo = ms.ToArray();
            ms.Close();

            return File(byteInfo, "application/pdf", "ReporteOrdenes.pdf");
        }

        // ---------------------
        // Proveedores PDF
        // ---------------------
        public ActionResult ExportarProveedoresPdf()
        {
            var proveedores = db.Proveedores.ToList();

            MemoryStream ms = new MemoryStream();
            Document doc = new Document(PageSize.A4, 25, 25, 30, 30);
            PdfWriter.GetInstance(doc, ms);
            doc.Open();

            var titulo = new Paragraph("Reporte de Proveedores Registrados", new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD));
            titulo.Alignment = Element.ALIGN_CENTER;
            titulo.SpacingAfter = 20f;
            doc.Add(titulo);

            PdfPTable tabla = new PdfPTable(5) { WidthPercentage = 100 };
            tabla.SetWidths(new float[] { 10f, 25f, 25f, 25f, 15f });

            BaseColor bgColor = new BaseColor(35, 68, 73);
            Font fontHeader = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, BaseColor.WHITE);
            Font fontBody = new Font(Font.FontFamily.HELVETICA, 11, Font.NORMAL);

            string[] headers = { "ID", "Nombre", "Contacto", "Correo", "Número" };
            foreach (var header in headers)
            {
                PdfPCell cell = new PdfPCell(new Phrase(header, fontHeader))
                {
                    BackgroundColor = bgColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 5
                };
                tabla.AddCell(cell);
            }

            foreach (var p in proveedores)
            {
                tabla.AddCell(new Phrase(p.id.ToString(), fontBody));
                tabla.AddCell(new Phrase(p.nombre ?? "", fontBody));
                tabla.AddCell(new Phrase(p.contacto ?? "", fontBody));
                tabla.AddCell(new Phrase(p.correo ?? "", fontBody));
                tabla.AddCell(new Phrase(p.numero ?? "", fontBody));
            }

            doc.Add(tabla);
            doc.Close();

            byte[] byteInfo = ms.ToArray();
            ms.Close();

            return File(byteInfo, "application/pdf", "ReporteProveedores.pdf");
        }

        // ---------------------
        // Usuarios PDF
        // ---------------------
        public ActionResult ExportarUsuariosPdf()
        {
            var usuarios = db.Usuarios.ToList();

            MemoryStream ms = new MemoryStream();
            Document doc = new Document(PageSize.A4, 25, 25, 30, 30);
            PdfWriter.GetInstance(doc, ms);
            doc.Open();

            var titulo = new Paragraph("Reporte de Usuarios", new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD));
            titulo.Alignment = Element.ALIGN_CENTER;
            titulo.SpacingAfter = 20f;
            doc.Add(titulo);

            PdfPTable tabla = new PdfPTable(4) { WidthPercentage = 100 };
            tabla.SetWidths(new float[] { 10f, 35f, 40f, 15f });

            BaseColor bgColor = new BaseColor(35, 68, 73);
            Font fontHeader = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, BaseColor.WHITE);
            Font fontBody = new Font(Font.FontFamily.HELVETICA, 11, Font.NORMAL);

            string[] headers = { "ID", "Nombre", "Email", "Activo" };
            foreach (var header in headers)
            {
                PdfPCell cell = new PdfPCell(new Phrase(header, fontHeader))
                {
                    BackgroundColor = bgColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 5
                };
                tabla.AddCell(cell);
            }

            foreach (var u in usuarios)
            {
                tabla.AddCell(new Phrase(u.Id.ToString(), fontBody));
                tabla.AddCell(new Phrase(u.Nombre ?? "", fontBody));
                tabla.AddCell(new Phrase(u.Email ?? "", fontBody));
                tabla.AddCell(new Phrase(u.Activo ? "Sí" : "No", fontBody));
            }

            doc.Add(tabla);
            doc.Close();

            byte[] byteInfo = ms.ToArray();
            ms.Close();

            return File(byteInfo, "application/pdf", "ReporteUsuarios.pdf");
        }

        // ---------------------
        // Inventario PDF
        // ---------------------
        public ActionResult ExportarInventarioPdf()
        {
            var inventario = db.Inventario.ToList();

            MemoryStream ms = new MemoryStream();
            Document doc = new Document(PageSize.A4, 25, 25, 30, 30);
            PdfWriter.GetInstance(doc, ms);
            doc.Open();

            var titulo = new Paragraph("Reporte de Inventario", new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD));
            titulo.Alignment = Element.ALIGN_CENTER;
            titulo.SpacingAfter = 20f;
            doc.Add(titulo);

            PdfPTable tabla = new PdfPTable(3) { WidthPercentage = 100 };
            tabla.SetWidths(new float[] { 15f, 40f, 20f });

            BaseColor bgColor = new BaseColor(35, 68, 73);
            Font fontHeader = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, BaseColor.WHITE);
            Font fontBody = new Font(Font.FontFamily.HELVETICA, 11, Font.NORMAL);

            string[] headers = { "ID", "ID Producto", "Stock" };
            foreach (var header in headers)
            {
                PdfPCell cell = new PdfPCell(new Phrase(header, fontHeader))
                {
                    BackgroundColor = bgColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 5
                };
                tabla.AddCell(cell);
            }

            foreach (var i in inventario)
            {
                tabla.AddCell(new Phrase(i.id.ToString(), fontBody));
                tabla.AddCell(new Phrase(i.id_producto.ToString(), fontBody));
                tabla.AddCell(new Phrase(i.stock.ToString(), fontBody));
            }

            doc.Add(tabla);
            doc.Close();

            byte[] byteInfo = ms.ToArray();
            ms.Close();

            return File(byteInfo, "application/pdf", "ReporteInventario.pdf");
        }

        // ---------------------
        // Clientes PDF
        // ---------------------
        public ActionResult ExportarClientesPdf()
        {
            var clientes = db.Clientes.ToList();

            MemoryStream ms = new MemoryStream();
            Document doc = new Document(PageSize.A4, 25, 25, 30, 30);
            PdfWriter.GetInstance(doc, ms);
            doc.Open();

            var titulo = new Paragraph("Reporte de Clientes", new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD));
            titulo.Alignment = Element.ALIGN_CENTER;
            titulo.SpacingAfter = 20f;
            doc.Add(titulo);

            PdfPTable tabla = new PdfPTable(4) { WidthPercentage = 100 };
            tabla.SetWidths(new float[] { 10f, 35f, 40f, 25f });

            BaseColor bgColor = new BaseColor(35, 68, 73);
            Font fontHeader = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, BaseColor.WHITE);
            Font fontBody = new Font(Font.FontFamily.HELVETICA, 11, Font.NORMAL);

            string[] headers = { "ID", "Nombre", "Correo", "Teléfono" };
            foreach (var header in headers)
            {
                PdfPCell cell = new PdfPCell(new Phrase(header, fontHeader))
                {
                    BackgroundColor = bgColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 5
                };
                tabla.AddCell(cell);
            }

            foreach (var c in clientes)
            {
                tabla.AddCell(new Phrase(c.id.ToString(), fontBody));
                tabla.AddCell(new Phrase(c.nombre ?? "", fontBody));
                tabla.AddCell(new Phrase(c.correo ?? "", fontBody));
                tabla.AddCell(new Phrase(c.telefono ?? "", fontBody));
            }

            doc.Add(tabla);
            doc.Close();

            byte[] byteInfo = ms.ToArray();
            ms.Close();

            return File(byteInfo, "application/pdf", "ReporteClientes.pdf");
        }

        // ---------------------
        // Ventas PDF
        // ---------------------
        public ActionResult ExportarVentasPdf()
        {
            var ventas = db.Ventas.ToList();

            MemoryStream ms = new MemoryStream();
            Document doc = new Document(PageSize.A4.Rotate(), 25, 25, 30, 30);
            PdfWriter.GetInstance(doc, ms);
            doc.Open();

            var titulo = new Paragraph("Reporte de Ventas", new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD));
            titulo.Alignment = Element.ALIGN_CENTER;
            titulo.SpacingAfter = 20f;
            doc.Add(titulo);

            PdfPTable tabla = new PdfPTable(6) { WidthPercentage = 100 };
            tabla.SetWidths(new float[] { 8f, 12f, 35f, 10f, 15f, 20f });

            BaseColor bgColor = new BaseColor(35, 68, 73);
            Font fontHeader = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, BaseColor.WHITE);
            Font fontBody = new Font(Font.FontFamily.HELVETICA, 11, Font.NORMAL);

            string[] headers = { "ID", "Cliente ID", "Detalle", "Monto", "Fecha", "N° Recibo" };
            foreach (var header in headers)
            {
                PdfPCell cell = new PdfPCell(new Phrase(header, fontHeader))
                {
                    BackgroundColor = bgColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 5
                };
                tabla.AddCell(cell);
            }

            foreach (var v in ventas)
            {
                tabla.AddCell(new Phrase(v.id.ToString(), fontBody));
                tabla.AddCell(new Phrase(v.id_cliente.ToString(), fontBody));
                tabla.AddCell(new Phrase(v.detalle ?? "", fontBody));
                tabla.AddCell(new Phrase(v.monto.ToString("C"), fontBody));
                tabla.AddCell(new Phrase(v.fecha?.ToString("yyyy-MM-dd") ?? "", fontBody));
                tabla.AddCell(new Phrase(v.NumeroRecibo ?? "", fontBody));
            }

            doc.Add(tabla);
            doc.Close();

            byte[] byteInfo = ms.ToArray();
            ms.Close();

            return File(byteInfo, "application/pdf", "ReporteVentas.pdf");
        }

        // ---------------------
        // Citas PDF
        // ---------------------

        public ActionResult ExportarCitasPdf()
        {
            var citas = db.Citas.ToList();

            MemoryStream ms = new MemoryStream();
            Document doc = new Document(PageSize.A4.Rotate(), 25, 25, 30, 30); // Usamos horizontal por el número de columnas
            PdfWriter.GetInstance(doc, ms);
            doc.Open();

            var titulo = new Paragraph("Reporte de Citas", new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD));
            titulo.Alignment = Element.ALIGN_CENTER;
            titulo.SpacingAfter = 20f;
            doc.Add(titulo);

            PdfPTable tabla = new PdfPTable(7) { WidthPercentage = 100 };
            tabla.SetWidths(new float[] { 6f, 10f, 16f, 12f, 14f, 15f, 12f });

            BaseColor bgColor = new BaseColor(35, 68, 73);
            Font fontHeader = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, BaseColor.WHITE);
            Font fontBody = new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL);

            string[] headers = { "ID", "Cliente", "Fecha", "Hora", "Profesional", "Estado", "Servicio" };
            foreach (var header in headers)
            {
                PdfPCell cell = new PdfPCell(new Phrase(header, fontHeader))
                {
                    BackgroundColor = bgColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 5
                };
                tabla.AddCell(cell);
            }

            foreach (var c in citas)
            {
                tabla.AddCell(new Phrase(c.id.ToString(), fontBody));
                tabla.AddCell(new Phrase(c.id_cliente.ToString(), fontBody));
                tabla.AddCell(new Phrase(c.fecha.ToString("yyyy-MM-dd"), fontBody));
                tabla.AddCell(new Phrase(c.hora.ToString(@"hh\:mm"), fontBody));
                tabla.AddCell(new Phrase(c.id_profesional.ToString(), fontBody));
                tabla.AddCell(new Phrase(c.Estado ?? "", fontBody));
                tabla.AddCell(new Phrase(c.id_servicio.ToString(), fontBody));
            }

            doc.Add(tabla);
            doc.Close();

            byte[] byteInfo = ms.ToArray();
            ms.Close();

            return File(byteInfo, "application/pdf", "ReporteCitas.pdf");
        }

        // ---------------------
        // ProductosmasvendidosPDF
        // ------------------

        public ActionResult ExportarProductosMasVendidosPdf()
        {
            // Agrupar los productos por el campo "detalle" y contarlos
            var productosMasVendidos = db.Ventas
                .GroupBy(v => v.detalle)
                .Select(g => new {
                    Producto = g.Key,
                    CantidadVendida = g.Count(),
                    TotalRecaudado = g.Sum(v => v.monto)
                })
                .OrderByDescending(p => p.CantidadVendida)
                .ToList();

            MemoryStream ms = new MemoryStream();
            Document doc = new Document(PageSize.A4, 25, 25, 30, 30);
            PdfWriter.GetInstance(doc, ms);
            doc.Open();

            var titulo = new Paragraph("Reporte de Productos Más Vendidos", new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD));
            titulo.Alignment = Element.ALIGN_CENTER;
            titulo.SpacingAfter = 20f;
            doc.Add(titulo);

            PdfPTable tabla = new PdfPTable(3) { WidthPercentage = 100 };
            tabla.SetWidths(new float[] { 50f, 20f, 30f });

            BaseColor bgColor = new BaseColor(35, 68, 73);
            Font fontHeader = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, BaseColor.WHITE);
            Font fontBody = new Font(Font.FontFamily.HELVETICA, 11, Font.NORMAL);

            string[] headers = { "Producto", "Cantidad Vendida", "Total Recaudado" };
            foreach (var header in headers)
            {
                PdfPCell cell = new PdfPCell(new Phrase(header, fontHeader))
                {
                    BackgroundColor = bgColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 5
                };
                tabla.AddCell(cell);
            }

            foreach (var p in productosMasVendidos)
            {
                tabla.AddCell(new Phrase(p.Producto ?? "", fontBody));
                tabla.AddCell(new Phrase(p.CantidadVendida.ToString(), fontBody));
                tabla.AddCell(new Phrase("₡" + p.TotalRecaudado.ToString("N2"), fontBody));
            }

            doc.Add(tabla);
            doc.Close();

            byte[] byteInfo = ms.ToArray();
            ms.Close();

            return File(byteInfo, "application/pdf", "ProductosMasVendidos.pdf");
        }



        // ---------------------
        // Servicios PDF
        // ---------------------
        public ActionResult ExportarServiciosPdf()
        {
            var servicios = db.Servicios.ToList();

            MemoryStream ms = new MemoryStream();
            Document doc = new Document(PageSize.A4, 25, 25, 30, 30);
            PdfWriter.GetInstance(doc, ms);
            doc.Open();

            var titulo = new Paragraph("Reporte de Servicios", new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD));
            titulo.Alignment = Element.ALIGN_CENTER;
            titulo.SpacingAfter = 20f;
            doc.Add(titulo);

            PdfPTable tabla = new PdfPTable(5) { WidthPercentage = 100 };
            tabla.SetWidths(new float[] { 10f, 25f, 30f, 15f, 20f });

            BaseColor bgColor = new BaseColor(35, 68, 73);
            Font fontHeader = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, BaseColor.WHITE);
            Font fontBody = new Font(Font.FontFamily.HELVETICA, 11, Font.NORMAL);

            string[] headers = { "ID", "Nombre", "Descripción", "Duración", "Precio" };
            foreach (var header in headers)
            {
                PdfPCell cell = new PdfPCell(new Phrase(header, fontHeader))
                {
                    BackgroundColor = bgColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 5
                };
                tabla.AddCell(cell);
            }

            foreach (var s in servicios)
            {
                tabla.AddCell(new Phrase(s.Id.ToString(), fontBody));
                tabla.AddCell(new Phrase(s.Nombre ?? "", fontBody));
                tabla.AddCell(new Phrase(s.Descripcion ?? "", fontBody));
                tabla.AddCell(new Phrase(s.Duracion ?? "", fontBody));
                tabla.AddCell(new Phrase(s.Precio.ToString("C"), fontBody));
            }

            doc.Add(tabla);
            doc.Close();

            byte[] byteInfo = ms.ToArray();
            ms.Close();

            return File(byteInfo, "application/pdf", "ReporteServicios.pdf");
        }
    }
}
