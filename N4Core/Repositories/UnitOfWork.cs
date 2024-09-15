using N4Core.Contexts.Bases;
using N4Core.Repositories.Bases;

namespace N4Core.Repositories
{
    public class UnitOfWork : UnitOfWorkBase
    {
        public UnitOfWork(IDb db) : base(db)
        {
        }
    }
}
