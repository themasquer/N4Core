using Microsoft.AspNetCore.Http;
using N4Core.Session.Utils.Bases;

namespace N4Core.Session.Utils
{
    public class SessionUtil : SessionUtilBase
    {
        public SessionUtil(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }
    }
}
