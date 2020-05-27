using Elsa.Models;
using System;

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
        public DateTime? CreateDate { get; set; }
        public DateTime? ScheduleDate { get; set; }
        // TO DO: ExecutionDate is not yet working as the task is removed from the table when executed
        public DateTime? ExecutionDate { get; set; }
    }
}