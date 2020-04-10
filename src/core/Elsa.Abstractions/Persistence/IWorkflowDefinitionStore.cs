using Elsa.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Elsa.Persistence
{
    public interface IWorkflowDefinitionStore
    {
        Task<WorkflowDefinition> SaveAsync(WorkflowDefinition definition, CancellationToken cancellationToken = default);
        Task<WorkflowDefinition> AddAsync(WorkflowDefinition definition, CancellationToken cancellationToken = default);
        Task<WorkflowDefinition> GetByIdAsync(int? tenantId, string id, CancellationToken cancellationToken = default);
        Task<IEnumerable<WorkflowDefinition>> ListAsync(int? tenantId, CancellationToken cancellationToken = default);
        Task<WorkflowDefinition> UpdateAsync(WorkflowDefinition definition, CancellationToken cancellationToken = default);
        Task<int> DeleteAsync(int? tenantId, string id, CancellationToken cancellationToken = default);
    }
}
