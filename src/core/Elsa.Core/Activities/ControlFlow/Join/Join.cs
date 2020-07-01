using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Attributes;
using Elsa.Design;
using Elsa.Extensions;
using Elsa.Messaging.Domain;
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
        RuntimeDescription = "x => !!x.state.joinMode.value ? `Merge workflow execution back into a single branch using mode <strong>${ x.state.joinMode }</strong>` : x.definition.description",
        Outcomes = new[] { OutcomeNames.Done }
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

            if (!allDone)
                return ExecutionResult(WorkflowStatus.OnHold);

            return ExecutionResult(WorkflowStatus.Completed);
        }
    }
}