using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Elsa.Services
{
    public interface IWorkflowInstanceTaskService
    {
        void Unblock(int? tenantId, string taskId, CancellationToken cancellationToken = default);
    }
}
