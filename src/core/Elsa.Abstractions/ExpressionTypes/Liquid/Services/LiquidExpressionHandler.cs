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
            template = "{{Foo}}";  // we should not use Variables.* in templates until we find a request for that

            var result = await liquidTemplateManager.RenderAsync(template, templateContext);
            return string.IsNullOrWhiteSpace(result) ? default : Convert.ChangeType(result, returnType);
        }

        private async Task<TemplateContext> CreateTemplateContextAsync(ActivityExecutionContext workflowContext)
        {
            var context = new TemplateContext();
            //context.SetValue("WorkflowExecutionContext", workflowContext);
            //await mediator.Publish(new EvaluatingLiquidExpression(context, workflowContext));
            //context.Model = workflowContext;

            // should be generated from workflowContext
            context.SetValue("Foo", "Bar" );
            
            return context;
        }
    }
}