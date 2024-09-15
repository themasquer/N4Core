using N4Core.Records.Bases;

namespace N4Core.Accounts.Entities
{
    public class AccountGroupUser : Record
    {
        public int UserId { get; set; }
        public AccountUser? User { get; set; }
        public int GroupId { get; set; }
        public AccountGroup? Group { get; set; }
    }
}
