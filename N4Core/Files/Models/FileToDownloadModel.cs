namespace N4Core.Files.Models
{
    public class FileToDownloadModel
    {
        public Stream? FileStream { get; set; }
        public string? FileContentType { get; set; }
        public string? FileName { get; set; }
    }
}
