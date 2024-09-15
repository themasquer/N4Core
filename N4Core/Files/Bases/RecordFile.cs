using N4Core.Records.Bases;

namespace N4Core.Files.Bases
{
    public class RecordFile : Record
    {
        public byte[]? FileData { get; set; }
        public string? FileContent { get; set; }
        public string? FilePath { get; set; }
    }
}
