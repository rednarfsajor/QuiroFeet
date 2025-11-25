using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Web;
using Analisis.BD;
using MimeKit;
using MailKit.Net.Smtp;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace Analisis.Jobs
{
    public class NotificacionCitasDiaria
    {
        public void NotificarCitasDelDia()
        {
            using (var db = new QuiroFeetEntities6())
            {
                var hoy = DateTime.Today;

                var citasDeHoy = db.Citas
                    .Where(c => DbFunctions.TruncateTime(c.fecha) == hoy)
                    .ToList();

                foreach (var cita in citasDeHoy)
                {
                    var cliente = db.Clientes.FirstOrDefault(c => c.id == cita.id_cliente);
                    if (cliente != null && !string.IsNullOrEmpty(cliente.telefono))
                    {
                        string mensaje = $@"
<html>
<head>
  <style>
    body {{
      font-family: Arial, sans-serif;
      background-color: #f6f6f6;
      margin: 0;
      padding: 0;
    }}
    .container {{
      max-width: 600px;
      margin: auto;
      background-color: #ffffff;
      padding: 20px;
      border-radius: 8px;
      box-shadow: 0 2px 5px rgba(0,0,0,0.1);
    }}
    h2 {{
      color: #2c3e50;
    }}
    p {{
      font-size: 16px;
      color: #555555;
      line-height: 1.5;
    }}
    .footer {{
      font-size: 12px;
      color: #999999;
      margin-top: 20px;
    }}
  </style>
</head>
<body>
  <div class='container'>
    <h2>Recordatorio de Cita</h2>
    <p>Hola <strong>{cliente.nombre}</strong>,</p>
    <p>Te recordamos que tienes una cita programada para <strong>hoy a las {cita.hora.ToString(@"hh\:mm")}</strong>.</p>
    <p>¡Te esperamos!</p>
    <div class='footer'>
      Este es un mensaje automático, por favor no responda a este correo.
    </div>
  </div>
</body>
</html>";
                        //EnviarWhatsapp(cliente.telefono, mensaje);
                        EnviarEmail(cliente.nombre, cliente.correo, mensaje);

                    }
                }
            }
        }

        private void EnviarWhatsapp(string numero, string mensaje)
        {
            // Aquí se implementará el envío vía API
            // Ejemplo: usar call a una API REST como CallMeBot (explicado abajo)
            string apiUrl = $"https://api.callmebot.com/whatsapp.php?phone={numero}&text={Uri.EscapeDataString(mensaje)}&apikey=5108866";
            using (var client = new HttpClient())
            {
                var result = client.GetAsync(apiUrl).Result;
                // Puedes validar result.StatusCode aquí
            }

        }
        private void EnviarEmail(string name, string correo, string mensaje) 
        {
            try
            {
                var mail = new MimeMessage();
                mail.From.Add(new MailboxAddress("QuiroFeet", "2027prueba2027@gmail.com"));
                mail.To.Add(new MailboxAddress(name, correo));
                mail.Subject = "QuiroFeet | Recordatorio de Cita del dia de hoy";
                mail.Body = new TextPart("html") { Text = mensaje };
                
                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, false);
                    client.Authenticate("2027prueba2027@gmail.com", "auss ddcv rsfd uyjc");
                    client.Send(mail);
                    client.Disconnect(true);
                }
                Console.WriteLine("Correo enviado correctamente.");
            }
            catch (Exception ex) { Console.WriteLine($"Error enviando correo: {ex.Message}"); }
        }
    }
}