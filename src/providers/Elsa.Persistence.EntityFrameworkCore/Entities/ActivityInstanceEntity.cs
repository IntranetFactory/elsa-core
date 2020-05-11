using Elsa.Models;

namespace Elsa.Persistence.EntityFrameworkCore.Entities
{
    // 'task' branch change
    public class ActivityInstanceEntity
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }
        public string ActivityId { get; set; }
        public WorkflowInstanceEntity WorkflowInstance { get; set; }
        public string Type { get; set; }
        public Variables State { get; set; }
        public Variable? Output { get; set; }
    }
}