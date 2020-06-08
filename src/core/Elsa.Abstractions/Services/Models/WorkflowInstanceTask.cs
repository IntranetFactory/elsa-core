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
        public string? Tag { get; set; }
        public IActivity? Activity { get; }
        public Variable? Input { get; }
        public WorkflowInstanceTaskStatus? Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public DateTime? ExecutionDate { get; set; }
        public int IterationCount { get; set; }
    }
}