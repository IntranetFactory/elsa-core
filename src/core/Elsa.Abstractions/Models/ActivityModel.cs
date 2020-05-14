namespace Elsa.Models
{
    public class ActivityModel
    {
        public ActivityModel()
        {
        }

        public ActivityModel(string id, int? tenantId, string type, int? left, int? top, Variables state, bool blocking, bool executed, bool faulted, ActivityMessageModel? message = null)
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

        public ActivityModel(WorkflowDefinitionActivity workflowDefinitionActivity) : this(
            workflowDefinitionActivity.Id,
            workflowDefinitionActivity.TenantId,
            workflowDefinitionActivity.Type,
            workflowDefinitionActivity.Left,
            workflowDefinitionActivity.Top,
            workflowDefinitionActivity.State,
            false,
            false,
            false)
        {
        }

        public ActivityModel(WorkflowDefinitionActivity workflowDefinitionActivity, bool blocking, bool executed, bool faulted, ActivityMessageModel? message = null) : this(
            workflowDefinitionActivity.Id,
            workflowDefinitionActivity.TenantId,
            workflowDefinitionActivity.Type,
            workflowDefinitionActivity.Left,
            workflowDefinitionActivity.Top,
            workflowDefinitionActivity.State,
            blocking,
            executed,
            faulted,
            message)
        {
        }

        public string? Id { get; set; }
        public int? TenantId { get; set; }
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
