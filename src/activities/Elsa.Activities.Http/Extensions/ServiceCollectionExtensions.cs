using System;
using Elsa.Activities.Http;
using Elsa.Activities.Http.Models;
using Elsa.Activities.Http.Options;
using Elsa.Activities.Http.Parsers;
//using Elsa.Activities.Http.RequestHandlers.Handlers;
using Elsa.Activities.Http.Services;
using Elsa.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHttp(this IServiceCollection services, Action<OptionsBuilder<HttpActivityOptions>> options = null) =>
            services
                .AddHttpServices(options)
                .AddHttpActivities();
        
        public static IServiceCollection AddHttpServices(this IServiceCollection services, Action<OptionsBuilder<HttpActivityOptions>> options = null)
        {
            options?.Invoke(services.AddOptions<HttpActivityOptions>());

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpClient(nameof(SendHttpRequest));

            // Commented out until we decide if it should be used with UserTask
            services
                //.AddSingleton<ITokenService, TokenService>()
                //.AddSingleton<IHttpRequestBodyParser, DefaultHttpRequestBodyParser>()
                //.AddSingleton<IHttpRequestBodyParser, JsonHttpRequestBodyParser>()
                //.AddSingleton<IHttpRequestBodyParser, FormHttpRequestBodyParser>()
                .AddSingleton<IHttpResponseBodyParser, DefaultHttpResponseBodyParser>()
                .AddSingleton<IHttpResponseBodyParser, JsonHttpResponseBodyParser>()
                .AddSingleton<IActionContextAccessor, ActionContextAccessor>()
                //.AddSingleton<IAbsoluteUrlProvider, DefaultAbsoluteUrlProvider>()
                .AddTypeAlias<HttpResponseHeaders>()
                .AddHttpContextAccessor()
                .AddNotificationHandlers(typeof(ServiceCollectionExtensions))
                .AddDataProtection();

            // Commented out until we decide if it should be used with UserTask
            return services;
                //.AddRequestHandler<TriggerRequestHandler>()
                //.AddRequestHandler<SignalRequestHandler>();
        }

        // Commented out until we decide if it should be used with UserTask
        public static IServiceCollection AddHttpActivities(this IServiceCollection services) =>
            services
                //.AddActivity<ReceiveHttpRequest>()
                .AddActivity<SendHttpRequest>();

        // Commented out until we decide if it should be used with UserTask
        //public static IServiceCollection AddRequestHandler<THandler>(this IServiceCollection services) where THandler : class, IRequestHandler => services.AddScoped<THandler>();
    }
}