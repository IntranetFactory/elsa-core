using System.Threading;
using System.Threading.Tasks;
using Elsa.Services;

// ReSharper disable once CheckNamespace
namespace Elsa
{
    public static class WorkflowSchedulerExtensions
    {
        public static async Task ScheduleNewWorkflow<T>(
            this IWorkflowScheduler scheduler,
            int? tenantId, 
            object? input = default,
            string? correlationId = default,
            CancellationToken cancellationToken = default) =>
            await scheduler.ScheduleNewWorkflowAsync(tenantId, typeof(T).Name, input, correlationId, cancellationToken);
    }
}