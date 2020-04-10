using System.Threading;
using System.Threading.Tasks;
using Elsa.Models;

namespace Elsa.Services
{
    public interface IWorkflowPublisher
    {
        WorkflowDefinitionVersion New(int? tenantId);
        
        Task<WorkflowDefinitionVersion> PublishAsync(int? tenantId, string id, CancellationToken cancellationToken = default);

        Task<WorkflowDefinitionVersion> PublishAsync(
            WorkflowDefinitionVersion workflowDefinitionVersion,
            CancellationToken cancellationToken = default);

        Task<WorkflowDefinitionVersion> GetDraftAsync(int? tenantId, string id, CancellationToken cancellationToken= default);

        Task<WorkflowDefinitionVersion> SaveDraftAsync(
            WorkflowDefinitionVersion workflowDefinitionVersion,
            CancellationToken cancellationToken = default);
    }
}