namespace Elsa.Models
{
    public class WorkflowDefinitionConnection
    {
        public WorkflowDefinitionConnection()
        {
        }

        public WorkflowDefinitionConnection(int? tenantId, string sourceActivityId, string destinationActivityId, string outcome)
        {
            TenantId = tenantId;
            SourceActivityId = sourceActivityId;
            DestinationActivityId = destinationActivityId;
            Outcome = outcome;
        }
        public int? TenantId { get; set; }
        public string? SourceActivityId { get; set; }
        public string? DestinationActivityId { get; set; }
        public string? Outcome { get; set; }
    }
}