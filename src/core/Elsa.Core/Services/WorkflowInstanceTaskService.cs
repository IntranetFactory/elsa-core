using Elsa.Models;
using Elsa.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace Elsa.Services
{
    public class WorkflowInstanceTaskService : IWorkflowInstanceTaskService
    {
        private readonly IWorkflowInstanceTaskStore workflowInstanceTaskStore;
        public WorkflowInstanceTaskService(IWorkflowInstanceTaskStore workflowInstanceTaskStore)
        {
            this.workflowInstanceTaskStore = workflowInstanceTaskStore;
        }

        public async Task<WorkflowInstanceTask> Unblock(WorkflowInstanceTask task, CancellationToken cancellationToken = default)
        {
            task.Status = WorkflowInstanceTaskStatus.Resume;
            await workflowInstanceTaskStore.SaveAsync(task, cancellationToken);
            return task;
        }
    }
}
