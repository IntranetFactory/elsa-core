using Elsa.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using SimpleJson;

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
        public ICollection<ExecutionLogEntry> ExecutionLog { get; set; }
        public ICollection<WorkflowInstanceBlockingActivityEntity> WorkflowInstanceBlockingActivities { get; set; }
        public ICollection<WorkflowInstanceTaskEntity> WorkflowInstanceTasks { get; set; }

        [NotMapped]
        public Variables? Variables
        {
            get
            {
                return ConvertJsonToVariables(this.Payload);
            }
            set
            {
                this.Payload = ConvertVariablesToJson(value);
            }
        }

        [NotMapped]
        public Variables? Input { get; set; }

        [Column(TypeName = "jsonb")]
        public virtual string Payload { get; set; }

        private string ConvertVariablesToJson(Variables variables)
        {
            dynamic vars = new SimpleJson.JsonObject();

            foreach (var variable in variables)
            {
                vars[variable.Key] = variable.Value;
            }

            return SimpleJson.SimpleJson.SerializeObject(vars);
        }

        private Variables ConvertJsonToVariables(string simpleJson)
        {
            dynamic vars = SimpleJson.SimpleJson.DeserializeObject<SimpleJson.JsonObject>(simpleJson);
            Variables variables = new Variables();

            foreach (var variable in vars)
            {
                variables.SetVariable(variable.Key, variable.Value);
            }

            return variables;
        }
    }
}