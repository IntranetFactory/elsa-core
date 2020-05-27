using Elsa.Models;
using Elsa.Services.Models;
using Microsoft.Extensions.Localization;

namespace Elsa.Results
{
    public class FaultResult : ActivityExecutionResult
    {
        public FaultResult(LocalizedString message, WorkflowInstanceTaskStatus status)
        {
            Message = message;
            Status = status;
        }
        public LocalizedString Message { get; }
        public WorkflowInstanceTaskStatus Status { get; set; }

        protected override void Execute(ActivityExecutionContext activityExecutionContext) => 
            activityExecutionContext.WorkflowExecutionContext.Fault(activityExecutionContext.Activity, Message);
    }
}