using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Dashboard.Areas.Elsa.ViewModels;
using Elsa.Dashboard.Extensions;
using Elsa.Dashboard.Models;
using Elsa.Dashboard.Options;
using Elsa.Dashboard.Services;
using Elsa.Extensions;
using Elsa.Models;
using Elsa.Persistence;
using Elsa.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Elsa.Dashboard.Areas.Elsa.Controllers
{
    [Area("Elsa")]
    [Route("[area]/workflow-instance")]
    public class WorkflowInstanceController : Controller
    {
        private readonly IWorkflowInstanceStore workflowInstanceStore;
        private readonly IWorkflowDefinitionVersionStore workflowDefinitionVersionStore;
        private readonly IWorkflowHost workflowHost;
        private readonly IOptions<ElsaDashboardOptions> options;
        private readonly INotifier notifier;

        public WorkflowInstanceController(
            IWorkflowInstanceStore workflowInstanceStore,
            IWorkflowDefinitionVersionStore workflowDefinitionVersionStore,
            IWorkflowHost workflowHost,
            IOptions<ElsaDashboardOptions> options,
            INotifier notifier)
        {
            this.workflowInstanceStore = workflowInstanceStore;
            this.workflowDefinitionVersionStore = workflowDefinitionVersionStore;
            this.workflowHost = workflowHost;
            this.options = options;
            this.notifier = notifier;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            int? tenantId,
            string definitionId,
            WorkflowStatus status,
            CancellationToken cancellationToken)
        {
            var definitionVersion = await workflowDefinitionVersionStore.GetByIdAsync(
                tenantId,
                definitionId,
                VersionOptions.Latest,
                cancellationToken
            );

            if (definitionVersion == null)
                return NotFound();

            var instances = await workflowInstanceStore
                .ListByStatusAsync(tenantId, definitionId, status, cancellationToken)
                .ToListAsync();

            var model = new WorkflowInstanceListViewModel
            {
                WorkflowDefinition = definitionVersion,
                ReturnUrl = Url.Action("Index", new { definitionId, status }),
                WorkflowInstances = instances.Select(
                        x => new WorkflowInstanceListItemModel
                        {
                            WorkflowInstance = x
                        }
                    )
                    .ToList()
            };

            return View(model);
        }

        [HttpGet("details/{tenantId}/{id}")]
        public async Task<IActionResult> Details(int? tenantId, string id, string returnUrl, CancellationToken cancellationToken)
        {
            var instance = await workflowInstanceStore.GetByIdAsync(tenantId, id, cancellationToken);

            if (instance == null)
                return NotFound();

            var definitionVersion = await workflowDefinitionVersionStore.GetByIdAsync(
                tenantId,
                instance.DefinitionId,
                VersionOptions.SpecificVersion(instance.Version),
                cancellationToken
            );

            var workflow = new WorkflowModel
            {
                Activities = definitionVersion.Activities.Select(x => CreateActivityModel(x, instance)).ToList(),
                Connections = definitionVersion.Connections.Select(x => new ConnectionModel(x)).ToList()
            };

            var model = new WorkflowInstanceDetailsModel(
                instance,
                definitionVersion,
                workflow,
                options.Value.WorkflowDefinitionActivities,
                returnUrl);

            return View(model);
        }

        [HttpPost("delete/{tenantId}/{id}")]
        public async Task<IActionResult> Delete(int? tenantId, string id, string returnUrl, CancellationToken cancellationToken)
        {
            var instance = await workflowInstanceStore.GetByIdAsync(tenantId, id, cancellationToken);

            if (instance == null)
                return NotFound();

            await workflowInstanceStore.DeleteAsync(tenantId, id, cancellationToken);
            notifier.Notify("Workflow instance successfully deleted.", NotificationType.Success);

            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "WorkflowDefinition", new { tenantId = tenantId });
        }

        [HttpGet("ListWorkflowInstances")]
        public async Task<IActionResult> ListWorkflowInstances()
        {
            int? tenantId = GetTenant();
            IEnumerable<WorkflowInstance> instancesList = await workflowInstanceStore.ListAllAsync(tenantId);

            if (instancesList.Count() > 0)
            {
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                };

                return Ok(JsonConvert.SerializeObject(instancesList, settings));
            }

            return NoContent();
        }

        [HttpPost("RunScheduledWorkflowInstance")]
        public async Task<IActionResult> RunScheduledWorkflowInstance(string instanceId, CancellationToken cancellationToken)
        {
            int? tenantId = GetTenant();
            await workflowHost.RunScheduledWorkflowInstanceAsync(tenantId, instanceId);
            return Ok();
        }
        // https://localhost:44332/Elsa/workflow-instance/Create/abc?correlationId=issue4711
        [HttpPost("Create/{definitionId}")]
        public async Task<IActionResult> CreateWorkflowInstance(string definitionId, string? correlationId = default, CancellationToken cancellationToken = default)
        {
            int? tenantId = GetTenant();
            string payload = null;

            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                payload = await reader.ReadToEndAsync();

                if (String.IsNullOrEmpty(payload))
                    payload = "{}";
            }

            var definitionVersion = await workflowDefinitionVersionStore.GetByIdAsync(tenantId, definitionId, VersionOptions.Latest, cancellationToken);

            if (definitionVersion == null)
                return NotFound();

            var workflowExecutionContext = await workflowHost.WorkflowInstanceCreateAsync(tenantId, definitionId, correlationId, payload);
            return Json(workflowExecutionContext.InstanceId);
        }

        // TO DO: implement this once UserTask works
        //[HttpPost("SubmitUserTaskDecision")]
        //public async Task<IActionResult> SubmitUserTaskDecision(string instanceId, string decision, CancellationToken cancellationToken)
        //{
        //    int? tenantId = GetTenant();
        //    var workflowInstance = await workflowInstanceStore.GetByIdAsync(tenantId, instanceId);

        //    if (workflowInstance == null)
        //    {
        //        return NotFound();
        //    }
        //    else
        //    {
        //        var blockingActivityId = workflowInstance.WorkflowInstanceBlockingActivities.Select(x => x.ActivityId).FirstOrDefault();
        //        await workflowHost.RunWorkflowInstanceAsync(tenantId, workflowInstance.Id, blockingActivityId, decision);
        //        return Ok();
        //    }
        //}

        // temporary solution that returns tenantId = 1 until integrated into the final project
        private int GetTenant()
        {
            return 1;
        }

        private ActivityModel CreateActivityModel(
            WorkflowDefinitionActivity workflowDefinitionActivity,
            WorkflowInstance workflowInstance)
        {
            var logEntry = workflowInstance.ExecutionLog.OrderByDescending(x => x.Timestamp)
                .FirstOrDefault(x => x.ActivityId == workflowDefinitionActivity.Id);
            var isExecuted = logEntry != null;
            var isFaulted = logEntry?.Faulted ?? false;
            var message = default(ActivityMessageModel);

            if (isFaulted)
                message = new ActivityMessageModel("Faulted", logEntry.Message);
            else if (isExecuted)
                message = new ActivityMessageModel("Executed", logEntry.Message);

            return new ActivityModel(workflowDefinitionActivity, isExecuted, isFaulted, message);
        }
    }
}