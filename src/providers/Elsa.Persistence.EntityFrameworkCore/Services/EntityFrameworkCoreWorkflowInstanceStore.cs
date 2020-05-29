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
                .Include(x => x.WorkflowInstanceTasks)
                .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.InstanceId == id, cancellationToken);

            var instance = Map(document);

            if(instance != null)
            {
                foreach (var workflowInstanceTaskEntity in document.WorkflowInstanceTasks)
                {
                    var workflowInstanceTask = Map(workflowInstanceTaskEntity);
                    instance.WorkflowInstanceTasks.Push(workflowInstanceTask);
                }
            }

            return instance;
        }

        // TO DO: include WorkflowInstanceTasks for methods below if necessary
        public async Task<WorkflowInstance> GetByCorrelationIdAsync(
            int? tenantId,
            string correlationId,
            CancellationToken cancellationToken = default)
        {
            var document = await dbContext
                .WorkflowInstances
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
                .Where(x => x.TenantId == tenantId && x.DefinitionId == definitionId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);

            return Map(documents);
        }

        public async Task<IEnumerable<WorkflowInstance>> ListAllAsync(int? tenantId, CancellationToken cancellationToken = default)
        {
            var documents = await dbContext
                .WorkflowInstances
                .Where(x => x.TenantId == tenantId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
            return Map(documents);
        }
        

        public async Task<IEnumerable<WorkflowInstance>> ListByStatusAsync(
            int? tenantId,
            string definitionId,
            WorkflowStatus status,
            CancellationToken cancellationToken = default)
        {
            var documents = await dbContext
                .WorkflowInstances
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

            dbContext.WorkflowInstances.Remove(record);
            
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        private WorkflowInstanceEntity Map(WorkflowInstance source) => mapper.Map<WorkflowInstanceEntity>(source);
        private WorkflowInstance Map(WorkflowInstanceEntity source) => mapper.Map<WorkflowInstance>(source);
        private WorkflowInstanceTask Map(WorkflowInstanceTaskEntity source) => mapper.Map<WorkflowInstanceTask>(source);
        private IEnumerable<WorkflowInstance> Map(IEnumerable<WorkflowInstanceEntity> source) => mapper.Map<IEnumerable<WorkflowInstance>>(source);
        private ICollection<WorkflowInstanceTaskEntity> Map(Stack<WorkflowInstanceTask> source) => mapper.Map<ICollection<WorkflowInstanceTaskEntity>>(source);

    }
}