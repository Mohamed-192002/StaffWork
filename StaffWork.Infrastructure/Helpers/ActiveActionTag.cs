using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffWork.Infrastructure.Helpers
{
    [HtmlTargetElement("li", Attributes = "active-controller, active-action")]
    public class ActiveActionTag : TagHelper
    {
        public string? ActiveController { get; set; }
        public string? ActiveAction { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext? ViewContextData { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrEmpty(ActiveController) && string.IsNullOrEmpty(ActiveAction))
                return;

            var currentController = ViewContextData?.RouteData.Values["controller"]?.ToString() ?? string.Empty;
            var currentAction = ViewContextData?.RouteData.Values["action"]?.ToString() ?? string.Empty;

            var controllers = ActiveController?.Split(',') ?? Array.Empty<string>();
            var actions = ActiveAction?.Split(',') ?? Array.Empty<string>();

            bool isActive = controllers.Contains(currentController, StringComparer.OrdinalIgnoreCase) &&
                            actions.Contains(currentAction, StringComparer.OrdinalIgnoreCase);

            if (isActive)
            {
                if (output.Attributes.ContainsName("class"))
                    output.Attributes.SetAttribute("class", $"{output.Attributes["class"].Value} active");
                else
                    output.Attributes.SetAttribute("class", "active");
            }
        }

    }
}
