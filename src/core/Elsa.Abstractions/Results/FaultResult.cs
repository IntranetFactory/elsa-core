using Elsa.Models;
using Elsa.Services.Models;
using Microsoft.Extensions.Localization;

namespace Elsa.Results
{
    public class FaultResult : ActivityExecutionResult
    {
        public FaultResult(LocalizedString message) => Message = message;
        public LocalizedString Message { get; }

        protected override void Execute(ActivityExecutionContext activityExecutionContext) => 
            activityExecutionContext.WorkflowExecutionContext.Fault(activityExecutionContext.Activity, Message);
    }
}