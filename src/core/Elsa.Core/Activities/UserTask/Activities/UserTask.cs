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
            string userAction = context.GetVariable(VariableName).ToString();
            return Actions.Contains(userAction, StringComparer.OrdinalIgnoreCase);
        }

        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
        {
            return ExecutionResult(WorkflowStatus.Blocked, this.Tag);
        }

        protected override IActivityExecutionResult OnResume(ActivityExecutionContext context)
        {
            // use _Decision_activityId as a default name for variable which holds the decision if VariableName is not provided.
            VariableName = (String.IsNullOrWhiteSpace(VariableName)) ? "_Decision_" + this.Id : VariableName;
            string userAction = context.GetVariable(VariableName).ToString();
            return ExecutionResult(WorkflowStatus.Completed, this.Tag, default, userAction);
        }
    }
}
