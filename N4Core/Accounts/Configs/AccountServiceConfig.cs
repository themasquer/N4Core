using N4Core.Culture;

namespace N4Core.Accounts.Configs
{
    public class AccountServiceConfig
    {
        public Languages? Language { get; set; }
        public string AuthenticationScheme { get; set; } = "AccountAuthScheme";
    }
}
