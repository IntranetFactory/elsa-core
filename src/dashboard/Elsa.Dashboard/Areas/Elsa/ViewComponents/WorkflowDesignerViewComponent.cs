using Elsa.Dashboard.Areas.Elsa.ViewModels;
using Elsa.Metadata;
using Elsa.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Elsa.Dashboard.Areas.Elsa.ViewComponents
{
    public class WorkflowDesignerViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            string id,
            ActivityDescriptor[]? activityDefinitions = null,
            WorkflowModel? workflow = null,
            bool? isReadonly = null)
        {
            var model = new WorkflowDesignerViewComponentModel(
                id,
                Serialize(activityDefinitions ?? new ActivityDescriptor[0]),
                Serialize(workflow ?? new WorkflowModel()),
                isReadonly.GetValueOrDefault()
            );

            return View(model);
        }

        private static string? Serialize(object? value)
        {
            if (value == null)
                return null;

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
            return JsonConvert.SerializeObject(value, settings);
        }
    }
}
