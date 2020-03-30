using Elsa.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elsa.ExpressionTypes
{
    public class JavaScriptExpression : WorkflowExpression
    {
        public const string ExpressionType = "JavaScript";

        public JavaScriptExpression(string expression) : base(ExpressionType)
        {
            Expression = expression;
        }

        public string Expression { get; }
    }

    public class JavaScriptExpression<T> : JavaScriptExpression, IWorkflowExpression<T>
    {
        public JavaScriptExpression(string expression) : base(expression)
        {
        }
    }
}
