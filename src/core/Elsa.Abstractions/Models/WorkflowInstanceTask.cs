using System;

namespace Elsa.Models
{
    public class WorkflowInstanceTask
    {
        public WorkflowInstanceTask()
        {
        }

        public WorkflowInstanceTask(string activityId, int? tenantId, string? tag, WorkflowInstanceTaskStatus? status, DateTime? createDate, DateTime? scheduleDate, DateTime? executionDate, Variable? input = default)
        {
            ActivityId = activityId;
            TenantId = tenantId;
            Tag = tag;
            Status = status;
            CreateDate = createDate;
            ScheduleDate = scheduleDate;
            ExecutionDate = executionDate;
            Input = input;
        }
        public string? ActivityId { get; set; }
        public int? TenantId { get; set; }
        public string InstanceId { get; set; }
        public string? Tag { get; set; }
        public WorkflowInstanceTaskStatus? Status { get; set; }
        public Variable? Input { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public DateTime? ExecutionDate { get; set; }
    }
}