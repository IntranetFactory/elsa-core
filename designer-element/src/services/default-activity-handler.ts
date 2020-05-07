import {ActivityHandler} from "./activity-handler";
import {Activity, ActivityDefinition, RenderDesignerResult} from "../models";
import {FormUpdater} from "../utils";

export class DefaultActivityHandler implements ActivityHandler {
  renderDesigner = (activity: Activity, definition: ActivityDefinition): RenderDesignerResult => {
    let description = null;

    if (activity.state.description)
      description = activity.state.description;
    else if (!!definition.runtimeDescription)
      description = definition.runtimeDescription;
    else
      description = definition.description;

    try {
      const fun = eval(description);

      description = fun({ activity, definition, state: activity.state });
    } catch {
    }

    return {
      title: activity.state.title || definition.displayName,
      description: description,
      icon: definition.icon || 'fas fa-cog'
    }
  };

  updateEditor = (activity: Activity, formData: FormData): Activity => FormUpdater.updateEditor(activity, formData);

  getOutcomes = (activity: Activity, definition: ActivityDefinition): Array<string> => {
    let outcomes = [];
    let lambdaOutcomes = [];
    let allOutcomes = [];

    if (!!definition) {
      const lambda = definition.outcomes;
      outcomes = lambda as Array<string>;

      outcomes.forEach(function(outcome) {
        if(outcome.indexOf('=>') >= 0)
        {
          const value = eval(outcome);

          if(value instanceof Array)
          {
            lambdaOutcomes = value;
          } else if(value instanceof Function) {
            try {
              lambdaOutcomes = value({ activity, definition, state: activity.state });
            } catch (e) {
              console.warn(e);
              lambdaOutcomes = [];
            }
          }

          lambdaOutcomes.forEach(function(lambdaOutcome) {
            allOutcomes.push(lambdaOutcome);
          });
        } else {
          allOutcomes.push(outcome);
        }
      });

    }

    return !!allOutcomes ? allOutcomes : [];
  }
}
