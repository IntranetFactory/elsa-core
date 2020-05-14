using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Comparers;
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
            IEnumerable<IActivity>? workflowInstanceBlockingActivities = default,
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
            WorkflowInstanceBlockingActivities = workflowInstanceBlockingActivities != null ? new HashSet<IActivity>(workflowInstanceBlockingActivities) : new HashSet<IActivity>();
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
        public HashSet<IActivity> WorkflowInstanceBlockingActivities { get; }
        public Variables Variables { get; }
        public bool HasWorkflowInstanceTasks => WorkflowInstanceTasks.Any();
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
        public void ScheduleWorkflowInstanceTask(WorkflowInstanceTask activity) => WorkflowInstanceTasks.Push(activity);

        public WorkflowInstanceTask PopScheduledWorkflowInstanceTask() => WorkflowInstanceTask = WorkflowInstanceTasks.Pop();
        public WorkflowInstanceTask PeekScheduledWorkflowInstanceTask() => WorkflowInstanceTasks.Peek();
        public IExpressionEvaluator ExpressionEvaluator { get; }
        public IClock Clock { get; }
        public string InstanceId { get; set; }
        public int Version { get; }
        public string CorrelationId { get; set; }
        public ICollection<ExecutionLogEntry> ExecutionLog { get; }
        public bool IsFirstPass { get; private set; }

        public bool AddBlockingActivity(IActivity activity) => WorkflowInstanceBlockingActivities.Add(activity);
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
            workflowInstance.WorkflowInstanceTasks = new Stack<Elsa.Models.WorkflowInstanceTask>(WorkflowInstanceTasks.Select(x => new Elsa.Models.WorkflowInstanceTask(x.Activity.Id, workflowInstance.TenantId, x.Input)));
            workflowInstance.WorkflowInstanceBlockingActivities = new HashSet<WorkflowInstanceBlockingActivity>(WorkflowInstanceBlockingActivities.Select(x => new WorkflowInstanceBlockingActivity(x.Id, workflowInstance.TenantId, x.Type, x.Tag)), new WorkflowInstanceBlockingActivityEqualityComparer());
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