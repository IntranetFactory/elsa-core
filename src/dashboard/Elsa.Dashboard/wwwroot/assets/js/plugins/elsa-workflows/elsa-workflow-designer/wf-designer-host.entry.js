import { r as registerInstance, c as createEvent, h, d as getElement } from './chunk-25ccd4a5.js';
import { c as createCommonjsModule, a as commonjsGlobal, d as deepClone } from './chunk-b68c6ae2.js';
import { D as DisplayManager } from './chunk-5dfd54dd.js';

var dragscroll = createCommonjsModule(function (module, exports) {
!function(e,n){"function"==typeof undefined&&undefined.amd?undefined(["exports"],n):n("undefined"!='object'?exports:e.dragscroll={});}(commonjsGlobal,function(e){var n,t,o=window,l=document,c="mousemove",r="mouseup",i="mousedown",m="EventListener",d="add"+m,s="remove"+m,f=[],u=function(e,m){for(e=0;e<f.length;)m=f[e++],m=m.container||m,m[s](i,m.md,0),o[s](r,m.mu,0),o[s](c,m.mm,0);for(f=[].slice.call(l.getElementsByClassName("dragscroll")),e=0;e<f.length;)!function(e,m,s,f,u,a){(a=e.container||e)[d](i,a.md=function(n){e.hasAttribute("nochilddrag")&&l.elementFromPoint(n.pageX,n.pageY)!=a||(f=1,m=n.clientX,s=n.clientY,n.preventDefault());},0),o[d](r,a.mu=function(){f=0;},0),o[d](c,a.mm=function(o){f&&((u=e.scroller||e).scrollLeft-=n=-m+(m=o.clientX),u.scrollTop-=t=-s+(s=o.clientY),e==l.body&&((u=l.documentElement).scrollLeft-=n,u.scrollTop-=t));},0);}(f[e++]);};"complete"==l.readyState?u():o[d]("load",u,0),e.reset=u;});
});

class BooleanFieldDriver {
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

class ExpressionFieldDriver {
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

class ListFieldDriver {
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

class TextFieldDriver {
    constructor() {
        this.displayEditor = (activity, property) => {
            const name = property.name;
            const label = property.label;
            const stateProperty = activity.state[name];
            const value = stateProperty != undefined ? stateProperty.value : '';
            return `<wf-text-field name="${name}" label="${label}" hint="${property.hint}" value="${value}"></wf-text-field>`;
        };
        this.updateEditor = (activity, property, formData) => {
            activity.state[property.name] = {
                value: formData.get(property.name).toString().trim()
            };
        };
    }
}

class SelectFieldDriver {
    constructor() {
        this.displayEditor = (activity, property) => {
            const name = property.name;
            const label = property.label;
            const stateProperty = activity.state[name];
            const value = stateProperty != undefined ? stateProperty.value : '';
            const items = property.options.Items || [];
            const itemsValues = [];
            items.forEach(function (item, index) {
                if (item["value"]) {
                    itemsValues.push(item);
                }
                else {
                    itemsValues.push(item["label"]);
                }
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

class DesignerHost {
    constructor(hostRef) {
        registerInstance(this, hostRef);
        this.activityDefinitions = [];
        this.onWorkflowChanged = (e) => {
            this.workflowChanged.emit(e.detail);
            this.workflowData = JSON.stringify(e.detail);
        };
        this.initActivityDefinitions = () => {
            if (!!this.activityDefinitionsData) {
                const definitions = JSON.parse(this.activityDefinitionsData);
                this.activityDefinitions = [...this.activityDefinitions, ...definitions];
            }
        };
        this.initFieldDrivers = () => {
            DisplayManager.addDriver('text', new TextFieldDriver());
            DisplayManager.addDriver('expression', new ExpressionFieldDriver());
            DisplayManager.addDriver('list', new ListFieldDriver());
            DisplayManager.addDriver('boolean', new BooleanFieldDriver());
            DisplayManager.addDriver('select', new SelectFieldDriver());
        };
        this.initWorkflow = () => {
            if (!!this.workflowData) {
                const workflow = JSON.parse(this.workflowData);
                if (!workflow.activities)
                    workflow.activities = [];
                if (!workflow.connections)
                    workflow.connections = [];
                this.designer.workflow = workflow;
            }
        };
        this.workflowChanged = createEvent(this, "workflowChanged", 7);
    }
    async newWorkflow() {
        await this.designer.newWorkflow();
    }
    async autoLayout() {
        await this.designer.autoLayout();
    }
    async getWorkflow() {
        return await this.designer.getWorkflow();
    }
    async showActivityPicker() {
        await this.activityPicker.show();
    }
    async export(formatDescriptor) {
        await this.importExport.export(this.designer, formatDescriptor);
    }
    async import() {
        await this.importExport.import();
    }
    async onActivityPicked(e) {
        await this.designer.addActivity(e.detail);
    }
    async onEditActivity(e) {
        this.activityEditor.activity = e.detail;
        this.activityEditor.show = true;
    }
    async onAddActivity() {
        await this.showActivityPicker();
    }
    async onUpdateActivity(e) {
        await this.designer.updateActivity(e.detail);
    }
    async onExportWorkflow(e) {
        if (!this.importExport)
            return;
        await this.importExport.export(this.designer, e.detail);
    }
    async onImportWorkflow(e) {
        this.designer.workflow = deepClone(e.detail);
    }
    componentWillLoad() {
        this.initActivityDefinitions();
        this.initFieldDrivers();
    }
    componentDidLoad() {
        this.initWorkflow();
    }
    render() {
        const activityDefinitions = this.activityDefinitions;
        return (h("host", null, h("wf-activity-picker", { activityDefinitions: activityDefinitions, ref: el => this.activityPicker = el }), h("wf-activity-editor", { activityDefinitions: activityDefinitions, ref: el => this.activityEditor = el }), h("wf-import-export", { ref: el => this.importExport = el }), h("div", { class: "workflow-designer-wrapper dragscroll" }, h("wf-designer", { activityDefinitions: activityDefinitions, ref: el => this.designer = el, canvasHeight: this.canvasHeight, workflow: this.workflow, readonly: this.readonly, onWorkflowChanged: this.onWorkflowChanged }))));
    }
    get el() { return getElement(this); }
    static get style() { return ".workflow-designer-wrapper {\n  height: 80vh;\n  overflow-y: auto;\n}"; }
}

export { DesignerHost as wf_designer_host };
