using System.Collections.Generic;
using Elsa.Comparers;
using NodaTime;

namespace Elsa.Models
{
    public class WorkflowInstance
    {
        public WorkflowInstance()
        {
            Variables = new Variables();
            WorkflowInstanceBlockingActivities = new HashSet<WorkflowInstanceBlockingActivity>(new WorkflowInstanceBlockingActivityEqualityComparer());
            ExecutionLog = new List<ExecutionLogEntry>();
            WorkflowInstanceTasks = new Stack<WorkflowInstanceTask>();
        }

        private Variables _variables;
        public string? Id { get; set; }
        public int? TenantId { get; set; }
        public string? DefinitionId { get; set; }
        public string? CorrelationId { get; set; }
        public int Version { get; set; }
        public Instant CreatedAt { get; set; } 
        public Instant? StartedAt { get; set; } // TO DO: implement for dashboard
        public Instant? CompletedAt { get; set; } // TO DO: implement for dashboard
        public Instant? FaultedAt { get; set; } // TO DO: implement for dashboard
        public Instant? CancelledAt { get; set; } // TO DO: implement for dashboard
        public WorkflowStatus Status { get; set; }
        public WorkflowFault? Fault { get; set; }
        public Variable? Output { get; set; }
        // Variables? Input is inserted because of mapping problems and until we figure out what Output is for.
        public Variables? Input { get; set; }
        public ICollection<ExecutionLogEntry> ExecutionLog { get; set; }
        public HashSet<WorkflowInstanceBlockingActivity> WorkflowInstanceBlockingActivities { get; set; }
        public Stack<WorkflowInstanceTask> WorkflowInstanceTasks { get; set; }
        public string? Payload
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

        private string ConvertVariablesToJson(Variables variables)
        {
            dynamic vars = new SimpleJson.JsonObject();

            if (variables != null)
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