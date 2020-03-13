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
        Task<WorkflowDefinitionVersion> GetByIdAsync(string tenantId, string id, CancellationToken cancellationToken = default);
        Task<WorkflowDefinitionVersion> GetByIdAsync(string tenantId, string definitionId, VersionOptions version, CancellationToken cancellationToken = default);
        Task<IEnumerable<WorkflowDefinitionVersion>> ListAsync(string tenantId, VersionOptions version, CancellationToken cancellationToken = default);
        Task<WorkflowDefinitionVersion> UpdateAsync(WorkflowDefinitionVersion definitionVersion, CancellationToken cancellationToken = default);
        Task<int> DeleteAsync(string tenantId, string id, CancellationToken cancellationToken = default);
    }
}