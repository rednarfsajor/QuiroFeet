using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Analisis.BD; // Asegúrate que este namespace es correcto para tu EDMX

namespace Analisis.Controllers
{
    public class ReportesController : Controller
    {
        private QuiroFeetEntities5 db = new QuiroFeetEntities5();

        public ActionResult Index()
        {
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
