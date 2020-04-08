//const modal = document.querySelector("#workflow-properties-modal");
let workflow = null;

function loadWorkflow(tenantId, workflowId) {
    var xhttp = new XMLHttpRequest();

    xhttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {

            var workflowDefinition = JSON.parse(this.responseText);
            var designerHost = document.createElement("wf-designer-host");
            designerHost.addEventListener('workflowChanged', onWorkflowChanged);
            designerHost.setAttribute("id", workflowDefinition.id);
            designerHost.setAttribute("data-activity-definitions", JSON.stringify(workflowDefinition.activityDefinitions));
            designerHost.setAttribute("data-workflow", JSON.stringify(workflowDefinition.workflowModel));
            document.body.appendChild(designerHost);
        }
    };

    xhttp.open("GET", "https://localhost:44332/Elsa/workflow-definition-version/LoadWorkflowDefinition?tenantId=" + tenantId + "&id=" + workflowId, true);
    xhttp.send();
}

function saveWorkflow(tenantid, workflowId) {
    var xhttp = new XMLHttpRequest();

    xhttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            console.log(this.responseText);
        }
    };

    var designerHost = document.getElementById(workflowId);

    model = {
        Id: workflowId,
        TenantId: tenantid,
        Json: designerHost.getAttribute("data-workflow"),
        SubmitAction: "draft",
        Name: "Simple Test Workflow"
    }

    xhttp.open("POST", "https://localhost:44332/Elsa/workflow-definition-version/SaveWorkflowDefinition", true);
    xhttp.setRequestHeader("Content-type", "application/json;charset=UTF-8");
    xhttp.send(JSON.stringify(model));
}

function triggerMouseEvent(node, eventType) {
    var clickEvent = document.createEvent('MouseEvents');
    clickEvent.initEvent(eventType, true, true);
    node.dispatchEvent(clickEvent);
}

function onWorkflowPropertiesSubmit(e) {
    e.preventDefault();
}

function onWorkflowChanged(e) {
    const workflow = e.detail;
    const json = JSON.stringify(workflow);
    var designerHost = document.querySelector('wf-designer-host');
    designerHost.setAttribute("data-workflow", json);
}