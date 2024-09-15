using Microsoft.AspNetCore.Razor.TagHelpers;
using N4Core.Culture;
using N4Core.Types.Extensions;

namespace N4Core.Views.TagHelpers.Bases
{
    public abstract class TagHelperBase : TagHelper
    {
        protected virtual string GetDisplayName(string value, Languages language)
        {
            return value.GetDisplayName(language);
        }

        protected virtual string GetErrorMessage(string value, Languages language)
        {
            return value.GetErrorMessage(language);
        }
    }
}
