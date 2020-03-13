namespace Elsa.Persistence.EntityFrameworkCore.Entities
{
    public class BlockingActivityEntity
    {
        public int Id { get; set; }
        public string? TenantId { get; set; }
        public WorkflowInstanceEntity WorkflowInstance { get; set; }
        public string ActivityId { get; set; }
        public string ActivityType { get; set; }
        public string? Tag { get; set; }
    }
}