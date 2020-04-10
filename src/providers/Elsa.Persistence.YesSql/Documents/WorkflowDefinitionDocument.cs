using System;

namespace Elsa.Persistence.YesSql.Documents
{
    public class WorkflowDefinitionDocument : YesSqlDocument
    {
        public string Id { get; set; }
        public int? TenantId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
