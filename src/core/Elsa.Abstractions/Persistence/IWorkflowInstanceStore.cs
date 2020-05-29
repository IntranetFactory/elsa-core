using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Models;

namespace Elsa.Persistence
{
    public interface IWorkflowInstanceStore
    {   
        Task<WorkflowInstance> SaveAsync(WorkflowInstance instance, CancellationToken cancellationToken = default);
        Task<WorkflowInstance> GetByIdAsync(int? tenantId, string id, CancellationToken cancellationToken = default);
        Task<WorkflowInstance> GetByCorrelationIdAsync(int? tenantId, string correlationId, CancellationToken cancellationToken = default);
        Task<IEnumerable<WorkflowInstance>> ListByDefinitionAsync(int? tenantId, string definitionId, CancellationToken cancellationToken = default);
        Task<IEnumerable<WorkflowInstance>> ListAllAsync(int? tenantId, CancellationToken cancellationToken = default);
        Task<IEnumerable<WorkflowInstance>> ListByStatusAsync(int? tenantId, string definitionId, WorkflowStatus status, CancellationToken cancellationToken = default);
        Task<IEnumerable<WorkflowInstance>> ListByStatusAsync(int? tenantId, WorkflowStatus status, CancellationToken cancellationToken = default);
        Task DeleteAsync(int? tenantId, string id, CancellationToken cancellationToken = default);
    }
}