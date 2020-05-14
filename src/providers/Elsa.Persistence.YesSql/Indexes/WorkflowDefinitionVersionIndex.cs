using System.Collections.Generic;
using System.Linq;
using Elsa.Models;
using Elsa.Persistence.YesSql.Documents;
using YesSql.Indexes;

namespace Elsa.Persistence.YesSql.Indexes
{
    public class WorkflowDefinitionVersionIndex : MapIndex
    {
        public string VersionId { get; set; }
        public int? TenantId { get; set; }
        public string WorkflowDefinitionId { get; set; }
        public int Version { get; set; }
        public bool IsLatest { get; set; }
        public bool IsPublished { get; set; }
        public bool IsDisabled { get; set; }
    }

    public class WorkflowDefinitionVersionStartActivitiesIndex : MapIndex
    {
        public string StartActivityId { get; set; }
        public string StartActivityType { get; set; }
        public bool IsDisabled { get; set; }
    }

    public class WorkflowDefinitionVersionIndexProvider : IndexProvider<WorkflowDefinitionVersionDocument>
    {
        public override void Describe(DescribeContext<WorkflowDefinitionVersionDocument> context)
        {
            context.For<WorkflowDefinitionVersionIndex>()
                .Map(
                    document => new WorkflowDefinitionVersionIndex
                    {
                        VersionId = document.VersionId,
                        TenantId = document.TenantId,
                        WorkflowDefinitionId = document.WorkflowDefinitionId,
                        Version = document.Version,
                        IsPublished = document.IsPublished,
                        IsLatest = document.IsLatest,
                        IsDisabled = document.IsDisabled
                    }
                );

            context.For<WorkflowDefinitionVersionStartActivitiesIndex>()
                .Map(
                    document => GetStartActivities(document)
                        .Select(
                            activity => new WorkflowDefinitionVersionStartActivitiesIndex
                            {
                                StartActivityId = activity.Id,
                                StartActivityType = activity.Type,
                                IsDisabled = document.IsDisabled
                            }
                        )
                );
        }

        private static IEnumerable<WorkflowDefinitionActivity> GetStartActivities(WorkflowDefinitionVersionDocument workflow)
        {
            var destinationActivityIds = workflow.Connections.Select(x => x.DestinationActivityId).Distinct().ToLookup(x => x);

            var query =
                from activity in workflow.Activities
                where !destinationActivityIds.Contains(activity.Id)
                select activity;

            return query;
        }
    }
}