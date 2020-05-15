// Commented out until we decide if the logic should be merged with UserTask

//using System;
//using System.Threading;
//using System.Threading.Tasks;
//using Elsa.Activities.Http.Models;
//using Elsa.Activities.Http.Services;
//using Elsa.ExpressionTypes.JavaScript.Messages;
//using Elsa.Services.Models;
//using MediatR;
//using Microsoft.AspNetCore.Http;

//namespace Elsa.Activities.Http.Handlers
//{
//    public class HttpJavaScriptHandler : INotificationHandler<EvaluatingJavaScriptExpression>
//    {
//        private readonly ITokenService tokenService;
//        private readonly IAbsoluteUrlProvider absoluteUrlProvider;
//        private readonly IHttpContextAccessor httpContextAccessor;

//        public HttpJavaScriptHandler(
//            ITokenService tokenService,
//            IAbsoluteUrlProvider absoluteUrlProvider,
//            IHttpContextAccessor httpContextAccessor)
//        {
//            this.tokenService = tokenService;
//            this.absoluteUrlProvider = absoluteUrlProvider;
//            this.httpContextAccessor = httpContextAccessor;
//        }

//        public Task Handle(EvaluatingJavaScriptExpression notification, CancellationToken cancellationToken)
//        {
//            var engine = notification.Engine;
//            var activityExecutionContext = notification.ActivityExecutionContext;

//            engine.SetValue(
//                "queryString",
//                (Func<string, string>)(key => httpContextAccessor.HttpContext.Request.Query[key].ToString())
//            );
//            engine.SetValue(
//                "absoluteUrl",
//                (Func<string, string>)(url => absoluteUrlProvider.ToAbsoluteUrl(url).ToString())
//            );
//            engine.SetValue(
//                "signalUrl",
//                (Func<string, string>)(signal => GenerateUrl(signal, activityExecutionContext))
//            );

//            return Task.CompletedTask;
//        }

//        private string GenerateUrl(string signal, ActivityExecutionContext activityExecutionContext)
//        {
//            var workflowInstanceId = activityExecutionContext.WorkflowExecutionContext.InstanceId;
//            // TO DO: Inspect if tenantId should be passed here
//            var payload = new Signal(null, signal, workflowInstanceId);
//            var token = tokenService.CreateToken(payload);
//            var url = $"/workflows/signal?token={token}";

//            return absoluteUrlProvider.ToAbsoluteUrl(url).ToString();
//        }
//    }
//}