using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Attributes;
using Elsa.Design;
using Elsa.Extensions;
using Elsa.Messaging.Domain;
using Elsa.Results;
using Elsa.Services;
using Elsa.Services.Models;
using MediatR;

// ReSharper disable once CheckNamespace
namespace Elsa.Activities.ControlFlow
{
    [WorkflowDefinitionActivity(
        Category = "Control Flow",
        Description = "Merge workflow execution back into a single branch.",
        Icon = "fas fa-code-branch",
        RuntimeDescription = "x => !!x.state.joinMode.value ? `Merge workflow execution back into a single branch using mode <strong>${ x.state.joinMode }</strong>` : x.definition.description",
        Outcomes = new[] { OutcomeNames.Done }
    )]
    public class Join : Activity
    {
        public Join()
        {
        }

        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
        {
            // TO DO: Join must be re-implemented because blocking activities have been removed
            //var workflowExecutionContext = context.WorkflowExecutionContext;
            //var inboundConnectionActivityIds = workflowExecutionContext.GetInboundConnections(this).Select(x => x.Source.Activity.Id).ToList();
            //var allDone = true;

            //foreach(string id in inboundConnectionActivityIds)
            //{
            //    if(workflowExecutionContext.WorkflowInstanceTasks.Where(x => x.Activity.Id == id).Any())
            //    {
            //        allDone = false;
            //    }
            //}

            //if(allDone)
            //{
            //    // Remove any inbound blocking activities.
            //    var ancestorActivityIds = workflowExecutionContext.GetInboundActivityPath(this).ToList();

            //    List<IActivity> blockingActivities = new List<IActivity>();

            //    foreach (var blockingActivity in workflowExecutionContext.WorkflowInstanceBlockingActivities)
            //    {
            //        if (ancestorActivityIds.Contains(blockingActivity.Id))
            //        {
            //            blockingActivities.Add(blockingActivity);
            //        }
            //        else
            //        {
            //            var thisActivity = workflowExecutionContext.WorkflowInstanceBlockingActivities.Where(x => x.Id == this.Id).FirstOrDefault();

            //            if (thisActivity != null)
            //                blockingActivities.Add(thisActivity);
            //        }
            //    }

            //    foreach (var blockingActivity in blockingActivities)
            //        workflowExecutionContext.WorkflowInstanceBlockingActivities.Remove(blockingActivity);
            //}

            //if (!allDone)
            //    return Noop();
            
            return Done();
        }
    }
}