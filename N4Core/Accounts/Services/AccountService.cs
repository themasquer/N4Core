using N4Core.Accounts.Entities;
using N4Core.Accounts.Services.Bases;
using N4Core.Accounts.Utils.Bases;
using N4Core.Culture.Utils.Bases;
using N4Core.Repositories.Bases;

namespace N4Core.Accounts.Services
{
    public class AccountService : AccountServiceBase
    {
        public AccountService(UnitOfWorkBase unitOfWork, RepoBase<AccountUser> userRepo, RepoBase<AccountGroup> groupRepo, AccountUtilBase accountUtil, CultureUtilBase cultureUtil) 
            : base(unitOfWork, userRepo, groupRepo, accountUtil, cultureUtil)
        {
        }
    }
}
