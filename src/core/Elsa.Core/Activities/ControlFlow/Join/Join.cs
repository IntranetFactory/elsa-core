using System.Linq;
using Elsa.Attributes;
using Elsa.Models;
using Elsa.Results;
using Elsa.Services;
using Elsa.Services.Models;

// ReSharper disable once CheckNamespace
namespace Elsa.Activities.ControlFlow
{
    [WorkflowDefinitionActivity(
        Category = "Control Flow",
        Description = "Merge workflow execution back into a single branch.",
        Icon = "fas fa-code-branch",
        RuntimeDescription = "x => x.definition.description",
        Outcomes = new[] { OutcomeNames.Done },
        AllowEdit = false
    )]
    public class Join : Activity
    {
        public Join()
        {
        }

        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
        {
            var workflowExecutionContext = context.WorkflowExecutionContext;
            var allDone = true;
            var checkedActivityIdList = workflowExecutionContext.EnumerateParents(this.Id);

            foreach(var checkedActivityId in checkedActivityIdList)
            {
                if (workflowExecutionContext.WorkflowInstanceTaskStack.Where(x => x.Activity.Id == checkedActivityId).Any()) allDone = false;
            }

            if (!allDone) return ExecutionResult(WorkflowStatus.OnHold);
            return ExecutionResult(WorkflowStatus.Completed);
        }
    }
}