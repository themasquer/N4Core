using Microsoft.AspNetCore.Http;
using N4Core.Cookie.Utils.Bases;

namespace N4Core.Cookie.Utils
{
    public class CookieUtil : CookieUtilBase
    {
        public CookieUtil(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }
    }
}
