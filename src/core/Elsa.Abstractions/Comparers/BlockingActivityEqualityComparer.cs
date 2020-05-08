using System.Collections.Generic;
using Elsa.Models;

namespace Elsa.Comparers
{
    public class BlockingActivityEqualityComparer : IEqualityComparer<WorkflowInstanceTask>
    {
        public bool Equals(WorkflowInstanceTask x, WorkflowInstanceTask y) => x.Id.Equals(y.Id);
        public int GetHashCode(WorkflowInstanceTask obj) => obj.Id.GetHashCode();
    }
}