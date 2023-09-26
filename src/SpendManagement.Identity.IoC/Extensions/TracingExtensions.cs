using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SpendManagement.Identity.Data.Constants;

namespace SpendManagement.Identity.IoC.Extensions
{
    public static class TracingExtensions
    {
        public static IServiceCollection AddTracing(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOpenTelemetry().WithTracing(tcb =>
            {
                tcb
                .AddSource(Tracing.ApplicationName)
                .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService(serviceName: Tracing.ApplicationName))
                .AddSqlClientInstrumentation(options => options.SetDbStatementForText = true)
                .AddAspNetCoreInstrumentation()
                .AddOtlpExporter(opt =>
                 {
                     opt.Endpoint = new Uri(configuration["TracingSettings:Uri"] + ":" + configuration["TracingSettings:Port"]);
                     opt.Protocol = OtlpExportProtocol.Grpc;
                 });
             });

            services.AddSingleton(TracerProvider.Default.GetTracer(Tracing.ApplicationName));

            return services;
        }
    }
}
