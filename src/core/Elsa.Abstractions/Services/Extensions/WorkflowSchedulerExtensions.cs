using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Elsa.Services
{
    public static class WorkflowSchedulerExtensions
    {
        public static Task RunAsync<T>(this IWorkflowScheduler workflowScheduler,int? tenantId, object? input = default, string? correlationId = default, CancellationToken cancellationToken = default) 
            => workflowScheduler.ScheduleNewWorkflowAsync(tenantId, typeof(T).Name, input, correlationId, cancellationToken);
    }
}