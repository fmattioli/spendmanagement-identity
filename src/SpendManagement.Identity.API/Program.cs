using SpendManagement.Identity.IoC.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTracing(builder.Configuration);
builder.Services.AddCors();
builder.Services.AddHealthCheckers(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication(builder.Configuration);
builder.Services.AddAuthorizationPolicies();
builder.Services.RegisterServices(builder.Configuration);
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
