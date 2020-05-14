using System.Collections.Generic;

namespace Elsa.Dashboard.Options
{
    public class ElsaDashboardOptions
    {
        public ElsaDashboardOptions()
        {
            WorkflowDefinitionActivities = new WorkflowDefinitionActivityList();
        }
        
        public WorkflowDefinitionActivityList WorkflowDefinitionActivities { get; set; }
    }
}