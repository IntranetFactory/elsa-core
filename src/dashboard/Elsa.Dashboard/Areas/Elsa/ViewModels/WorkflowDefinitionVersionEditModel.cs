using Elsa.Metadata;
using Elsa.Models;

namespace Elsa.Dashboard.Areas.Elsa.ViewModels
{
    public class WorkflowDefinitionVersionEditModel
    {
        public string Id { get; set; }
        public int? TenantId { get; set; }
        public string Json { get; set; }
        public string SubmitAction { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsSingleton { get; set; }
        public bool IsDisabled { get; set; }
        public ActivityDescriptor[] ActivityDefinitions { get; set; }
        public WorkflowModel WorkflowModel { get; set; }
    }
}