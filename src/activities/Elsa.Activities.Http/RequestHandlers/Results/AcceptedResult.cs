// Commented out until we decide if the logic should be merged with UserTask

//using System.Net;
//using System.Threading.Tasks;
//using Elsa.Activities.Http.Services;
//using Microsoft.AspNetCore.Http;

//namespace Elsa.Activities.Http.RequestHandlers.Results
//{
//    public class AcceptedResult : IRequestHandlerResult
//    {
//        public Task ExecuteResultAsync(HttpContext httpContext, RequestDelegate next)
//        {
//            httpContext.Response.StatusCode = (int)HttpStatusCode.Accepted;
//            return Task.CompletedTask;
//        }
//    }
//}