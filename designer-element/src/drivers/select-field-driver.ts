import { FieldDriver } from "../services/field-driver";
import { Activity, ActivityPropertyDescriptor, RenderResult } from "../models";
import { SelectItem } from "../components/field-editors/select-field/models";

export class SelectFieldDriver implements FieldDriver {
  displayEditor = (activity: Activity, property: ActivityPropertyDescriptor): RenderResult => {
    const name: string = property.name;
    const label: string = property.label;
    const stateProperty = activity.state[name];
    const value = stateProperty != undefined ? stateProperty.value : '';
    const items: Array<SelectItem> = property.options.Items || [];
    const itemsValues = [];

    items.forEach(function(item, index) {
      itemsValues.push(item["label"]);
    });

    const itemsJson = encodeURI(JSON.stringify(itemsValues));

    return `<wf-select-field name="${ name }" label="${ label }" hint="${ property.hint }" data-items="${ itemsJson }" value="${ value }"></wf-select-field>`;
  };

  updateEditor = (activity: Activity, property: ActivityPropertyDescriptor, formData: FormData) => {
    const value = formData.get(property.name).toString();
    activity.state[property.name] = {
      value: value.trim()
    };
  };

}
