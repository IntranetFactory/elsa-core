using AutoMapper;
using Elsa.Models;
using Elsa.Persistence.EntityFrameworkCore.DbContexts;
using Elsa.Persistence.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Elsa.Persistence.EntityFrameworkCore.Services
{
    public class EntityFrameworkCoreWorkflowDefinitionActivityStore : IWorkflowDefinitionActivityStore
    {
        private readonly ElsaContext dbContext;
        private readonly IMapper mapper;

        public EntityFrameworkCoreWorkflowDefinitionActivityStore(ElsaContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<WorkflowDefinitionActivity> AddAsync(WorkflowDefinitionActivity activity, string? definitionId = default, CancellationToken cancellationToken = default)
        {
            var entity = Map(activity);
            var workflowDefinitionVersion = dbContext.WorkflowDefinitionVersions
                .Include(x => x.Activities)
                .Include(x => x.Connections)
                .Include(x => x.WorkflowDefinition)
                .Where(x => x.DefinitionId == definitionId).FirstOrDefault();

            if (workflowDefinitionVersion != null)
                workflowDefinitionVersion.Activities.Add(entity);

            await dbContext.SaveChangesAsync(cancellationToken);
            return Map(entity);
        }

        public async Task<WorkflowDefinitionActivity> GetByIdAsync(int? tenantId, string id, CancellationToken cancellationToken = default)
        {
            var query = dbContext
                .WorkflowDefinitionActivities
                .Include(x => x.WorkflowDefinitionVersion)
                .Where(x => x.TenantId == tenantId && x.ActivityId == id);

            var entity = await query.FirstOrDefaultAsync(cancellationToken);

            return Map(entity);
        }

        public async Task<IEnumerable<WorkflowDefinitionActivity>> ListAsync(int? tenantId, CancellationToken cancellationToken = default)
        {
            var query = dbContext.WorkflowDefinitionActivities.Where(x => x.TenantId == tenantId).AsQueryable();

            var entities = await query.ToListAsync(cancellationToken);

            return mapper.Map<IEnumerable<WorkflowDefinitionActivity>>(entities);
        }

        public async Task<WorkflowDefinitionActivity> UpdateAsync(WorkflowDefinitionActivity activity, CancellationToken cancellationToken = default)
        {
            var entity = await dbContext
                .WorkflowDefinitionActivities
                .FirstOrDefaultAsync(x => x.TenantId == activity.TenantId && x.ActivityId == activity.Id, cancellationToken);

            entity = mapper.Map(activity, entity);

            dbContext.WorkflowDefinitionActivities.Update(entity);

            await dbContext.SaveChangesAsync(cancellationToken);

            return Map(entity);
        }

        public async Task<int> DeleteAsync(int? tenantId, string id, CancellationToken cancellationToken = default)
        {
            var definitionActivities = await dbContext.WorkflowDefinitionActivities
                .Where(x => x.TenantId == tenantId && x.ActivityId == id)
                .ToListAsync(cancellationToken);

            var activityConnections = dbContext.WorkflowDefinitionConnections.Where(x => x.SourceActivityId == id || x.DestinationActivityId == id).ToList();
            var instanceTasks = dbContext.WorkflowInstanceTasks.Where(x => x.ActivityId == id).ToList();

            dbContext.WorkflowDefinitionActivities.RemoveRange(definitionActivities);
            dbContext.WorkflowDefinitionConnections.RemoveRange(activityConnections);
            dbContext.WorkflowInstanceTasks.RemoveRange(instanceTasks);

            await dbContext.SaveChangesAsync(cancellationToken);

            return definitionActivities.Count;
        }

        private WorkflowDefinitionActivityEntity Map(WorkflowDefinitionActivity source) => mapper.Map<WorkflowDefinitionActivityEntity>(source);
        private WorkflowDefinitionActivity Map(WorkflowDefinitionActivityEntity source) => mapper.Map<WorkflowDefinitionActivity>(source);
    }
}
