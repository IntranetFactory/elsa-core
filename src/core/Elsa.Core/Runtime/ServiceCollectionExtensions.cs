using Elsa.StartupTasks;
using Microsoft.Extensions.DependencyInjection;

namespace Elsa.Runtime
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStartupRunner(this IServiceCollection services)
        {
            return services
                .AddTransient<IStartupRunner, StartupRunner>()
                .AddHostedService<StartupRunnerHostedService>();

        }

        public static IServiceCollection AddStartupTask<TStartupTask>(this IServiceCollection services)
            where TStartupTask : class, IStartupTask
        {
            return services
                .AddTransient<IStartupTask, TStartupTask>();
        }

        public static IServiceCollection AddTaskExecutingServer(this IServiceCollection services)
        {
            return services
                .AddStartupRunner()
                .AddHostedService<StartupRunnerHostedService>();
        }

        public static IServiceCollection AddWorkflowTasksRunner(this IServiceCollection services)
        {
            return services
                .AddHostedService<WorkflowTasksRunner>();
        }
    }
}