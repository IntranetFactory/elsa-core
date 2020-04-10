using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Dashboard.Areas.Elsa.ViewModels;
using Elsa.Dashboard.Extensions;
using Elsa.Dashboard.Models;
using Elsa.Dashboard.Options;
using Elsa.Dashboard.Services;
using Elsa.Extensions;
using Elsa.Metadata;
using Elsa.Models;
using Elsa.Persistence;
using Elsa.Serialization;
using Elsa.Serialization.Formatters;
using Elsa.Services;
using Elsa.WorkflowDesigner.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Elsa.Dashboard.Areas.Elsa.Controllers
{
    [Area("Elsa")]
    [Route("[area]/workflow-definition-version")]
    public class WorkflowDefinitionVersionController : Controller
    {
        private readonly IWorkflowDefinitionVersionStore workflowDefinitionVersionStore;
        private readonly IWorkflowInstanceStore workflowInstanceStore;
        private readonly IWorkflowPublisher publisher;
        private readonly IWorkflowSerializer serializer;
        private readonly IOptions<ElsaDashboardOptions> options;
        private readonly INotifier notifier;

        public WorkflowDefinitionVersionController(
            IWorkflowDefinitionVersionStore workflowDefinitionVersionStore,
            IWorkflowInstanceStore workflowInstanceStore,
            IWorkflowPublisher publisher,
            IWorkflowSerializer serializer,
            IOptions<ElsaDashboardOptions> options,
            INotifier notifier)
        {
            this.publisher = publisher;
            this.workflowDefinitionVersionStore = workflowDefinitionVersionStore;
            this.workflowInstanceStore = workflowInstanceStore;
            this.serializer = serializer;
            this.options = options;
            this.notifier = notifier;
        }

        [HttpGet]
        public async Task<ViewResult> Index(int? tenantId, CancellationToken cancellationToken)
        {
            var workflows = await workflowDefinitionVersionStore.ListAsync(
                tenantId,
                VersionOptions.LatestOrPublished,
                cancellationToken
            );

            var workflowModels = new List<WorkflowDefinitionVersionListItemModel>();

            foreach (var workflow in workflows)
            {
                var workflowModel = await CreateWorkflowDefinitionListItemModelAsync(workflow, cancellationToken);
                workflowModels.Add(workflowModel);
            }

            var groups = workflowModels.GroupBy(x => x.WorkflowDefinitionVersion.DefinitionId);
            var model = new WorkflowDefinitionVersionListViewModel
            {
                WorkflowDefinitionVersions = groups.ToList()
            };
            return View(model);
        }

        [HttpGet("create")]
        public ViewResult Create(int? tenantId)
        {
            var workflowDefinitionVersion = publisher.New(tenantId);

            var model = new WorkflowDefinitionVersionEditModel
            {
                Name = workflowDefinitionVersion.Name,
                TenantId = tenantId,
                Json = serializer.Serialize(workflowDefinitionVersion, JsonTokenFormatter.FormatName),
                ActivityDefinitions = options.Value.ActivityDefinitions.ToArray(),
                IsSingleton = workflowDefinitionVersion.IsSingleton,
                IsDisabled = workflowDefinitionVersion.IsDisabled,
                Description = workflowDefinitionVersion.Description
            };


            return View(model);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(WorkflowDefinitionVersionEditModel model, CancellationToken cancellationToken)
        {
            var workflowDefinitionVersion = new WorkflowDefinitionVersion();
            return await SaveAsync(model, workflowDefinitionVersion, cancellationToken);
        }

        [HttpGet("edit/{tenantId}/{id}", Name = "EditWorkflowDefinitionVersion")]
        public async Task<IActionResult> Edit(int? tenantId, string id, CancellationToken cancellationToken)
        {
            var workflowDefinitionVersion = await publisher.GetDraftAsync(tenantId, id, cancellationToken);

            if (workflowDefinitionVersion == null)
                return NotFound();

            var workflowModel = new WorkflowModel
            {
                Activities = workflowDefinitionVersion.Activities.Select(x => new ActivityModel(x)).ToList(),
                Connections = workflowDefinitionVersion.Connections.Select(x => new ConnectionModel(x)).ToList()
            };

            var model = new WorkflowDefinitionVersionEditModel
            {
                Id = workflowDefinitionVersion.DefinitionId,
                TenantId = workflowDefinitionVersion.TenantId,
                Name = workflowDefinitionVersion.Name,
                Json = serializer.Serialize(workflowModel, JsonTokenFormatter.FormatName),
                Description = workflowDefinitionVersion.Description,
                IsSingleton = workflowDefinitionVersion.IsSingleton,
                IsDisabled = workflowDefinitionVersion.IsDisabled,
                ActivityDefinitions = options.Value.ActivityDefinitions.ToArray(),
                WorkflowModel = workflowModel
            };

            return View(model);
        }

        [HttpPost("edit/{tenantId}/{id}")]
        public async Task<IActionResult> Edit(
            int? tenantId,
            string id,
            WorkflowDefinitionVersionEditModel model,
            CancellationToken cancellationToken)
        {
            var workflowDefinitionVersion = await workflowDefinitionVersionStore.GetByIdAsync(tenantId, id, VersionOptions.Latest, cancellationToken);
            return await SaveAsync(model, workflowDefinitionVersion, cancellationToken);
        }

        [HttpPost("delete/{tenantId}/{id}")]
        public async Task<IActionResult> Delete(int? tenantId, string id, CancellationToken cancellationToken)
        {
            await workflowDefinitionVersionStore.DeleteAsync(tenantId, id, cancellationToken);
            notifier.Notify("Workflow successfully deleted.", NotificationType.Success);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("WorkflowDefinitionVersionEditorStandalone")]
        public async Task<IActionResult> WorkflowDefinitionVersionEditorStandalone()
        {
            return View("WorkflowDefinitionVersionEditorStandalone");
        }

        [HttpPost("SaveWorkflowDefinition")]
        public async Task<IActionResult> SaveWorkflowDefinition([FromBody]WorkflowDefinitionVersionEditModel model, CancellationToken cancellationToken)
        {
            WorkflowDefinitionVersion workflowDefinitionVersion = new WorkflowDefinitionVersion();

            if (model.Id == null)
            {
                workflowDefinitionVersion = await UpdateExistingDefinitionVersionPropertiesFromModel(model, workflowDefinitionVersion);
                workflowDefinitionVersion = await publisher.SaveDraftAsync(workflowDefinitionVersion, cancellationToken);
                notifier.Notify("Workflow successfully saved as a draft.", NotificationType.Success);
                await SaveAsync(model, workflowDefinitionVersion, cancellationToken);
                var workflowJson = CreateWorkflowDesignerJson(workflowDefinitionVersion);
                return Ok(workflowJson);
            }
            else
            {
                workflowDefinitionVersion = await workflowDefinitionVersionStore.GetByIdAsync(model.TenantId, model.Id, VersionOptions.Latest, cancellationToken);
                workflowDefinitionVersion = await UpdateExistingDefinitionVersionPropertiesFromModel(model, workflowDefinitionVersion);
                await publisher.SaveDraftAsync(workflowDefinitionVersion, cancellationToken);
                notifier.Notify("Workflow successfully saved as a draft.", NotificationType.Success);
                return NoContent();
            }
        }

        [HttpPost("PublishWorkflowDefinition")]
        public async Task<IActionResult> PublishWorkflowDefinition([FromBody]WorkflowDefinitionVersionEditModel model, CancellationToken cancellationToken)
        {
            var workflowDefinitionVersion = await publisher.GetDraftAsync(model.TenantId, model.Id, cancellationToken);
            workflowDefinitionVersion = await UpdateExistingDefinitionVersionPropertiesFromModel(model, workflowDefinitionVersion);
            await publisher.PublishAsync(workflowDefinitionVersion, cancellationToken);
            notifier.Notify("Workflow successfully published.", NotificationType.Success);
            return NoContent();
        }

        [HttpGet("LoadWorkflowDefinition")]
        public async Task<IActionResult> LoadWorkflowDefinition(string id, CancellationToken cancellationToken)
        {
            int? tenantId = GetTenant();

            var workflowDefinitionVersion = await publisher.GetDraftAsync(tenantId, id, cancellationToken);

            if (workflowDefinitionVersion == null)
                return NotFound();

            var workflowJson = CreateWorkflowDesignerJson(workflowDefinitionVersion);
            return Ok(workflowJson);
        }

        [HttpPost("DeleteWorkflowDefinition")]
        public async Task<IActionResult> DeleteWorkflowDefinition(string id)
        {
            int? tenantId = GetTenant();

            int numberOfDeletedVersions = await workflowDefinitionVersionStore.DeleteAsync(tenantId, id);

            if (numberOfDeletedVersions > 0)
            {
                notifier.Notify("Workflow successfully deleted.", NotificationType.Success);
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("ListWorkflowDefinitions")]
        public async Task<IActionResult> ListWorkflowDefinitions()
        {
            int? tenantId = GetTenant();
            IEnumerable<WorkflowDefinitionVersion> definitionsList = await workflowDefinitionVersionStore.ListAsync(tenantId, VersionOptions.Latest);

            if (definitionsList.Count() > 0)
            {
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                };

                return Ok(JsonConvert.SerializeObject(definitionsList, settings));
            }

            return NoContent();
        }

        private async Task<IActionResult> SaveAsync(
            WorkflowDefinitionVersionEditModel model,
            WorkflowDefinitionVersion workflowDefinitionVersion,
            CancellationToken cancellationToken)
        {
            workflowDefinitionVersion = await UpdateExistingDefinitionVersionPropertiesFromModel(model, workflowDefinitionVersion);

            var publish = model.SubmitAction == "publish";

            if (publish)
            {
                workflowDefinitionVersion = await publisher.PublishAsync(workflowDefinitionVersion, cancellationToken);
                notifier.Notify("Workflow successfully published.", NotificationType.Success);
            }
            else
            {
                workflowDefinitionVersion = await publisher.SaveDraftAsync(workflowDefinitionVersion, cancellationToken);
                notifier.Notify("Workflow successfully saved as a draft.", NotificationType.Success);
            }

            return RedirectToRoute("EditWorkflowDefinitionVersion", new { tenantId = workflowDefinitionVersion.TenantId, id = workflowDefinitionVersion.DefinitionId });
        }

        // temporary solution that returns tenantId = 1 until integrated into the final project
        private int GetTenant()
        {
            return 1;
        }

        // this is used to create workflowData and serialize it so that it can be used in standalone designer
        private string CreateWorkflowDesignerJson(WorkflowDefinitionVersion workflowDefinitionVersion)
        {
            var hiddenActivityNames = new List<string>() { "ReadLine", "WriteLine", "Redirect", "WriteHttpResponse", "Inline" };
            List<ActivityDescriptor> activityDefinitions = new List<ActivityDescriptor>();

            foreach (var activity in options.Value.ActivityDefinitions)
            {
                if (!hiddenActivityNames.Contains(activity.Type))
                    activityDefinitions.Add(activity);
            }

            var workflowModel = new
            {
                Activities = workflowDefinitionVersion.Activities.Select(x => new ActivityModel(x)).ToList(),
                Connections = workflowDefinitionVersion.Connections.Select(x => new ConnectionModel(x)).ToList(),
            };

            dynamic workflowData = new
            {
                Id = workflowDefinitionVersion.DefinitionId,
                TenantId = workflowDefinitionVersion.TenantId,
                ActivityDefinitions = activityDefinitions.ToArray(),
                WorkflowModel = workflowModel
            };

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };

            return JsonConvert.SerializeObject(workflowData, settings);
        }

        // deserializes workflow posted from view and updates existing workflow definition properties
        private async Task<WorkflowDefinitionVersion> UpdateExistingDefinitionVersionPropertiesFromModel(WorkflowDefinitionVersionEditModel model, WorkflowDefinitionVersion workflowDefinitionVersion = null)
        {
            var postedWorkflow = serializer.Deserialize<WorkflowModel>(model.Json, JsonTokenFormatter.FormatName);

            workflowDefinitionVersion.Activities = postedWorkflow.Activities
                .Select(x => new ActivityDefinition(x.Id, x.TenantId, x.Type, x.State, x.Left, x.Top))
                .ToList();

            foreach (var activity in workflowDefinitionVersion.Activities)
            {
                activity.TenantId = model.TenantId;
            }

            workflowDefinitionVersion.Connections = postedWorkflow.Connections.Select(
                x => new ConnectionDefinition(x.TenantId, x.SourceActivityId, x.DestinationActivityId, x.Outcome)).ToList();

            foreach (var connection in workflowDefinitionVersion.Connections)
            {
                connection.TenantId = model.TenantId;
            }

            workflowDefinitionVersion.TenantId = model.TenantId;
            workflowDefinitionVersion.Description = model.Description;
            workflowDefinitionVersion.Name = model.Name;
            workflowDefinitionVersion.IsDisabled = model.IsDisabled;
            workflowDefinitionVersion.IsSingleton = model.IsSingleton;

            return workflowDefinitionVersion;
        }

        private async Task<WorkflowDefinitionVersionListItemModel> CreateWorkflowDefinitionListItemModelAsync(
            WorkflowDefinitionVersion workflowDefinitionVersion,
            CancellationToken cancellationToken)
        {
            var instances = await workflowInstanceStore
                .ListByDefinitionAsync(workflowDefinitionVersion.TenantId, workflowDefinitionVersion.DefinitionId, cancellationToken)
                .ToListAsync();

            return new WorkflowDefinitionVersionListItemModel
            {
                WorkflowDefinitionVersion = workflowDefinitionVersion,
                IdleCount = instances.Count(x => x.Status == WorkflowStatus.Idle),
                RunningCount = instances.Count(x => x.Status == WorkflowStatus.Running),
                CompletedCount = instances.Count(x => x.Status == WorkflowStatus.Completed),
                SuspendedCount = instances.Count(x => x.Status == WorkflowStatus.Suspended),
                FaultedCount = instances.Count(x => x.Status == WorkflowStatus.Faulted),
                CancelledCount = instances.Count(x => x.Status == WorkflowStatus.Cancelled),
            };
        }
    }
}