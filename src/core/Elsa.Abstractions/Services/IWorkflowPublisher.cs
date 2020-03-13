using System.Threading;
using System.Threading.Tasks;
using Elsa.Models;

namespace Elsa.Services
{
    public interface IWorkflowPublisher
    {
        WorkflowDefinitionVersion New(string tenantId);
        
        Task<WorkflowDefinitionVersion> PublishAsync(string tenantId, string id, CancellationToken cancellationToken = default);

        Task<WorkflowDefinitionVersion> PublishAsync(
            WorkflowDefinitionVersion workflowDefinitionVersion,
            CancellationToken cancellationToken = default);

        Task<WorkflowDefinitionVersion> GetDraftAsync(string tenantId, string id, CancellationToken cancellationToken= default);

        Task<WorkflowDefinitionVersion> SaveDraftAsync(
            WorkflowDefinitionVersion workflowDefinitionVersion,
            CancellationToken cancellationToken = default);
    }
}