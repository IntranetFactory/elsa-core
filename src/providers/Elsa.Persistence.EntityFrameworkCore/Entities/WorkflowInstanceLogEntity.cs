using System;
using System.Collections.Generic;
using System.Text;

namespace Elsa.Persistence.EntityFrameworkCore.Entities
{
    public class WorkflowInstanceLogEntity
    {
        public int Id { get; set; }
        public string ActivityId { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Faulted { get; set; }
        public string? Message { get; set; }
        public WorkflowInstanceEntity WorkflowInstance { get; set; }
    }
}
