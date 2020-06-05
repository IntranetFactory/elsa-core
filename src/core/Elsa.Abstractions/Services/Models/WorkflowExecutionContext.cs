using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Expressions;
using Elsa.Models;
using Microsoft.Extensions.Localization;
using NodaTime;

namespace Elsa.Services.Models
{
    public class WorkflowExecutionContext
    {
        public WorkflowExecutionContext(
            IExpressionEvaluator expressionEvaluator,
            IClock clock,
            IServiceProvider serviceProvider,
            string definitionId,
            int? tenantId,
            string instanceId,
            int version,
            IEnumerable<IActivity> activities,
            IEnumerable<Connection> connections,
            IEnumerable<WorkflowInstanceTask>? workflowInstanceTasks = default,
            string? correlationId = default,
            Variables? variables = default,
            WorkflowStatus status = WorkflowStatus.Running,
            WorkflowFault? workflowFault = default,
            IEnumerable<ExecutionLogEntry>? executionLog = default)
        {
            ServiceProvider = serviceProvider;
            DefinitionId = definitionId;
            TenantId = tenantId;
            InstanceId = instanceId;
            Version = version;
            CorrelationId = correlationId;
            Activities = activities.ToList();
            Connections = connections.ToList();
            ExpressionEvaluator = expressionEvaluator;
            Clock = clock;
            WorkflowInstanceTasks = workflowInstanceTasks != null ? new Stack<WorkflowInstanceTask>(workflowInstanceTasks) : new Stack<WorkflowInstanceTask>();
            Variables = variables ?? new Variables();
            Status = status;
            WorkflowFault = workflowFault;
            ExecutionLog = executionLog?.ToList() ?? new List<ExecutionLogEntry>();
            IsFirstPass = true;
        }

