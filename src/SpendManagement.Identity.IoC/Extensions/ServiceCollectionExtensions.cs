using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SpendManagement.Identity.Application.Services;
using SpendManagement.Identity.Data.Data;
using Microsoft.AspNetCore.Identity;
using SpendManagement.Identity.IoC.Models;

namespace SpendManagement.Identity.IoC.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterServices(this IServiceCollection services, SqlServerSettings? sqlserver)
        {
            services.AddDbContext<IdentityDataContext>(options =>
                options.UseSqlServer(sqlserver?.ConnectionString ?? throw new Exception("Invalid sqlserver connection string"))
            );

            services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<IdentityDataContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IIdentityService, IdentityService>();
        }
    }
}
