using Elsa.Models;

namespace Elsa.Persistence.EntityFrameworkCore.Entities
{
    public class WorkflowInstanceTaskEntity
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }
        public WorkflowInstanceEntity WorkflowInstance { get; set; }
        public string ActivityId { get; set; }
        public Variable? Input { get; set; }
        public WorkflowInstanceTaskStatus? Status { get; set; }
    }
}