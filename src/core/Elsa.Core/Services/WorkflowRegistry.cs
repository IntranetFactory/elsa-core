using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Extensions;
using Elsa.Models;
using Elsa.Services.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Elsa.Services
{
    public class WorkflowRegistry : IWorkflowRegistry
    {
        private readonly IServiceProvider serviceProvider;

        public WorkflowRegistry(
            IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        
        public async Task<IEnumerable<WorkflowDefinitionActiveVersion>> GetWorkflowDefinitionActiveVersionsAsync(int? tenantId, CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();
            var providers = scope.ServiceProvider.GetServices<IWorkflowProvider>();
            var tasks = await Task.WhenAll(providers.Select(x => x.GetWorkflowDefinitionActiveVersionsAsync(tenantId, cancellationToken)));
            return tasks.SelectMany(x => x).ToList();
        }

        public async Task<WorkflowDefinitionActiveVersion> GetWorkflowDefinitionActiveVersionAsync(int? tenantId, string id, VersionOptions version, CancellationToken cancellationToken)
        {
            var workflowDefinitionActiveVersions = await GetWorkflowDefinitionActiveVersionsAsync(tenantId, cancellationToken).ToListAsync();

            return workflowDefinitionActiveVersions
                .Where(x => x.DefinitionId == id)
                .OrderByDescending(x => x.Version)
                .WithVersion(version).FirstOrDefault();
        }
    }
}