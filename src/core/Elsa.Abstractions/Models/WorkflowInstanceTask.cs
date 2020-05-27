using System;

namespace Elsa.Models
{
    public class WorkflowInstanceTask
    {
        public WorkflowInstanceTask()
        {
        }

        public WorkflowInstanceTask(string activityId, int? tenantId, WorkflowInstanceTaskStatus? status, DateTime? createDate, DateTime? scheduleDate, DateTime? executionDate, Variable? input = default)
        {
            ActivityId = activityId;
            TenantId = tenantId;
            Status = status;
            CreateDate = createDate;
            ScheduleDate = scheduleDate;
            ExecutionDate = executionDate;
            Input = input;
        }
        public string? ActivityId { get; set; }
        public int? TenantId { get; set; }
        public WorkflowInstanceTaskStatus? Status { get; set; }
        public Variable? Input { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ScheduleDate { get; set; }
        // TO DO: ExecutionDate is not yet working as the task is removed from the table when executed
        public DateTime? ExecutionDate { get; set; }
    }
}