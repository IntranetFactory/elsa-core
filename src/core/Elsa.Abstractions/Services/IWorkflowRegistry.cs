using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Models;
using Elsa.Services.Models;

namespace Elsa.Services
{
    public interface IWorkflowRegistry
    {
        Task<IEnumerable<WorkflowDefinitionActiveVersion>> GetWorkflowDefinitionActiveVersionsAsync(int? tenantId, CancellationToken cancellationToken = default);
        Task<WorkflowDefinitionActiveVersion> GetWorkflowDefinitionActiveVersionAsync(int? tenantId, string id, VersionOptions version, CancellationToken cancellationToken = default);
    }
}