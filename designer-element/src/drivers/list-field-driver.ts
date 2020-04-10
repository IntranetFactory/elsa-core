import { FieldDriver } from "../services/field-driver";
import { Activity, ActivityPropertyDescriptor, RenderResult } from "../models";

export class ListFieldDriver implements FieldDriver {
  displayEditor = (activity: Activity, property: ActivityPropertyDescriptor): RenderResult => {
    const name = property.name;
    const label = property.label;
    var stateItems;

    if(activity.state[name])
    {
      if(activity.state[name].value)
      {
        stateItems = activity.state[name].value as Array<any> || [];
      }
    }

    const value = stateItems != undefined ? stateItems.join(', ') : '';

    return `<wf-list-field name="${ name }" label="${ label }" hint="${ property.hint }" items="${ value }"></wf-list-field>`;
  };

  updateEditor = (activity: Activity, property: ActivityPropertyDescriptor, formData: FormData) => {
    const value = formData.get(property.name).toString();

    activity.state[property.name] = {
      value: value.split(',').map(x => x.trim())
    }
  };

}
