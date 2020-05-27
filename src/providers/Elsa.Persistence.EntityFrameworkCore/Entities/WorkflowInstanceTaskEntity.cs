using Elsa.Models;
using System;

namespace Elsa.Persistence.EntityFrameworkCore.Entities
{
    public class WorkflowInstanceTaskEntity
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }
        public string ActivityId { get; set; }
        public Variable? Input { get; set; }
        public WorkflowInstanceEntity WorkflowInstance { get; set; }
        public WorkflowInstanceTaskStatus? Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ScheduleDate { get; set; }
        // TO DO: ExecutionDate is not yet working as the task is removed from the table when executed
        public DateTime? ExecutionDate { get; set; }
    }
}