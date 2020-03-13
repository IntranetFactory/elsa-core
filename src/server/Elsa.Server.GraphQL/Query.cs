using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Elsa.Metadata;
using Elsa.Models;
using Elsa.Persistence;
using Elsa.Server.GraphQL.Models;
using Elsa.Services;
using HotChocolate;

namespace Elsa.Server.GraphQL
{
    public class Query
    {
        public IEnumerable<ActivityDescriptor> GetActivityDescriptors(
            [Service] IActivityResolver activityResolver,
            [Service] IActivityDescriber describer) =>
            activityResolver.GetActivityTypes().Select(describer.Describe).ToList();

        public ActivityDescriptor? GetActivityDescriptor(
            [Service]IActivityResolver activityResolver,
            [Service] IActivityDescriber describer,
            string typeName)
        {
            var type = activityResolver.GetActivityType(typeName);

            return type == null ? default : describer.Describe(type);
        }

        public async Task<IEnumerable<WorkflowDefinitionVersion>> GetWorkflowDefinitionVersions(
            string tenantId, 
            VersionOptionsInput? version,
            [Service] IWorkflowDefinitionVersionStore store,
            [Service] IMapper mapper,
            CancellationToken cancellationToken)
        {
            var mappedVersion = mapper.Map<VersionOptions?>(version);
            return await store.ListAsync(tenantId, mappedVersion ?? VersionOptions.Latest, cancellationToken);
        }
        
        public async Task<WorkflowDefinitionVersion> GetWorkflowDefinitionVersion(
            string? tenantId, 
            string? id,
            string? definitionId,
            VersionOptionsInput? version,
            [Service] IWorkflowDefinitionVersionStore store,
            [Service] IMapper mapper,
            CancellationToken cancellationToken)
        {
            if (id != null)
                return await store.GetByIdAsync(tenantId, id, cancellationToken);
            
            var mappedVersion = mapper.Map<VersionOptions?>(version);
            return await store.GetByIdAsync(tenantId, definitionId, mappedVersion ?? VersionOptions.Latest, cancellationToken);
        }

        public async Task<IEnumerable<WorkflowInstance>> GetWorkflowInstances(
            string tenantId, 
            string definitionId, 
            WorkflowStatus? status,
            [Service] IWorkflowInstanceStore store,
            CancellationToken cancellationToken)
        {
            if(status == null)
                return await store.ListByDefinitionAsync(tenantId, definitionId, cancellationToken);

            return await store.ListByStatusAsync(definitionId, status.Value, cancellationToken);
        }
        
        public async Task<WorkflowInstance> GetWorkflowInstance(
            string tenantId, 
            string id,
            [Service] IWorkflowInstanceStore store,
            CancellationToken cancellationToken)
        {
            return await store.GetByIdAsync(tenantId, id, cancellationToken);
        }
    }
}