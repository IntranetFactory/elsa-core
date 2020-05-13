using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Extensions;
using Elsa.Models;
using Elsa.Persistence;
using Elsa.Services;
using Elsa.Services.Models;

namespace Elsa.WorkflowProviders
{
    /// <summary>
    /// Provides workflow definitions from the workflow definition store.
    /// </summary>
    public class StoreWorkflowProvider : IWorkflowProvider
    {
        private readonly IWorkflowDefinitionVersionStore store;
        private readonly IActivityResolver activityResolver;

        public StoreWorkflowProvider(IWorkflowDefinitionVersionStore store, IActivityResolver activityResolver)
        {
            this.store = store;
            this.activityResolver = activityResolver;
        }

        public async Task<IEnumerable<WorkflowDefinitionActiveVersion>> GetWorkflowDefinitionActiveVersionsAsync(int? tenantId, CancellationToken cancellationToken)
        {
            var workflowDefinitionVersions = await store.ListAsync(tenantId, VersionOptions.All, cancellationToken);
            return workflowDefinitionVersions.Select(CreateWorkflow);
        }

        private WorkflowDefinitionActiveVersion CreateWorkflow(WorkflowDefinitionVersion definitionVersion)
        {
            var resolvedActivities = definitionVersion.Activities.Select(ResolveActivity).ToDictionary(x => x.Id);

            var workflowDefinitionActiveVersion = new WorkflowDefinitionActiveVersion
            {
                DefinitionId = definitionVersion.DefinitionId,
                TenantId = definitionVersion.TenantId,
                Description = definitionVersion.Description,
                Name = definitionVersion.Name,
                Version = definitionVersion.Version,
                IsLatest = definitionVersion.IsLatest,
                IsPublished = definitionVersion.IsPublished,
                IsDisabled = definitionVersion.IsDisabled,
                IsSingleton = definitionVersion.IsSingleton,
                Activities = resolvedActivities.Values,
                Connections = definitionVersion.Connections.Select(x => ResolveConnection(x, resolvedActivities)).ToList()
            };

            return workflowDefinitionActiveVersion;
        }

        private static Connection ResolveConnection(ConnectionDefinition connectionDefinition, IReadOnlyDictionary<string?, IActivity> activityDictionary)
        {
            var source = activityDictionary[connectionDefinition.SourceActivityId];
            var target = activityDictionary[connectionDefinition.DestinationActivityId];
            var outcome = connectionDefinition.Outcome;

            return new Connection(source, target, outcome);
        }

        private IActivity ResolveActivity(ActivityDefinition activityDefinition) => activityResolver.ResolveActivity(activityDefinition);
    }
}