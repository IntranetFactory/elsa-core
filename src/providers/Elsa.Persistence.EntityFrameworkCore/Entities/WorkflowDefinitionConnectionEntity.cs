namespace Elsa.Persistence.EntityFrameworkCore.Entities
{
    public class WorkflowDefinitionConnectionEntity
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }
        public WorkflowDefinitionVersionEntity WorkflowDefinitionVersion { get; set; }
        public string SourceActivityId { get; set; }
        public string? DestinationActivityId { get; set; }
        public string Outcome { get; set; }
    }
}