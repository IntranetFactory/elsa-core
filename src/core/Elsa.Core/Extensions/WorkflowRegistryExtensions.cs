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
        public static Task<WorkflowDefinitionActiveVersion> GetWorkflowDefinitionActiveVersionAsync<T>(this IWorkflowRegistry workflowRegistry, int? tenantId, CancellationToken cancellationToken) =>
            workflowRegistry.GetWorkflowDefinitionActiveVersionAsync(tenantId, typeof(T).Name, VersionOptions.Latest, cancellationToken);

        public static Task<IEnumerable<(WorkflowDefinitionActiveVersion WorkflowDefinitionActiveVersion, IActivity Activity)>> GetWorkflowsByStartActivityAsync<T>(
            this IWorkflowRegistry workflowRegistry,
            int? tenantId, 
            CancellationToken cancellationToken = default)
            where T : IActivity =>
            workflowRegistry.GetWorkflowsByStartActivityAsync(tenantId, typeof(T).Name, cancellationToken);

        public static async Task<IEnumerable<(WorkflowDefinitionActiveVersion WorkflowDefinitionActiveVersion, IActivity Activity)>> GetWorkflowsByStartActivityAsync(
            this IWorkflowRegistry workflowRegistry,
            int? tenantId, 
            string activityType,
            CancellationToken cancellationToken = default)
        {
            // TO DO: inspect if tenantId should be passed here
            var workflowDefinitionActiveVersions = await workflowRegistry.GetWorkflowDefinitionActiveVersionsAsync(tenantId, cancellationToken);

            var query =
                from workflowDefinitionActiveVersion in workflowDefinitionActiveVersions
                where workflowDefinitionActiveVersion.IsPublished
                from activity in workflowDefinitionActiveVersion.GetStartActivities()
                where activity.Type == activityType
                select (workflowDefinitionActiveVersion, activity);

            return query.Distinct();
        }
    }
}