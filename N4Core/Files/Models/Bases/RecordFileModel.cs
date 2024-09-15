using Microsoft.AspNetCore.Http;
using N4Core.Files.Bases;
using Newtonsoft.Json;
using System.ComponentModel;

namespace N4Core.Files.Models.Bases
{
    public class RecordFileModel : RecordFile
    {
        [DisplayName("{Image;İmaj}")]
        [JsonIgnore]
        public IFormFile? FormFile { get; set; }

        [DisplayName("{Image;İmaj}")]
        public string? FileImgSrc { get; set; }
    }
}
