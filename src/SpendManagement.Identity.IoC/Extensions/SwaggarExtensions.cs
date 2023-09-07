using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace SpendManagement.Identity.IoC.Extensions
{
    public static class SwaggarExtensions
    {
        public static void AddSwaggerExtensions(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSwaggerGen(c =>
             {
                 c.SwaggerDoc("v1", new OpenApiInfo { Title = "SpendManagement.Identity", Version = "v1", Description = "The users management related to the SpendManagement project." });
                 c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                 {
                     In = ParameterLocation.Header,
                     Description = "Please insert token",
                     Name = "Authorization",
                     Type = SecuritySchemeType.Http,
                     Scheme = "Bearer"
                 });
                 c.AddSecurityRequirement(new OpenApiSecurityRequirement
                 {
                     {
                         new OpenApiSecurityScheme
                         {
                             Reference = new OpenApiReference
                             {
                                 Type = ReferenceType.SecurityScheme,
                                 Id = "Bearer"
                             }
                         },
                         Array.Empty<string>()
                     }
                 });
             });
        }
    }
}
