using Elsa.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elsa.Messaging.Domain
{
    public class ActivityScheduled : ActivityNotification
    {
        public ActivityScheduled(ActivityExecutionContext activityExecutionContext) : base(activityExecutionContext)
        {
        }
    }
}
