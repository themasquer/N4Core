#nullable disable

using Microsoft.AspNetCore.Mvc.ModelBinding;
using N4Core.Culture;

namespace N4Core.Types.Extensions
{
    public static class ModelStateExtensions
    {
        public static ModelStateDictionary Update(this ModelStateDictionary modelState, Languages language)
        {
            var errorDictionary = modelState.Where(ms => ms.Value != null && ms.Value.Errors.Any()).ToDictionary(
                    ms => ms.Key,
                    ms => ms.Value?.Errors.Select(e => e.ErrorMessage.GetErrorMessage(language)).ToArray()
                );
            string[] valueDictionary;
            if (errorDictionary is not null && errorDictionary.Any())
            {
                modelState.Clear();
                foreach (var key in errorDictionary.Keys)
                {
                    valueDictionary = errorDictionary[key];
                    foreach (var value in valueDictionary)
                    {
                        modelState.AddModelError(key, value);
                    }
                }
            }
            return modelState;
        }

        public static string GetErrorMessages(this ModelStateDictionary modelState, Languages language)
        {
            modelState = modelState.Update(language);
            return string.Join(", ", modelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
        }
    }
}
