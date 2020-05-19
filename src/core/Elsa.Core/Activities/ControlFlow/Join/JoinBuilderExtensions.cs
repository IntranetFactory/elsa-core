using Elsa.Builders;

// ReSharper disable once CheckNamespace
namespace Elsa.Activities.ControlFlow
{
    public static class JoinBuilderExtensions
    {
        public static ActivityBuilder Join(this IBuilder builder) => builder.Then<Join>();
    }
}