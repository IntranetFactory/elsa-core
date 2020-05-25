using Elsa.Models;
using System.ComponentModel.DataAnnotations.Schema;
using SimpleJson;

namespace Elsa.Persistence.EntityFrameworkCore.Entities
{
    public class WorkflowDefinitionActivityEntity
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }
        public string ActivityId { get; set; }
        public WorkflowDefinitionVersionEntity WorkflowDefinitionVersion { get; set; }
        public string Type { get; set; }
        public int Left { get; set; }
        public int Top { get; set; }
        public Variable? Output { get; set; }

        [NotMapped]
        public Variables? State
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

        [Column(TypeName = "jsonb")]
        public virtual string? Payload { get; set; }

        private string ConvertVariablesToJson(Variables variables)
        {
            dynamic vars = new JsonObject();

            foreach (var variable in variables)
            {
                vars[variable.Key] = variable.Value;
            }

            return SimpleJson.SimpleJson.SerializeObject(vars);
        }

        private Variables ConvertJsonToVariables(string simpleJson)
        {
            dynamic vars = SimpleJson.SimpleJson.DeserializeObject<JsonObject>(simpleJson);
            Variables variables = new Variables();

            foreach (var variable in vars)
            {
                variables.SetVariable(variable.Key, variable.Value);
            }

            return variables;
        }
    }
}