using System.Collections.Generic;
using System.Linq;
using Elsa.Models;
using Elsa.Services;
using Elsa.Services.Models;
using WorkflowInstanceTask = Elsa.Services.Models.WorkflowInstanceTask;

namespace Elsa.Results
{
    public class WorkflowInstanceTasksResult : ActivityExecutionResult
    {
        public WorkflowInstanceTasksResult(IEnumerable<IActivity> activities, Variable? input = default)
        {
            Activities = activities.Select(x => new WorkflowInstanceTask(x, input));
        }

        public WorkflowInstanceTasksResult(IEnumerable<WorkflowInstanceTask> activities)
        {
            Activities = activities;
        }

        public IEnumerable<WorkflowInstanceTask> Activities { get; }

        protected override void Execute(ActivityExecutionContext activityExecutionContext)
        {
            activityExecutionContext.WorkflowExecutionContext.ScheduleWorkflowInstanceTasks(Activities);
        }
    }
}