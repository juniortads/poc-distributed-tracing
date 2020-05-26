using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTracing.Util;
using Order.API.ExternalServices;
using Order.API.Useful;
using Polly;
using Polly.Contrib.Simmy;

namespace Order.API
{
    static class Configurations
    {
        public static IServiceCollection AddPaymentService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IPaymentExternalService, PaymentExternalService>();

            var faultMessage = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
            {
                Content = new StringContent("Service temporarily unavailable")
            };

            var faultPolicy = MonkeyPolicy.InjectFaultAsync(faultMessage, injectionRate: 0.6, enabled: () => true);

            var retryPolicy = Policy
                       .HandleResult<HttpResponseMessage>(
                           r => r.StatusCode == HttpStatusCode.ServiceUnavailable)
                       .RetryAsync(3, onRetry: (message, retryCount) =>
                       {
                           string msg = $"Unexpected error, I will try to call the payment service again. Retry number: {retryCount}";

                           Console.BackgroundColor = ConsoleColor.Black;
                           Console.ForegroundColor = ConsoleColor.Red;

                           //$" say {msg}".Bash();

                           Console.Out.WriteLineAsync(msg);
                       });

            var policyWrap = Policy.WrapAsync(retryPolicy, faultPolicy);

            services.AddHttpClient("PaymentServiceClient", client =>
            {
                client.BaseAddress = new Uri(configuration["ENDPOINT_PAYMENT_API"]);
            })
            .AddPolicyHandler(policyWrap);

            return services;
        }
        public static IServiceCollection AddJaegerOpenTracing(this IServiceCollection services, IConfiguration configuration)
        {
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

            return services;
        }
    }
}
