using Elsa.Models;
using Elsa.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Elsa.Services
{
    public class WorkflowTaskRunner : IWorkflowTaskRunner
    {
        private readonly IWorkflowInstanceStore workflowInstanceStore;
        private readonly IWorkflowInstanceTaskStore workflowInstanceTaskStore;
        private readonly IWorkflowHost workflowHost;

        public WorkflowTaskRunner(IWorkflowInstanceTaskStore workflowInstanceTaskStore,
            IWorkflowInstanceStore workflowInstanceStore,
            IWorkflowHost workflowHost)
        {
            this.workflowInstanceStore = workflowInstanceStore;
            this.workflowInstanceTaskStore = workflowInstanceTaskStore;
            this.workflowHost = workflowHost;
        }
        public async Task<WorkflowInstanceTask> RunWorkflowInstanceTaskAsync(CancellationToken cancellationToken = default)
        {
            var task = await workflowInstanceTaskStore.GetTopScheduledTask(cancellationToken);

            if (task == null)
                return null;

            var workflowInstance = await workflowInstanceStore.GetByIdAsync(task.TenantId, task.InstanceId, cancellationToken);

            // check if instance is already locked, if yes skip, get next task
            // lock instance

            // TO DO: this should probably be modified once the WorkflowHost is changed to depend on WorkflowInstanceTaskStatus instead of WorkflowInstance.Status
            await workflowHost.RunWorkflowInstanceAsync(workflowInstance, task.ActivityId, cancellationToken: cancellationToken);

            // unlock instance 

            return task;
        }
    }
}
