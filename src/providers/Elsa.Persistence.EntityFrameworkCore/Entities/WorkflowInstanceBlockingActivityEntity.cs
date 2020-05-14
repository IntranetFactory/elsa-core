namespace Elsa.Persistence.EntityFrameworkCore.Entities
{
    public class WorkflowInstanceBlockingActivityEntity
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }
        public WorkflowInstanceEntity WorkflowInstance { get; set; }
        public string ActivityId { get; set; }
        public string ActivityType { get; set; }
        public string? Tag { get; set; }
    }
}