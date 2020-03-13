namespace Elsa.Models
{
    public class BlockingActivity
    {
        public BlockingActivity()
        {
        }

        public BlockingActivity(string activityId, string tenantId, string activityType, string tag)
        {
            ActivityId = activityId;
            TenantId = tenantId;
            ActivityType = activityType;
            Tag = tag;
        }
        
        public string? ActivityId { get; set; }
        public string? TenantId { get; set; }
        public string? ActivityType { get; set; }
        public string Tag { get; set; }
    }
}