#nullable disable

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using N4Core.Culture;
using N4Core.Views.TagHelpers.Bases;

namespace N4Core.Views.TagHelpers
{
    [HtmlTargetElement("displayname", Attributes = "asp-for,asp-language")]
    public class DisplayNameTagHelper : TagHelperBase
    {
        [HtmlAttributeName("asp-for")]
        public ModelExpression AspFor { get; set; }

        [HtmlAttributeName("asp-language")]
        public Languages AspLanguage { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            ModelMetadata aspForMetadata = AspFor.Metadata;
            string displayName;
            if (!string.IsNullOrWhiteSpace(aspForMetadata.DisplayName))
            {
                displayName = aspForMetadata.DisplayName;
            }
            else if (!string.IsNullOrWhiteSpace(aspForMetadata.PropertyName))
            {
                displayName = aspForMetadata.PropertyName;
            }
            else
            {
                displayName = aspForMetadata.Name;
            }
            displayName = GetDisplayName(displayName, AspLanguage);
            output.TagName = "label";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.SetContent(displayName);
        }
    }
}
