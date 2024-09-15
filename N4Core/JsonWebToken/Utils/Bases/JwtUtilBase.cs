#nullable disable

using Microsoft.IdentityModel.Tokens;
using N4Core.Accounts.Models;
using N4Core.Expiration.Models;
using N4Core.JsonWebToken.Models;
using N4Core.JsonWebToken.Settings;
using N4Core.Settings.Bases;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace N4Core.JsonWebToken.Utils.Bases
{
    public abstract class JwtUtilBase
    {
        public JwtSettings JwtSettings { get; protected set; }

        protected JwtUtilBase(AppSettingsBase appSettings)
        {
            JwtSettings = new JwtSettings();
            appSettings.Bind(JwtSettings);
        }

        public virtual JwtModel GetJwt(AccountUserModel model)
        {
            if (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.RoleName))
                return null;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings.SecurityKey));
            var signingCredentials = new SigningCredentials(securityKey, JwtSettings.SecurityAlgorithm);
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, model.UserName),
                new Claim(ClaimTypes.Role, model.RoleName),
                new Claim(ClaimTypes.PrimarySid, model.Id.ToString())
            };
            if (!string.IsNullOrWhiteSpace(model.Guid))
                claims.Add(new Claim(ClaimTypes.Sid, model.Guid.ToString()));
            var expire = new ExpireModel(0, JwtSettings.ExpirationInMinutes);
            var expiration = expire.DateTime;
            var jwtSecurityToken = new JwtSecurityToken(JwtSettings.Issuer, JwtSettings.Audience, claims, DateTime.Now, expiration, signingCredentials);
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);
            return new JwtModel()
            {
                Token = "Bearer " + token,
                Expiration = expiration
            };
        }
    }
}
