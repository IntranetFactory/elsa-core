using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Builders;
using Elsa.Dashboard.Areas.Elsa.ViewModels;
using Elsa.Dashboard.Options;
using Elsa.Dashboard.Services;
using Elsa.Models;
using Elsa.Persistence;
using Elsa.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NodaTime;

namespace Elsa.Dashboard.Areas.Elsa.Controllers
{
    [Area("Elsa")]
    [Route("[area]/[controller]")]
    public class TestController : Controller
    {
        private readonly IWorkflowDefinitionVersionStore workflowDefinitionVersionStore;
        private readonly IWorkflowDefinitionStore workflowDefinitionStore;
        private readonly IWorkflowInstanceStore workflowInstanceStore;
        private readonly IOptions<ElsaDashboardOptions> options;
        private readonly IIdGenerator idGenerator;
        private readonly IClock clock;

        public TestController(
            IWorkflowDefinitionVersionStore workflowDefinitionVersionStore,
            IWorkflowDefinitionStore workflowDefinitionStore,
            IWorkflowInstanceStore workflowInstanceStore,
            IOptions<ElsaDashboardOptions> options,
            IIdGenerator idGenerator,
            IClock clock)
        {
            this.workflowDefinitionVersionStore = workflowDefinitionVersionStore;
            this.workflowDefinitionStore = workflowDefinitionStore;
            this.workflowInstanceStore = workflowInstanceStore;
            this.options = options;
            this.idGenerator = idGenerator;
            this.clock = clock;
        }

        #region QueryUserTask
        [HttpGet("QueryUserTask/{activityType}/{tag}")]
        public async Task<IActionResult> QueryUserTask(string activityType, string tag, CancellationToken cancellationToken)
        {
            var tuples = await workflowInstanceStore.ListByBlockingActivityTagAsync(activityType, tag);
            List<BlockingActivity> pendingUserTasks = new List<BlockingActivity>();

            foreach (var item in tuples)
            {
                if (item.BlockingActivity != null)
                    pendingUserTasks.Add(item.BlockingActivity);
            }

            return Ok(pendingUserTasks);
        }
        #endregion

        #region MultitenancySupport
        [HttpGet("TestMultitenancy")]
        public async Task<IActionResult> TestMultitenancy(CancellationToken cancellationToken)
        {
            string tenantId = HttpContext.Request.Query["tenantId"];
            string userId = HttpContext.Request.Query["userId"];

            IEnumerable<WorkflowDefinition> workflowDefinitionsForTenantId = await workflowDefinitionStore.ListAsync(tenantId, cancellationToken);
            List<WorkflowInstance> workflowInstancesForTenantId = new List<WorkflowInstance>();
            List<BlockingActivity> blockingActivitiesForTenantAndUser = new List<BlockingActivity>();

            foreach (var workflowDefinition in workflowDefinitionsForTenantId)
            {
                IEnumerable<WorkflowInstance> workflowInstancesForDefinition = await workflowInstanceStore.ListByDefinitionAsync(workflowDefinition.Id);

                foreach(var workflowInstance in workflowInstancesForDefinition)
                {
                    foreach(var blockingActivity in workflowInstance.BlockingActivities)
                    {
                        if (blockingActivity.Tag == userId)
                            blockingActivitiesForTenantAndUser.Add(blockingActivity);
                    }

                    workflowInstancesForTenantId.Add(workflowInstance);
                }
            }

            TestMultitenancyViewModel model = new TestMultitenancyViewModel
            {
                TenantId = tenantId,
                UserId = userId,
                WorkflowDefinitionsByTenantId = workflowDefinitionsForTenantId,
                WorkflowInstancesByTenantId = workflowInstancesForTenantId,
                BlockingActivitiesByTenantIdAndTag = blockingActivitiesForTenantAndUser
            };


            return View(model);
        }

        [HttpPost("SaveWorkflowDefinition/{definitionId}")]
        public async Task<IActionResult> SaveWorkflowDefinition(string definitionId, CancellationToken cancellationToken)
        {
            string tenantId = "1";
            var workflowDefinition = await workflowDefinitionStore.GetByIdAsync(tenantId, definitionId, cancellationToken);
            workflowDefinition.CreatedAt = clock.GetCurrentInstant();
            await workflowDefinitionStore.SaveAsync(workflowDefinition, cancellationToken);

            return Content("Save");
        }

        [HttpPost("AddWorkflowDefinition")]
        public async Task<IActionResult> AddWorkflowDefinition(CancellationToken cancellationToken)
        {
            WorkflowDefinition newDefinition = new WorkflowDefinition();
            newDefinition.Id = idGenerator.Generate();
            newDefinition.TenantId = Guid.NewGuid().ToString();
            newDefinition.CreatedAt = clock.GetCurrentInstant();
            await workflowDefinitionStore.AddAsync(newDefinition);

            return Content("Add");
        }

        [HttpGet("GetWorkflowDefinitionById/{definitionId}")]
        public async Task<IActionResult> GetWorkflowDefinition(string definitionId, CancellationToken cancellationToken)
        {
            string tenantId = "1";
            var workflowDefinition = await workflowDefinitionStore.GetByIdAsync(tenantId, definitionId, cancellationToken);
            return Ok(workflowDefinition);
        }

        [HttpGet("ListWorkflowDefinitionsByTenantId/{tenantId}")]
        public async Task<IActionResult> ListWorkflowDefinitionsByTenantId(string tenantId, CancellationToken cancellationToken)
        {
            var workflowDefinitions = await workflowDefinitionStore.ListAsync(tenantId, cancellationToken);
            return Ok(workflowDefinitions);
        }

        [HttpPost("UpdateWorkflowDefinition/{definitionId}")]
        public async Task<IActionResult> UpdateWorkflowDefinition(string definitionId, CancellationToken cancellationToken)
        {
            string tenantId = "1";
            var workflowDefinition = await workflowDefinitionStore.GetByIdAsync(tenantId, definitionId, cancellationToken);
            workflowDefinition.WorkflowDefinitionVersions.Clear();
            await workflowDefinitionStore.UpdateAsync(workflowDefinition, cancellationToken);

            return Content("Update");
        }

        [HttpPost("DeleteWorkflowDefinition/{definitionId}")]
        public async Task<IActionResult> DeleteWorkflowDefinition(string definitionId, CancellationToken cancellationToken)
        {
            string tenantId = "1";
            var count = await workflowDefinitionStore.DeleteAsync(tenantId, definitionId, cancellationToken);
            return Content(count.ToString());
        }
        #endregion
    }
}
