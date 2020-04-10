using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Extensions;
using Elsa.Models;
using WorkflowInstance = Elsa.Models.WorkflowInstance;

namespace Elsa.Persistence.Memory
{
    public class MemoryWorkflowInstanceStore : IWorkflowInstanceStore
    {
        private readonly IDictionary<string, WorkflowInstance> workflowInstances =
            new ConcurrentDictionary<string, WorkflowInstance>();

        public Task<WorkflowInstance> SaveAsync(WorkflowInstance instance, CancellationToken cancellationToken)
        {
            workflowInstances[instance.Id] = instance;
            return Task.FromResult(instance);
        }

        public Task<WorkflowInstance> GetByIdAsync(int? tenantId, string id, CancellationToken cancellationToken)
        {

            if(workflowInstances.ContainsKey(id))
            {
                var instance = workflowInstances[id];

                if (instance.TenantId == tenantId)
                    return Task.FromResult(instance);
            }

            return default;
        }

        public Task<WorkflowInstance> GetByCorrelationIdAsync(
            int? tenantId, 
            string correlationId,
            CancellationToken cancellationToken = default)
        {
            var instance = workflowInstances.Values.FirstOrDefault(x => x.TenantId == tenantId && x.CorrelationId == correlationId);
            return Task.FromResult(instance);
        }

        public Task<IEnumerable<WorkflowInstance>> ListByDefinitionAsync(
            int? tenantId, 
            string definitionId,
            CancellationToken cancellationToken)
        {
            var workflows = workflowInstances.Values.Where(x => x.TenantId == tenantId && x.DefinitionId == definitionId);
            return Task.FromResult(workflows);
        }

        public Task<IEnumerable<WorkflowInstance>> ListAllAsync(int? tenantId, CancellationToken cancellationToken)
        {
            var workflows = workflowInstances.Values.Where(x => x.TenantId == tenantId).AsEnumerable();
            return Task.FromResult(workflows);
        }
        public Task<IEnumerable<(WorkflowInstance, BlockingActivity)>> ListByBlockingActivityTagAsync(
            int? tenantId, 
            string activityType,
            string tag,
            string? correlationId = null, 
            CancellationToken cancellationToken = default)
        {
            var query = workflowInstances.Values.AsQueryable();

            query = query.Where(x => x.Status == WorkflowStatus.Suspended && x.TenantId == tenantId);

            if (!string.IsNullOrWhiteSpace(correlationId))
                query = query.Where(x => x.CorrelationId == correlationId);

            query = query.Where(
                x => x.BlockingActivities.Any(y => y.ActivityType == activityType && y.Tag == tag && y.TenantId == tenantId)
            );

            return Task.FromResult(query.AsEnumerable().GetBlockingActivities());
        }

        public Task<IEnumerable<(WorkflowInstance, BlockingActivity)>> ListByBlockingActivityTagAsync(
            int? tenantId,
            string tag,
            string? correlationId = null,
            CancellationToken cancellationToken = default)
        {
            var query = workflowInstances.Values.AsQueryable();

            query = query.Where(x => x.Status == WorkflowStatus.Suspended && x.TenantId == tenantId);

            if (!string.IsNullOrWhiteSpace(correlationId))
                query = query.Where(x => x.CorrelationId == correlationId);

            query = query.Where(
                x => x.BlockingActivities.Any(y => y.Tag == tag && y.TenantId == tenantId)
            );

            return Task.FromResult(query.AsEnumerable().GetBlockingActivities());
        }

        public Task<IEnumerable<(WorkflowInstance, BlockingActivity)>> ListByBlockingActivityAsync(
            int? tenantId, 
            string activityType,
            string? correlationId = default, 
            CancellationToken cancellationToken = default)
        {
            var query = workflowInstances.Values.AsQueryable();

            query = query.Where(x => x.Status == WorkflowStatus.Suspended && x.TenantId == tenantId);

            if (!string.IsNullOrWhiteSpace(correlationId))
                query = query.Where(x => x.CorrelationId == correlationId);

            query = query.Where(
                x => x.BlockingActivities.Any(y => y.ActivityType == activityType && y.TenantId == tenantId)
            );

            return Task.FromResult(query.AsEnumerable().GetBlockingActivities());
        }

        public Task<IEnumerable<WorkflowInstance>> ListByStatusAsync(
            int? tenantId, 
            string definitionId,
            WorkflowStatus status,
            CancellationToken cancellationToken)
        {
            var query = workflowInstances.Values.Where(x => x.TenantId == tenantId && x.DefinitionId == definitionId && x.Status == status);
            return Task.FromResult(query);
        }

        public Task<IEnumerable<WorkflowInstance>> ListByStatusAsync(
            int? tenantId, 
            WorkflowStatus status,
            CancellationToken cancellationToken)
        {
            var query = workflowInstances.Values.Where(x => x.TenantId == tenantId && x.Status == status);
            return Task.FromResult(query);
        }

        public Task DeleteAsync(int? tenantId, string id, CancellationToken cancellationToken = default)
        {
            var instance = workflowInstances[id];

            if(instance.TenantId == tenantId)
                workflowInstances.Remove(id);

            return Task.CompletedTask;
        }
    }
}