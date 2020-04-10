using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Elsa.Extensions;
using Elsa.Models;
using Elsa.Persistence.YesSql.Documents;
using Elsa.Persistence.YesSql.Indexes;
using YesSql;

namespace Elsa.Persistence.YesSql.Services
{
    public class YesSqlWorkflowInstanceStore : IWorkflowInstanceStore
    {
        private readonly ISession session;
        private readonly IMapper mapper;

        public YesSqlWorkflowInstanceStore(
            ISession session,
            IMapper mapper)
        {
            this.session = session;
            this.mapper = mapper;
        }

        public async Task<WorkflowInstance> SaveAsync(WorkflowInstance instance, CancellationToken cancellationToken)
        {
            string instanceId = instance.Id;

            WorkflowInstanceDocument existingInstance = await session
                .Query<WorkflowInstanceDocument, WorkflowInstanceIndex>(x => x.TenantId == instance.TenantId && x.WorkflowInstanceId == instanceId)
                .FirstOrDefaultAsync();

            WorkflowInstanceDocument document = (existingInstance != null)
                ? mapper.Map(instance, existingInstance)
                : mapper.Map<WorkflowInstanceDocument>(instance);

            session.Save(document);
            await session.CommitAsync();
            return mapper.Map<WorkflowInstance>(document);
        }

        public async Task<WorkflowInstance> GetByIdAsync(
            int? tenantId,
            string id,
            CancellationToken cancellationToken)
        {
            var document = await session
                .Query<WorkflowInstanceDocument, WorkflowInstanceIndex>(x => x.TenantId == tenantId && x.WorkflowInstanceId == id)
                .FirstOrDefaultAsync();
            return mapper.Map<WorkflowInstance>(document);
        }

        public async Task<WorkflowInstance> GetByCorrelationIdAsync(
            int? tenantId, 
            string correlationId,
            CancellationToken cancellationToken = default)
        {
            var document = await session
                .Query<WorkflowInstanceDocument, WorkflowInstanceIndex>(x => x.TenantId == tenantId && x.CorrelationId == correlationId)
                .FirstOrDefaultAsync();
            return mapper.Map<WorkflowInstance>(document);
        }

        public async Task<IEnumerable<WorkflowInstance>> ListByDefinitionAsync(
            int? tenantId, 
            string definitionId,
            CancellationToken cancellationToken)
        {
            var documents = await session
                .Query<WorkflowInstanceDocument, WorkflowInstanceIndex>(x => x.TenantId == tenantId && x.WorkflowDefinitionId == definitionId)
                .OrderByDescending(x => x.CreatedAt)
                .ListAsync();
            return mapper.Map<IEnumerable<WorkflowInstance>>(documents);
        }

        public async Task<IEnumerable<WorkflowInstance>> ListAllAsync(int? tenantId, CancellationToken cancellationToken)
        {
            var documents = await session.Query<WorkflowInstanceDocument, WorkflowInstanceIndex>()
                .Where(x => x.TenantId == tenantId)
                .OrderByDescending(x => x.CreatedAt)
                .ListAsync();
            return mapper.Map<IEnumerable<WorkflowInstance>>(documents);
        }
        public async Task<IEnumerable<(WorkflowInstance, BlockingActivity)>> ListByBlockingActivityTagAsync(
            int? tenantId,
            string activityType, 
            string tag, 
            string correlationId = null, 
            CancellationToken cancellationToken = default)
        {
            var query = session.Query<WorkflowInstanceDocument, WorkflowInstanceBlockingActivitiesIndex>();

            query = query.Where(x => x.ProcessStatus == WorkflowStatus.Suspended && x.TenantId == tenantId);

            if (!string.IsNullOrWhiteSpace(correlationId))
                query = query.Where(x => x.CorrelationId == correlationId);

            query = query.Where(x => x.ActivityType == activityType && x.Tag == tag && x.TenantId == tenantId);
            query = query.OrderByDescending(x => x.CreatedAt);

            var documents = await query.ListAsync();
            var instances = mapper.Map<IEnumerable<WorkflowInstance>>(documents);

            return instances.GetBlockingActivities(activityType);
        }

        public async Task<IEnumerable<(WorkflowInstance, BlockingActivity)>> ListByBlockingActivityTagAsync(
            int? tenantId,
            string tag,
            string correlationId = null,
            CancellationToken cancellationToken = default)
        {
            var query = session.Query<WorkflowInstanceDocument, WorkflowInstanceBlockingActivitiesIndex>();

            query = query.Where(x => x.ProcessStatus == WorkflowStatus.Suspended && x.TenantId == tenantId);

            if (!string.IsNullOrWhiteSpace(correlationId))
                query = query.Where(x => x.CorrelationId == correlationId);

            query = query.Where(x => x.Tag == tag && x.TenantId == tenantId);
            query = query.OrderByDescending(x => x.CreatedAt);

            var documents = await query.ListAsync();
            var instances = mapper.Map<IEnumerable<WorkflowInstance>>(documents);

            return instances.GetBlockingActivities();
        }

        public async Task<IEnumerable<(WorkflowInstance, BlockingActivity)>> ListByBlockingActivityAsync(
            int? tenantId, 
            string activityType,
            string correlationId = default,
            CancellationToken cancellationToken = default)
        {
            var query = session.Query<WorkflowInstanceDocument, WorkflowInstanceBlockingActivitiesIndex>();

            query = query.Where(x => x.ProcessStatus == WorkflowStatus.Suspended && x.TenantId == tenantId);

            if (!string.IsNullOrWhiteSpace(correlationId))
                query = query.Where(x => x.CorrelationId == correlationId);

            query = query.Where(x => x.ActivityType == activityType && x.TenantId == tenantId);
            query = query.OrderByDescending(x => x.CreatedAt);

            var documents = await query.ListAsync();
            var instances = mapper.Map<IEnumerable<WorkflowInstance>>(documents);

            return instances.GetBlockingActivities(activityType);
        }

        public async Task<IEnumerable<WorkflowInstance>> ListByStatusAsync(
            int? tenantId, 
            string definitionId,
            WorkflowStatus status,
            CancellationToken cancellationToken)
        {
            var documents = await session
                .Query<WorkflowInstanceDocument, WorkflowInstanceIndex>(
                    x => x.WorkflowDefinitionId == definitionId && x.WorkflowStatus == status && x.TenantId == tenantId)
                .OrderByDescending(x => x.CreatedAt)
                .ListAsync();
            return mapper.Map<IEnumerable<WorkflowInstance>>(documents);
        }

        public async Task<IEnumerable<WorkflowInstance>> ListByStatusAsync(
            int? tenantId, 
            WorkflowStatus status,
            CancellationToken cancellationToken)
        {
            var documents = await session
                .Query<WorkflowInstanceDocument, WorkflowInstanceIndex>(x => x.WorkflowStatus == status && x.TenantId == tenantId)
                .OrderByDescending(x => x.CreatedAt)
                .ListAsync();
            return mapper.Map<IEnumerable<WorkflowInstance>>(documents);
        }

        public async Task DeleteAsync(
            int? tenantId, 
            string id,
            CancellationToken cancellationToken = default)
        {
            var document = await session
                .Query<WorkflowInstanceDocument, WorkflowInstanceIndex>(x => x.WorkflowInstanceId == id && x.TenantId == tenantId)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            if (document == null)
                return;

            session.Delete(document);
        }
    }
}