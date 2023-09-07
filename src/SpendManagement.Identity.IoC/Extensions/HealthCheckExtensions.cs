using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SpendManagement.Identity.IoC.Extensions
{
    public static class HealthCheckExtensions
    {
        public static void AddHealthCheckers(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddSqlServer(configuration.GetConnectionString("SpendManagementIdentity"), name: "SqlServer", tags: new string[] { "db", "data" });

            services.AddHealthChecksUI()
                .AddInMemoryStorage();
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
