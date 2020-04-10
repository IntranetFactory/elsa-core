using Elsa.Models;

namespace Elsa.Messaging.Distributed
{
    public class RunWorkflow
    {
        public RunWorkflow()
        {
        }

        public RunWorkflow(int? tenantId, string instanceId, string? activityId = default, Variable? input = default)
        {
            TenantId = tenantId;
            InstanceId = instanceId;
            ActivityId = activityId;
            Input = input;
        }
        
        public int? TenantId { get; set; }
        public string InstanceId { get; set; }
        public string? ActivityId { get; set; }
        public Variable? Input { get; set; }
    }
}