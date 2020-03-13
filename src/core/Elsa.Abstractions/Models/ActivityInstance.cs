namespace Elsa.Models
{
    public class ActivityInstance
    {
        public ActivityInstance()
        {
        }

        public ActivityInstance(string id, string tenantId, string type, Variables state, Variable? output)
        {
            Id = id;
            TenantId = tenantId;
            Type = type;
            State = state;
            Output = output;
        }
        
        public string? Id { get; set; }
        public string? TenantId { get; set; }
        public string? Type { get; set; }
        public Variables? State { get; set; }
        public Variable? Output { get; set; }
    }
}