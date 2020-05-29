using Elsa.Models;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;

namespace Elsa.Results
{
    public class ExecutionResult : ActivityExecutionResult
    {
        public ExecutionResult(WorkflowInstanceTaskStatus? status = default, string? tag = default, LocalizedString? message = default, IEnumerable<string>? outcomes = default, Variable? output = default)
        {
            var outcomeList = outcomes?.ToList() ?? new List<string>();
            _outcomes = outcomeList;

            Status = status;
            Tag = tag;
            Message = message;
            Output = output;
        }

        private IReadOnlyCollection<string> _outcomes;
        public WorkflowInstanceTaskStatus? Status { get; set; }
        public string? Tag { get; set; }
        public LocalizedString? Message { get; }
        public IReadOnlyCollection<string> Outcomes
        {
            get
            {
                if(!_outcomes.Any())
                {
                    var outcomeList = new List<string>(1);
                    outcomeList.Add(OutcomeNames.Done);
                    _outcomes = outcomeList;
                    return _outcomes;
                } else
                {
                    return _outcomes;
                }
            }
        }
        public Variable? Output { get; }
    }
}
