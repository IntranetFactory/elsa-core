namespace Elsa.Models
{
    // This enum is used for both WorkflowInstance and WorkflowInstanceTask
    public enum WorkflowStatus
    {
        // Tasks which are queried should be placed within < 60 range
        Execute = 0,
        Resume = 1,
        Scheduled = 2,

        // Tasks which are not queried should be placed within 60 - 90 range
        Running = 60,
        Faulted = 61,
        Blocked = 62,
        OnHold = 63,

        // Tasks which are done should be placed in range >= 90
        Completed = 90
    }
}
