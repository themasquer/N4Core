using N4Core.Accounts.Entities.Bases;

namespace N4Core.Accounts.Entities
{
    public class AccountGroup : AccountGroupBase
    {
        public List<AccountGroupUser>? GroupUsers { get; set; }
    }
}
