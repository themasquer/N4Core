using AutoMapper;
using N4Core.Culture;
using N4Core.Files.Managers;

namespace N4Core.Services.Configs
{
    public class ServiceConfig : DirectoryManager
    {
        public Languages? Language { get; set; }
        public bool NoEntityTracking { get; set; }
        public bool PageOrderFilter { get; set; }
        public string[]? RecordsPerPageCounts { get; set; }

        private bool _usePageSession;
        public bool UsePageSession
        {
            get
            {
                return PageOrderFilter == false ? false : _usePageSession;
            }
            set
            {
                _usePageSession = value;
            }
        }

        public bool? ListCards { get; set; }
        public bool Modal { get; set; }
        public bool FileOperations { get; set; }
        public bool ExportOperation { get; set; }
        public bool TimePicker { get; set; }
        public string FileExtensions { get; set; } = ".jpg, .jpeg, .png";
        public double FileLengthInMegaBytes { get; set; } = 1;
        public bool IsExcelLicenseCommercial { get; set; }

        public Profile[]? MapperProfiles { get; private set; }
        public void SetMapperProfiles(params Profile[] mapperProfiles) => MapperProfiles = mapperProfiles?.ToArray();
    }
}
