using System.Threading;
using System.Threading.Tasks;
using Elsa.Models;
using Elsa.Services.Models;

namespace Elsa.Services
{
    public interface IWorkflowActivator
    {
        Task<WorkflowInstance> ActivateAsync(
            int? tenantId,
            string definitionId, 
            string? correlationId = default, 
            CancellationToken cancellationToken = default);
        
        Task<WorkflowInstance> ActivateAsync(
            WorkflowDefinitionActiveVersion workflowDefinitionActiveVersion, 
            string? correlationId = default, 
            CancellationToken cancellationToken = default);
    }
}