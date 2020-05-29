using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Dashboard.Options;
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
    }
}
