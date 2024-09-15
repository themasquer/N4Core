#nullable disable

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace N4Core.Settings.Bases
{
    public abstract class SettingsBase
    {
        #region App config independent from appsettings.json
        [JsonIgnore]
        public string Name { get; protected set; } = "AppSettings";
        #endregion

        protected readonly IConfiguration _configuration;
        protected readonly IWebHostEnvironment _webHostEnvironment;

        protected SettingsBase(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public virtual SettingsBase Bind()
        {
            if (string.IsNullOrWhiteSpace(Name) || _configuration is null)
                return null;
            _configuration.GetSection(Name).Bind(this);
            return this;
        }

        public virtual SettingsBase Bind(SettingsBase settings)
        {
            if (string.IsNullOrWhiteSpace(settings.Name) || _configuration is null)
                return null;
            _configuration.GetSection(settings.Name).Bind(settings);
            return settings;
        }

        public virtual SettingsBase Update(SettingsBase settings)
        {
            if (string.IsNullOrWhiteSpace(settings.Name) || _webHostEnvironment is null)
                return null;
            string[] paths =
            [
                $@"{Path.Combine(_webHostEnvironment.ContentRootPath, "appsettings.json")}",
                $@"{Path.Combine(_webHostEnvironment.ContentRootPath, "appsettings.Development.json")}",
                $@"{Path.Combine(_webHostEnvironment.ContentRootPath, "appsettings.production.json")}"
            ];
            foreach (string path in paths)
            {
                if (File.Exists(path))
                {
                    string text = File.ReadAllText(path);
                    if (!string.IsNullOrEmpty(text))
                    {
                        var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(text);
                        if (json is not null && json.ContainsKey(settings.Name))
                        {
                            json[settings.Name] = settings;
                        }
                        File.WriteAllText(path, JsonConvert.SerializeObject(json, Formatting.Indented));
                    }
                }
            }
            return settings;
        }
    }
}
