namespace Elsa.Activities.Http.Models
{
    public class Signal
    {
        public Signal()
        {
        }

        public Signal(string tenantId, string name, string workflowInstanceId)
        {
            TenantId = tenantId;
            Name = name;
            WorkflowInstanceId = workflowInstanceId;
        }

        public string? TenantId { get; set; }
        public string Name { get; set; }
        public string WorkflowInstanceId { get; set; }
    }
}