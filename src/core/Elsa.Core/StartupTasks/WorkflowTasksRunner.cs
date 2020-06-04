using System;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Persistence;
using Elsa.Runtime;
using Elsa.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Elsa.StartupTasks
{
    public class WorkflowTasksRunner : IStartupTask
    {
        private readonly IWorkflowInstanceStore workflowInstanceStore;
        private readonly IWorkflowInstanceTaskStore workflowInstanceTaskStore;
        private readonly IDistributedLockProvider distributedLockProvider;
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<WorkflowTasksRunner> logger;

        public WorkflowTasksRunner(
            IWorkflowInstanceStore workflowInstanceStore,
            IWorkflowInstanceTaskStore workflowInstanceTaskStore,
            IDistributedLockProvider distributedLockProvider,
            IServiceProvider serviceProvider,
            ILogger<WorkflowTasksRunner> logger)
        {
            this.workflowInstanceStore = workflowInstanceStore;
            this.workflowInstanceTaskStore = workflowInstanceTaskStore;
            this.distributedLockProvider = distributedLockProvider;
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        public async Task ExecuteAsync(int? tenantId, CancellationToken stoppingToken)
        {
            // TO DO: implement locking
            bool recordsFound = true;

            do
            {
                using var scope = serviceProvider.CreateScope();
                var workflowTaskRunner = scope.ServiceProvider.GetRequiredService<IWorkflowTaskRunner>();
                var task = await workflowTaskRunner.RunWorkflowInstanceTaskAsync(stoppingToken);

                if (task == null)
                    recordsFound = false;

            } while (recordsFound == true);
        }
    }
}