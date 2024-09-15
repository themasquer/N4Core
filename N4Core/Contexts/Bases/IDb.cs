using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace N4Core.Contexts.Bases
{
    public interface IDb : IDisposable
    {
        public DbSet<TEntity> Set<TEntity>() where TEntity : class;
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        public int SaveChanges();
        public ChangeTracker ChangeTracker { get; }
    }
}
