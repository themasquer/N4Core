using Microsoft.AspNetCore.Mvc.Rendering;
using N4Core.Types.Extensions;

namespace N4Core.Types.Extensions
{
    public static class EnumExtension
    {
        public static Dictionary<int, string> ToDictionary(this Enum value)
        {
            return Enum.GetValues(value.GetType()).Cast<Enum>().ToDictionary(e => (int)(object)e, e => e.ToString());
        }

        public static SelectList ToSelectList(this Enum value)
        {
            return new SelectList(value.ToDictionary().Select(v => new SelectListItem(v.Value, v.Key.ToString())), "Value", "Text");
        }

        public static SelectList ToSelectList(this Enum value, string selectedValue)
        {
            return new SelectList(value.ToDictionary().Select(v => new SelectListItem(v.Value, v.Key.ToString())), "Value", "Text", selectedValue);
        }
    }
}
