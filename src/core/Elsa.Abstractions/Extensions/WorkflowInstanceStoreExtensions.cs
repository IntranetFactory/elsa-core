// TO DO: this is commented out because of workflowInstance.Activities error until we decide if it should be kept.
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Elsa.Models;
//using Elsa.Persistence;
//using Elsa.Services;

//namespace Elsa.Extensions
//{
//    public static class WorkflowInstanceStoreExtensions
//    {
//        public static Task<IEnumerable<(WorkflowInstance WorkflowInstance, ActivityDefinition BlockingActivity)>> ListByBlockingActivityAsync<TActivity>(
//            this IWorkflowInstanceStore store,
//            int? tenantId,
//            string? correlationId = default,
//            Func<Variables, bool>? activityStatePredicate = default,
//            CancellationToken cancellationToken = default) where TActivity : IActivity =>
//            store.ListByBlockingActivityAsync(tenantId, typeof(TActivity).Name, correlationId, activityStatePredicate, cancellationToken);

//        public static async Task<IEnumerable<(WorkflowInstance WorkflowInstance, ActivityDefinition BLockingActivity)>> ListByBlockingActivityAsync(
//            this IWorkflowInstanceStore store,
//            int? tenantId,
//            string activityType,
//            string? correlationId,
//            Func<Variables, bool>? activityStatePredicate = default,
//            CancellationToken cancellationToken = default)
//        {
//            var tuples = await store.ListByBlockingActivityAsync(tenantId, activityType, correlationId, cancellationToken);
//            var query =
//                from item in tuples
//                let workflowInstance = item.Item1
//                let blockingActivity = item.Item2
//                let activityInstance = workflowInstance.Activities.First(x => x.ActivityId == blockingActivity.ActivityId)
//                select (workflowInstance, activityInstance);

//            if (activityStatePredicate != null)
//            {
//                query = query.Where(tuple => activityStatePredicate(tuple.activityInstance.State));
//            }

//            return query;
//        }
//    }
//}