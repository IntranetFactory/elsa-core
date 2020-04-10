using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Models;

namespace Elsa.Persistence
{
    public interface IWorkflowDefinitionVersionStore
    {
        Task<WorkflowDefinitionVersion> SaveAsync(WorkflowDefinitionVersion definitionVersion, CancellationToken cancellationToken = default);
        Task<WorkflowDefinitionVersion> AddAsync(WorkflowDefinitionVersion definitionVersion, CancellationToken cancellationToken = default);
        Task<WorkflowDefinitionVersion> GetByIdAsync(int? tenantId, string id, CancellationToken cancellationToken = default);
        Task<WorkflowDefinitionVersion> GetByIdAsync(int? tenantId, string definitionId, VersionOptions version, CancellationToken cancellationToken = default);
        Task<IEnumerable<WorkflowDefinitionVersion>> ListAsync(int? tenantId, VersionOptions version, CancellationToken cancellationToken = default);
        Task<WorkflowDefinitionVersion> UpdateAsync(WorkflowDefinitionVersion definitionVersion, CancellationToken cancellationToken = default);
        Task<int> DeleteAsync(int? tenantId, string id, CancellationToken cancellationToken = default);
    }
}