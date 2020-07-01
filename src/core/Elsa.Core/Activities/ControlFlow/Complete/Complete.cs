using System.Threading;
using System.Threading.Tasks;
using Elsa.Attributes;
using Elsa.Models;
using Elsa.Results;
using Elsa.Services;
using Elsa.Services.Models;

// ReSharper disable once CheckNamespace
namespace Elsa.Activities.ControlFlow
{
    [WorkflowDefinitionActivity(
        Category = "Workflows",
        Description = "Is used to mark the ending of the workflow diagram.",
        Icon = "fas fa-flag-checkered",
        Outcomes = new string[0],
        AllowEdit = false
    )]
    public class Complete : Activity
    {
        protected override async Task<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context, CancellationToken cancellationToken)
        {
            return new ExecutionResult(WorkflowStatus.Completed);
        }
    }
}