using System.Threading;
using System.Threading.Tasks;
using Elsa.Models;
using Elsa.Services.Models;

namespace Elsa.Services
{
    public interface IWorkflowHost
    {
        Task<WorkflowExecutionContext?> RunWorkflowInstanceAsync(int? tenantId, string workflowInstanceId, string? activityId = default, object? input = default, CancellationToken cancellationToken = default);
        Task<WorkflowExecutionContext?> RunWorkflowInstanceAsync(WorkflowInstance workflowInstance, string? activityId = default, object? input = default, CancellationToken cancellationToken = default);
        Task<WorkflowExecutionContext> WorkflowInstanceCreateAsync(int? tenantId, string workflowDefinitionId, string? correlationId = default, string? payload = default,CancellationToken cancellationToken = default);
        Task<WorkflowExecutionContext> RunScheduledWorkflowInstanceAsync(int? tenantId, string instanceId);
    }
}