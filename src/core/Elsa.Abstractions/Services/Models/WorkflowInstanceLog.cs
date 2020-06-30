using NodaTime;

namespace Elsa.Services.Models
{
    public class WorkflowInstanceLog
    {
        public WorkflowInstanceLog(IActivity activity, Instant timestamp)
        {
            Activity = activity;
            Timestamp = timestamp;
        }

        public IActivity Activity { get; }
        public Instant Timestamp { get; }
    }
}