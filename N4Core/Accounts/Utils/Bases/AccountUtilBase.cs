#nullable disable

using Microsoft.AspNetCore.Http;
using N4Core.Accounts.Entities;
using N4Core.Accounts.Models;
using System.Security.Claims;

namespace N4Core.Accounts.Utils.Bases
{
    public abstract class AccountUtilBase
    {
        protected readonly IHttpContextAccessor _httpContextAccessor;

        protected AccountUtilBase(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected AccountUtilBase()
        {
        }

        public virtual AccountUserModel GetUser()
        {
            AccountUserModel user = null;
            if (_httpContextAccessor is not null && _httpContextAccessor.HttpContext.User.Identity is not null && _httpContextAccessor.HttpContext.User.Claims is not null && _httpContextAccessor.HttpContext.User.Claims.Any())
            {
                var primarySidClaim = _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(u => u.Type == ClaimTypes.PrimarySid);
                if (primarySidClaim is not null)
                {
                    user = new AccountUserModel()
                    {
                        UserName = _httpContextAccessor.HttpContext.User.Identity.Name,
                        RoleNames = _httpContextAccessor.HttpContext.User.Claims.Where(u => u.Type == ClaimTypes.Role).Select(u => u.Value).ToList(),
                        Guid = _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(u => u.Type == ClaimTypes.Sid)?.Value,
                        Id = Convert.ToInt32(primarySidClaim.Value)
                    };
                }
                else
                {
                    user = new AccountUserModel()
                    {
                        UserName = _httpContextAccessor.HttpContext.User.Identity.Name,
                        RoleNames = _httpContextAccessor.HttpContext.User.Claims.Where(u => u.Type == ClaimTypes.Role).Select(u => u.Value).ToList()
                    };
                }
            }
            return user;
        }

        public virtual AccountUserModel GetUser(AccountUser accountUser)
        {
            AccountUserModel user = null;
            if (accountUser is not null && !string.IsNullOrWhiteSpace(accountUser.UserName) && accountUser.Role is not null && !string.IsNullOrWhiteSpace(accountUser.Role.RoleName))
            {
                user = new AccountUserModel()
                {
                    UserName = accountUser.UserName,
                    Guid = accountUser.Guid,
                    Id = accountUser.Id,
                    RoleNames = new List<string>() { accountUser.Role.RoleName }
                };
            }
            return user;
        }

        public virtual ClaimsPrincipal GetPrincipal(AccountUserModel model, string authenticationScheme)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, model.UserName),
                new Claim(ClaimTypes.Role, model.RoleName),
                new Claim(ClaimTypes.PrimarySid, model.Id.ToString())
            };
            var identity = new ClaimsIdentity(claims, authenticationScheme);
            return new ClaimsPrincipal(identity);
        }
    }
}
