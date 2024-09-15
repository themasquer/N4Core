using Microsoft.AspNetCore.Mvc.Rendering;
using N4Core.Files.Enums;
using N4Core.Services.Models;

namespace N4Core.Files.Models
{
    public class FileBrowserModel : PageOrderModel
    {
        public string? Path { get; set; }
        public string? FirstLevelPath { get; set; }
        public List<SelectListItem>? FirstLevelPaths { get; set; }
        public string? StartLink { get; set; }
        public List<FileBrowserItemModel>? Contents { get; set; }
        public string? Title { get; set; }
        public string? FileContent { get; set; }
        public string? FileCodeContent { get; set; }
        public byte[]? FileBinaryContent { get; set; }
        public FileTypes FileType { get; set; }
        public string? FileContentType { get; set; }
        public string? HierarchicalDirectoryLinks { get; set; }
        public FileBrowserOperations Operation { get; set; }
        public string? OperationMessage { get; set; }
        public List<FileBrowserItemModel>? FilteredItems { get; set; }
        public FileBrowserFilterModel? Filter { get; set; }
        public bool HasPath => !string.IsNullOrWhiteSpace(Path);
        public bool HasFileContent => !string.IsNullOrWhiteSpace(FileContent);
    }
}
