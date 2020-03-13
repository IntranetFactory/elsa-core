using System.Threading;
using System.Threading.Tasks;

namespace Elsa.Runtime
{
    public interface IStartupTask
    {
        Task ExecuteAsync(string tenantId, CancellationToken cancellationToken = default);
    }
}