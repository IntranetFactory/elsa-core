using Elsa.Models;

namespace Elsa.Dashboard.Areas.Elsa.ViewModels
{
    public class WorkflowDefinitionVersionListItemModel
    {
        public WorkflowDefinitionVersion WorkflowDefinitionVersion { get; set; }
        public int ExecuteCount { get; set; }
        public int ResumeCount { get; set; }
        public int ScheduledCount { get; set; }
        public int RunningCount { get; set; }
        public int FaultedCount { get; set; }
        public int BlockedCount { get; set; }
        public int OnHoldCount { get; set; }
        public int CompletedCount { get; set; }
    }
}