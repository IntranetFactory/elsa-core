using Elsa.Models;

namespace Elsa.Services.Models
{
    public class WorkflowInstanceTask
    {
        public WorkflowInstanceTask(IActivity activity, object? input = default) : this(activity, Variable.From(input))
        {
        }
        
        public WorkflowInstanceTask(IActivity activity, Variable? input = default)
        {
            Activity = activity;
            Input = input;
        }
        
        public IActivity? Activity { get; }
        public Variable? Input { get; }
        public WorkflowInstanceTaskStatus? Status { get; set; }
    }
}