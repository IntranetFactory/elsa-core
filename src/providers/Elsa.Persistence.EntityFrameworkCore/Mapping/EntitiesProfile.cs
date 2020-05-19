using AutoMapper;
using Elsa.Models;
using Elsa.Persistence.EntityFrameworkCore.Entities;

namespace Elsa.Persistence.EntityFrameworkCore.Mapping
{
    public class EntitiesProfile : Profile
    {
        public EntitiesProfile()
        {
            CreateMap<WorkflowDefinition, WorkflowDefinitionEntity>();
            CreateMap<WorkflowDefinitionEntity, WorkflowDefinition>();

            CreateMap<WorkflowInstanceTask, WorkflowInstanceTaskEntity>()
                .ForMember(d => d.ActivityId, d => d.MapFrom(s => s.ActivityId))
                .ForMember(d => d.TenantId, d => d.MapFrom(s => s.TenantId))
                .ForMember(d => d.Input, d => d.MapFrom(s => s.Input))
                .ForMember(d => d.Status, d => d.MapFrom(s => s.Status));

            CreateMap<WorkflowInstanceTaskEntity, WorkflowInstanceTask>(MemberList.Destination)
                .ForMember(d => d.ActivityId, d => d.MapFrom(s => s.ActivityId))
                .ForMember(d => d.Input, d => d.MapFrom(s => s.Input))
                .ForMember(d => d.TenantId, d => d.MapFrom(s => s.TenantId))
                .ForMember(d => d.Status, d => d.MapFrom(s => s.Status));

            CreateMap<WorkflowDefinitionVersion, WorkflowDefinitionVersionEntity>()
                .ForMember(d => d.VersionId, d => d.MapFrom(s => s.Id))
                .ForMember(d => d.Id, d => d.Ignore());

            CreateMap<WorkflowDefinitionVersionEntity, WorkflowDefinitionVersion>()
                .ForCtorParam("id", p => p.MapFrom(s => s.VersionId))
                .ForMember(d => d.Id, d => d.MapFrom(s => s.VersionId));

            CreateMap<WorkflowInstance, WorkflowInstanceEntity>()
                .ForMember(d => d.Id, d => d.Ignore())
                .ForMember(d => d.InstanceId, d => d.MapFrom(s => s.Id))
                .ForMember(d => d.WorkflowInstanceTasks, d => d.Ignore());

            CreateMap<WorkflowInstanceEntity, WorkflowInstance>()
                .ForMember(d => d.Id, d => d.MapFrom(s => s.InstanceId))
                .ForMember(d => d.WorkflowInstanceTasks, d => d.Ignore());

            CreateMap<WorkflowDefinitionActivity, WorkflowDefinitionActivityEntity>()
                .ForMember(d => d.Id, d => d.Ignore())
                .ForMember(d => d.ActivityId, d => d.MapFrom(s => s.Id));

            CreateMap<WorkflowDefinitionActivityEntity, WorkflowDefinitionActivity>()
                .ForCtorParam("id", p => p.MapFrom(s => s.ActivityId))
                .ForMember(d => d.Id, d => d.MapFrom(s => s.ActivityId));

            CreateMap<WorkflowInstanceBlockingActivity, WorkflowInstanceBlockingActivityEntity>().ReverseMap();
            CreateMap<WorkflowDefinitionConnection, WorkflowDefinitionConnectionEntity>().ReverseMap();
        }
    }
}