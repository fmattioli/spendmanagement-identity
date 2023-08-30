using Microsoft.OpenApi.Models;

using SpendManagement.Identity.IoC.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SpendManagement.Identity.API", Version = "v1", Description = "The users management related to the SpendManagement project." });
});
builder.Services.AddAuthentication(builder.Configuration);
builder.Services.AddAuthorizationPolicies();
builder.Services.RegisterServices(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SpendManagement.Identity"));
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors(builder => builder
    .SetIsOriginAllowed(_ => true)
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());
app.MapControllers();
app.Run();
