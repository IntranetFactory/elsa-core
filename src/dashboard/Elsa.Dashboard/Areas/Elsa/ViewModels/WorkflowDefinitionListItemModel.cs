using Elsa.Models;

namespace Elsa.Dashboard.Areas.Elsa.ViewModels
{
    public class WorkflowDefinitionListItemModel
    {
        public WorkflowDefinitionVersion WorkflowDefinition { get; set; }
        public int IdleCount { get; set; }
        public int RunningCount { get; set; }
        public int CompletedCount { get; set; }
        public int SuspendedCount { get; set; }
        public int FaultedCount { get; set; }
        public int CancelledCount { get; set; }
    }
}