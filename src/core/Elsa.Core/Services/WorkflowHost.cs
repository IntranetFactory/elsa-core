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
            return await RunAsync(workflowDefinitionActiveVersion, workflowInstance, workflowInstance.WorkflowInstanceTasks.Pop().ActivityId);
        }

        public async Task<WorkflowExecutionContext> WorkflowInstanceCreateAsync(int? tenantId, string workflowDefinitionId, string? correlationId = default, string? payload = default, CancellationToken cancellationToken = default)
        {
            var workflowDefinitionActiveVersion = await workflowRegistry.GetWorkflowDefinitionActiveVersionAsync(tenantId, workflowDefinitionId, VersionOptions.Published, cancellationToken);
            var workflowInstance = await workflowActivator.ActivateAsync(workflowDefinitionActiveVersion, correlationId, cancellationToken);

            if(!String.IsNullOrEmpty(payload))
            {
                workflowInstance.Payload = payload;
            }

            var workflowExecutionContext = await CreateWorkflowExecutionContext(workflowDefinitionActiveVersion, workflowInstance);
            var activity = workflowExecutionContext.GetStartActivities().First();

            if (await CanExecuteAsync(workflowExecutionContext, activity, null, cancellationToken))
            {
                workflowExecutionContext.Status = WorkflowStatus.Scheduled;
                workflowExecutionContext.ScheduleWorkflowInstanceTask(activity, null);

                var workflowInstanceTask = workflowExecutionContext.PeekScheduledWorkflowInstanceTask();
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
            var activity = activityId != null ? workflowDefinitionActiveVersion.GetActivity(activityId) : default;

            switch (workflowExecutionContext.Status)
            {
                case WorkflowStatus.Idle:
                    await BeginWorkflow(workflowExecutionContext, activity, input, cancellationToken);
                    break;
                case WorkflowStatus.Running:
                    await RunWorkflowAsync(workflowExecutionContext, cancellationToken);
                    break;
                case WorkflowStatus.Suspended:
                    await ResumeWorkflowAsync(workflowExecutionContext, activity, input, cancellationToken);
                    break;
                case WorkflowStatus.Scheduled:
                    await BeginWorkflow(workflowExecutionContext, activity, input, cancellationToken);
                    break;
            }

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

            if (statusEvent != null)
                await mediator.Publish(statusEvent, cancellationToken);
            
            return workflowExecutionContext;
        }

        private async Task BeginWorkflow(WorkflowExecutionContext workflowExecutionContext, IActivity? activity, object? input, CancellationToken cancellationToken)
        {
            if (activity == null)
                activity = workflowExecutionContext.GetStartActivities().First();

            if (!await CanExecuteAsync(workflowExecutionContext, activity, input, cancellationToken))
                return;

            workflowExecutionContext.Status = WorkflowStatus.Running;
            workflowExecutionContext.ScheduleWorkflowInstanceTask(activity, input);
            await RunAsync(workflowExecutionContext, Execute, cancellationToken);
        }

        private async Task RunWorkflowAsync(WorkflowExecutionContext workflowExecutionContext, CancellationToken cancellationToken)
        {
            await RunAsync(workflowExecutionContext, Execute, cancellationToken);
        }

        private async Task ResumeWorkflowAsync(WorkflowExecutionContext workflowExecutionContext, IActivity activity, object? input, CancellationToken cancellationToken)
        {
            if (!await CanExecuteAsync(workflowExecutionContext, activity, input, cancellationToken))
                return;
            
            workflowExecutionContext.WorkflowInstanceBlockingActivities.Remove(activity);
            workflowExecutionContext.Status = WorkflowStatus.Running;
            workflowExecutionContext.ScheduleWorkflowInstanceTask(activity, input);
            await RunAsync(workflowExecutionContext, Resume, cancellationToken);
        }
        
        private Task<bool> CanExecuteAsync(WorkflowExecutionContext workflowExecutionContext, IActivity activity, object? input, CancellationToken cancellationToken)
        {
            var activityExecutionContext = new ActivityExecutionContext(workflowExecutionContext, activity, Variable.From(input));
            return activity.CanExecuteAsync(activityExecutionContext, cancellationToken);
        }

        private async Task RunAsync(
            WorkflowExecutionContext workflowExecutionContext,
            ActivityOperation activityOperation,
            CancellationToken cancellationToken = default)
        {

            while (workflowExecutionContext.HasWorkflowInstanceTasks())
            {
                var workflowInstanceTask = workflowExecutionContext.PeekScheduledWorkflowInstanceTask();
                workflowExecutionContext.SetWorkflowInstanceTaskStatusToRunning();

                var currentActivity = workflowInstanceTask.Activity;
                var activityExecutionContext = new ActivityExecutionContext(workflowExecutionContext, currentActivity, workflowInstanceTask.Input);
                var result = await activityOperation(activityExecutionContext, currentActivity, cancellationToken);

                if(result is ExecutionResult)
                {
                    var executionResult = (ExecutionResult)result;

                    if(executionResult.Status == WorkflowInstanceTaskStatus.Completed)
                    {
                        workflowExecutionContext.PopScheduledWorkflowInstanceTask();
                    }

                    if(executionResult.Status == WorkflowInstanceTaskStatus.Faulted)
                    {
                        workflowExecutionContext.SetWorkflowInstanceTaskStatusToFailed();
                    }

                    ExecuteActivityResult(activityExecutionContext, executionResult);
                }
                else
                {
                    //await mediator.Publish(new ActivityExecuting(activityExecutionContext), cancellationToken);

                    if (result.GetType().Name == "FaultResult")
                    {
                        workflowExecutionContext.SetWorkflowInstanceTaskStatusToFailed();
                    }
                    else
                    {
                        workflowExecutionContext.PopScheduledWorkflowInstanceTask();
                    }

                    await result.ExecuteAsync(activityExecutionContext, cancellationToken);
                }

                //await mediator.Publish(new ActivityExecuted(activityExecutionContext), cancellationToken);
                await SaveWorkflowInstanceAsync(workflowExecutionContext, cancellationToken);

                activityOperation = Execute;
                workflowExecutionContext.CompletePass();
            }

            if (workflowExecutionContext.Status == WorkflowStatus.Running)
                workflowExecutionContext.Complete();
        }

        private WorkflowInstanceTask CreateScheduledWorkflowInstanceTask(Elsa.Models.WorkflowInstanceTask workflowInstanceTaskModel, IDictionary<string, IActivity> activityLookup)
        {
            var activity = activityLookup[workflowInstanceTaskModel.ActivityId];
            return new WorkflowInstanceTask(activity, workflowInstanceTaskModel.Input);
        }

        private async Task<WorkflowExecutionContext> CreateWorkflowExecutionContext(WorkflowDefinitionActiveVersion workflowDefinitionActiveVersion, WorkflowInstance workflowInstance)
        {
            var activityLookup = workflowDefinitionActiveVersion.Activities.ToDictionary(x => x.Id);
            var workflowDefinitionVersion = await workflowDefinitionVersionStore.GetByIdAsync(workflowInstance.TenantId, workflowInstance.DefinitionId, VersionOptions.Latest);
            var workflowInstanceTasks = new Stack<WorkflowInstanceTask>(workflowInstance.WorkflowInstanceTasks.Reverse().Select(x => CreateScheduledWorkflowInstanceTask(x, activityLookup)));
            var workflowInstanceBlockingActivities = new HashSet<IActivity>(workflowInstance.WorkflowInstanceBlockingActivities.Select(x => activityLookup[x.ActivityId]));
            var variables = workflowInstance.Variables;
            var status = workflowInstance.Status;

            foreach (var activity in workflowDefinitionActiveVersion.Activities)
            {
                if (!workflowDefinitionVersion.Activities.Any(x => x.Id == activity.Id))
                    continue;

                var activityInstance = workflowDefinitionVersion.Activities.Where(x => x.Id == activity.Id).FirstOrDefault();

                if(activityInstance != null)
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
                workflowInstanceBlockingActivities,
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
            IEnumerable<IActivity>? workflowInstanceBlockingActivities = default,
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
                workflowInstanceBlockingActivities,
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
            if (executionResult.Output != null)
                activityExecutionContext.Output = executionResult.Output;

            activityExecutionContext.Outcomes = executionResult.Outcomes.ToList();

            var workflowExecutionContext = activityExecutionContext.WorkflowExecutionContext;
            var nextActivities = GetNextActivities(workflowExecutionContext, activityExecutionContext.Activity, executionResult.Outcomes).ToList();

            workflowExecutionContext.ScheduleWorkflowInstanceTasks(nextActivities, executionResult.Output);
        }

        private IEnumerable<IActivity> GetNextActivities(WorkflowExecutionContext workflowContext, IActivity source, IEnumerable<string> outcomes)
        {
            var query =
                from connection in workflowContext.Connections
                from outcome in outcomes
                where connection.Source.Activity == source && (connection.Source.Outcome ?? OutcomeNames.Done).Equals(outcome, StringComparison.OrdinalIgnoreCase)
                select connection.Target.Activity;

            return query.Distinct();
        }
    }
}