using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Attributes;
using Elsa.Design;
using Elsa.Expressions;
using Elsa.Models;
using Elsa.Results;
using Elsa.Services;
using Elsa.Services.Models;

namespace Elsa.Activities
{
    [WorkflowDefinitionActivity(
        Category = "Timers",
        Description = "Triggers the timer."
    )]
    public class Timer : Activity
    {
        [ActivityProperty(Hint = "An expression that evaluates to date.")]
        public IWorkflowExpression<string> DateExpression
        {
            get => GetState<IWorkflowExpression<string>>();
            set => SetState(value);
        }

        [ActivityProperty(Type = ActivityPropertyTypes.Select, Hint = "The type of offset to use.")]
        [SelectOptions("GetOffsetTypeOptions", typeof(Timer))]
        public string OffsetType
        {
            get => GetState(() => "h");
            set => SetState(value);
        }

        [ActivityProperty(Hint = "Offset number.")]
        public int Offset
        {
            get => GetState(() => 1);
            set => SetState(value);
        }

        private DateTime? Date { get; set; }

        protected override async Task<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context, CancellationToken cancellationToken)
        {
            var evaluatedDateExpression = await context.EvaluateAsync(DateExpression, cancellationToken);

            if (String.IsNullOrWhiteSpace(evaluatedDateExpression))
            {
                Date = DateTime.UtcNow;
            }
            else
            {
                Date = Convert.ToDateTime(evaluatedDateExpression);
            }

            switch (OffsetType)
            {
                case "m":
                    Date = Date.Value.AddMinutes(Offset);
                    break;

                case "h":
                    Date = Date.Value.AddHours(Offset);
                    break;

                case "d":
                    Date = Date.Value.AddDays(Offset);
                    break;

                case "w":
                    Date = Date.Value.AddDays(Offset * 7);
                    break;
            }

            return new ExecutionResult(WorkflowStatus.Scheduled, null, null, null, Variable.From(Date));
        }

        // OnResumeAsync method is called when Scheduled task is due
        protected override async Task<IActivityExecutionResult> OnResumeAsync(ActivityExecutionContext context, CancellationToken cancellationToken)
        {
            return new ExecutionResult(WorkflowStatus.Completed);
        }

        public static List<SelectOption> GetOffsetTypeOptions()
        {
            return new List<SelectOption>()
            {
                new SelectOption("Minutes", "m"),
                new SelectOption("Hours", "h"),
                new SelectOption("Days", "d"),
                new SelectOption("Weeks", "w"),
            };
        }
    }
}
