using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Expressions;
using Elsa.Extensions;
using Elsa.Messaging.Domain;
using Elsa.Models;
using Elsa.Persistence;
using Elsa.Results;
using Elsa.Services.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using NodaTime;
using WorkflowInstanceTask = Elsa.Services.Models.WorkflowInstanceTask;

namespace Elsa.Services
{
    public class WorkflowHost : IWorkflowHost
    {
        private delegate Task<IActivityExecutionResult> ActivityOperation(ActivityExecutionContext activityExecutionContext, IActivity activity, CancellationToken cancellationToken);

        private static readonly ActivityOperation Execute = (context, activity, cancellationToken) => activity.ExecuteAsync(context, cancellationToken);
        private static readonly ActivityOperation Resume = (context, activity, cancellationToken) => activity.ResumeAsync(context, cancellationToken);

        private readonly IWorkflowRegistry workflowRegistry;
        private readonly IWorkflowInstanceStore workflowInstanceStore;
        private readonly IWorkflowDefinitionVersionStore workflowDefinitionVersionStore;
        private readonly IWorkflowActivator workflowActivator;
        private readonly IExpressionEvaluator expressionEvaluator;
        private readonly IClock clock;
        private readonly IMediator mediator;
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;

        public WorkflowHost(
            IWorkflowRegistry workflowRegistry,
            IWorkflowInstanceStore workflowInstanceStore,
            IWorkflowDefinitionVersionStore workflowDefinitionVersionStore,
            IWorkflowActivator workflowActivator,
            IExpressionEvaluator expressionEvaluator,
            IClock clock,
            IMediator mediator,
            IServiceProvider serviceProvider,
            ILogger<WorkflowHost> logger)
        {
            this.workflowRegistry = workflowRegistry;
            this.workflowInstanceStore = workflowInstanceStore;
            this.workflowDefinitionVersionStore = workflowDefinitionVersionStore;
            this.workflowActivator = workflowActivator;
            this.expressionEvaluator = expressionEvaluator;
            this.clock = clock;
            this.mediator = mediator;
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        public async Task<WorkflowExecutionContext?> RunWorkflowInstanceAsync(int? tenantId, string workflowInstanceId, string? activityId = default, object? input = default, CancellationToken cancellationToken = default)
        {
            var workflowInstance = await workflowInstanceStore.GetByIdAsync(tenantId, workflowInstanceId, cancellationToken);

            if (workflowInstance == null)
            {
                logger.LogDebug("Workflow instance {WorkflowInstanceId} does not exist.", workflowInstanceId);
                return null;
            }

            return await RunWorkflowInstanceAsync(workflowInstance, activityId, input, cancellationToken);
        }

        public async Task<WorkflowExecutionContext?> RunWorkflowInstanceAsync(WorkflowInstance workflowInstance, string? activityId = default, object? input = default, CancellationToken cancellationToken = default)
        {
            var workflowDefinitionActiveVersion = await workflowRegistry.GetWorkflowDefinitionActiveVersionAsync(workflowInstance.TenantId, workflowInstance.DefinitionId, VersionOptions.SpecificVersion(workflowInstance.Version), cancellationToken);
            return await RunAsync(workflowDefinitionActiveVersion, workflowInstance, activityId, input, cancellationToken);
        }

        public async Task<WorkflowExecutionContext> RunWorkflowDefinitionAsync(int? tenantId, string workflowDefinitionId, string? activityId, object? input = default, string? correlationId = default, CancellationToken cancellationToken = default)
        {
            var workflowDefinitionActiveVersion = await workflowRegistry.GetWorkflowDefinitionActiveVersionAsync(tenantId, workflowDefinitionId, VersionOptions.Published, cancellationToken);
            var workflowInstance = await workflowActivator.ActivateAsync(workflowDefinitionActiveVersion, correlationId, cancellationToken);

            return await RunAsync(workflowDefinitionActiveVersion, workflowInstance, activityId, input, cancellationToken);
        }
        public async Task<WorkflowExecutionContext> RunScheduledWorkflowInstanceAsync(int? tenantId, string instanceId)
        {
            var workflowInstance = await workflowInstanceStore.GetByIdAsync(tenantId, instanceId);
            var workflowDefinitionActiveVersion = await workflowRegistry.GetWorkflowDefinitionActiveVersionAsync(tenantId, workflowInstance.DefinitionId, VersionOptions.Published);
            return await RunAsync(workflowDefinitionActiveVersion, workflowInstance, workflowInstance.WorkflowInstanceTasks.Peek().ActivityId);
        }

        public async Task<WorkflowExecutionContext> WorkflowInstanceCreateAsync(int? tenantId, string workflowDefinitionId, string? correlationId = default, string? payload = default, CancellationToken cancellationToken = default)
        {
            var workflowDefinitionActiveVersion = await workflowRegistry.GetWorkflowDefinitionActiveVersionAsync(tenantId, workflowDefinitionId, VersionOptions.Published, cancellationToken);
            var workflowInstance = await workflowActivator.ActivateAsync(workflowDefinitionActiveVersion, correlationId, cancellationToken);

            if (!String.IsNullOrEmpty(payload)) workflowInstance.Payload = payload;

            var workflowExecutionContext = await CreateWorkflowExecutionContext(workflowDefinitionActiveVersion, workflowInstance);
            var activity = workflowExecutionContext.GetStartActivities().First();

            if (await CanExecuteAsync(workflowExecutionContext, activity, null, cancellationToken))
            {
                workflowExecutionContext.Status = WorkflowStatus.Scheduled;
                workflowExecutionContext.ScheduleWorkflowInstanceTask(activity, null);

                var workflowInstanceTask = workflowExecutionContext.NextScheduledWorkflowInstanceTask();
                var currentActivity = workflowInstanceTask.Activity;
                var activityExecutionContext = new ActivityExecutionContext(workflowExecutionContext, currentActivity, workflowInstanceTask.Input);

                //await mediator.Publish(new ActivityScheduled(activityExecutionContext), cancellationToken);
                await SaveWorkflowInstanceAsync(workflowExecutionContext, cancellationToken);
            }

            return workflowExecutionContext;
        }

        public async Task<WorkflowExecutionContext> RunWorkflowAsync(WorkflowDefinitionActiveVersion workflowDefinitionActiveVersion, string? activityId = default, object? input = default, string? correlationId = default, CancellationToken cancellationToken = default)
        {
            var workflowInstance = await workflowActivator.ActivateAsync(workflowDefinitionActiveVersion, correlationId, cancellationToken);
            return await RunAsync(workflowDefinitionActiveVersion, workflowInstance, activityId, input, cancellationToken);
        }

        private async Task<WorkflowExecutionContext> RunAsync(WorkflowDefinitionActiveVersion workflowDefinitionActiveVersion, WorkflowInstance workflowInstance, string? activityId = default, object? input = default, CancellationToken cancellationToken = default)
        {
            var workflowExecutionContext = await CreateWorkflowExecutionContext(workflowDefinitionActiveVersion, workflowInstance);

            await RunWorkflowAsync(workflowExecutionContext, cancellationToken);

            await mediator.Publish(new WorkflowExecuted(workflowExecutionContext), cancellationToken);

            var statusEvent = default(object);

            switch (workflowExecutionContext.Status)
            {
                case WorkflowStatus.Cancelled:
                    statusEvent = new WorkflowCancelled(workflowExecutionContext);
                    break;

                case WorkflowStatus.Completed:
                    statusEvent = new WorkflowCompleted(workflowExecutionContext);
                    break;

                case WorkflowStatus.Faulted:
                    statusEvent = new WorkflowFaulted(workflowExecutionContext);
                    break;

                case WorkflowStatus.Suspended:
                    statusEvent = new WorkflowSuspended(workflowExecutionContext);
                    break;
            }

            if (statusEvent != null) await mediator.Publish(statusEvent, cancellationToken);

            return workflowExecutionContext;
        }

        private async Task RunWorkflowAsync(WorkflowExecutionContext workflowExecutionContext, CancellationToken cancellationToken)
        {
            await RunAsync(workflowExecutionContext, cancellationToken);
        }

        private Task<bool> CanExecuteAsync(WorkflowExecutionContext workflowExecutionContext, IActivity activity, object? input, CancellationToken cancellationToken)
        {
            var activityExecutionContext = new ActivityExecutionContext(workflowExecutionContext, activity, Variable.From(input));
            return activity.CanExecuteAsync(activityExecutionContext, cancellationToken);
        }

        private async Task RunAsync(WorkflowExecutionContext workflowExecutionContext, CancellationToken cancellationToken = default)
        {
            int iterationCount = 0;

            while (workflowExecutionContext.HasWorkflowInstanceActiveTasks())
            {
                ActivityOperation activityOperation = null;
                var workflowInstanceTask = workflowExecutionContext.NextScheduledWorkflowInstanceTask();

                switch (workflowInstanceTask.Status)
                {
                    case WorkflowInstanceTaskStatus.Execute:
                        activityOperation = Execute;
                        break;

                    case WorkflowInstanceTaskStatus.Running:
                        activityOperation = Execute;
                        break;

                    case WorkflowInstanceTaskStatus.Resume:
                        activityOperation = Resume;
                        break;

                    case WorkflowInstanceTaskStatus.Faulted:
                        break;

                    case WorkflowInstanceTaskStatus.Blocked:
                        break;

                    case WorkflowInstanceTaskStatus.OnHold:
                        if (iterationCount == workflowInstanceTask.IterationCount) return;

                        activityOperation = Execute;
                        break;

                    case WorkflowInstanceTaskStatus.Scheduled:
                        activityOperation = Resume;
                        break;

                    case WorkflowInstanceTaskStatus.Completed:
                        break;
                }

                workflowExecutionContext.SetWorkflowInstanceTaskStatusToRunning();

                var currentActivity = workflowInstanceTask.Activity;
                var activityExecutionContext = new ActivityExecutionContext(workflowExecutionContext, currentActivity, workflowInstanceTask.Input);
                var result = await activityOperation(activityExecutionContext, currentActivity, cancellationToken);

                if (result is ExecutionResult)
                {
                    var executionResult = (ExecutionResult)result;

                    switch (executionResult.Status)
                    {
                        case WorkflowInstanceTaskStatus.Faulted:
                            workflowExecutionContext.SetWorkflowInstanceTaskStatusToFailed();
                            break;

                        case WorkflowInstanceTaskStatus.Blocked:
                            workflowExecutionContext.SetWorkflowInstanceTaskStatusToBlocked();
                            break;

                        case WorkflowInstanceTaskStatus.Completed:
                            workflowExecutionContext.PopScheduledWorkflowInstanceTask(currentActivity.Id);
                            break;

                        case WorkflowInstanceTaskStatus.OnHold:
                            workflowExecutionContext.SetWorkflowInstanceTaskStatusToOnHold();
                            break;

                        case WorkflowInstanceTaskStatus.Resume:
                            workflowExecutionContext.SetWorkflowInstanceTaskStatusToResume(Convert.ToDateTime(executionResult.Output.Value));
                            break;

                        case WorkflowInstanceTaskStatus.Scheduled:
                            workflowExecutionContext.SetWorkflowInstanceTaskStatusToScheduled(Convert.ToDateTime(executionResult.Output.Value));
                            break;
                    }

                    ExecuteActivityResult(activityExecutionContext, executionResult);
                }
                else
                {
                    if (result.GetType().Name == "FaultResult")
                    {
                        workflowExecutionContext.SetWorkflowInstanceTaskStatusToFailed();
                    }
                    else
                    {
                        workflowExecutionContext.PopScheduledWorkflowInstanceTask(currentActivity.Id);
                    }

                    await result.ExecuteAsync(activityExecutionContext, cancellationToken);
                }

                await SaveWorkflowInstanceAsync(workflowExecutionContext, cancellationToken);

                workflowExecutionContext.CompletePass();
                iterationCount++;
            }

            if (workflowExecutionContext.Status == WorkflowStatus.Running) workflowExecutionContext.Complete();

        }

        private WorkflowInstanceTask CreateScheduledWorkflowInstanceTask(Elsa.Models.WorkflowInstanceTask workflowInstanceTaskModel, IDictionary<string, IActivity> activityLookup)
        {
            var activity = activityLookup[workflowInstanceTaskModel.ActivityId];
            WorkflowInstanceTask task = new WorkflowInstanceTask(activity, workflowInstanceTaskModel.ActivityId);
            task.Tag = workflowInstanceTaskModel.Tag;
            task.Status = workflowInstanceTaskModel.Status;
            task.CreateDate = workflowInstanceTaskModel.CreateDate;
            task.ScheduleDate = workflowInstanceTaskModel.ScheduleDate;
            task.ExecutionDate = workflowInstanceTaskModel.ExecutionDate;
            return task;
        }

        private async Task<WorkflowExecutionContext> CreateWorkflowExecutionContext(WorkflowDefinitionActiveVersion workflowDefinitionActiveVersion, WorkflowInstance workflowInstance)
        {
            var activityLookup = workflowDefinitionActiveVersion.Activities.ToDictionary(x => x.Id);
            var workflowDefinitionVersion = await workflowDefinitionVersionStore.GetByIdAsync(workflowInstance.TenantId, workflowInstance.DefinitionId, VersionOptions.Latest);
            var workflowInstanceTasks = new Stack<WorkflowInstanceTask>(workflowInstance.WorkflowInstanceTasks.Reverse().Select(x => CreateScheduledWorkflowInstanceTask(x, activityLookup)));
            var variables = workflowInstance.Variables;
            var status = workflowInstance.Status;

            foreach (var activity in workflowDefinitionActiveVersion.Activities)
            {
                if (!workflowDefinitionVersion.Activities.Any(x => x.Id == activity.Id)) continue;

                var activityInstance = workflowDefinitionVersion.Activities.Where(x => x.Id == activity.Id).FirstOrDefault();

                if (activityInstance != null)
                {
                    activity.State = activityInstance.State;
                    activity.Output = activityInstance.Output;
                }
            }

            return CreateWorkflowExecutionContext(
                workflowInstance.Id,
                workflowInstance.TenantId,
                workflowDefinitionActiveVersion.DefinitionId,
                workflowDefinitionActiveVersion.Version,
                workflowDefinitionActiveVersion.Activities,
                workflowDefinitionActiveVersion.Connections,
                workflowInstanceTasks,
                workflowInstance.CorrelationId,
                variables,
                status);
        }

        private WorkflowExecutionContext CreateWorkflowExecutionContext(
            string workflowInstanceId,
            int? tenantId,
            string workflowDefinitionId,
            int version,
            IEnumerable<IActivity> activities,
            IEnumerable<Connection> connections,
            IEnumerable<WorkflowInstanceTask>? workflowInstanceTasks = default,
            string? correlationId = default,
            Variables? variables = default,
            WorkflowStatus status = WorkflowStatus.Running)
            => new WorkflowExecutionContext(
                expressionEvaluator,
                clock,
                serviceProvider,
                workflowDefinitionId,
                tenantId,
                workflowInstanceId,
                version,
                activities,
                connections,
                workflowInstanceTasks,
                correlationId,
                variables,
                status);
        private async Task SaveWorkflowInstanceAsync(WorkflowExecutionContext workflowExecutionContext, CancellationToken cancellationToken)
        {
            var workflowInstance = await workflowInstanceStore.GetByIdAsync(workflowExecutionContext.TenantId, workflowExecutionContext.InstanceId, cancellationToken);

            if (workflowInstance == null)
                workflowInstance = workflowExecutionContext.CreateWorkflowInstance();
            else
                workflowInstance = workflowExecutionContext.UpdateWorkflowInstance(workflowInstance);

            await workflowInstanceStore.SaveAsync(workflowInstance, cancellationToken);
        }

        private void ExecuteActivityResult(ActivityExecutionContext activityExecutionContext, ExecutionResult executionResult)
        {
            if (executionResult.Output != null) activityExecutionContext.Output = executionResult.Output;

            activityExecutionContext.Outcomes = executionResult.Outcomes.ToList();
            var workflowExecutionContext = activityExecutionContext.WorkflowExecutionContext;
            var nextActivities = GetNextActivities(workflowExecutionContext, activityExecutionContext.Activity, executionResult).ToList();
            workflowExecutionContext.ScheduleWorkflowInstanceTasks(nextActivities, executionResult.Output);
        }

        private IEnumerable<IActivity> GetNextActivities(WorkflowExecutionContext workflowContext, IActivity source, ExecutionResult executionResult)
        {
            // if any of the listed executionResult.Statuses happen - do not query for next activities
            switch (executionResult.Status)
            {
                case WorkflowInstanceTaskStatus.Blocked:
                    return new List<IActivity>();

                case WorkflowInstanceTaskStatus.OnHold:
                    return new List<IActivity>();

                case WorkflowInstanceTaskStatus.Faulted:
                    return new List<IActivity>();

                case WorkflowInstanceTaskStatus.Scheduled:
                    return new List<IActivity>();
            }

            IEnumerable<string> outcomes = executionResult.Outcomes;

            var query =
                from connection in workflowContext.Connections
                from outcome in outcomes
                where connection.Source.Activity == source && (connection.Source.Outcome ?? OutcomeNames.Done).Equals(outcome, StringComparison.OrdinalIgnoreCase)
                select connection.Target.Activity;

            return query.Distinct();
        }
    }
}