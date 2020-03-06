using Elsa.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elsa.Dashboard.Areas.Elsa.ViewModels
{
    public class TestMultitenancyViewModel
    {
        public string TenantId { get; set; }
        public string UserId { get; set; }
        public IEnumerable<WorkflowDefinition> WorkflowDefinitionsByTenantId { get; set; }
        public IEnumerable<WorkflowInstance> WorkflowInstancesByTenantId { get; set; }
        public IEnumerable<BlockingActivity> BlockingActivitiesByTenantIdAndTag { get; set; }
    }
}
