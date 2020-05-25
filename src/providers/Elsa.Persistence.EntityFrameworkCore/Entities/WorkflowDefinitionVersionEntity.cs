using Elsa.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using SimpleJson;

namespace Elsa.Persistence.EntityFrameworkCore.Entities
{
    public class WorkflowDefinitionVersionEntity
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }
        public string VersionId { get; set; }
        public string DefinitionId { get; set; }
        public int Version { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsSingleton { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsPublished { get; set; }
        public bool IsLatest { get; set; }
        public WorkflowDefinitionEntity WorkflowDefinition { get; set; }
        public ICollection<WorkflowDefinitionActivityEntity> Activities { get; set; }
        public ICollection<WorkflowDefinitionConnectionEntity> Connections { get; set; }

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