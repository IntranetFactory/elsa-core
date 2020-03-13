namespace Elsa.Models
{
    public class ConnectionDefinition
    {
        public ConnectionDefinition()
        {
        }

        public ConnectionDefinition(string tenantId,string sourceActivityId, string targetActivityId, string outcome)
        {
            TenantId = tenantId;
            SourceActivityId = sourceActivityId;
            TargetActivityId = targetActivityId;
            Outcome = outcome;
        }
        public string? TenantId { get; set; }
        public string? SourceActivityId { get; set; }
        public string? TargetActivityId { get; set; }
        public string? Outcome { get; set; }
    }
}