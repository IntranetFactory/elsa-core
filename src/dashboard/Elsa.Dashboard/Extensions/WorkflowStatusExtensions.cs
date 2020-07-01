using Elsa.Models;

namespace Elsa.Dashboard.Extensions
{
    public static class WorkflowStatusExtensions
    {
        public static string GetStatusClass(this WorkflowStatus workflowStatus) =>
            workflowStatus switch
            {
                WorkflowStatus.Execute => "bg-info",
                WorkflowStatus.Resume => "bg-info",
                WorkflowStatus.Scheduled => "bg-warning",
                WorkflowStatus.Running => "bg-info",
                WorkflowStatus.Faulted => "bg-danger",
                WorkflowStatus.Blocked => "bg-warning",
                WorkflowStatus.OnHold => "bg-info",
                WorkflowStatus.Completed => "bg-success",
                _ => "bg-default",
            };
    }
}