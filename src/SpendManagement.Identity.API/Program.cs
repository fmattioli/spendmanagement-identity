using SpendManagement.Identity.IoC.Extensions;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

builder.Configuration
    .AddJsonFile("appsettings.json", false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", true, reloadOnChange: true)
    .AddEnvironmentVariables();


var applicationSettings = builder.Configuration.GetApplicationSettings(builder.Environment);

builder.Logging
    .ClearProviders()
    .AddFilter("Microsoft", LogLevel.Warning)
    .AddFilter("Microsoft", LogLevel.Critical);

// Add services to the container.
builder.Services
    .AddLoggingDependency()
    .AddTracing(applicationSettings!.TracingSettings)
    .AddHealthCheckers(applicationSettings!.SqlServerSettings)
    .AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services
    .AddAuthentication(applicationSettings.JwtOptionsSettings)
    .AddAuthorizationPolicies()
    .RegisterServices(applicationSettings.SqlServerSettings);

builder.Services
    .AddCors()
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
