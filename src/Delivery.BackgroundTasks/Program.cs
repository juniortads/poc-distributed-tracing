using System;
using Delivery.BackgroundTasks.Configuration;
using Delivery.BackgroundTasks.Tasks;
using EventBus.Sqs.Configuration;
using EventBus.Sqs.Tracing.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTracing.Util;

namespace Delivery.BackgroundTasks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;

                    Environment.SetEnvironmentVariable("JAEGER_SERVICE_NAME", configuration["JAEGER_SERVICE_NAME"]);
                    Environment.SetEnvironmentVariable("JAEGER_AGENT_HOST", configuration["JAEGER_AGENT_HOST"]);
                    Environment.SetEnvironmentVariable("JAEGER_AGENT_PORT", configuration["JAEGER_AGENT_PORT"]);
                    Environment.SetEnvironmentVariable("JAEGER_SAMPLER_TYPE", configuration["JAEGER_SAMPLER_TYPE"]);

                    services.AddSingleton(serviceProvider =>
                    {
                        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

                        var config = Jaeger.Configuration.FromEnv(loggerFactory);
                        var tracer = config.GetTracer();

                        if (!GlobalTracer.IsRegistered())
                            GlobalTracer.Register(tracer);

                        return tracer;
                    });

                    services.AddOpenTracing(builder => {
                        builder.ConfigureAspNetCore(options => {
                            options.Hosting.IgnorePatterns.Add(x => {
                                return x.Request.Path == "/health";
                            });
                            options.Hosting.IgnorePatterns.Add(x => {
                                return x.Request.Path == "/metrics";
                            });
                        });
                    });

                    services.AddEventBusSQS(configuration)
                            .AddOpenTracing();

                    services.AddScopeHostedService<DeliveryTask>();
                });
    }
}
