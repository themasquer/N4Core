#nullable disable

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using N4Core.Culture;
using N4Core.Views.TagHelpers.Bases;

namespace N4Core.Views.TagHelpers
{
    [HtmlTargetElement("validation", Attributes = "asp-for,asp-language")]
    public class ValidationTagHelper : TagHelperBase
    {
        [HtmlAttributeName("asp-for")]
        public ModelExpression AspFor { get; set; }

        [HtmlAttributeName("asp-language")]
        public Languages AspLanguage { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var modelName = AspFor.Name;
            var modelState = ViewContext.ViewData.ModelState;
            if (modelState.TryGetValue(modelName, out var entry) && entry.Errors.Count > 0)
            {
                var errorMessage = entry.Errors[0].ErrorMessage;
                errorMessage = GetErrorMessage(errorMessage, AspLanguage);
                output.TagName = "span";
                output.Content.SetHtmlContent(errorMessage);
            }
            else
            {
                output.SuppressOutput();
            }
        }
    }
}
