namespace N4Core.Records.Bases
{
    public interface ISoftDelete
    {
        public bool? IsDeleted { get; set; }
    }
}
