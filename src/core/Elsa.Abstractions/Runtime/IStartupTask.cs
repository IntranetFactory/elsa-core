using System.Threading;
using System.Threading.Tasks;

namespace Elsa.Runtime
{
    public interface IStartupTask
    {
        Task ExecuteAsync(int? tenantId, CancellationToken cancellationToken = default);
    }
}