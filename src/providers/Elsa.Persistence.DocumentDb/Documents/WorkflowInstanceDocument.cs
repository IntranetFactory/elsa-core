using Elsa.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elsa.Persistence.DocumentDb.Documents
{
    public class WorkflowInstanceDocument
    {
        [JsonProperty(PropertyName = "id")] 
        public string Id { get; set; }

        [JsonProperty(PropertyName = "tenantId")]
        public int? TenantId { get; set; }

        [JsonProperty(PropertyName = "definitionId")]
        public string DefinitionId { get; set; }

        [JsonProperty(PropertyName = "type")] 
        public string Type { get; } = nameof(WorkflowInstanceDocument);

        [JsonProperty(PropertyName = "version")]
        public int Version { get; set; }

        [JsonProperty(PropertyName = "status")]
        public WorkflowStatus Status { get; set; }

        [JsonProperty(PropertyName = "correlationId")]
        public string CorrelationId { get; set; }

        [JsonProperty(PropertyName = "createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty(PropertyName = "startedAt")]
        public DateTime? StartedAt { get; set; }

        [JsonProperty(PropertyName = "finishedAt")]
        public DateTime? FinishedAt { get; set; }

        [JsonProperty(PropertyName = "faultedAt")]
        public DateTime? FaultedAt { get; set; }

        [JsonProperty(PropertyName = "abortedAt")]
        public DateTime? AbortedAt { get; set; }

        [JsonProperty(PropertyName = "activities")]
        public IDictionary<string, WorkflowInstanceTask> WorkflowInstanceTasks { get; set; } = new Dictionary<string, WorkflowInstanceTask>();

        [JsonProperty(PropertyName = "variables")]
        public Variables Variables { get; set; }

        [JsonProperty(PropertyName = "input")] public Variable? Input { get; set; }
        [JsonProperty(PropertyName = "input")] public Variable? Output { get; set; }

        [JsonProperty(PropertyName = "blockingActivities")]
        public HashSet<WorkflowInstanceTask> BlockingActivities { get; set; }

        [JsonProperty(PropertyName = "scheduledActivities")]
        public Stack<string> ScheduledActivities { get; set; }

        [JsonProperty(PropertyName = "executionLog")]
        public ICollection<ExecutionLogEntry> ExecutionLog { get; set; }

        [JsonProperty(PropertyName = "fault")] public WorkflowFault Fault { get; set; }
    }
}