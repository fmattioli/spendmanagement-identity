using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SpendManagement.Identity.IoC.Models;

namespace SpendManagement.Identity.IoC.Extensions
{
    public static class ConfigurationBuilderExtensions
    {
        public static Settings GetApplicationSettings(this IConfiguration configuration, IHostEnvironment env)
        {
            var settings = configuration.GetSection("Settings").Get<Settings>();

            if (!env.IsDevelopment())
            {
                settings!.SqlServerSettings!.ConnectionString = GetConnectionStringFromRenderSecret();
            }

            return settings!;
        }

        private static string GetConnectionStringFromRenderSecret()
        {
            return Environment.GetEnvironmentVariable("ConnectionString") ?? "";
        }
    }
}
