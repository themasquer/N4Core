namespace N4Core.Reflection.Models
{
    public class ReflectionRecordModel
    {
        public string? IsDeleted { get; set; }
        public string? CreateDate { get; set; }
        public string? UpdateDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public string? Guid { get; set; }
        public string? FileData { get; set; }
        public string? FileContent { get; set; }
        public string? FilePath { get; set; }
        public bool HasIsDeleted => !string.IsNullOrWhiteSpace(IsDeleted);
        public bool HasModifiedBy => !string.IsNullOrWhiteSpace(CreateDate) && !string.IsNullOrWhiteSpace(CreatedBy)
            && !string.IsNullOrWhiteSpace(UpdateDate) && !string.IsNullOrWhiteSpace(UpdatedBy);
        public bool HasGuid => !string.IsNullOrWhiteSpace(Guid);
        public bool HasFile => !string.IsNullOrWhiteSpace(FileData) && !string.IsNullOrWhiteSpace(FileContent) && !string.IsNullOrWhiteSpace(FilePath);
    }
}
