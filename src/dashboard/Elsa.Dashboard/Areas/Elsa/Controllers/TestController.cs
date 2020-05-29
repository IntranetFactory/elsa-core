using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Elsa.Dashboard.Options;
using Elsa.Persistence;
using Elsa.Services;
using NodaTime;

namespace Elsa.Dashboard.Areas.Elsa.Controllers
{
    [Area("Elsa")]
    [Route("[area]/[controller]")]
    public class TestController : Controller
    {
        private readonly IWorkflowDefinitionVersionStore workflowDefinitionVersionStore;
        private readonly IWorkflowInstanceTaskService workflowInstanceTaskService;
        private readonly IWorkflowInstanceTaskStore workflowInstanceTaskStore;
        private readonly IWorkflowDefinitionStore workflowDefinitionStore;
        private readonly IWorkflowInstanceStore workflowInstanceStore;
        private readonly IWorkflowHost workflowHost;
        private readonly IOptions<ElsaDashboardOptions> options;
        private readonly IIdGenerator idGenerator;
        private readonly IClock clock;

        public TestController(
            IWorkflowDefinitionVersionStore workflowDefinitionVersionStore,
            IWorkflowInstanceTaskService workflowInstanceTaskService,
            IWorkflowInstanceTaskStore workflowInstanceTaskStore,
            IWorkflowDefinitionStore workflowDefinitionStore,
            IWorkflowInstanceStore workflowInstanceStore,
            IWorkflowHost workflowHost,
            IOptions<ElsaDashboardOptions> options,
            IIdGenerator idGenerator,
            IClock clock)
        {
            this.workflowDefinitionVersionStore = workflowDefinitionVersionStore;
            this.workflowInstanceTaskService = workflowInstanceTaskService;
            this.workflowInstanceTaskStore = workflowInstanceTaskStore;
            this.workflowDefinitionStore = workflowDefinitionStore;
            this.workflowInstanceStore = workflowInstanceStore;
            this.workflowHost = workflowHost;
            this.options = options;
            this.idGenerator = idGenerator;
            this.clock = clock;
        }
    }
}
