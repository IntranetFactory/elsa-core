export class SelectFieldDriver {
    constructor() {
        this.displayEditor = (activity, property) => {
            const name = property.name;
            const label = property.label;
            const stateProperty = activity.state[name];
            const value = stateProperty != undefined ? stateProperty.value : '';
            const items = property.options.Items || [];
            const itemsValues = [];
            items.forEach(function (item, index) {
                itemsValues.push(item["label"]);
            });
            const itemsJson = encodeURI(JSON.stringify(itemsValues));
            return `<wf-select-field name="${name}" label="${label}" hint="${property.hint}" data-items="${itemsJson}" value="${value}"></wf-select-field>`;
        };
        this.updateEditor = (activity, property, formData) => {
            const value = formData.get(property.name).toString();
            activity.state[property.name] = {
                value: value.trim()
            };
        };
    }
}
