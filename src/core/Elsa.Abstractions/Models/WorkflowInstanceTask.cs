namespace Elsa.Models
{
    public class WorkflowInstanceTask
    {
        public WorkflowInstanceTask()
        {
        }

        public WorkflowInstanceTask(string activityId, int? tenantId, Variable? input = default)
        {
            ActivityId = activityId;
            TenantId = tenantId;
            Input = input;
        }
        public string? ActivityId { get; set; }
        public int? TenantId { get; set; }
        public Variable? Input { get; set; }
    }
}