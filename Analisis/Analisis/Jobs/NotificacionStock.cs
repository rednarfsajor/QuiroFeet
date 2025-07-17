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
            using (var db = new QuiroFeetEntities5())
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

                // Construir cuerpo HTML del correo
                var sb = new StringBuilder();
                sb.AppendLine("<h3>Productos activos sin stock</h3>");

                foreach (var grupo in productosAgrupados)
                {
                    sb.AppendLine($"<h4>Proveedor: {grupo.Key.ProveedorNombre} - Tel: {grupo.Key.ProveedorTelefono}</h4>");
                    sb.AppendLine("<ul>");
                    foreach (var producto in grupo)
                    {
                        sb.AppendLine($"<li>{producto.ProductoNombre}</li>");
                    }
                    sb.AppendLine("</ul>");
                }

                string cuerpoHtml = sb.ToString();

                // Enviar correo
                EnviarCorreo("franderrojas20@gmail.com", "2027prueba2027@gmail.com", "Productos activos sin stock", cuerpoHtml);
            }
        }

        private void EnviarCorreo(string para, string de, string asunto, string cuerpoHtml)
        {
            var mensaje = new MimeMessage();
            mensaje.From.Add(MailboxAddress.Parse(de));
            mensaje.To.Add(MailboxAddress.Parse(para));
            mensaje.Subject = asunto;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = cuerpoHtml
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