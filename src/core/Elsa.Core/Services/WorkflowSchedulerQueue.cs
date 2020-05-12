using System.Collections.Generic;
using Elsa.Services.Models;

namespace Elsa.Services
{
    public class WorkflowSchedulerQueue : IWorkflowSchedulerQueue
    {
        private readonly IDictionary<(string WorkflowDefinitionId, string ActivityId), (WorkflowDefinitionActiveVersion WorkflowDefinitionActiveVersion, IActivity Activity, object? Input, string? CorrelationId)> nextWorkflowInstances;

        public WorkflowSchedulerQueue() =>
            nextWorkflowInstances = new Dictionary<(string WorkflowDefinitionId, string ActivityId), (WorkflowDefinitionActiveVersion WorkflowDefinitionActiveVersion, IActivity Activity, object? Input, string? CorrelationId)>();

        public void Enqueue(WorkflowDefinitionActiveVersion workflowDefinitionActiveVersion, IActivity activity, object? input, string? correlationId)
            => nextWorkflowInstances[(workflowDefinitionActiveVersion.DefinitionId, activity.Id)] = (workflowDefinitionActiveVersion, activity, input, correlationId);

        public (WorkflowDefinitionActiveVersion WorkflowDefinitionActiveVersion, IActivity Activity, object? Input, string? CorrelationId)? Dequeue(string workflowDefinitionId, string activityId)
        {
            var key = (workflowDefinitionId, activityId);
            if(!nextWorkflowInstances.ContainsKey(key))
                return default;
            
            var entry = nextWorkflowInstances[key];
            nextWorkflowInstances.Remove(key);
            
            return entry;
        }
    }
}