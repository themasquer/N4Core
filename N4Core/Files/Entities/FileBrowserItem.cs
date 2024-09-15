using N4Core.Records.Bases;

namespace N4Core.Files.Entities
{
    public class FileBrowserItem : Record
    {
        public string Title { get; set; } = null!;
        public string? Extension { get; set; }
        public string Path { get; set; } = null!;
        public string? ParentPath { get; set; }
        public int Level { get; set; }
        public long? Length { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
