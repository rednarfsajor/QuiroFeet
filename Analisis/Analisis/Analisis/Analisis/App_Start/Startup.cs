using System;
using System.Collections.Generic;
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
        private IEnumerable<IDisposable> GetHangfireServers()
        {
            GlobalConfiguration.Configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage("Server=sql1004.site4now.net; Database=db_abc856_hangfire; User Id=db_abc856_hangfire_admin; Password=B1Admin2021$;");

            yield return new BackgroundJobServer();
        }

        public void Configuration(IAppBuilder app)
        {
            app.UseHangfireAspNet(GetHangfireServers);
            app.UseHangfireDashboard("/hangfire");

            // Tareas recurrentes
            RecurringJob.AddOrUpdate<NotificacionCitasDiaria>(
                "notificacion-diaria-citas",
                x => x.NotificarCitasDelDia(),
                Cron.Daily(7)); // 7:00 AM

            RecurringJob.AddOrUpdate<NotificacionStock>(
                "notificar-articulos-sin-stock",
                x => x.NotificarArticulosSinStock(),
                Cron.Daily(8)); // 8:00 AM
        }
    }
}
