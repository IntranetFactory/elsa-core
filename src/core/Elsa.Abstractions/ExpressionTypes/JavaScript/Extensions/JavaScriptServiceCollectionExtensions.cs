// This is moved to Elsa.Core
//using System;
//using Elsa.Expressions;
//using Elsa.ExpressionTypes;
//using Microsoft.Extensions.Options;
//using Elsa.ExpressionTypes.JavaScript.Options;
//using Elsa.ExpressionTypes.JavaScript.Services;
//using Elsa.ExpressionTypes.JavaScript.Extensions;

//// ReSharper disable once CheckNamespace
//namespace Microsoft.Extensions.DependencyInjection
//{
//    public static class JavaScriptServiceCollectionExtensions
//    {
//        public static IServiceCollection AddJavaScriptExpressionEvaluator(this IServiceCollection services)
//        {
//            return services
//                .TryAddProvider<IExpressionHandler, JavaScriptHandler>(ServiceLifetime.Scoped)
//                .AddNotificationHandlers(typeof(JavaScriptServiceCollectionExtensions))
//                .AddTypeAlias(typeof(JavaScriptExpression<>), "JavaScriptExpression");
//        }

//        public static IServiceCollection WithJavaScriptOptions(this IServiceCollection services, Action<OptionsBuilder<ScriptOptions>> options)
//        {
//            var scriptOptions = services.AddOptions<ScriptOptions>();
//            options(scriptOptions);

//            return services;
//        }
//    }
//}