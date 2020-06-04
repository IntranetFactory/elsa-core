using Elsa.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Elsa.Services
{
    public interface IWorkflowTaskRunner
    {
        Task<WorkflowInstanceTask> RunWorkflowInstanceTaskAsync(CancellationToken cancellationToken = default);
    }
}
