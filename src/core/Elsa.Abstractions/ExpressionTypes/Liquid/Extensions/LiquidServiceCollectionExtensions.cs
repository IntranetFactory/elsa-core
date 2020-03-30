//using Elsa.Expressions;
//using Elsa.ExpressionTypes.Liquid.Filters;
//using Elsa.ExpressionTypes.Liquid.Options;
//using Elsa.ExpressionTypes.Liquid.Services;
//using Microsoft.Extensions.DependencyInjection;

//namespace Elsa.ExpressionTypes.Liquid.Extensions
//{
//    //public static class LiquidServiceCollectionExtensions
//    //{
//    //    public static IServiceCollection AddLiquidExpressionEvaluator(this IServiceCollection services)
//    //    {
//    //        return services
//    //            .TryAddProvider<IExpressionHandler, LiquidExpressionHandler>(ServiceLifetime.Scoped)
//    //            .AddMemoryCache()
//    //            .AddNotificationHandlers(typeof(LiquidServiceCollectionExtensions))
//    //            .AddScoped<ILiquidTemplateManager, LiquidTemplateManager>()
//    //            .AddLiquidFilter<JsonFilter>("json")
//    //            .AddTypeAlias(typeof(LiquidExpression<>), "LiquidExpression");
//    //    }
        
//    //    public static IServiceCollection AddLiquidFilter<T>(this IServiceCollection services, string name) where T : class, ILiquidFilter
//    //    {
//    //        services.Configure<LiquidOptions>(options => options.FilterRegistrations.Add(name, typeof(T)));
//    //        services.AddScoped<T>();
//    //        return services;
//    //    }
//    //}
//}