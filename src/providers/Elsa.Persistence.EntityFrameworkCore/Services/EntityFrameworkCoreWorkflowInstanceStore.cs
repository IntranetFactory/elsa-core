using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Elsa.Extensions;
using Elsa.Models;
using Elsa.Persistence.EntityFrameworkCore.DbContexts;
using Elsa.Persistence.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Elsa.Persistence.EntityFrameworkCore.Services
{
    public class EntityFrameworkCoreWorkflowInstanceStore : IWorkflowInstanceStore
    {
        private readonly ElsaContext dbContext;
        private readonly IMapper mapper;

        public EntityFrameworkCoreWorkflowInstanceStore(ElsaContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<WorkflowInstance> SaveAsync(
            WorkflowInstance instance,
            CancellationToken cancellationToken = default)
        {
            var existingEntity = await dbContext
                .WorkflowInstances
                .Include(x => x.WorkflowInstanceBlockingActivities)
                .FirstOrDefaultAsync(x => x.TenantId == instance.TenantId && x.InstanceId == instance.Id, cancellationToken: cancellationToken);

            if (existingEntity == null)
            {
                var entity = Map(instance);
                entity.WorkflowInstanceTasks = Map(instance.WorkflowInstanceTasks);

                await dbContext.WorkflowInstances.AddAsync(entity, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
                return Map(entity);
            }
            else
            {
                dbContext.WorkflowInstanceBlockingActivities.RemoveRange(existingEntity.WorkflowInstanceBlockingActivities);
                existingEntity.WorkflowInstanceBlockingActivities.Clear();

                var entity = mapper.Map(instance, existingEntity);
                entity.WorkflowInstanceTasks = Map(instance.WorkflowInstanceTasks);

                dbContext.WorkflowInstances.Update(entity);
                await dbContext.SaveChangesAsync(cancellationToken);
                return Map(entity);
            }
        }

        public async Task<WorkflowInstance> GetByIdAsync(
            int? tenantId, 
            string id,
            CancellationToken cancellationToken = default)
        {
            var document = await dbContext
                .WorkflowInstances
                .Include(x => x.WorkflowInstanceBlockingActivities)
                .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.InstanceId == id, cancellationToken);

            return Map(document);
        }

        public async Task<WorkflowInstance> GetByCorrelationIdAsync(
            int? tenantId,
            string correlationId,
            CancellationToken cancellationToken = default)
        {
            var document = await dbContext
                .WorkflowInstances
                .Include(x => x.WorkflowInstanceBlockingActivities)
                .Where(x => x.TenantId == tenantId && x.CorrelationId == correlationId)
                .FirstOrDefaultAsync(cancellationToken);

            return Map(document);
        }

        public async Task<IEnumerable<WorkflowInstance>> ListByDefinitionAsync(
            int? tenantId,
            string definitionId,
            CancellationToken cancellationToken = default)
        {
            var documents = await dbContext
                .WorkflowInstances
                .Include(x => x.WorkflowInstanceBlockingActivities)
                .Where(x => x.TenantId == tenantId && x.DefinitionId == definitionId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);

            return Map(documents);
        }

        public async Task<IEnumerable<WorkflowInstance>> ListAllAsync(int? tenantId, CancellationToken cancellationToken = default)
        {
            var documents = await dbContext
                .WorkflowInstances
                .Include(x => x.WorkflowInstanceBlockingActivities)
                .Where(x => x.TenantId == tenantId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
            return Map(documents);
        }
        public async Task<IEnumerable<(WorkflowInstance, WorkflowInstanceBlockingActivity)>> ListByBlockingActivityTagAsync(
            int? tenantId, 
            string activityType,
            string tag,
            string correlationId = default,
            CancellationToken cancellationToken = default)
        {
            var query = dbContext
                .WorkflowInstances
                .Include(x => x.WorkflowInstanceBlockingActivities)
                .AsQueryable();

            query = query.Where(x => x.Status == WorkflowStatus.Suspended && x.TenantId == tenantId);

            if (!string.IsNullOrWhiteSpace(correlationId))
                query = query.Where(x => x.CorrelationId == correlationId);

            query = query.Where(x => x.WorkflowInstanceBlockingActivities.Any(y => y.ActivityType == activityType && y.Tag == tag && y.TenantId == tenantId));
            query = query.OrderByDescending(x => x.CreatedAt);

            var documents = await query.ToListAsync(cancellationToken);
            var instances = Map(documents);

            return instances.GetBlockingActivities(activityType);
        }

        public async Task<IEnumerable<(WorkflowInstance, WorkflowInstanceBlockingActivity)>> ListByBlockingActivityTagAsync(
            int? tenantId,
            string tag,
            string correlationId = default,
            CancellationToken cancellationToken = default)
        {
            var query = dbContext
                .WorkflowInstances
                .Include(x => x.WorkflowInstanceBlockingActivities)
                .AsQueryable();

            query = query.Where(x => x.Status == WorkflowStatus.Suspended && x.TenantId == tenantId);

            if (!string.IsNullOrWhiteSpace(correlationId))
                query = query.Where(x => x.CorrelationId == correlationId);

            query = query.Where(x => x.WorkflowInstanceBlockingActivities.Any(y => y.Tag == tag && y.TenantId == tenantId));
            query = query.OrderByDescending(x => x.CreatedAt);

            var documents = await query.ToListAsync(cancellationToken);
            var instances = Map(documents);

            return instances.GetBlockingActivities();
        }

        public async Task<IEnumerable<(WorkflowInstance, WorkflowInstanceBlockingActivity)>> ListByBlockingActivityAsync(
            int? tenantId, 
            string activityType,
            string correlationId = default,
            CancellationToken cancellationToken = default)
        {
            var query = dbContext
                .WorkflowInstances
                .Include(x => x.WorkflowInstanceBlockingActivities)
                .AsQueryable();

            query = query.Where(x => x.Status == WorkflowStatus.Suspended && x.TenantId == tenantId);

            if (!string.IsNullOrWhiteSpace(correlationId))
                query = query.Where(x => x.CorrelationId == correlationId);

            query = query.Where(x => x.WorkflowInstanceBlockingActivities.Any(y => y.ActivityType == activityType && y.TenantId == tenantId));
            query = query.OrderByDescending(x => x.CreatedAt);

            var documents = await query.ToListAsync(cancellationToken);
            var instances = Map(documents);

            return instances.GetBlockingActivities(activityType);
        }

        public async Task<IEnumerable<WorkflowInstance>> ListByStatusAsync(
            int? tenantId,
            string definitionId,
            WorkflowStatus status,
            CancellationToken cancellationToken = default)
        {
            var documents = await dbContext
                .WorkflowInstances
                .Include(x => x.WorkflowInstanceBlockingActivities)
                .Where(x => x.TenantId == tenantId && x.DefinitionId == definitionId && x.Status == status)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);

            return Map(documents);
        }

        public async Task<IEnumerable<WorkflowInstance>> ListByStatusAsync(
            int? tenantId,
            WorkflowStatus status,
            CancellationToken cancellationToken = default)
        {
            var documents = await dbContext
                .WorkflowInstances
                .Include(x => x.WorkflowInstanceBlockingActivities)
                .Where(x => x.TenantId == tenantId && x.Status == status)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);

            return Map(documents);
        }

        public async Task DeleteAsync(int? tenantId, string id, CancellationToken cancellationToken = default)
        {
            var record = await dbContext.WorkflowInstances.FirstOrDefaultAsync(x => x.TenantId == tenantId && x.InstanceId == id, cancellationToken);

            if (record == null)
                return;

            var blockingActivityRecords = await dbContext.WorkflowInstanceBlockingActivities
                .Where(x => x.TenantId == tenantId && x.WorkflowInstance.InstanceId == id)
                .ToListAsync(cancellationToken);
            
            dbContext.WorkflowInstanceBlockingActivities.RemoveRange(blockingActivityRecords);
            dbContext.WorkflowInstances.Remove(record);
            
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        private WorkflowInstanceEntity Map(WorkflowInstance source) => mapper.Map<WorkflowInstanceEntity>(source);
        private WorkflowInstance Map(WorkflowInstanceEntity source) => mapper.Map<WorkflowInstance>(source);
        private IEnumerable<WorkflowInstance> Map(IEnumerable<WorkflowInstanceEntity> source) => mapper.Map<IEnumerable<WorkflowInstance>>(source);
        private ICollection<WorkflowInstanceTaskEntity> Map(Stack<WorkflowInstanceTask> source) => mapper.Map<ICollection<WorkflowInstanceTaskEntity>>(source);

    }
}