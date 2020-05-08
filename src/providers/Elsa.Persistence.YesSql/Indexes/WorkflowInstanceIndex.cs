using System;
using System.Linq;
using Elsa.Models;
using Elsa.Persistence.YesSql.Documents;
using YesSql.Indexes;

namespace Elsa.Persistence.YesSql.Indexes
{
    public class WorkflowInstanceIndex : MapIndex
    {
        public string WorkflowInstanceId { get; set; }
        public int? TenantId { get; set; }
        public string WorkflowDefinitionId { get; set; }
        public string CorrelationId { get; set; }
        public WorkflowStatus WorkflowStatus { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class WorkflowInstanceBlockingActivitiesIndex : MapIndex
    {
        public string ActivityId { get; set; }
        public int? TenantId { get; set; }
        public string ActivityType { get; set; }
        public string Tag { get; set; }
        public string CorrelationId { get; set; }
        public WorkflowStatus ProcessStatus { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class WorkflowInstanceIndexProvider : IndexProvider<WorkflowInstanceDocument>
    {
        public override void Describe(DescribeContext<WorkflowInstanceDocument> context)
        {
            context.For<WorkflowInstanceIndex>()
                .Map(
                    workflowInstance => new WorkflowInstanceIndex
                    {
                        TenantId = workflowInstance.TenantId,
                        WorkflowInstanceId = workflowInstance.WorkflowInstanceId,
                        WorkflowDefinitionId = workflowInstance.DefinitionId,
                        WorkflowStatus = workflowInstance.Status,
                        CorrelationId = workflowInstance.CorrelationId,
                        CreatedAt = workflowInstance.CreatedAt
                    });

            context.For<WorkflowInstanceBlockingActivitiesIndex>()
                .Map(
                    workflowInstance => workflowInstance.BlockingActivities
                        .Select(
                            activity => new WorkflowInstanceBlockingActivitiesIndex
                            {
                                ActivityId = activity.Id,
                                TenantId = activity.TenantId,
                                ActivityType = activity.Type,
                                Tag = activity.Tag,
                                CorrelationId = workflowInstance.CorrelationId,
                                ProcessStatus = workflowInstance.Status,
                                CreatedAt = workflowInstance.CreatedAt
                            }));
        }
    }
}