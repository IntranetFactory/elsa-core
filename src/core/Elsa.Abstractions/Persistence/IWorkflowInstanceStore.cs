using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Models;

namespace Elsa.Persistence
{
    public interface IWorkflowInstanceStore
    {   
        Task<WorkflowInstance> SaveAsync(WorkflowInstance instance, CancellationToken cancellationToken = default);
        Task<WorkflowInstance> GetByIdAsync(string tenantId, string id, CancellationToken cancellationToken = default);
        Task<WorkflowInstance> GetByCorrelationIdAsync(string tenantId, string correlationId, CancellationToken cancellationToken = default);
        Task<IEnumerable<WorkflowInstance>> ListByDefinitionAsync(string tenantId, string definitionId, CancellationToken cancellationToken = default);
        Task<IEnumerable<WorkflowInstance>> ListAllAsync(string tenantId, CancellationToken cancellationToken = default);
        Task<IEnumerable<(WorkflowInstance WorkflowInstance, BlockingActivity BlockingActivity)>> ListByBlockingActivityTagAsync(string tenantId, string activityType, string tag, string? correlationId = default, CancellationToken cancellationToken = default);
        Task<IEnumerable<(WorkflowInstance WorkflowInstance, BlockingActivity BlockingActivity)>> ListByBlockingActivityAsync(string tenantId, string activityType, string? correlationId = default, CancellationToken cancellationToken = default);
        Task<IEnumerable<WorkflowInstance>> ListByStatusAsync(string tenantId, string definitionId, WorkflowStatus status, CancellationToken cancellationToken = default);
        Task<IEnumerable<WorkflowInstance>> ListByStatusAsync(string tenantId, WorkflowStatus status, CancellationToken cancellationToken = default);
        Task DeleteAsync(string tenantId, string id, CancellationToken cancellationToken = default);
    }
}