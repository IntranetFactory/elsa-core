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
        RuntimeDescription = "x => x.definition.description + `<br/>` + ((x.state.from != undefined && x.state.from.value.expression != '') ? `<strong>From: ${x.state.from.value.expression}</strong><br/>${x.state.from.value.type} expression<br/>` : ``)" +
        " + ((x.state.to != undefined && x.state.to.value.expression != '') ? `<strong>To: ${x.state.to.value.expression}</strong><br/>${x.state.to.value.type} expression<br/>` : ``)" +
        " + ((x.state.subject != undefined && x.state.subject.value.expression != '') ? `<strong>Subject: ${x.state.subject.value.expression}</strong><br/>${x.state.subject.value.type} expression<br/>` : ``)"
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