using NodaTime;

namespace Elsa.Models
{
    public class WorkflowInstanceLog
    {
        public WorkflowInstanceLog()
        {
        }

        public WorkflowInstanceLog(string activityId, Instant timestamp)
        {
            ActivityId = activityId;
            Timestamp = timestamp;
        }

        public int Id { get; set; }
        public string ActivityId { get; set; }
        public string InstanceId { get; set; }
        public Instant Timestamp { get; set; }
        public bool Faulted { get; set; }
        public string Message { get; set; }
    }
}