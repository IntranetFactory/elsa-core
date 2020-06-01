using Elsa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Elsa.Persistence.Memory
{
    public class MemoryWorkflowDefinitionActivityStore : IWorkflowDefinitionActivityStore
    {
        private readonly List<WorkflowDefinitionActivity> activities;
        public MemoryWorkflowDefinitionActivityStore()
        {
            activities = new List<WorkflowDefinitionActivity>();
        }

        public async Task<WorkflowDefinitionActivity> AddAsync(WorkflowDefinitionActivity activity, string? definitionId = default, CancellationToken cancellationToken = default)
        {
            var existingActivityDefinition = await GetByIdAsync(activity.TenantId, activity.Id, cancellationToken);

            if (existingActivityDefinition != null)
            {
                throw new ArgumentException($"A workflow definition activity with ID '{activity.Id}' already exists.");
            }

            activities.Add(activity);
            return activity;
        }

        public Task<WorkflowDefinitionActivity> GetByIdAsync(int? tenantId, string id, CancellationToken cancellationToken = default)
        {
            var activity = activities.FirstOrDefault(x => x.TenantId == tenantId && x.Id == id);
            return Task.FromResult(activity);
        }

        public async Task<IEnumerable<WorkflowDefinitionActivity>> ListAsync(int? tenantId, CancellationToken cancellationToken = default)
        {
            return activities.Where(x => x.TenantId == tenantId).AsEnumerable();
        }

        public async Task<WorkflowDefinitionActivity> UpdateAsync(WorkflowDefinitionActivity activity, CancellationToken cancellationToken = default)
        {
            var existingActivity = await GetByIdAsync(activity.TenantId, activity.Id, cancellationToken);
            var index = activities.IndexOf(existingActivity);

            activities[index] = activity;
            return activity;
        }

        public Task<int> DeleteAsync(int? tenantId, string id, CancellationToken cancellationToken = default)
        {
            var count = activities.RemoveAll(x => x.TenantId == tenantId && x.Id == id);
            return Task.FromResult(count);
        }

        
    }
}
