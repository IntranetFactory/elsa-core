using System.Collections.Generic;
using System.Linq;
using Elsa.Models;
using Elsa.Services;
using Elsa.Services.Models;

namespace Elsa.Extensions
{
    public static class WorkflowExtensions
    {
        public static IEnumerable<WorkflowDefinitionActiveVersion> WithVersion(this IEnumerable<WorkflowDefinitionActiveVersion> query, VersionOptions version) 
            => query.AsQueryable().WithVersion(version);

        public static IQueryable<WorkflowDefinitionActiveVersion> WithVersion(this IQueryable<WorkflowDefinitionActiveVersion> query, VersionOptions version)
        {
            if (version.IsDraft)
                query = query.Where(x => !x.IsPublished);
            else if (version.IsLatest)
                query = query.Where(x => x.IsLatest);
            else if (version.IsPublished)
                query = query.Where(x => x.IsPublished);
            else if (version.IsLatestOrPublished)
                query = query.Where(x => x.IsPublished || x.IsLatest);
            else if (version.AllVersions)
            {
                // Nothing to filter.
            }
            else if (version.Version > 0)
                query = query.Where(x => x.Version == version.Version);

            return query.OrderByDescending(x => x.Version);
        }
        
        public static IEnumerable<IActivity> GetStartActivities(this WorkflowDefinitionActiveVersion workflowDefinitionActiveVersion)
        {
            var destinationActivityIds = workflowDefinitionActiveVersion.Connections.Select(x => x.Target.Activity.Id).Distinct().ToLookup(x => x);

            var query =
                from activity in workflowDefinitionActiveVersion.Activities
                where !destinationActivityIds.Contains(activity.Id)
                select activity;

            return query;
        }

        public static IActivity GetActivity(this WorkflowDefinitionActiveVersion workflowDefinitionActiveVersion, string id) => workflowDefinitionActiveVersion.Activities.FirstOrDefault(x => x.Id == id);

        public static IEnumerable<Connection> GetInboundConnections(this WorkflowDefinitionActiveVersion workflowDefinitionActiveVersion, string activityId)
        {
            return workflowDefinitionActiveVersion.Connections.Where(x => x.Target.Activity.Id == activityId).ToList();
        }

        public static IEnumerable<Connection> GetOutboundConnections(this WorkflowDefinitionActiveVersion workflowDefinitionActiveVersion, string activityId)
        {
            return workflowDefinitionActiveVersion.Connections.Where(x => x.Source.Activity.Id == activityId).ToList();
        }

        /// <summary>
        /// Returns the full path of incoming activities.
        /// </summary>
        public static IEnumerable<string> GetInboundActivityPath(this WorkflowDefinitionActiveVersion workflowDefinitionActiveVersion, string activityId)
        {
            return workflowDefinitionActiveVersion.GetInboundActivityPathInternal(activityId, activityId).Distinct().ToList();
        }

        private static IEnumerable<string> GetInboundActivityPathInternal(this WorkflowDefinitionActiveVersion workflowDefinitionActiveVersionInstance, string activityId, string startingPointActivityId)
        {
            foreach (var connection in workflowDefinitionActiveVersionInstance.GetInboundConnections(activityId))
            {
                // Circuit breaker: Detect workflows that implement repeating flows to prevent an infinite loop here.
                if (connection.Source.Activity.Id == startingPointActivityId)
                    yield break;

                yield return connection.Source.Activity.Id;

                foreach (var parentActivityId in workflowDefinitionActiveVersionInstance
                    .GetInboundActivityPathInternal(connection.Source.Activity.Id, startingPointActivityId)
                    .Distinct())
                    yield return parentActivityId;
            }
        }
    }
}