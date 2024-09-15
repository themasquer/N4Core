#nullable disable

using N4Core.Contexts.Bases;

namespace N4Core.Repositories.Bases
{
    public abstract class UnitOfWorkBase : IDisposable
    {
        protected readonly IDb _db;

        protected UnitOfWorkBase(IDb db)
        {
            _db = db;
        }

        public virtual async Task<int> SaveAsync(CancellationToken cancellationToken = default) => await _db.SaveChangesAsync(cancellationToken);
        public virtual int Save() => _db.SaveChanges();
        
        public void Dispose()
        {
            _db?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
