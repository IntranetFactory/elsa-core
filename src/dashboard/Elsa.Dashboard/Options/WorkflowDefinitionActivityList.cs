using System.Collections;
using System.Collections.Generic;
using Elsa.Metadata;

namespace Elsa.Dashboard.Options
{
    public class WorkflowDefinitionActivityList : IEnumerable<ActivityDescriptor>
    {
        public WorkflowDefinitionActivityList()
        {
            Items = new Dictionary<string, ActivityDescriptor>();
        }

        private IDictionary<string, ActivityDescriptor> Items { get; }
        
        public WorkflowDefinitionActivityList Add(ActivityDescriptor item)
        {
            Items[item.Type] = item;
            return this;
        }

        public IEnumerator<ActivityDescriptor> GetEnumerator() => Items.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}