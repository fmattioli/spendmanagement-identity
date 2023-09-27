using SpendManagement.Identity.IoC.Extensions;
using SpendManagement.Identity.IoC.Models;

var builder = WebApplication.CreateBuilder(args);

var enviroment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

builder.Configuration
    .AddJsonFile("appsettings.json", false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{enviroment}.json", true, reloadOnChange: true)
    .AddEnvironmentVariables();

var applicationSettings = builder.Configuration.GetSection("Settings").Get<Settings>();

// Add services to the container.
builder.Services.AddTracing(applicationSettings.TracingSettings);
builder.Services.AddHealthCheckers(applicationSettings.SqlServerSettings);
builder.Services.AddControllers();
builder.Services.AddAuthentication(applicationSettings.JwtOptionsSettings);
builder.Services.AddAuthorizationPolicies();
builder.Services.RegisterServices(applicationSettings.SqlServerSettings);
builder.Services.AddCors();
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerExtensions();

var app = builder.Build();

app.RunMigrationsOnApplicationStart();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SpendManagement.Identity"));
app.UseHealthCheckers();

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
