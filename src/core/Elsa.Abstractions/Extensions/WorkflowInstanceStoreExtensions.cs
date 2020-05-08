using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Models;
using Elsa.Persistence;
using Elsa.Services;

namespace Elsa.Extensions
{
    public static class WorkflowInstanceStoreExtensions
    {
        public static Task<IEnumerable<(WorkflowInstance WorkflowInstance, WorkflowInstanceTask blockingWorkflowInstanceTask)>> ListByBlockingActivityAsync<TActivity>(
            this IWorkflowInstanceStore store,
            int? tenantId, 
            string? correlationId = default,
            Func<Variables, bool>? activityStatePredicate = default,
            CancellationToken cancellationToken = default) where TActivity : IActivity =>
            store.ListByBlockingActivityAsync(tenantId, typeof(TActivity).Name, correlationId, activityStatePredicate, cancellationToken);

        public static async Task<IEnumerable<(WorkflowInstance WorkflowInstance, WorkflowInstanceTask blockingWorkflowInstanceTask)>> ListByBlockingActivityAsync(
            this IWorkflowInstanceStore store,
            int? tenantId, 
            string activityType,
            string? correlationId,
            Func<Variables, bool>? activityStatePredicate = default,
            CancellationToken cancellationToken = default)
        {
            var tuples = await store.ListByBlockingActivityAsync(tenantId, activityType, correlationId, cancellationToken);
            var query =
                from item in tuples
                let workflowInstance = item.Item1
                let blockingActivity = item.Item2
                let workflowInstanceTask = workflowInstance.WorkflowInstanceTasks.First(x => x.Id == blockingActivity.Id)
                select (workflowInstance, workflowInstanceTask);

            if (activityStatePredicate != null)
            {
                query = query.Where(tuple => activityStatePredicate(tuple.workflowInstanceTask.State));
            }

            return query;
        }
    }
}