using N4Core.Records.Bases;

namespace N4Core.Accounts.Models
{
    public class AccountUserModel : Record
    {
        public string UserName { get; set; } = null!;
        public List<string>? RoleNames { get; set; }
        public string? RoleName => RoleNames?.FirstOrDefault();
    }
}
