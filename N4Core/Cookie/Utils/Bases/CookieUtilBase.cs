#nullable disable

using Microsoft.AspNetCore.Http;
using N4Core.Expiration.Models;

namespace N4Core.Cookie.Utils.Bases
{
    public abstract class CookieUtilBase
    {
        protected readonly IHttpContextAccessor _httpContextAccessor;

        protected CookieUtilBase(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public virtual void Set(string key, string value, CookieOptions cookieOptions)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Append(key, value, cookieOptions);
        }

        public virtual void Set(string key, string value)
        {
            var cookieOptions = new CookieOptions()
            {
                Expires = new ExpireModel().DateTimeOffset,
                HttpOnly = true
            };
            Set(key, value, cookieOptions);
        }

        public virtual void Set(string key, string value, ExpireModel cookieExpireModel)
        {
            var cookieOptions = new CookieOptions()
            {
                Expires = cookieExpireModel.DateTimeOffset,
                HttpOnly = true
            };
            Set(key, value, cookieOptions);
        }

        public virtual string Get(string key)
        {
            return _httpContextAccessor.HttpContext.Request.Cookies[key];
        }

        public virtual void Remove(string key)
        {
            var cookieOptions = new CookieOptions()
            {
                Expires = new ExpireModel(-1).DateTimeOffset,
                HttpOnly = true
            };
            Set(key, string.Empty, cookieOptions);
        }
    }
}
