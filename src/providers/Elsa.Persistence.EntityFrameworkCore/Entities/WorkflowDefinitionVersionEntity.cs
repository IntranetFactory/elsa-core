using Elsa.Models;
using System.Collections.Generic;

namespace Elsa.Persistence.EntityFrameworkCore.Entities
{
    public class WorkflowDefinitionVersionEntity
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }
        public string VersionId { get; set; }
        public string DefinitionId { get; set; }
        public int Version { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public Variables Variables { get; set; }
        public bool IsSingleton { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsPublished { get; set; }
        public bool IsLatest { get; set; }
        public WorkflowDefinitionEntity WorkflowDefinition { get; set; }
        public ICollection<WorkflowDefinitionActivityEntity> Activities { get; set; }
        public ICollection<WorkflowDefinitionConnectionEntity> Connections { get; set; }
    }
}