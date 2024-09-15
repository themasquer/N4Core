namespace N4Core.Records.Bases
{
    public class Record
    {
        public int Id { get; set; }
        public string? Guid { get; set; }

        public Record(int id)
        {
            Id = id;
        }

        public Record()
        {
        }
    }
}
