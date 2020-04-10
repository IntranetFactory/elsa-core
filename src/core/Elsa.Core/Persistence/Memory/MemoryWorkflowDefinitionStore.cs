using Elsa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Elsa.Persistence.Memory
{
    public class MemoryWorkflowDefinitionStore : IWorkflowDefinitionStore
    {
        private readonly List<WorkflowDefinition> definitions;
        public MemoryWorkflowDefinitionStore()
        {
            definitions = new List<WorkflowDefinition>();
        }
        public async Task<WorkflowDefinition> SaveAsync(WorkflowDefinition definition, CancellationToken cancellationToken = default)
        {
            var existingDefinition = await GetByIdAsync(definition.TenantId, definition.Id, cancellationToken);

            if (existingDefinition == null)
                await AddAsync(definition, cancellationToken);
            else
                await UpdateAsync(definition, cancellationToken);

            return definition;
        }
        public async Task<WorkflowDefinition> AddAsync(WorkflowDefinition definition, CancellationToken cancellationToken = default)
        {
            var existingDefinition = await GetByIdAsync(definition.TenantId, definition.Id, cancellationToken);

            if (existingDefinition != null)
            {
                throw new ArgumentException($"A workflow definition with ID '{definition.Id}' already exists.");
            }

            definitions.Add(definition);
            return definition;
        }
        public Task<WorkflowDefinition> GetByIdAsync(int? tenantId, string id, CancellationToken cancellationToken = default)
        {
            var definition = definitions.FirstOrDefault(x => x.TenantId == tenantId && x.Id == id);
            return Task.FromResult(definition);
        }
        public Task<IEnumerable<WorkflowDefinition>> ListAsync(int? tenantId, CancellationToken cancellationToken = default)
        {
            var query = definitions.Where(x => x.TenantId == tenantId).AsQueryable();
            return Task.FromResult(query.AsEnumerable());
        }

        public async Task<WorkflowDefinition> UpdateAsync(WorkflowDefinition definition, CancellationToken cancellationToken = default)
        {
            var existingDefinition = await GetByIdAsync(definition.TenantId, definition.Id, cancellationToken);
            var index = definitions.IndexOf(existingDefinition);

            definitions[index] = definition;
            return definition;
        }

        public Task<int> DeleteAsync(int? tenantId, string id, CancellationToken cancellationToken = default)
        {
            var count = definitions.RemoveAll(x => x.TenantId == tenantId && x.Id == id);
            return Task.FromResult(count);
        }
    }
}
