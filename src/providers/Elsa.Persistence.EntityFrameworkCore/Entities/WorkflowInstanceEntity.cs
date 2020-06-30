using Elsa.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elsa.Persistence.EntityFrameworkCore.Entities
{
    public class WorkflowInstanceEntity
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }
        public string? InstanceId { get; set; }
        public string? DefinitionId { get; set; }
        public string? CorrelationId { get; set; }
        public int? Version { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public DateTime? FaultedAt { get; set; }
        public DateTime? AbortedAt { get; set; }
        public WorkflowStatus? Status { get; set; }
        public WorkflowFault? Fault { get; set; }
        public ICollection<WorkflowInstanceLogEntity> ExecutionLog { get; set; }
        // this property is mapped to WorkflowInstance WorkflowInstanceTaskStack
        public ICollection<WorkflowInstanceTaskEntity> WorkflowInstanceTasks { get; set; }

        [Column(TypeName = "jsonb")]
        public virtual string Payload { get; set; }
    }
}