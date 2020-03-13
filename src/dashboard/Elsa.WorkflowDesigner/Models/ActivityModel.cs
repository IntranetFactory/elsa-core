using Elsa.Models;
using Newtonsoft.Json.Linq;

namespace Elsa.WorkflowDesigner.Models
{
    public class ActivityModel
    {
        public ActivityModel()
        {
        }

        public ActivityModel(string id, string tenantId, string type, int? left, int? top, Variables state, bool blocking, bool executed, bool faulted, ActivityMessageModel? message = null)
        {
            Id = id;
            TenantId = tenantId;
            Type = type;
            Left = left;
            Top = top;
            State = state;
            Blocking = blocking;
            Executed = executed;
            Faulted = faulted;
            Message = message;
        }

        public ActivityModel(ActivityDefinition activityDefinition) : this(
            activityDefinition.Id,
            activityDefinition.TenantId,
            activityDefinition.Type,
            activityDefinition.Left,
            activityDefinition.Top,
            activityDefinition.State,
            false,
            false,
            false)
        {
        }

        public ActivityModel(ActivityDefinition activityDefinition, bool blocking, bool executed, bool faulted, ActivityMessageModel? message = null) : this(
            activityDefinition.Id,
            activityDefinition.TenantId,
            activityDefinition.Type,
            activityDefinition.Left,
            activityDefinition.Top,
            activityDefinition.State,
            blocking,
            executed,
            faulted,
            message)
        {
        }

        public string? Id { get; set; }
        public string? TenantId { get; set; }
        public string? Type { get; set; }
        public int? Left { get; set; }
        public int? Top { get; set; }
        public Variables? State { get; set; }
        public bool Blocking { get; set; }
        public bool Executed { get; set; }
        public bool Faulted { get; set; }
        public ActivityMessageModel? Message { get; set; }
    }
}