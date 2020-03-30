using Elsa.Services.Models;
using Jint;
using MediatR;

namespace Elsa.ExpressionTypes.JavaScript.Messages
{
    public class EvaluatingJavaScriptExpression : INotification
    {
        public EvaluatingJavaScriptExpression(Engine engine, ActivityExecutionContext activityExecutionContext)
        {
            Engine = engine;
            ActivityExecutionContext = activityExecutionContext;
        }

        public Engine Engine { get; }
        public ActivityExecutionContext ActivityExecutionContext { get; }
    }
}