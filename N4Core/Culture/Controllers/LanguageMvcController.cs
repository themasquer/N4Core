#nullable disable

using Microsoft.AspNetCore.Mvc;
using N4Core.Cookie.Utils.Bases;
using N4Core.Views.Extensions;

namespace N4Core.Culture.Controllers
{
    public class LanguageMvcController : Controller
    {
        protected readonly CookieUtilBase _cookieUtil;

        public LanguageMvcController(CookieUtilBase cookieUtil)
        {
            _cookieUtil = cookieUtil;
        }

        public virtual IActionResult Index(int language, string returnUrl = null)
        {
            _cookieUtil.Set("lang", language.ToString());
            return Redirect(Url.GetReturnRoute(returnUrl));
        }
    }
}
