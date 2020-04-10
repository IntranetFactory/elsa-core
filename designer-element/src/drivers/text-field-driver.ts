import { FieldDriver } from "../services/field-driver";
import { Activity, ActivityPropertyDescriptor, RenderDesignerResult, RenderResult } from "../models";

export class TextFieldDriver implements FieldDriver
{
  displayEditor = (activity: Activity, property: ActivityPropertyDescriptor): RenderResult => {
    const name = property.name;
    const label = property.label;
    const stateProperty = activity.state[name];
    const value = stateProperty != undefined ? stateProperty.value : '';

    return `<wf-text-field name="${name}" label="${label}" hint="${property.hint}" value="${value}"></wf-text-field>`;
  };

  updateEditor = (activity: Activity, property: ActivityPropertyDescriptor, formData: FormData) => {
    activity.state[property.name] = {
      value: formData.get(property.name).toString().trim()
    };
  };

}
