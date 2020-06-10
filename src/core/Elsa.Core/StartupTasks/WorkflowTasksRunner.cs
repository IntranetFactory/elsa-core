using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Elsa.Persistence;
using Elsa.Services;

namespace Elsa.StartupTasks
{
    public class WorkflowTasksRunner : BackgroundService
    {
        private readonly IDistributedLockProvider distributedLockProvider;
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<WorkflowTasksRunner> logger;
        public WorkflowTasksRunner(
            IDistributedLockProvider distributedLockProvider,
            IServiceProvider serviceProvider,
            ILogger<WorkflowTasksRunner> logger)
        {
            this.distributedLockProvider = distributedLockProvider;
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = serviceProvider.CreateScope();
                var workflowInstanceTaskStore = scope.ServiceProvider.GetRequiredService<IWorkflowInstanceTaskStore>();
                // returns top 10 scheduled tasks - hardcoded number is for test purposes now
                var pendingTasks = await workflowInstanceTaskStore.GetTopScheduledTasksAsync(10, stoppingToken);

                if (pendingTasks.Count() == 0)
                {
                    await Task.Delay(30000, stoppingToken);
                    continue;
                }

                foreach (var task in pendingTasks)
                {
                    if (await distributedLockProvider.AcquireLockAsync(task.InstanceId, stoppingToken))
                    {
                        try
                        {
                            var workflowInstanceStore = scope.ServiceProvider.GetRequiredService<IWorkflowInstanceStore>();
                            var workflowHost = scope.ServiceProvider.GetRequiredService<IWorkflowHost>();
                            var workflowInstance = await workflowInstanceStore.GetByIdAsync(task.TenantId, task.InstanceId, stoppingToken);
                            await workflowHost.RunWorkflowInstanceAsync(workflowInstance, task.ActivityId, cancellationToken: stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, $"Exception occured while invoking workflows.");
                        }
                        finally
                        {
                            await distributedLockProvider.ReleaseLockAsync(task.InstanceId, stoppingToken);
                        }
                    }
                }
            }
        }
    }
}