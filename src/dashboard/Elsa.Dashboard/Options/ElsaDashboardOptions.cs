using System.Collections.Generic;

namespace Elsa.Dashboard.Options
{
    public class ElsaDashboardOptions
    {
        public ElsaDashboardOptions()
        {
            ActivityDefinitions = new ActivityDefinitionList();
        }
        
        public ActivityDefinitionList ActivityDefinitions { get; set; }
    }
}