using Elsa.Models;

namespace Elsa.WorkflowDesigner.Models
{
    public class ConnectionModel
    {
        public ConnectionModel()
        {
        }

        public ConnectionModel(string tenantId, string sourceActivityId, string destinationActivityId, string outcome)
        {
            TenantId = tenantId;
            SourceActivityId = sourceActivityId;
            DestinationActivityId = destinationActivityId;
            Outcome = outcome;
        }

        public ConnectionModel(ConnectionDefinition connectionDefinition) : this(
            connectionDefinition.TenantId,
            connectionDefinition.SourceActivityId,
            connectionDefinition.DestinationActivityId,
            connectionDefinition.Outcome)
        {
        }
        public string? TenantId { get; set; }
        public string? SourceActivityId { get; set; }
        public string? DestinationActivityId { get; set; }
        public string? Outcome { get; set; }
    }
}