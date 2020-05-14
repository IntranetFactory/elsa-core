namespace Elsa.Models
{
    public class ConnectionModel
    {
        public ConnectionModel()
        {
        }

        public ConnectionModel(int? tenantId, string sourceActivityId, string destinationActivityId, string outcome)
        {
            TenantId = tenantId;
            SourceActivityId = sourceActivityId;
            DestinationActivityId = destinationActivityId;
            Outcome = outcome;
        }

        public ConnectionModel(WorkflowDefinitionConnection workflowDefinitionConnection) : this(
            workflowDefinitionConnection.TenantId,
            workflowDefinitionConnection.SourceActivityId,
            workflowDefinitionConnection.DestinationActivityId,
            workflowDefinitionConnection.Outcome)
        {
        }
        public int? TenantId { get; set; }
        public string? SourceActivityId { get; set; }
        public string? DestinationActivityId { get; set; }
        public string? Outcome { get; set; }
    }
}
