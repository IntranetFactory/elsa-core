// Commented out until we decide if the logic should be merged with UserTask

//using System.Threading.Tasks;
//using Elsa.Activities.Http.Services;
//using Microsoft.AspNetCore.Http;

//namespace Elsa.Activities.Http.RequestHandlers.Results
//{
//    public class NextResult : IRequestHandlerResult
//    {
//        public Task ExecuteResultAsync(HttpContext httpContext, RequestDelegate next) => next(httpContext);
//    }
//}