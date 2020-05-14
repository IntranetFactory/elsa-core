using System.Collections.Generic;
using Elsa.Models;

namespace Elsa.Comparers
{
    public class WorkflowInstanceBlockingActivityEqualityComparer : IEqualityComparer<WorkflowInstanceBlockingActivity>
    {
        public bool Equals(WorkflowInstanceBlockingActivity x, WorkflowInstanceBlockingActivity y) => x.ActivityId.Equals(y.ActivityId);
        public int GetHashCode(WorkflowInstanceBlockingActivity obj) => obj.ActivityId.GetHashCode();
    }
}