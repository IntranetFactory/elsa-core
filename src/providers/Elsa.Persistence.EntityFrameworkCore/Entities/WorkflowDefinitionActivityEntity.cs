using Elsa.Models;

namespace Elsa.Persistence.EntityFrameworkCore.Entities
{
    public class WorkflowDefinitionActivityEntity
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }
        public string ActivityId { get; set; }
        public WorkflowDefinitionVersionEntity WorkflowDefinitionVersion { get; set; }
        public string Type { get; set; }
        public int Left { get; set; }
        public int Top { get; set; }
        public Variables State { get; set; }
        public Variable? Output { get; set; }
    }
}