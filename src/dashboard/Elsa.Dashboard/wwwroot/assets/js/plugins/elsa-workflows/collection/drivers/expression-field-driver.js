export class ExpressionFieldDriver {
    constructor() {
        this.displayEditor = (activity, property) => {
            const name = property.name;
            const label = property.label;
            const value = activity.state[name] || { Expression: '', Type: 'Literal' };
            const syntaxValue = value["value"] != undefined ? value["value"].Type : value.Type;
            const multiline = (property.options || {}).multiline || false;
            const expressionValue = value["value"] != undefined ? value["value"].Expression.replace(/"/g, '&quot;') : value.Expression.replace(/"/g, '&quot;');
            return `<wf-expression-field name="${name}" label="${label}" hint="${property.hint}" value="${expressionValue}" syntax="${syntaxValue}" multiline="${multiline}"></wf-expression-field>`;
        };
        this.updateEditor = (activity, property, formData) => {
            const expressionFieldName = `${property.name}.expression`;
            const syntaxFieldName = `${property.name}.syntax`;
            const expression = formData.get(expressionFieldName).toString().trim();
            const syntax = formData.get(syntaxFieldName).toString();
            activity.state[property.name] = {
                value: {
                    Type: syntax,
                    Expression: expression,
                    TypeName: syntax + "Expression",
                }
            };
        };
    }
}
