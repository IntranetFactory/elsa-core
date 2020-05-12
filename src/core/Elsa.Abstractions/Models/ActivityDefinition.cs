using Elsa.Services;

namespace Elsa.Models
{
    public class ActivityDefinition
    {
        public static ActivityDefinition FromActivity(IActivity activity)
        {
            return new ActivityDefinition
            {
                Id = activity.Id,
                TenantId = activity.TenantId,
                Type = activity.Type,
                State = activity.State,
                Name = activity.Name,
                DisplayName = activity.DisplayName
            };
        }

        public ActivityDefinition() { }

        public ActivityDefinition(string id, int? tenantId, string type, Variables? state, int? left, int? top)
        {
            Id = id;
            TenantId = tenantId;
            Type = type;
            State = state;
            Left = left;
            Top = top;
        }

        public string Id { get; set; }
        public int? TenantId { get; set; }
        public string Type { get; set; }
        public string? Name { get; set; }
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public int? Left { get; set; }
        public int? Top { get; set; }
        public bool PersistWorkflow { get; set; }
        public Variables? State { get; set; }
        public Variable? Output { get; set; }
    }

    public class ActivityDefinition<T> : ActivityDefinition where T : IActivity
    {
        public ActivityDefinition()
        {
            Type = typeof(T).Name;
        }
    }
}