using Elsa.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elsa.Persistence.EntityFrameworkCore.Entities
{
    public class WorkflowInstanceEntity
    {
        private Variables _variables;
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
                return this._variables;
            }
            set
            {
                this._variables = value;
            }
        }

        [Column(TypeName = "jsonb")]
        public virtual string Payload
        {
            get
            {
                return ConvertVariablesToJson(this._variables);
            }
            set
            {
                this._variables = ConvertJsonToVariables(value);
            }
        }

        private string ConvertVariablesToJson(Variables variables)
        {
            dynamic vars = new SimpleJson.JsonObject();

            if(variables != null)
            {
                foreach (var variable in variables)
                {
                    vars[variable.Key] = variable.Value.Value;
                }
            }

            return SimpleJson.SimpleJson.SerializeObject(vars);
        }

        private Variables ConvertJsonToVariables(string jsonString)
        {
            dynamic vars = SimpleJson.SimpleJson.DeserializeObject<SimpleJson.JsonObject>(jsonString);

            Variables variables = new Variables();

            foreach (var variable in vars)
            {
                variables.SetVariable(variable.Key, variable.Value);
            }

            return variables;
        }
    }
}