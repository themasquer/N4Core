using N4Core.Accounts.Utils.Bases;
using N4Core.Contexts.Bases;
using N4Core.Records.Bases;
using N4Core.Reflection.Utils.Bases;
using N4Core.Repositories.Bases;

namespace N4Core.Repositories
{
    public class Repo<TEntity> : RepoBase<TEntity> where TEntity : Record, new()
    {
        public Repo(IDb db, ReflectionUtilBase reflectionUtil, AccountUtilBase accountUtil) : base(db, reflectionUtil, accountUtil)
        {
        }
    }
}
