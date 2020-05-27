namespace Elsa.Models
{
    public enum WorkflowInstanceTaskStatus
    {
        Scheduled = 0,
        Running = 1,
        Faulted = 3,
        Blocked = 4,
        OnHold = 5,
        Completed = 6
    }
}
