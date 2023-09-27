using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SpendManagement.Identity.IoC.Models;

namespace SpendManagement.Identity.IoC.Extensions
{
    public static class HealthCheckExtensions
    {
        public static void AddHealthCheckers(this IServiceCollection services, SqlServerSettings? settings)
        {
            if (settings?.ConnectionString is not null)
            {
                services
                    .AddHealthChecks()
                    .AddSqlServer(settings.ConnectionString, name: "SqlServer", tags: new string[] { "db", "data" });

                services
                    .AddHealthChecksUI()
                    .AddInMemoryStorage();
            }
        }

        public static void UseHealthCheckers(this IApplicationBuilder app)
        {
            app.UseHealthChecks("/health", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseHealthChecksUI(options => options.UIPath = "/monitor");
        }
    }
}
