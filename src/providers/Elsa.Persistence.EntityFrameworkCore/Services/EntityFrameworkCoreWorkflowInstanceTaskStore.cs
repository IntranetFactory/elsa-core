using AutoMapper;
using Elsa.Models;
using Elsa.Persistence.EntityFrameworkCore.DbContexts;
using Elsa.Persistence.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Elsa.Persistence.EntityFrameworkCore.Services
{
    public class EntityFrameworkCoreWorkflowInstanceTaskStore : IWorkflowInstanceTaskStore
    {
        private readonly ElsaContext dbContext;
        private readonly IMapper mapper;

        public EntityFrameworkCoreWorkflowInstanceTaskStore(ElsaContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<WorkflowInstanceTask> SaveAsync(WorkflowInstanceTask task, CancellationToken cancellationToken = default)
        {
            var existingEntity = await dbContext
                .WorkflowInstanceTasks
                .FirstOrDefaultAsync(x => x.TenantId == task.TenantId && x.ActivityId == task.ActivityId, cancellationToken: cancellationToken);

            if (existingEntity == null)
            {
                var entity = Map(task);
                await dbContext.WorkflowInstanceTasks.AddAsync(entity, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
                return Map(entity);
            }
            else
            {
                var entity = mapper.Map(task, existingEntity);
                dbContext.WorkflowInstanceTasks.Update(entity);
                await dbContext.SaveChangesAsync(cancellationToken);
                return Map(entity);
            }
        }

        public async Task<WorkflowInstanceTask> GetByIdAsync(int? tenantId, string id, CancellationToken cancellationToken = default)
        {
            var taskEntity = await dbContext
                .WorkflowInstanceTasks
                .Include(x => x.WorkflowInstance)
                .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.ActivityId == id, cancellationToken);

            return Map(taskEntity);
        }

        public async Task<WorkflowInstanceTask> GetFirstBlockingTaskByInstanceIdAsync(int? tenantId, string instanceId, CancellationToken cancellationToken = default)
        {
            var workflowInstanceEntity = await dbContext
                .WorkflowInstances
                .Include(x => x.WorkflowInstanceTasks)
                .Where(x => x.InstanceId == instanceId).FirstOrDefaultAsync(cancellationToken);

            var taskEntity = workflowInstanceEntity.WorkflowInstanceTasks.Where(x => x.Status == WorkflowInstanceTaskStatus.Blocked).FirstOrDefault();

            return Map(taskEntity);
        }

        public async Task<IEnumerable<WorkflowInstanceTask>> ListAllAsync(int? tenantId, CancellationToken cancellationToken = default)
        {
            var tasks = await dbContext
                .WorkflowInstanceTasks
                .Where(x => x.TenantId == tenantId)
                .ToListAsync(cancellationToken);

            return Map(tasks);
        }

        public async Task DeleteAsync(int? tenantId, string id, CancellationToken cancellationToken = default)
        {
            var record = await dbContext.WorkflowInstanceTasks.FirstOrDefaultAsync(x => x.TenantId == tenantId && x.ActivityId == id, cancellationToken);

            if (record == null)
                return;

            dbContext.WorkflowInstanceTasks.Remove(record);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<WorkflowInstanceTask>> GetTopScheduledTasksAsync(int numberOfTasks, CancellationToken cancellationToken = default)
        {
            var records = await dbContext.WorkflowInstanceTasks
                .Include(x => x.WorkflowInstance)
                .OrderBy(x => x.ScheduleDate)
                .Where(x => x.ScheduleDate <= DateTime.UtcNow && (int)x.Status < 60) // query tasks where the task status falls within 0-60 range
                .Take(numberOfTasks)
                .ToListAsync(cancellationToken);

            return Map(records);
        }

        private WorkflowInstanceTaskEntity Map(WorkflowInstanceTask source) => mapper.Map<WorkflowInstanceTaskEntity>(source);
        private WorkflowInstanceTask Map(WorkflowInstanceTaskEntity source) => mapper.Map<WorkflowInstanceTask>(source);
        private IEnumerable<WorkflowInstanceTask> Map(IEnumerable<WorkflowInstanceTaskEntity> source) => mapper.Map<IEnumerable<WorkflowInstanceTask>>(source);

    }
}
