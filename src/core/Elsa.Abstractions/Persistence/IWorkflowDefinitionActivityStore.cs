using Elsa.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Elsa.Persistence
{
    public interface IWorkflowDefinitionActivityStore
    {
        Task<WorkflowDefinitionActivity> AddAsync(WorkflowDefinitionActivity activity, string? definitionId = default, CancellationToken cancellationToken = default);
        Task<WorkflowDefinitionActivity> GetByIdAsync(int? tenantId, string id, CancellationToken cancellationToken = default);
        Task<IEnumerable<WorkflowDefinitionActivity>> ListAsync(int? tenantId, CancellationToken cancellationToken = default);
        Task<WorkflowDefinitionActivity> UpdateAsync(WorkflowDefinitionActivity activity, CancellationToken cancellationToken = default);
        Task<int> DeleteAsync(int? tenantId, string id, CancellationToken cancellationToken = default);
    }
}
