#nullable disable

using Microsoft.AspNetCore.Http;
using N4Core.Session.Extensions;

namespace N4Core.Session.Utils.Bases
{
    public abstract class SessionUtilBase
    {
        protected readonly IHttpContextAccessor _httpContextAccessor;

        protected SessionUtilBase(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public virtual void Remove(string sessionKey)
        {
            _httpContextAccessor.HttpContext.Session.Remove(sessionKey);
        }

        public virtual T Get<T>(string sessionKey) where T : class
        {
            return _httpContextAccessor.HttpContext.Session.Get<T>(sessionKey);
        }

        public virtual void Set<T>(T sessionObject, string sessionKey) where T : class
        {
            _httpContextAccessor.HttpContext.Session.Set(sessionKey, sessionObject);
        }
    }
}
