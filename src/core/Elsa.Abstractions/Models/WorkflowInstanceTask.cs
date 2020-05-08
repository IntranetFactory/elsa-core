namespace Elsa.Models
{
    public class WorkflowInstanceTask
    {
        public WorkflowInstanceTask()
        {
        }

        public WorkflowInstanceTask(string id, int? tenantId, string type, string tag, Variables state, Variable? output)
        {
            Id = id;
            TenantId = tenantId;
            Type = type;
            Tag = tag;
            State = state;
            Output = output;
        }
        
        public string? Id { get; set; }
        public int? TenantId { get; set; }
        public string ActivityId { get; set; }
        public string? Type { get; set; }
        public string Tag { get; set; }
        public Variables? State { get; set; }
        public Variable? Output { get; set; }
    }
}