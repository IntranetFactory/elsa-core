using System.Threading;
using System.Threading.Tasks;
using Elsa.Models;
using Elsa.Services.Models;

namespace Elsa.Services
{
    public interface IWorkflowHost
    {
        Task<WorkflowExecutionContext> RunWorkflowAsync(WorkflowDefinitionActiveVersion workflowDefinitionActiveVersion, string? activityId = default, object? input = default, string? correlationId = default, CancellationToken cancellationToken = default);
        Task<WorkflowExecutionContext?> RunWorkflowInstanceAsync(int? tenantId, string workflowInstanceId, string? activityId = default, object? input = default, CancellationToken cancellationToken = default);
        Task<WorkflowExecutionContext?> RunWorkflowInstanceAsync(WorkflowInstance workflowInstance, string? activityId = default, object? input = default, CancellationToken cancellationToken = default);
        Task<WorkflowExecutionContext> RunWorkflowDefinitionAsync(int? tenantId, string workflowDefinitionId, string? activityId, object? input = default, string? correlationId = default, CancellationToken cancellationToken = default);
        Task<WorkflowExecutionContext> WorkflowInstanceCreateAsync(int? tenantId, string workflowDefinitionId, string? activityId, object? input = default, string? correlationId = default, CancellationToken cancellationToken = default);
        Task<WorkflowExecutionContext> RunScheduledWorkflowInstanceAsync(int? tenantId, string workflowInstanceId, string? activityId = default, object? input = default, string? correlationId = default, CancellationToken cancellationToken = default);
        // /// <summary>
        // /// Resume a workflow instance.
        // /// </summary>
        // Task ResumeAsync(string workflowInstanceId, string activityId, object? input = default, CancellationToken cancellationToken = default);
    }
}