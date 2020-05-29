using Elsa.Services.Models;

namespace Elsa.Results
{
    public class SuspendResult : ActivityExecutionResult
    {
        protected override void Execute(ActivityExecutionContext activityExecutionContext)
        {
            activityExecutionContext.WorkflowExecutionContext.Suspend();
        }
    }
}