using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Analisis.BD;
using MimeKit;
using MailKit.Net.Smtp;

namespace Analisis.Jobs
{
    public class NotificacionStock
    {

        public void NotificarArticulosSinStock()
        {
            using (var db = new QuiroFeetEntities6())
            {
                var productosSinStock = (from inv in db.Inventario
                                         join prod in db.Productos on inv.id_producto equals prod.id
                                         where prod.activo == 1 && inv.stock == 0
                                         join prov in db.Proveedores on prod.id_proveedor equals prov.id
                                         select new
                                         {
                                             ProductoNombre = prod.nombre,
                                             ProveedorId = prov.id,
                                             ProveedorNombre = prov.nombre,
                                             ProveedorTelefono = prov.numero
                                         }).ToList();

                if (!productosSinStock.Any()) return;

                // Agrupar por proveedor
                var productosAgrupados = productosSinStock
                    .GroupBy(p => new { p.ProveedorId, p.ProveedorNombre, p.ProveedorTelefono })
                    .OrderBy(g => g.Key.ProveedorNombre)
                    .ToList();

                // Crear el cuerpo HTML con estilos
                var sb = new StringBuilder();
                sb.AppendLine("<html>");
                sb.AppendLine("<head>");
                sb.AppendLine("<style>");
                sb.AppendLine("body { font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }");
                sb.AppendLine(".container { max-width: 700px; margin: auto; background-color: #fff; padding: 20px; border-radius: 8px; box-shadow: 0 2px 5px rgba(0,0,0,0.1); }");
                sb.AppendLine("h3 { color: #2c3e50; }");
                sb.AppendLine("h4 { color: #34495e; margin-top: 20px; }");
                sb.AppendLine("table { width: 100%; border-collapse: collapse; margin-top: 10px; }");
                sb.AppendLine("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; font-size: 14px; }");
                sb.AppendLine("th { background-color: #3498db; color: white; }");
                sb.AppendLine("tr:nth-child(even) { background-color: #f9f9f9; }");
                sb.AppendLine(".footer { font-size: 12px; color: #999; margin-top: 20px; text-align: center; }");
                sb.AppendLine("</style>");
                sb.AppendLine("</head>");
                sb.AppendLine("<body>");
                sb.AppendLine("<div class='container'>");

                sb.AppendLine("<h3>📦 Productos activos sin stock</h3>");
                sb.AppendLine("<p>Este es el reporte diario de productos activos que actualmente no cuentan con stock disponible.</p>");

                // Generar secciones por proveedor 
                foreach (var grupo in productosAgrupados)
                {
                    sb.AppendLine($"<h4>Proveedor: {grupo.Key.ProveedorNombre} &nbsp;|&nbsp; Tel: {grupo.Key.ProveedorTelefono}</h4>");
                    sb.AppendLine("<table>");
                    sb.AppendLine("<tr><th>Producto</th></tr>");

                    foreach (var producto in grupo)
                    {
                        sb.AppendLine($"<tr><td>{producto.ProductoNombre}</td></tr>");
                    }

                    sb.AppendLine("</table>");
                }

                sb.AppendLine("<div class='footer'>Este es un mensaje automático. Por favor, no responda a este correo.</div>");
                sb.AppendLine("</div>");
                sb.AppendLine("</body>");
                sb.AppendLine("</html>");

                

                // Enviar correo
                EnviarCorreo("franderrojas20@gmail.com", "2027prueba2027@gmail.com", "Productos activos sin stock", sb);
            }
        }

        private void EnviarCorreo(string para, string de, string asunto, StringBuilder cuerpoHtml)
        {
            var mensaje = new MimeMessage();
            mensaje.From.Add(MailboxAddress.Parse(de));
            mensaje.To.Add(MailboxAddress.Parse(para));
            mensaje.Subject = asunto;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = cuerpoHtml.ToString()
            };
            mensaje.Body = bodyBuilder.ToMessageBody();

            using (var smtp = new SmtpClient())
            {
                smtp.Connect("smtp.gmail.com", 587, false);
                smtp.Authenticate("2027prueba2027@gmail.com", "auss ddcv rsfd uyjc");
                smtp.Send(mensaje);
                smtp.Disconnect(true);
            }
        }
    }
}

