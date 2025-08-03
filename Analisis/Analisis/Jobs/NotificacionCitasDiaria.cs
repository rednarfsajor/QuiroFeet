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
                        string mensaje = $"Hola {cliente.nombre}, te recordamos tu cita hoy a las {cita.hora.ToString(@"hh\:mm")}. ¡Te esperamos!";
                        EnviarWhatsapp(cliente.telefono, mensaje);
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
                mail.Body = new TextPart("plain") { Text = mensaje };

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