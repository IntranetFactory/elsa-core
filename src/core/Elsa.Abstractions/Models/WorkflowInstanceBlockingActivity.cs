namespace Elsa.Models
{
    public class WorkflowInstanceBlockingActivity
    {
        public WorkflowInstanceBlockingActivity()
        {
        }

        public WorkflowInstanceBlockingActivity(string activityId, int? tenantId, string activityType, string tag)
        {
            ActivityId = activityId;
            TenantId = tenantId;
            ActivityType = activityType;
            Tag = tag;
        }
        
        public string? ActivityId { get; set; }
        public int? TenantId { get; set; }
        public string? ActivityType { get; set; }
        public string Tag { get; set; }
    }
}