using System.Threading;
using System.Threading.Tasks;
using Elsa.Activities.Email.Options;
using Elsa.Activities.Email.Services;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Results;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Elsa.Activities.Email.Activities
{
    [WorkflowDefinitionActivity(
        Category = "Email", 
        Description = "Send an email message.",
        RuntimeDescription = "x => x.definition.description + `<br/>` + ((x.state.From != undefined && x.state.From.value.Expression != '') ? `<strong>From: ${x.state.From.value.Expression}</strong><br/>${x.state.From.value.Type} expression<br/>` : ``)" +
        " + ((x.state.To != undefined && x.state.To.value.Expression != '') ? `<strong>To: ${x.state.To.value.Expression}</strong><br/>${x.state.To.value.Type} expression<br/>` : ``)" +
        " + ((x.state.Subject != undefined && x.state.Subject.value.Expression != '') ? `<strong>Subject: ${x.state.Subject.value.Expression}</strong><br/>${x.state.Subject.value.Type} expression<br/>` : ``)"
        )]
    public class SendEmail : Activity
    {
        private readonly ISmtpService smtpService;
        private readonly IOptions<SmtpOptions> options;

        public SendEmail(ISmtpService smtpService, IOptions<SmtpOptions> options)
        {
            this.smtpService = smtpService;
            this.options = options;
        }

        [ActivityProperty(Hint = "The sender's email address.")]
        public IWorkflowExpression<string> From
        {
            get => GetState<IWorkflowExpression<string>>();
            set => SetState(value);
        }

        [ActivityProperty(Hint = "The recipient's email address.")]
        public IWorkflowExpression<string> To
        {
            get => GetState<IWorkflowExpression<string>>();
            set => SetState(value);
        }

        [ActivityProperty(Hint = "The subject of the email message.")]
        public IWorkflowExpression<string> Subject
        {
            get => GetState<IWorkflowExpression<string>>();
            set => SetState(value);
        }

        [ActivityProperty(Hint = "The body of the email message.")]
        [WorkflowExpressionOptions(Multiline = true)]
        public IWorkflowExpression<string> Body
        {
            get => GetState<IWorkflowExpression<string>>();
            set => SetState(value);
        }

        protected override async Task<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context, CancellationToken cancellationToken)
        {
            var from = (await context.EvaluateAsync(From, cancellationToken)) ?? options.Value.DefaultSender;
            var to = await context.EvaluateAsync(To, cancellationToken);
            var subject = await context.EvaluateAsync(Subject, cancellationToken);
            var body = await context.EvaluateAsync(Body, cancellationToken);
            var message = new MimeMessage();
            
            message.From.Add(new MailboxAddress(@from));
            message.Subject = subject;
            
            message.Body = new TextPart(TextFormat.Html)
            {
                Text = body
            };

            message.To.Add(new MailboxAddress(to));

            await smtpService.SendAsync(message, cancellationToken);

            return Done();
        }
    }
}