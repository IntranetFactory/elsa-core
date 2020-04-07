export class BooleanFieldDriver {
    constructor() {
        this.displayEditor = (activity, property) => {
            const name = property.name;
            const label = property.label;
            const stateProperty = activity.state[name];
            const checked = stateProperty != undefined ? stateProperty.value : false;
            return `<wf-boolean-field name="${name}" label="${label}" hint="${property.hint}" checked="${checked}"></wf-boolean-field>`;
        };
        this.updateEditor = (activity, property, formData) => {
            activity.state[property.name] = {
                value: formData.get(property.name)
            };
        };
    }
}
