using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Extensions;
using Elsa.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Elsa.Persistence.MongoDb.Services
{
    public class MongoWorkflowDefinitionStore : IWorkflowDefinitionStore
    {
        private readonly IMongoCollection<WorkflowDefinitionVersion> workflowDefinitionVersionCollection;
        private readonly IMongoCollection<WorkflowDefinition> workflowDefinitionCollection;
        private readonly IMongoCollection<WorkflowInstance> workflowInstanceCollection;
        public MongoWorkflowDefinitionStore(
            IMongoCollection<WorkflowDefinitionVersion> workflowDefinitionVersionCollection,
            IMongoCollection<WorkflowDefinition> workflowDefinitionCollection,
            IMongoCollection<WorkflowInstance> workflowInstanceCollection)
        {
            this.workflowDefinitionVersionCollection = workflowDefinitionVersionCollection;
            this.workflowDefinitionCollection = workflowDefinitionCollection;
            this.workflowInstanceCollection = workflowInstanceCollection;
        }
        public async Task<WorkflowDefinition> SaveAsync(WorkflowDefinition definition, CancellationToken cancellationToken = default)
        {
            await workflowDefinitionCollection.ReplaceOneAsync(
                x => x.TenantId == definition.TenantId && x.Id == definition.Id, 
                definition,
                new ReplaceOptions { IsUpsert = true },
                cancellationToken
            );

            return definition;
        }
        public async Task<WorkflowDefinition> AddAsync(WorkflowDefinition definition, CancellationToken cancellationToken = default)
        {
            await workflowDefinitionCollection.InsertOneAsync(definition, new InsertOneOptions(), cancellationToken);
            return definition;
        }
        public Task<WorkflowDefinition> GetByIdAsync(int? tenantId, string id, CancellationToken cancellationToken = default)
        {
            return workflowDefinitionCollection.AsQueryable().FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<WorkflowDefinition>> ListAsync(int? tenantId, CancellationToken cancellationToken = default)
        {
            var query = workflowDefinitionCollection.AsQueryable().Where(x => x.TenantId == tenantId);
            var results = await query.ToListAsync(cancellationToken);
            return results;
        }

        public async Task<WorkflowDefinition> UpdateAsync(WorkflowDefinition definition, CancellationToken cancellationToken = default)
        {
            await workflowDefinitionCollection.ReplaceOneAsync(
                x => x.TenantId == definition.TenantId && x.Id == definition.Id,
                definition,
                new ReplaceOptions { IsUpsert = false },
                cancellationToken
            );

            return definition;
        }
        public async Task<int> DeleteAsync(int? tenantId, string id, CancellationToken cancellationToken = default)
        {
            await workflowDefinitionCollection.DeleteManyAsync(x => x.TenantId == tenantId && x.Id == id, cancellationToken);
            var result = await workflowDefinitionVersionCollection.DeleteManyAsync(x => x.DefinitionId == id, cancellationToken);

            return (int)result.DeletedCount;
        }
    }
}
