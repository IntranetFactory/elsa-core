using Elsa.Models;
using System;

namespace Elsa.Persistence.EntityFrameworkCore.Entities
{
    public class WorkflowInstanceTaskEntity
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }
        public string ActivityId { get; set; }
        public string? Tag { get; set; }
        public Variable? Input { get; set; }
        public WorkflowInstanceEntity WorkflowInstance { get; set; }
        public WorkflowInstanceTaskStatus? Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public DateTime? ExecutionDate { get; set; }
    }
}