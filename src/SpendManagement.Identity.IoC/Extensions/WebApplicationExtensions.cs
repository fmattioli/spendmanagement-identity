using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SpendManagement.Identity.Data.Data;

namespace SpendManagement.Identity.IoC.Extensions
{
    public static class WebApplicationExtensions
    {
        public static void RunMigrationsOnApplicationStart(this WebApplication webApplication)
        {
            using (var scope = webApplication.Services.CreateScope())
            {
                var dataContext = scope.ServiceProvider.GetRequiredService<IdentityDataContext>();
                dataContext.Database.Migrate();
            }
        }
    }
}
