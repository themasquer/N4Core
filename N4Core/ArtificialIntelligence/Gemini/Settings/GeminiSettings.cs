#nullable disable

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using N4Core.Settings.Bases;

namespace N4Core.ArtificialIntelligence.Gemini.Settings
{
    public class GeminiSettings : SettingsBase
    {
        public string ApiKey { get; set; }
        public string ApiTextUrl { get; set; }
        public string ApiImageUrl { get; set; }

        public GeminiSettings(IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            Name = nameof(GeminiSettings);
        }
    }
}
