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
        private readonly IWorkflowHost workflowHost;
        private readonly IOptions<ElsaDashboardOptions> options;
        private readonly IIdGenerator idGenerator;
        private readonly IClock clock;

        public TestController(
            IWorkflowDefinitionVersionStore workflowDefinitionVersionStore,
            IWorkflowDefinitionStore workflowDefinitionStore,
            IWorkflowInstanceStore workflowInstanceStore,
            IWorkflowHost workflowHost,
            IOptions<ElsaDashboardOptions> options,
            IIdGenerator idGenerator,
            IClock clock)
        {
            this.workflowDefinitionVersionStore = workflowDefinitionVersionStore;
            this.workflowDefinitionStore = workflowDefinitionStore;
            this.workflowInstanceStore = workflowInstanceStore;
            this.workflowHost = workflowHost;
            this.options = options;
            this.idGenerator = idGenerator;
            this.clock = clock;
        }

        [HttpGet("InvokeUserAction")]
        public async Task<IActionResult> InvokeUserAction(string instanceId, string actionName, CancellationToken cancellationToken)
        {
            int? tenantId = 1;

            var workflowInstance = await workflowInstanceStore.GetByIdAsync(tenantId, instanceId);

            if (workflowInstance == null)
            {
                return NotFound();
            } else
            {
                var blockingActivityId = workflowInstance.BlockingActivities.Select(x => x.ActivityId).FirstOrDefault();
                await workflowHost.RunWorkflowInstanceAsync(tenantId, workflowInstance.Id, blockingActivityId, actionName);
                return Ok();
            }
        }
    }
}
