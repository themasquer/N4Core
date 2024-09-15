using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace N4Core.Views.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string GetCurrentRoute(this IUrlHelper helper, ViewContext viewContext, bool includeQueryString = false)
        {
            string currentRoute = $"/{viewContext.RouteData.Values["Controller"]}/{viewContext.RouteData.Values["Action"]}";
            if (viewContext.RouteData.Values["Area"] is not null)
                currentRoute = $"/{viewContext.RouteData.Values["Area"]}" + currentRoute;
            if (viewContext.RouteData.Values["id"] is not null)
                currentRoute += $"/{viewContext.RouteData.Values["id"]}";
            if (includeQueryString)
                currentRoute += viewContext.HttpContext.Request.QueryString.Value;
            return currentRoute;
        }

        public static string GetHomeRoute(this IUrlHelper helper) => $"/Home/Index";

        public static string GetHomeRoute(this IUrlHelper helper, string area) => $"/{area}{GetHomeRoute(helper)}";

        public static string GetHomeRoute(this IUrlHelper helper, ViewContext viewContext)
        {
            string homeRoute = GetHomeRoute(helper);
            if (viewContext.RouteData.Values["Area"] is not null)
                homeRoute = $"/{viewContext.RouteData.Values["Area"]}" + homeRoute;
            return homeRoute;
        }

        public static string GetReturnRoute(this IUrlHelper helper, string returnUrl) => string.IsNullOrWhiteSpace(returnUrl) || returnUrl.Contains("//") ?
            GetHomeRoute(helper) : returnUrl;
    }
}
