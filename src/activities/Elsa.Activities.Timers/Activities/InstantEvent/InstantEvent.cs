using System.Threading;
using System.Threading.Tasks;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Results;
using Elsa.Services;
using Elsa.Services.Models;
using NodaTime;
// TO DO: Inspect why InstantEvent activity creates only an idle workflow and doesn't execute at the specified moment in time.

// ReSharper disable once CheckNamespace
namespace Elsa.Activities.Timers
{
    /// <summary>
    /// Triggers at a specific instant in the future.
    /// </summary>
    [WorkflowDefinitionActivity(
        Category = "Timers",
        Description = "Triggers at a specified moment in time."
    )]
    public class InstantEvent : Activity
    {
        private readonly IClock clock;

        public InstantEvent(IClock clock) => this.clock = clock;

        /// <summary>
        /// An expression that evaluates to an <see cref="NodaTime.Instant"/>
        /// </summary>
        [ActivityProperty(Hint = "An expression that evaluates to a NodaTime Instant")]
        public IWorkflowExpression<Instant> Instant
        {
            get => GetState<IWorkflowExpression<Instant>>();
            set => SetState(value);
        }
        
        public Instant? ExecutedAt
        {
            get => GetState<Instant?>();
            set => SetState(value);
        }
        
        protected override async Task<bool> OnCanExecuteAsync(ActivityExecutionContext context, CancellationToken cancellationToken)
        {
            return ExecutedAt == null || await IsExpiredAsync(context, cancellationToken);
        }

        protected override Task<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context, CancellationToken cancellationToken) => 
            OnResumeAsync(context, cancellationToken);

        protected override async Task<IActivityExecutionResult> OnResumeAsync(ActivityExecutionContext context, CancellationToken cancellationToken)
        {
            if (await IsExpiredAsync(context, cancellationToken))
            {
                ExecutedAt = clock.GetCurrentInstant();
                return Done();
            }

            return Suspend();
        }

        private async Task<bool> IsExpiredAsync(ActivityExecutionContext context, CancellationToken cancellationToken)
        {
            var instant = await context.EvaluateAsync(Instant, cancellationToken);
            var now = clock.GetCurrentInstant();

            return now >= instant;
        }
    }
}