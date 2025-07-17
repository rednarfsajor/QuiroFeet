using System;
using System.Collections.Generic;
using System.Diagnostics;
using Analisis.Jobs;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(BackupsTarea.Startup))]

namespace BackupsTarea
{
    public class Startup
    {
        /// <summary>
        /// Configura los servidores de Hangfire.
        /// </summary>
        private IEnumerable<IDisposable> GetHangfireServers()
        {
            GlobalConfiguration.Configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage("Server=.\\SQLEXPRESS; Database=QuiroFeetJobs; Integrated Security=True;Trust Server Certificate=True");

            yield return new BackgroundJobServer();
        }

        /// <summary>
        /// Método ejecutado al iniciar la aplicación.
        /// </summary>
        public void Configuration(IAppBuilder app)
        {
            app.UseHangfireAspNet(GetHangfireServers);
       
            app.UseHangfireDashboard("/hangfire");
            HangfireAspNet.Use(GetHangfireServers);
            
            RecurringJob.AddOrUpdate<NotificacionCitasDiaria>(
            "notificacion-diaria-citas",
            x => x.NotificarCitasDelDia(),
            Cron.Daily(7)); // Ejecuta todos los días a las 7:00 AM

            RecurringJob.AddOrUpdate<NotificacionStock>(
            "notificar-articulos-sin-stock",
            x => x.NotificarArticulosSinStock(),
            Cron.Daily(8)); // A las 8:00 AM
        }

    }








    }

