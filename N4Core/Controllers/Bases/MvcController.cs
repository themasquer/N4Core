#nullable disable

using Microsoft.AspNetCore.Mvc;
using N4Core.Cookie.Utils.Bases;
using N4Core.Culture.Utils.Bases;
using N4Core.Session.Utils.Bases;
using System.Globalization;

namespace N4Core.Controllers.Bases
{
    public abstract class MvcController : Controller
    {
        protected readonly CultureUtilBase _cultureUtil;
        protected readonly CookieUtilBase _cookieUtil;
        protected readonly SessionUtilBase _sessionUtil;

        protected MvcController(CultureUtilBase cultureUtil, CookieUtilBase cookieUtil, SessionUtilBase sessionUtil)
        {
            _cultureUtil = cultureUtil;
            _cookieUtil = cookieUtil;
            _sessionUtil = sessionUtil;
            string language = _cookieUtil.Get("lang");
            CultureInfo culture = _cultureUtil.GetCulture(language);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        protected virtual async Task SetViewData(string message = null)
        {
            ViewBag.Message = message; // End message in service with '.' for success, '!' for danger Bootstrap CSS classes to be used in the View
        }

        protected virtual void SetTempData(string message)
        {
            TempData["Message"] = message; // End message in service with '.' for success, '!' for danger Bootstrap CSS classes to be used in the View
        }
    }
}
