using Elsa.Expressions;

namespace Elsa.ExpressionTypes
{
    public class LiquidExpression : WorkflowExpression
    {
        public const string ExpressionType = "Liquid";

        public LiquidExpression(string expression) : base(ExpressionType)
        {
            Expression = expression;
        }

        public string Expression { get; }
    }

    public class LiquidExpression<T> : LiquidExpression, IWorkflowExpression<T>
    {
        public LiquidExpression(string expression) : base(expression)
        {
        }
    }
}
