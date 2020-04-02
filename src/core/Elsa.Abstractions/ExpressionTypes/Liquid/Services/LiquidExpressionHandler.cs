using System;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Expressions;
using Elsa.ExpressionTypes.Liquid.Extensions;
using Elsa.ExpressionTypes.Liquid.Messages;
using Elsa.Services.Models;
using Fluid;
using MediatR;

namespace Elsa.ExpressionTypes.Liquid.Services
{
    public class LiquidExpressionHandler : IExpressionHandler
    {
        private readonly ILiquidTemplateManager liquidTemplateManager;
        private readonly IMediator mediator;

        public LiquidExpressionHandler(ILiquidTemplateManager liquidTemplateManager, IMediator mediator)
        {
            this.liquidTemplateManager = liquidTemplateManager;
            this.mediator = mediator;
        }

        public string Type => LiquidExpression.ExpressionType;

        public async Task<object> EvaluateAsync(IWorkflowExpression expression, Type returnType, ActivityExecutionContext context, CancellationToken cancellationToken)
        {
            var liquidExpression = (LiquidExpression)expression;
            var templateContext = await CreateTemplateContextAsync(context);
            string template = liquidExpression.Expression;
            var result = await liquidTemplateManager.RenderAsync(template, templateContext);
            return string.IsNullOrWhiteSpace(result) ? default : Convert.ChangeType(result, returnType);
        }

        private async Task<TemplateContext> CreateTemplateContextAsync(ActivityExecutionContext workflowContext)
        {
            var context = new TemplateContext();

            // this is commented out until it's fully understood
            //context.SetValue("WorkflowExecutionContext", workflowContext);
            //await mediator.Publish(new EvaluatingLiquidExpression(context, workflowContext));
            //context.Model = workflowContext;

            foreach (var variable in workflowContext.WorkflowExecutionContext.Variables)
            {
                context.SetValue(variable.Key, variable.Value.Value);
            }

            return context;
        }
    }
}