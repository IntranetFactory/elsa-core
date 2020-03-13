using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Models;
using Elsa.Services;
using Elsa.Services.Models;

namespace Elsa.Extensions
{
    public static class WorkflowRegistryExtensions
    {
        public static Task<Workflow> GetWorkflowAsync<T>(this IWorkflowRegistry workflowRegistry, string tenantId, CancellationToken cancellationToken) =>
            workflowRegistry.GetWorkflowAsync(tenantId, typeof(T).Name, VersionOptions.Latest, cancellationToken);

        public static Task<IEnumerable<(Workflow Workflow, IActivity Activity)>> GetWorkflowsByStartActivityAsync<T>(
            this IWorkflowRegistry workflowRegistry,
            string tenantId, 
            CancellationToken cancellationToken = default)
            where T : IActivity =>
            workflowRegistry.GetWorkflowsByStartActivityAsync(tenantId, typeof(T).Name, cancellationToken);

        public static async Task<IEnumerable<(Workflow Workflow, IActivity Activity)>> GetWorkflowsByStartActivityAsync(
            this IWorkflowRegistry workflowRegistry,
            string tenantId, 
            string activityType,
            CancellationToken cancellationToken = default)
        {
            // TO DO: inspect if tenantId should be passed here
            var workflows = await workflowRegistry.GetWorkflowsAsync(tenantId, cancellationToken);

            var query =
                from workflow in workflows
                where workflow.IsPublished
                from activity in workflow.GetStartActivities()
                where activity.Type == activityType
                select (workflow, activity);

            return query.Distinct();
        }
    }
}