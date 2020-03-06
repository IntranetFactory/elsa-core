using System.Collections.Generic;
using System.Linq;

namespace Elsa.Dashboard.Areas.Elsa.ViewModels
{
    public class WorkflowDefinitionVersionListViewModel
    {
        public IList<IGrouping<string, WorkflowDefinitionVersionListItemModel>> WorkflowDefinitionVersions { get; set; }
    }
}