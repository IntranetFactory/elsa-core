using Elsa.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Elsa.Persistence
{
    public interface IWorkflowInstanceTaskStore
    {
        Task<WorkflowInstanceTask> SaveAsync(WorkflowInstanceTask task, CancellationToken cancellationToken = default);
        Task<WorkflowInstanceTask> GetByIdAsync(int? tenantId, string id, CancellationToken cancellationToken = default);
        Task<IEnumerable<WorkflowInstanceTask>> ListAllAsync(int? tenantId, CancellationToken cancellationToken = default);
        Task DeleteAsync(int? tenantId, string id, CancellationToken cancellationToken = default);
        Task<WorkflowInstanceTask> GetTopScheduledTask(CancellationToken cancellationToken = default);
    }
}
