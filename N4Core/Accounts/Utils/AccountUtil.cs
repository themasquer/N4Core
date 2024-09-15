using Microsoft.AspNetCore.Http;
using N4Core.Accounts.Utils.Bases;

namespace N4Core.Accounts.Utils
{
    public class AccountUtil : AccountUtilBase
    {
        public AccountUtil(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }

        public AccountUtil() : base()
        {
        }
    }
}
