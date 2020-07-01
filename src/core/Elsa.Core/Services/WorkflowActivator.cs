using System.Threading;
using System.Threading.Tasks;
using Elsa.Models;
using Elsa.Services.Models;
using NodaTime;

namespace Elsa.Services
{
    public class WorkflowActivator : IWorkflowActivator
    {
        private readonly IWorkflowRegistry workflowRegistry;
        private readonly IClock clock;
        private readonly IIdGenerator idGenerator;

        public WorkflowActivator(IWorkflowRegistry workflowRegistry, IClock clock, IIdGenerator idGenerator)
        {
            this.workflowRegistry = workflowRegistry;
            this.clock = clock;
            this.idGenerator = idGenerator;
        }
        
        public async Task<WorkflowInstance> ActivateAsync(int? tenantId, string definitionId, string? correlationId = default, CancellationToken cancellationToken = default)
        {
            var workflowDefinitionActiveVersion = await workflowRegistry.GetWorkflowDefinitionActiveVersionAsync(tenantId, definitionId, VersionOptions.Published, cancellationToken);
            return await ActivateAsync(workflowDefinitionActiveVersion, correlationId, cancellationToken);
        }

        public Task<WorkflowInstance> ActivateAsync(WorkflowDefinitionActiveVersion workflowDefinitionActiveVersion, string? correlationId = default, CancellationToken cancellationToken = default)
        {
            var workflowInstance = new WorkflowInstance
            {
                Id = idGenerator.Generate(),
                TenantId = workflowDefinitionActiveVersion.TenantId,
                Status = WorkflowStatus.Running,
                Version = workflowDefinitionActiveVersion.Version,
                CorrelationId = correlationId,
                CreatedAt = clock.GetCurrentInstant(),
                DefinitionId = workflowDefinitionActiveVersion.DefinitionId
            };

            return Task.FromResult(workflowInstance);
        }
    }
}