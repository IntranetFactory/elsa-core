using System.Collections.Generic;
using System.Linq;
using Elsa.Models;

namespace Elsa.Extensions
{
    public static class WorkflowInstanceCollectionExtensions
    {
        public static IEnumerable<(WorkflowInstance, WorkflowInstanceTask)> GetBlockingActivities(this IEnumerable<WorkflowInstance> instances, string? activityType = null)
        {
            var query =
                from workflowInstance in instances
                from blockingActivity in workflowInstance.BlockingActivities
                select (workflowInstance, blockingActivity);
            
            if (!string.IsNullOrWhiteSpace(activityType))
                query = query.Where(x => x.blockingActivity.Type == activityType);
            
            return query.Distinct();
        }
    }
}