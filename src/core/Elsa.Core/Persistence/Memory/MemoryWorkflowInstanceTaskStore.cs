using Elsa.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Elsa.Persistence.Memory
{
    public class MemoryWorkflowInstanceTaskStore : IWorkflowInstanceTaskStore
    {
        private readonly IDictionary<string, WorkflowInstanceTask> workflowInstanceTasks =
            new ConcurrentDictionary<string, WorkflowInstanceTask>();

        public Task<WorkflowInstanceTask> SaveAsync(WorkflowInstanceTask task, CancellationToken cancellationToken = default)
        {
            workflowInstanceTasks[task.ActivityId] = task;
            return Task.FromResult(task);
        }

        public Task<WorkflowInstanceTask> GetByIdAsync(int? tenantId, string id, CancellationToken cancellationToken = default)
        {
            if (workflowInstanceTasks.ContainsKey(id))
            {
                var task = workflowInstanceTasks[id];

                if (task.TenantId == tenantId)
                    return Task.FromResult(task);
            }

            return default;
        }

        public Task<IEnumerable<WorkflowInstanceTask>> ListAllAsync(int? tenantId, CancellationToken cancellationToken = default)
        {
            var workflowInstanceTasksList = workflowInstanceTasks.Values.Where(x => x.TenantId == tenantId).AsEnumerable();
            return Task.FromResult(workflowInstanceTasksList);
        }

        public Task DeleteAsync(int? tenantId, string id, CancellationToken cancellationToken = default)
        {
            var task = workflowInstanceTasks[id];

            if (task.TenantId == tenantId)
                workflowInstanceTasks.Remove(id);

            return Task.CompletedTask;
        }

        public Task<WorkflowInstanceTask> GetTopScheduledTask(CancellationToken cancellationToken = default)
        {
            var task = workflowInstanceTasks.Values.Where(x => x.Status == WorkflowInstanceTaskStatus.Execute || x.Status == WorkflowInstanceTaskStatus.Resume).OrderByDescending(x => x.ScheduleDate).FirstOrDefault();
            return Task.FromResult(task);
        }
    }
}
