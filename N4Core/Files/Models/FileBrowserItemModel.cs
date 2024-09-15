using N4Core.Records.Bases;

namespace N4Core.Files.Models
{
    public class FileBrowserItemModel : Record
    {
        public string? Title { get; set; }
        public string? Extension { get; set; }
        public string? Path { get; set; }
        public string? ParentPath { get; set; }
        public int Level { get; set; }
        public long? Length { get; set; }
        public DateTime CreateDate { get; set; }
        public string? Content { get; set; }
        public string? Folders { get; set; }
        public bool IsFile { get; set; }
    }
}
