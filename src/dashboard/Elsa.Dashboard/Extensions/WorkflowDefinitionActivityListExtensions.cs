using System;
using Elsa.Dashboard.Options;
using Elsa.Metadata;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace Elsa.Dashboard.Extensions
{
    public static class WorkflowDefinitionActivityListExtensions
    {
        private static IActivityDescriber _activityDescriber { get; set; }
        public static WorkflowDefinitionActivityList Add<T>(this WorkflowDefinitionActivityList list) where T : IActivity
        {
            return list.Add(_activityDescriber.Describe<T>());
        }

        public static WorkflowDefinitionActivityList Discover(
            this WorkflowDefinitionActivityList list,
            Action<ITypeSourceSelector> selector)
        {
            var typeSourceSelector = new TypeSourceSelector();
            selector(typeSourceSelector);

            var serviceCollection = new ServiceCollection();
            typeSourceSelector.Populate(serviceCollection, RegistrationStrategy.Replace(ReplacementBehavior.All));

            foreach (var service in serviceCollection)
            {
                list.Add(_activityDescriber.Describe(service.ImplementationType));
            }

            return list;
        }

        public static ElsaDashboardOptions DiscoverActivities(
            this ElsaDashboardOptions options, IActivityDescriber activityDescriber)
        {
            _activityDescriber = activityDescriber;

            options.WorkflowDefinitionActivities
                // Add all activities from all referenced assemblies.
                .Discover(
                    selector => selector.FromApplicationDependencies(x => !x.FullName.StartsWith("Microsoft.AspNetCore")) // TODO: Prevents type load exception. Needs more investigation.
                        .AddClasses(x => x.AssignableTo<IActivity>()));

            return options;
        }
    }
}