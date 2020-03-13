using System.Threading;
using System.Threading.Tasks;
using Elsa.Messaging.Distributed;
using Elsa.Runtime;
using Rebus.Bus;

namespace Elsa.StartupTasks
{
    public class StartServiceBusTask : IStartupTask
    {
        private readonly IBus serviceBus;
        public StartServiceBusTask(IBus serviceBus) => this.serviceBus = serviceBus;
        // tenantId has no other use than to enable project compilation and prevent errors
        public Task ExecuteAsync(string tenantId, CancellationToken cancellationToken = default) => serviceBus.Subscribe<RunWorkflow>();
    }
}