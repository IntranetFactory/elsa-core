namespace Elsa.Models
{
    public class WorkflowInstanceTask
    {
        public WorkflowInstanceTask()
        {
        }

        public WorkflowInstanceTask(string activityId, int? tenantId, WorkflowInstanceTaskStatus? status, Variable? input = default)
        {
            ActivityId = activityId;
            TenantId = tenantId;
            Status = status;
            Input = input;
        }
        public string? ActivityId { get; set; }
        public int? TenantId { get; set; }
        public Variable? Input { get; set; }
        public WorkflowInstanceTaskStatus? Status { get; set; }
    }
}