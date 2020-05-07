import { FormUpdater } from "../utils";
export class DefaultActivityHandler {
    constructor() {
        this.renderDesigner = (activity, definition) => {
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
            }
            catch (_a) {
            }
            return {
                title: activity.state.title || definition.displayName,
                description: description,
                icon: definition.icon || 'fas fa-cog'
            };
        };
        this.updateEditor = (activity, formData) => FormUpdater.updateEditor(activity, formData);
        this.getOutcomes = (activity, definition) => {
            let outcomes = [];
            let lambdaOutcomes = [];
            let allOutcomes = [];
            if (!!definition) {
                const lambda = definition.outcomes;
                outcomes = lambda;
                outcomes.forEach(function (outcome) {
                    if (outcome.indexOf('=>') >= 0) {
                        const value = eval(outcome);
                        if (value instanceof Array) {
                            lambdaOutcomes = value;
                        }
                        else if (value instanceof Function) {
                            try {
                                lambdaOutcomes = value({ activity, definition, state: activity.state });
                            }
                            catch (e) {
                                console.warn(e);
                                lambdaOutcomes = [];
                            }
                        }
                        lambdaOutcomes.forEach(function (lambdaOutcome) {
                            allOutcomes.push(lambdaOutcome);
                        });
                    }
                    else {
                        allOutcomes.push(outcome);
                    }
                });
            }
            return !!allOutcomes ? allOutcomes : [];
        };
    }
}
