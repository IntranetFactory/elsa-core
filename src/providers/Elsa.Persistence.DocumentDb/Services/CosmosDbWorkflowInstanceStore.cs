using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Elsa.Extensions;
using Elsa.Models;
using Elsa.Persistence.DocumentDb.Documents;
using Elsa.Persistence.DocumentDb.Helpers;

namespace Elsa.Persistence.DocumentDb.Services
{
    public class CosmosDbWorkflowInstanceStore : IWorkflowInstanceStore
    {
        private readonly IMapper mapper;
        private readonly DocumentDbStorage storage;
        private Uri? collectionUrl;

        public CosmosDbWorkflowInstanceStore(DocumentDbStorage storage, IMapper mapper)
        {
            this.storage = storage;
            this.mapper = mapper;
            collectionUrl = default;
        }

        public async Task DeleteAsync(
            int? tenantId, 
            string id,
            CancellationToken cancellationToken = default)
        {
            var client = storage.Client;
            await client.DeleteDocumentAsync(id, cancellationToken: cancellationToken);
        }

        public async Task<WorkflowInstance> GetByCorrelationIdAsync(
            int? tenantId, 
            string correlationId,
            CancellationToken cancellationToken = default)
        {
            var client = storage.Client;
            var collectionUrl = await GetCollectionUriAsync(cancellationToken);
            var query = client.CreateDocumentQuery<WorkflowInstanceDocument>(collectionUrl)
                .Where(c => c.TenantId == tenantId && c.CorrelationId == correlationId);
            var document = query.AsEnumerable().FirstOrDefault();
            return Map(document);
        }

        public async Task<WorkflowInstance> GetByIdAsync(
            int? tenantId, 
            string id,
            CancellationToken cancellationToken = default)
        {
            var client = storage.Client;
            var collectionUrl = await GetCollectionUriAsync(cancellationToken);
            var query = client.CreateDocumentQuery<WorkflowInstanceDocument>(collectionUrl)
                .Where(c => c.TenantId == tenantId && c.Id == id);
            var document = query.AsEnumerable().FirstOrDefault();
            return Map(document);
        }

        public async Task<IEnumerable<WorkflowInstance>> ListAllAsync(int? tenantId, CancellationToken cancellationToken = default)
        {
            var client = storage.Client;
            var collectionUrl = await GetCollectionUriAsync(cancellationToken);
            var query = client
                .CreateDocumentQuery<WorkflowInstanceDocument>(collectionUrl)
                .Where(x => x.TenantId == tenantId)
                .OrderByDescending(x => x.CreatedAt);
            return mapper.Map<IEnumerable<WorkflowInstance>>(query);
        }

        public async Task<IEnumerable<WorkflowInstance>> ListByDefinitionAsync(
            int? tenantId, 
            string definitionId,
            CancellationToken cancellationToken = default)
        {
            var client = storage.Client;
            var collectionUrl = await GetCollectionUriAsync(cancellationToken);
            var query = client.CreateDocumentQuery<WorkflowInstanceDocument>(collectionUrl)
                .Where(c => c.DefinitionId == definitionId && c.TenantId == tenantId)
                .OrderByDescending(x => x.CreatedAt);
            return Map(query.ToList());
        }

        public async Task<IEnumerable<WorkflowInstance>> ListByStatusAsync(
            int? tenantId, 
            string definitionId,
            WorkflowStatus status,
            CancellationToken cancellationToken = default)
        {
            var client = storage.Client;
            var collectionUrl = await GetCollectionUriAsync(cancellationToken);
            var query = client.CreateDocumentQuery<WorkflowInstanceDocument>(collectionUrl)
                .Where(c => c.DefinitionId == definitionId && c.Status == status && c.TenantId == tenantId)
                .OrderByDescending(x => x.CreatedAt);
            return Map(query.ToList());
        }

        public async Task<IEnumerable<WorkflowInstance>> ListByStatusAsync(
            int? tenantId, 
            WorkflowStatus status, 
            CancellationToken cancellationToken = default)
        {
            var client = storage.Client;
            var collectionUrl = await GetCollectionUriAsync(cancellationToken);
            var query = client.CreateDocumentQuery<WorkflowInstanceDocument>(collectionUrl)
                .Where(c => c.Status == status && c.TenantId == tenantId)
                .OrderByDescending(x => x.CreatedAt);
            return Map(query.ToList());
        }

        public async Task<WorkflowInstance> SaveAsync(WorkflowInstance instance, CancellationToken cancellationToken = default)
        {
            var document = Map(instance);
            var client = storage.Client;
            var collectionUrl = await GetCollectionUriAsync(cancellationToken);
            var response = await client.UpsertDocumentWithRetriesAsync(
                collectionUrl,
                document,
                cancellationToken: cancellationToken);

            document = (dynamic)response.Resource;
            return Map(document);
        }
        
        private async Task<Uri> GetCollectionUriAsync(CancellationToken cancellationToken)
        {
            if (collectionUrl == null) 
                collectionUrl = await storage.GetCollectionAsync("WorkflowInstances", cancellationToken);

            return collectionUrl;
        }

        private WorkflowInstanceDocument Map(WorkflowInstance source) => mapper.Map<WorkflowInstanceDocument>(source);
        private WorkflowInstance Map(WorkflowInstanceDocument source) => mapper.Map<WorkflowInstance>(source);

        private IEnumerable<WorkflowInstance> Map(IEnumerable<WorkflowInstanceDocument> source) => mapper.Map<IEnumerable<WorkflowInstance>>(source);
    }
}