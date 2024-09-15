namespace N4Core.Expiration.Models
{
    public class ExpireModel
    {
        public TimeSpan TimeSpan { get; private set; }
        public DateTime DateTime { get; private set; }
        public DateTimeOffset DateTimeOffset { get; private set; }

        public ExpireModel()
        {
            TimeSpan = TimeSpan.MaxValue;
            DateTime = DateTime.MaxValue;
            DateTimeOffset = DateTimeOffset.MaxValue;
        }

        public ExpireModel(TimeSpan timeSpan)
        {
            TimeSpan = timeSpan;
            DateTime = DateTime.Now.Add(TimeSpan);
            DateTimeOffset = DateTime.SpecifyKind(DateTime, DateTimeKind.Utc);
        }

        public ExpireModel(int hours, int minutes) : this(new TimeSpan(0, hours, minutes, 0))
        {
        }

        public ExpireModel(int days) : this(new TimeSpan(days, 0, 0, 0))
        {
        }
    }
}
