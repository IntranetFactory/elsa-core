using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Extensions;
using Elsa.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Elsa.Persistence.MongoDb.Services
{
    public class MongoWorkflowInstanceStore : IWorkflowInstanceStore
    {
        private readonly IMongoCollection<WorkflowInstance> collection;

        public MongoWorkflowInstanceStore(IMongoCollection<WorkflowInstance> collection)
        {
            this.collection = collection;
        }

        public async Task<WorkflowInstance> SaveAsync(WorkflowInstance instance, CancellationToken cancellationToken)
        {
            await collection.ReplaceOneAsync(
                x => x.Id == instance.Id,
                instance,
                new ReplaceOptions { IsUpsert = true },
                cancellationToken);

            return instance;
        }

        public async Task<WorkflowInstance> GetByIdAsync(
            int? tenantId,
            string id,
            CancellationToken cancellationToken)
        {
            return await collection.AsQueryable().Where(x => x.TenantId == tenantId && x.Id == id).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<WorkflowInstance> GetByCorrelationIdAsync(
            int? tenantId,
            string correlationId,
            CancellationToken cancellationToken = default)
        {
            return await collection.AsQueryable()
                .Where(x => x.TenantId == tenantId && x.CorrelationId == correlationId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<WorkflowInstance>> ListByDefinitionAsync(
            int? tenantid, 
            string definitionId,
            CancellationToken cancellationToken)
        {
            return await collection.AsQueryable()
                .Where(x => x.TenantId == tenantid && x.DefinitionId == definitionId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<WorkflowInstance>> ListAllAsync(int? tenantId, CancellationToken cancellationToken)
        {
            return await collection
                .AsQueryable()
                .Where(x => x.TenantId == tenantId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<WorkflowInstance>> ListByStatusAsync(
            int? tenantId,
            string definitionId,
            WorkflowStatus status,
            CancellationToken cancellationToken)
        {
            return await collection
                .AsQueryable()
                .Where(x => x.TenantId == tenantId && x.DefinitionId == definitionId && x.Status == status)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<WorkflowInstance>> ListByStatusAsync(
            int? tenantId,
            WorkflowStatus status,
            CancellationToken cancellationToken)
        {
            return await collection
                .AsQueryable()
                .Where(x => x.TenantId == tenantId && x.Status == status)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task DeleteAsync(
            int? tenantId, 
            string id,
            CancellationToken cancellationToken = default)
        {
            await collection.DeleteOneAsync(x => x.TenantId == tenantId && x.Id == id, cancellationToken);
        }
    }
}