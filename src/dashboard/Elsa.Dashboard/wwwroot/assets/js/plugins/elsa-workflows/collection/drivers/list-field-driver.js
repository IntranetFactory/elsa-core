export class ListFieldDriver {
    constructor() {
        this.displayEditor = (activity, property) => {
            const name = property.name;
            const label = property.label;
            var stateItems;
            if (activity.state[name]) {
                if (activity.state[name].value) {
                    stateItems = activity.state[name].value || [];
                }
            }
            const value = stateItems != undefined ? stateItems.join(', ') : '';
            return `<wf-list-field name="${name}" label="${label}" hint="${property.hint}" items="${value}"></wf-list-field>`;
        };
        this.updateEditor = (activity, property, formData) => {
            const value = formData.get(property.name).toString();
            activity.state[property.name] = {
                value: value.split(',').map(x => x.trim())
            };
        };
    }
}