        public IServiceProvider ServiceProvider { get; }
        public string DefinitionId { get; }
        public int? TenantId { get; set; }
        public ICollection<IActivity> Activities { get; }
        public ICollection<Connection> Connections { get; }
        public WorkflowStatus Status { get; set; }
        public Stack<WorkflowInstanceTask> WorkflowInstanceTasks { get; }
        public Variables Variables { get; }
        public bool HasWorkflowInstanceActiveTasks()
        {
            int count = WorkflowInstanceTasks.Where(x => x.Status != WorkflowInstanceTaskStatus.Faulted && x.Status != WorkflowInstanceTaskStatus.Blocked).Count();

            if(count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public WorkflowInstanceTask? WorkflowInstanceTask { get; private set; }
        public WorkflowFault? WorkflowFault { get; private set; }
        public Variable? Output { get; set; }

        public void ScheduleWorkflowInstanceTasks(IEnumerable<IActivity> activities, Variable? input = default)
        {
            foreach (var activity in activities)
                ScheduleWorkflowInstanceTask(activity, input);
        }

        public void ScheduleWorkflowInstanceTasks(IEnumerable<WorkflowInstanceTask> activities)
        {
            foreach (var activity in activities)
                ScheduleWorkflowInstanceTask(activity);
        }

        public void ScheduleWorkflowInstanceTask(IActivity activity, object? input = default) => ScheduleWorkflowInstanceTask(new WorkflowInstanceTask(activity, input));
        public void ScheduleWorkflowInstanceTask(IActivity activity, Variable? input = default) => ScheduleWorkflowInstanceTask(new WorkflowInstanceTask(activity, input));
        public void ScheduleWorkflowInstanceTask(WorkflowInstanceTask activity)
        {
            // do not schedule an already scheduled task
            // this can happen during Join activity execution when Join executes but previous activities didn't finish
            // example: join doesn't complete -> return execution to previous activity -> previous activity executes -> next activities (join) are scheduled
            if (WorkflowInstanceTasks.Where(x => x.Activity.Id == activity.Activity.Id).Any())
                return;

            activity.Status = WorkflowInstanceTaskStatus.Execute;
            activity.ScheduleDate = DateTime.UtcNow;
            activity.CreateDate = DateTime.UtcNow;
            activity.Tag = activity.Activity.Tag;
            WorkflowInstanceTasks.Push(activity);
        }

        public WorkflowInstanceTask PopScheduledWorkflowInstanceTask() => WorkflowInstanceTask = WorkflowInstanceTasks.Pop();
        public WorkflowInstanceTask PeekScheduledWorkflowInstanceTask() => WorkflowInstanceTasks.Peek();
        public void SetWorkflowInstanceTaskStatusToRunning()
        {
            var task = WorkflowInstanceTasks.Pop();
            task.Status = WorkflowInstanceTaskStatus.Running;
            task.ExecutionDate = DateTime.UtcNow;
            WorkflowInstanceTasks.Push(task);
        }
        public void SetWorkflowInstanceTaskStatusToFailed()
        {
            var task = WorkflowInstanceTasks.Pop();
            task.Status = WorkflowInstanceTaskStatus.Faulted;
            WorkflowInstanceTasks.Push(task);
        }
        public void SetWorkflowInstanceTaskStatusToBlocked()
        {
            var task = WorkflowInstanceTasks.Pop();
            task.Status = WorkflowInstanceTaskStatus.Blocked;
            WorkflowInstanceTasks.Push(task);
        }
        public IExpressionEvaluator ExpressionEvaluator { get; }
        public IClock Clock { get; }
        public string InstanceId { get; set; }
        public int Version { get; }
        public string CorrelationId { get; set; }
        public ICollection<ExecutionLogEntry> ExecutionLog { get; }
        public bool IsFirstPass { get; private set; }
        public void SetVariable(string name, object value) => Variables.SetVariable(name, value);
        public T GetVariable<T>(string name) => (T)GetVariable(name);
        public object GetVariable(string name) => Variables.GetVariable(name);
        public void CompletePass() => IsFirstPass = false;

        public Task<T> EvaluateAsync<T>(IWorkflowExpression<T> expression, ActivityExecutionContext activityExecutionContext, CancellationToken cancellationToken) =>
            ExpressionEvaluator.EvaluateAsync(expression, activityExecutionContext, cancellationToken);

        public Task<object> EvaluateAsync(IWorkflowExpression expression, Type targetType, ActivityExecutionContext activityExecutionContext, CancellationToken cancellationToken) =>
            ExpressionEvaluator.EvaluateAsync(expression, targetType, activityExecutionContext, cancellationToken);

        public void Suspend()
        {
            Status = WorkflowStatus.Suspended;
        }

        public void Fault(IActivity? activity, LocalizedString? message)
        {
            Status = WorkflowStatus.Faulted;
            WorkflowFault = new WorkflowFault(activity, message);
        }

        public void Complete()
        {
            Status = WorkflowStatus.Completed;
        }

        public IActivity GetActivity(string id) => Activities.FirstOrDefault(x => x.Id == id);

        public WorkflowInstance CreateWorkflowInstance()
        {
            return UpdateWorkflowInstance(new WorkflowInstance
            {
                Id = InstanceId,
                TenantId = TenantId,
                DefinitionId = DefinitionId,
                CorrelationId = CorrelationId,
                Version = Version,
                CreatedAt = Clock.GetCurrentInstant()
            });
        }

        public WorkflowInstance UpdateWorkflowInstance(WorkflowInstance workflowInstance)
        {
            workflowInstance.Variables = Variables;
            workflowInstance.WorkflowInstanceTasks = new Stack<Elsa.Models.WorkflowInstanceTask>(WorkflowInstanceTasks.Select(x => new Elsa.Models.WorkflowInstanceTask(x.Activity.Id, workflowInstance.TenantId, x.Activity.Tag, x.Status, x.CreateDate, x.ScheduleDate, x.ExecutionDate, x.Input)));
            workflowInstance.Status = Status;
            workflowInstance.CorrelationId = CorrelationId;
            workflowInstance.Output = Output;

            var executionLog = workflowInstance.ExecutionLog.Concat(ExecutionLog.Select(x => new Elsa.Models.ExecutionLogEntry(x.Activity.Id, x.Timestamp)));
            workflowInstance.ExecutionLog = executionLog.ToList();

            if (WorkflowFault != null)
            {
                workflowInstance.Fault = new Elsa.Models.WorkflowFault
                {
                    FaultedActivityId = WorkflowFault.FaultedActivity?.Id,
                    Message = WorkflowFault.Message
                };
            }

            return workflowInstance;
        }
    }
}