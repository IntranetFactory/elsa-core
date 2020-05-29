using Elsa.Models;
using Elsa.Persistence;
using System.Threading;

namespace Elsa.Services
{
    public class WorkflowInstanceTaskService : IWorkflowInstanceTaskService
    {
        private readonly IWorkflowInstanceTaskStore workflowInstanceTaskStore;
        public WorkflowInstanceTaskService(IWorkflowInstanceTaskStore workflowInstanceTaskStore)
        {
            this.workflowInstanceTaskStore = workflowInstanceTaskStore;
        }
        public async void Unblock(int? tenantId, string taskId, CancellationToken cancellationToken = default)
        {
            var task = await workflowInstanceTaskStore.GetByIdAsync(tenantId, taskId, cancellationToken);

            if(task != null)
            {
                task.Status = WorkflowInstanceTaskStatus.Resume;
                await workflowInstanceTaskStore.SaveAsync(task, cancellationToken);
            }
        }
    }
}
