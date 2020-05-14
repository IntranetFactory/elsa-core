using Elsa.Models;
using Elsa.Services;

namespace Elsa.Extensions
{
    public static class ActivityResolverExtensions
    {
        public static IActivity ResolveActivity(this IActivityResolver activityResolver, WorkflowDefinitionActivity workflowDefinitionActivity)
        {
            var activity = activityResolver.ResolveActivity(workflowDefinitionActivity.Type);
            activity.Description = workflowDefinitionActivity.Description;
            activity.Id = workflowDefinitionActivity.Id;
            activity.Name = workflowDefinitionActivity.Name;
            activity.DisplayName = workflowDefinitionActivity.DisplayName;
            activity.PersistWorkflow = workflowDefinitionActivity.PersistWorkflow;
            activity.State = workflowDefinitionActivity.State;
            return activity;
        }
    }
}