#nullable disable

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace N4Core.Settings.Bases
{
    public class AppSettingsBase : SettingsBase
    {
        #region App config independent from appsettings.json
        [JsonIgnore]
        public bool UseIdentity { get; private set; }

        [JsonIgnore]
        public bool AppIsEnvironmentDevelopment { get; private set; }
        #endregion

        public AppSettingsBase(IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
        }

        public bool ShowRegister { get; set; }
        public bool ShowEmailOnRegister { get; set; }
        public int AuthenticationCookieExpirationInMinutes { get; set; } = 180;
        public int SessionExpirationInMinutes { get; set; } = 60;

        public virtual AppSettingsBase Bind(bool useIdentity = false, bool isAppEnvironmentDevelopment = false)
        {
            UseIdentity = useIdentity;
            AppIsEnvironmentDevelopment = isAppEnvironmentDevelopment;
            return base.Bind() as AppSettingsBase;
        }
    }
}
