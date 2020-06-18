using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Models;
using Elsa.Results;
using Elsa.Services;
using Elsa.Services.Models;
using SimpleJson;

// ReSharper disable once CheckNamespace
namespace Elsa.Activities.ControlFlow
{
    [WorkflowDefinitionActivity(
        Category = "Control Flow",
        Description = "Iterate over a collection.",
        Icon = "far fa-circle",
        Outcomes = new[] { OutcomeNames.Iterate, OutcomeNames.Done }
    )]
    public class ForEach : Activity
    {
        [ActivityProperty(Hint = "Enter the name of an array of items to iterate over.")]
        public string ArrayName
        {
            get => GetState<string>();
            set => SetState(value);
        }

        [ActivityProperty(Hint = "Enter the name of the variable that holds iterated items.")]
        public string ItemName
        {
            get => GetState<string>();
            set => SetState(value);
        }

        private int? CurrentIndex
        {
            get => GetState<int?>();
            set => SetState(value);
        }

        protected override async Task<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context, CancellationToken cancellationToken)
        {
            int? currentIndex;

            // finish iteration if ArrayName is not provided, array is null or the type of an array is different than SimpleJson.JsonArray
            if (String.IsNullOrWhiteSpace(ArrayName)) return Done();
            if (context.GetVariable(ArrayName) == null || context.GetVariable(ArrayName).GetType().FullName != "SimpleJson.JsonArray") return Done();

            // we have to cast to JsonArray in order to be able to use .Count() because context returns object{SimpleJson.JsonArray}
            var itemsArray = (JsonArray)context.GetVariable(ArrayName);

            if(String.IsNullOrWhiteSpace(ItemName)) ItemName = "Item";

            if(CurrentIndex == null)
            {
                var currentIndexVariable = context.GetVariable("_ForEach_" + this.Id);
                currentIndex = (currentIndexVariable != null) ? Convert.ToInt32(currentIndexVariable) : 0;
            } 
            else
            {
                currentIndex = CurrentIndex;
            }

            if (currentIndex < itemsArray.Count())
            {
                var currentItem = itemsArray[currentIndex.GetValueOrDefault()];
                CurrentIndex = currentIndex + 1;
                context.SetVariable("_ForEach_" + this.Id, CurrentIndex);
                context.SetVariable(ItemName, currentItem);
                return Done(OutcomeNames.Iterate, Variable.From(currentItem));
            }

            CurrentIndex = null;
            context.SetVariable("_ForEach_" + this.Id, null);
            context.SetVariable(ItemName, null);
            return Done();
        }
    }
}