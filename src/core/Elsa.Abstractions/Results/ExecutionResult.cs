using Elsa.Models;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;

namespace Elsa.Results
{
    public class ExecutionResult : ActivityExecutionResult
    {
        public ExecutionResult(WorkflowInstanceTaskStatus status, LocalizedString? message = default, IEnumerable<string>? outcomes = default, Variable? output = default)
        {
            var outcomeList = outcomes?.ToList() ?? new List<string>(1);

            if (!outcomeList.Any())
                outcomeList.Add(OutcomeNames.Done);
            
            Status = status;
            Message = message;
            Outcomes = outcomeList;
            Output = output;
        }
        public WorkflowInstanceTaskStatus Status { get; set; }
        public LocalizedString? Message { get; }
        public IReadOnlyCollection<string> Outcomes { get; }
        public Variable? Output { get; }
    }
}
