using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTracing.Util;

namespace Payment.API
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
