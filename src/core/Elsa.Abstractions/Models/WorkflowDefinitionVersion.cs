using System.Collections.Generic;
using System.Linq;

namespace Elsa.Models
{
    public class WorkflowDefinitionVersion
    {
        public WorkflowDefinitionVersion()
        {
            Variables = new Variables();
            Activities = new List<WorkflowDefinitionActivity>();
            Connections = new List<WorkflowDefinitionConnection>();
        }

        public WorkflowDefinitionVersion(string id,
            int? tenantId,
            string definitionId,
            int version,
            string name,
            string description,
            Variables variables,
            bool isSingleton,
            IEnumerable<WorkflowDefinitionActivity> activities,
            IEnumerable<WorkflowDefinitionConnection> connections,
            bool isDisabled
            )
        {
            Id = id;
            TenantId = tenantId;
            DefinitionId = definitionId;
            Version = version;
            Name = name;
            Description = description;
            Variables = variables;
            IsSingleton = isSingleton;
            Activities = activities.ToList();
            Connections = connections.ToList();
            IsDisabled = isDisabled;
        }

        public string? Id { get; set; }
        public int? TenantId { get; set; }
        public string? DefinitionId { get; set; }
        public int Version { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Variables? Variables { get; set; }
        public bool IsSingleton { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsPublished { get; set; }
        public bool IsLatest { get; set; }
        public ICollection<WorkflowDefinitionActivity> Activities { get; set; }
        public ICollection<WorkflowDefinitionConnection> Connections { get; set; }
    }
}