using Elsa.Attributes;
using Elsa.Design;
using Elsa.Models;
using Elsa.Results;
using Elsa.Services;
using Elsa.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elsa.Activities.UserTask.Activities
{
    /// <summary>
    /// Stores a set of possible user actions and halts the workflow until one of the actions has been performed.
    /// </summary>
    [WorkflowDefinitionActivity(
        Category = "User Tasks",
        Description = "Triggers when a user action is received.",
        Outcomes = new[] { OutcomeNames.Done, "x => x.state.actions.value" }
    )]
    public class UserTask : Activity
    {
        [ActivityProperty(
            Type = ActivityPropertyTypes.List,
            Hint = "Enter a comma-separated list of available actions"
        )]
        public ICollection<string> Actions
        {
            get => GetState(() => new string[0]);
            set => SetState(value);
        }

        [ActivityProperty(
            Type = ActivityPropertyTypes.Text,
            Hint = "The name of the variable to store the decision into.")]
        public string VariableName
        {
            get => GetState<string>();
            set => SetState(value);
        }

        /// <summary>
        /// Only a user or a group of users that belong to this tag will see the activity. 
        /// </summary>
        [ActivityProperty(
            Type = ActivityPropertyTypes.Text,
            Hint = "Only a user or a group of users that belong to this tag will see the activity."
        )]
        public override string Tag
        {
            get => GetState<string>();
            set => SetState(value);
        }

        protected override bool OnCanExecute(ActivityExecutionContext context)
        {
            var userAction = GetUserAction(context);

            return Actions.Contains(userAction, StringComparer.OrdinalIgnoreCase);
        }

        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
        {
            return ExecutionResult(WorkflowInstanceTaskStatus.Blocked, this.Tag);
        }

        protected override IActivityExecutionResult OnResume(ActivityExecutionContext context)
        {
            var userAction = GetUserAction(context);
            // We set the variable with specified name so that it can be used in next activities.
            VariableName = VariableName == "" ? "Decision" : VariableName;
            context.SetVariable(VariableName, userAction);
            return ExecutionResult(WorkflowInstanceTaskStatus.Completed, this.Tag, default, userAction);
        }

        private string GetUserAction(ActivityExecutionContext context) => context.Input?.GetValue<string>();
    }
}
