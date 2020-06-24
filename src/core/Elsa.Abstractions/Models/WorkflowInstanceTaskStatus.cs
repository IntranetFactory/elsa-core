namespace Elsa.Models
{
    public enum WorkflowInstanceTaskStatus
    {
        Execute = 0,
        Running = 1,
        Resume = 2,
        Faulted = 3,
        Blocked = 4,
        OnHold = 5,
        Scheduled = 6,
        Completed = 7
    }
}
