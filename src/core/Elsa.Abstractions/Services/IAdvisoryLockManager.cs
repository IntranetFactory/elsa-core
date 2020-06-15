using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Elsa.Services
{
    public interface IAdvisoryLockManager
    {
        Task<bool> Lock(string name, CancellationToken cancellationToken = default);
        Task Unlock(string name, CancellationToken cancellationToken = default);
    }
}
