using Elsa.Models;

namespace Elsa.WorkflowDesigner.Models
{
    public class ConnectionModel
    {
        public ConnectionModel()
        {
        }

        public ConnectionModel(string tenantId, string sourceActivityId, string targetActivityId, string outcome)
        {
            TenantId = tenantId;
            SourceActivityId = sourceActivityId;
            TargetActivityId = targetActivityId;
            Outcome = outcome;
        }

        public ConnectionModel(ConnectionDefinition connectionDefinition) : this(
            connectionDefinition.TenantId,
            connectionDefinition.SourceActivityId,
            connectionDefinition.TargetActivityId,
            connectionDefinition.Outcome)
        {
        }
        public string? TenantId { get; set; }
        public string? SourceActivityId { get; set; }
        public string? TargetActivityId { get; set; }
        public string? Outcome { get; set; }
    }
}