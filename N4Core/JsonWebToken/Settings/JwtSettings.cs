#nullable disable

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using N4Core.Settings.Bases;
using Newtonsoft.Json;
using System.Text;

namespace N4Core.JsonWebToken.Settings
{
    public class JwtSettings : SettingsBase
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public int ExpirationInMinutes { get; set; }
        public string SecurityKey { get; set; }

        [JsonIgnore]
        public string SecurityAlgorithm { get; set; } = SecurityAlgorithms.HmacSha256Signature;

        [JsonIgnore]
        public SecurityKey SigningKey => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKey));

        public JwtSettings(IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            Name = nameof(JwtSettings);
        }

        public JwtSettings() : this(default, default)
        {
        }
    }
}
