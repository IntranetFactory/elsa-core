import {FieldDriver} from "../services/field-driver";
import {Activity, ActivityPropertyDescriptor, RenderResult, WorkflowExpression} from "../models";

export class ExpressionFieldDriver implements FieldDriver {
  displayEditor = (activity: Activity, property: ActivityPropertyDescriptor): RenderResult => {
    const name = property.name;
    const label = property.label;
    const value: WorkflowExpression = activity.state[name] || { Expression: '', Type: 'Literal' };
    const syntaxValue = value["value"] != undefined ? value["value"].Type : value.Type;
    const multiline: boolean = (property.options || {}).multiline || false;
    const expressionValue = value["value"] != undefined ? value["value"].Expression.replace(/"/g, '&quot;') : value.Expression.replace(/"/g, '&quot;');

    return `<wf-expression-field name="${name}" label="${label}" hint="${property.hint}" value="${expressionValue}" syntax="${syntaxValue}" multiline="${multiline}"></wf-expression-field>`;
  };

  updateEditor = (activity: Activity, property: ActivityPropertyDescriptor, formData: FormData) => {
    const expressionFieldName = `${property.name}.expression`;
    const syntaxFieldName = `${property.name}.syntax`;
    const expression = formData.get(expressionFieldName).toString().trim();
    const syntax = formData.get(syntaxFieldName).toString();

    activity.state[property.name] = {
      value: {
        type: syntax,
        expression: expression,
        typeName: syntax + "Expression",
      }
    };
  };

}
