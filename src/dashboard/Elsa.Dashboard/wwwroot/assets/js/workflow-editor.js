const designer = document.querySelector("#designerHost");
const modal = document.querySelector("#workflow-properties-modal");
let workflow = null;

designer.addEventListener('workflowChanged', onWorkflowChanged);

function autoLayout() {
    var g = new dagre.graphlib.Graph();
    var allNodes = document.querySelectorAll("[data-activity-id]");
    g.setGraph({ nodesep: 100, ranksep: 100, marginx: 100, marginy: 100 });
    g.setDefaultEdgeLabel(function () { return {}; });

    allNodes.forEach(function (element) {
        var elDataId = element.dataset.activityId;
        g.setNode(elDataId, {
            width: element.offsetWidth,
            height: element.offsetHeight
        });
    });

    var workflow = JSON.parse(designer.workflowData);

    workflow.connections.forEach(function (edge) {
        g.setEdge(
            edge.sourceActivityId,
            edge.destinationActivityId
        );
    });

    dagre.layout(g);

    g.nodes().forEach(function (n) {
        var node = g.node(n);
        var idNode = document.querySelector("[data-activity-id='" + n + "']");

        if (node != undefined) {
            var top = node.y - node.height / 2 + 'px';
            var left = node.x - node.width / 2 + 'px';
            $('#' + idNode.id).css({ left: left, top: top });

            if (idNode) {
                triggerMouseEvent(idNode, "mouseover");
                triggerMouseEvent(idNode, "mousedown");
                triggerMouseEvent(idNode, "mousemove");
                triggerMouseEvent(idNode, "mouseup");
            }
        }
    });
}

function triggerMouseEvent(node, eventType) {
    var clickEvent = document.createEvent('MouseEvents');
    clickEvent.initEvent(eventType, true, true);
    node.dispatchEvent(clickEvent);
}

function addActivity() {
    designer.showActivityPicker();
}

function createNewWorkflow() {
    if (confirm('Are you sure you want to discard current changes?'))
        designer.newWorkflow();
}

function importWorkflow() {
    designer.import();
}

function exportWorkflow() {
    designer.export({
        format: 'json',
        fileExtension: '.json',
        mimeType: 'application/json',
        displayName: 'JSON'
    });
}

function onWorkflowPropertiesSubmit(e) {
    e.preventDefault();
    const formData = new FormData(e.target);
    const name = formData.get("Name").toString();
    const description = formData.get("Description").toString();
    const isDisabled = formData.get("IsDisabled").toString() === 'true';
    const isSingleton = formData.get("IsSingleton").toString() === 'true';
    const editorCaption = document.querySelector("#editorCaption");
    const editorDescription = document.querySelector("#editorDescription");
    const workflowNameInput = document.querySelector("#workflowName");
    const workflowDescriptionInput = document.querySelector("#workflowDescription");
    const workflowIsDisabledInput = document.querySelector("#workflowIsDisabled");
    const workflowSingletonInput = document.querySelector("#workflowIsSingleton");
    
    editorCaption.innerHTML = workflowNameInput.value = name;
    editorDescription.innerHTML = workflowDescriptionInput.value = description;
    workflowIsDisabledInput.value = isDisabled.toString();
    workflowSingletonInput.value = isSingleton.toString();

    $(modal).modal('hide');
}

function onWorkflowChanged(e) {
    const workflow = e.detail;
    const json = JSON.stringify(workflow);
    const input = document.querySelector('#workflowData');

    input.value = json;
}