using System;
using EventBus.Sqs.Configuration;
using EventBus.Sqs.Tracing.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTracing.Util;
using Order.API.ExternalServices;
using Order.API.Services;

namespace Order.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPaymentExternalService, PaymentExternalService>();

            services.AddHttpClient("PaymentServiceClient", client =>
            {
                client.BaseAddress = new Uri(Configuration["ENDPOINT_PAYMENT_API"]);
            });

            Environment.SetEnvironmentVariable("JAEGER_SERVICE_NAME", Configuration["JAEGER_SERVICE_NAME"]);
            Environment.SetEnvironmentVariable("JAEGER_AGENT_HOST", Configuration["JAEGER_AGENT_HOST"]);
            Environment.SetEnvironmentVariable("JAEGER_AGENT_PORT", Configuration["JAEGER_AGENT_PORT"]);
            Environment.SetEnvironmentVariable("JAEGER_SAMPLER_TYPE", Configuration["JAEGER_SAMPLER_TYPE"]);

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

            services.AddEventBusSQS(Configuration)
                    .AddOpenTracing();

            services.AddHealthChecks();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers();
            });
        }
    }
}
