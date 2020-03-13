using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Models;
using Elsa.Services.Models;

namespace Elsa.Services
{
    public interface IWorkflowRegistry
    {
        Task<IEnumerable<Workflow>> GetWorkflowsAsync(string tenantId, CancellationToken cancellationToken = default);
        Task<Workflow> GetWorkflowAsync(string tenantId, string id, VersionOptions version, CancellationToken cancellationToken = default);
    }
}