using System;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Expressions;
using Elsa.Services.Models;

namespace Elsa.ExpressionTypes
{
    public class VariableHandler : IExpressionHandler
    {
        public string Type => VariableExpression.ExpressionType;

        public Task<object> EvaluateAsync(
            IWorkflowExpression expression,
            Type returnType,
            ActivityExecutionContext context,
            CancellationToken cancellationToken)
        {
            var variableExpression = (VariableExpression)expression;
            var result = context.GetVariable(variableExpression.VariableName);
            return Task.FromResult(result);
        }
    }
}