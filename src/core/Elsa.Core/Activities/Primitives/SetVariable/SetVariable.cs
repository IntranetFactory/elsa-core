using System.Threading;
using System.Threading.Tasks;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Models;
using Elsa.Results;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.Extensions.Localization;

// ReSharper disable once CheckNamespace
namespace Elsa.Activities.Primitives
{
    [WorkflowDefinitionActivity(
        DisplayName = "Set Variable",
        Description = "Set variable on the workflow.",
        Category = "Primitives",
        RuntimeDescription = "x => (x.state.variableName != undefined && !!x.state.variableName.value) ? `<strong>${x.state.variableName.value}</strong> = <strong>${x.state.value.value.expression}</strong><br/>${x.state.value.value.type} expression` : x.definition.description",
        Outcomes = new[] { OutcomeNames.Done }
    )]
    public class SetVariable : Activity
    {
        public SetVariable()
        {
        }

        public SetVariable(string name, IWorkflowExpression value)
        {
            VariableName = name;
            Value = value;
        }
        
        [ActivityProperty(Hint = "The name of the variable to store the value into.")]
        public string VariableName
        {
            get => GetState<string>();
            set => SetState(value);
        }

        [ActivityProperty(Hint = "An expression that evaluates to the value to store in the variable.")]
        public IWorkflowExpression Value
        {
            get => GetState<IWorkflowExpression>();
            set => SetState(value);
        }

        protected override async Task<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context, CancellationToken cancellationToken)
        {
            var value = await context.EvaluateAsync(Value, cancellationToken);

            context.SetVariable(VariableName, value);
            return ExecutionResult(WorkflowStatus.Completed);
        }
    }
}