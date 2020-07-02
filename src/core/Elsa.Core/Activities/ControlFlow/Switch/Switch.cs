using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Models;
using Elsa.Results;
using Elsa.Services;
using Elsa.Services.Models;

// ReSharper disable once CheckNamespace
namespace Elsa.Activities.ControlFlow
{
    [WorkflowDefinitionActivity(
        Category = "Control Flow",
        Description = "Switch execution based on a given expression.",
        Icon = "far fa-list-alt",
        RuntimeDescription = "x => (x.state.Value != undefined && x.state.Value.value.Expression != '') ? `Switch execution based on <strong>${x.state.Value.value.Expression}</strong>.` : x.definition.description",
        Outcomes = new[] { "x => x.state.Cases.value", OutcomeNames.Default }
    )]
    public class Switch : Activity
    {
        public Switch()
        {
            Cases = new HashSet<string>()
            {
                OutcomeNames.Default
            };
        }

        [ActivityProperty(Hint = "The value to evaluate. The evaluated value will be used to switch on.")]
        public IWorkflowExpression<string> Value
        {
            get => GetState<IWorkflowExpression<string>>();
            set => SetState(value);
        }

        [ActivityProperty(Hint = "A comma-separated list of possible outcomes of the expression.")]
        public ICollection<string> Cases
        {
            get => GetState(() => new string[0]);
            set => SetState(value);
        }

        protected override async Task<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context, CancellationToken cancellationToken)
        {
            var result = await context.EvaluateAsync(Value, cancellationToken);

            if (ContainsCase(result))
            {
                return ExecutionResult(WorkflowStatus.Completed, null, null, result);
            }

            return ExecutionResult(WorkflowStatus.Completed, null, null, OutcomeNames.Default);
        }

        private bool ContainsCase(string @case)
        {
            return Cases.Contains(@case) ? true : false;
        }
    }
}