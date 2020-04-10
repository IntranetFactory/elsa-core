namespace Elsa.Models
{
    public class ScheduledActivity
    {
        public ScheduledActivity()
        {
        }

        public ScheduledActivity(string activityId, int? tenantId, Variable? input = default)
        {
            ActivityId = activityId;
            TenantId = tenantId;
            Input = input;
        }
        public string? ActivityId { get; set; }
        public int? TenantId { get; set; }
        public Variable? Input { get; set; }
    }
}