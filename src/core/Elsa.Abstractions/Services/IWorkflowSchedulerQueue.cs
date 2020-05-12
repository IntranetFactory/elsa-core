using Elsa.Services.Models;

namespace Elsa.Services
{
    public interface IWorkflowSchedulerQueue
    {
        void Enqueue(WorkflowDefinitionActiveVersion workflowDefinitionActiveVersion, IActivity activity, object? input, string? correlationId);
        (WorkflowDefinitionActiveVersion WorkflowDefinitionActiveVersion, IActivity Activity, object? Input, string? CorrelationId)? Dequeue(string workflowDefinitionId, string activityId);
    }
}