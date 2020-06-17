using System;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Models;
using Elsa.Results;
using Elsa.Services;
using Elsa.Services.Models;
using Elsa.ExpressionTypes;

// ReSharper disable once CheckNamespace
namespace Elsa.Activities.ControlFlow
{
    [WorkflowDefinitionActivity(
        Category = "Control Flow",
        Description = "Iterate between two numbers.",
        Icon = "far fa-circle",
        Outcomes = new[] { OutcomeNames.Iterate, OutcomeNames.Done }
    )]
    public class For : Activity
    {
        [ActivityProperty(Hint = "An expression that evaluates to the starting number.")]
        public IWorkflowExpression<int> Start
        {
            get => GetState<IWorkflowExpression<int>>();
            set => SetState(value);
        }

        [ActivityProperty(Hint = "An expression that evaluates to the ending number.")]
        public IWorkflowExpression<int> End
        {
            get => GetState<IWorkflowExpression<int>>();
            set => SetState(value);
        }

        [ActivityProperty(Hint = "An expression that evaluates to the incrementing number on each step.")]
        public IWorkflowExpression<int> Step
        {
            get => GetState<IWorkflowExpression<int>>(() => new CodeExpression<int>(() => 1));
            set => SetState(value);
        }

        private int? CurrentValue
        {
            get => GetState<int?>();
            set => SetState(value);
        }

        protected override async Task<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context, CancellationToken cancellationToken)
        {
            var startValue = await context.EvaluateAsync(Start, cancellationToken);
            var endValue = await context.EvaluateAsync(End, cancellationToken);
            var step = await context.EvaluateAsync(Step, cancellationToken);
            int? currentValue;

            if(CurrentValue == null)
            {
                var currentValueVariable = context.GetVariable("For_" + this.Id);
                currentValue = (currentValueVariable != null) ? Convert.ToInt32(currentValueVariable) : startValue;
            }
            else
            {
                currentValue = CurrentValue;
            }

            if (currentValue < endValue)
            {
                currentValue += step;
                CurrentValue = currentValue;
                context.SetVariable("For_" + this.Id, CurrentValue);
                return Combine(Schedule(this), Done(OutcomeNames.Iterate, Variable.From(currentValue)));
                // TO DO: check how to remove Combine result and make this work: return Done(OutcomeNames.Iterate, Variable.From(input));
            }

            CurrentValue = null;
            return Done();
        }
    }
}