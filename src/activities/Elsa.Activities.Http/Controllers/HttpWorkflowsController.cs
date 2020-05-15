// Commented out until we decide if the logic should be merged with UserTask

//using System.Threading;
//using System.Threading.Tasks;
//using Elsa.Activities.Http.Models;
//using Elsa.Activities.Http.Services;
//using Elsa.Models;
//using Elsa.Persistence;
//using Elsa.Services;
//using Microsoft.AspNetCore.Mvc;

//namespace Elsa.Activities.Http.Controllers
//{
//    [ApiController]
//    [Route("workflows")]
//    public class HttpWorkflowsController : ControllerBase
//    {
//        private readonly ITokenService tokenService;
//        private readonly IWorkflowHost workflowHost;
//        private readonly IWorkflowInstanceStore workflowInstanceStore;

//        public HttpWorkflowsController(
//            ITokenService tokenService,
//            IWorkflowHost workflowHost,
//            IWorkflowInstanceStore workflowInstanceStore)
//        {
//            this.tokenService = tokenService;
//            this.workflowHost = workflowHost;
//            this.workflowInstanceStore = workflowInstanceStore;
//        }

//        [Route("trigger/{token}")]
//        [HttpGet, HttpPost]
//        public async Task<IActionResult> Trigger(string token, CancellationToken cancellationToken)
//        {
//            if (!tokenService.TryDecryptToken(token, out Signal signal))
//                return NotFound();

//            var workflowInstance = await workflowInstanceStore.GetByIdAsync(signal.TenantId, signal.WorkflowInstanceId, cancellationToken);

//            if (workflowInstance == null)
//                return NotFound();

//            var input = Variable.From(signal.Name);

//            //await processRunner.ResumeAsync(workflowInstance, input, cancellationToken: cancellationToken);

//            return HttpContext.Items.ContainsKey(WorkflowHttpResult.Instance)
//                ? (IActionResult)new EmptyResult()
//                : Accepted();
//        }
//    }
//}