#nullable disable

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using N4Core.Accounts.Utils.Bases;
using N4Core.Contexts.Bases;
using N4Core.Records.Bases;
using N4Core.Reflection.Models;
using N4Core.Reflection.Utils.Bases;
using System.Linq.Expressions;

namespace N4Core.Repositories.Bases
{
    public abstract class RepoBase<TEntity> : IDisposable where TEntity : Record, new()
    {
        protected readonly IDb _db;
        protected readonly ReflectionUtilBase _reflectionUtil;

        protected string _modifiedBy;

        private bool _applyRecordChanges = true;

        internal ReflectionRecordModel ReflectionRecordModel { get; }

        public string Collation { get; protected set; } = "Turkish_CI_AS";

        protected RepoBase(IDb db, ReflectionUtilBase reflectionUtil, AccountUtilBase accountUtil)
        {
            _db = db;
            _reflectionUtil = reflectionUtil;
            _modifiedBy = accountUtil.GetUser()?.UserName;
            ReflectionRecordModel = _reflectionUtil.GetReflectionRecordModel<TEntity>();
        }

        public void Set(bool applyRecordChanges)
        {
            _applyRecordChanges = applyRecordChanges;
        }

        public void Set(string collation)
        {
            Collation = collation;
        }

        public void Set(bool applyRecordChanges, string collation)
        {
            Set(applyRecordChanges);
            Set(collation);
        }

        public virtual IQueryable<TEntity> Query(bool isNoTracking = false)
        {
            var query = isNoTracking ? _db.Set<TEntity>().AsNoTracking() : _db.Set<TEntity>();
            if (ReflectionRecordModel is not null && ReflectionRecordModel.HasIsDeleted)
                query = query.Where(q => (EF.Property<bool?>(q, ReflectionRecordModel.IsDeleted) ?? false) == false).AsQueryable();
            return query;
        }

        public virtual void Create(TEntity entity)
        {
            _reflectionUtil.TrimStringProperties(entity);
            _db.Set<TEntity>().Add(entity); 
            if (_applyRecordChanges)
                ApplyRecordChanges(); 
        }

        public virtual void Update(TEntity entity)
        {
            _reflectionUtil.TrimStringProperties(entity);
            _db.Set<TEntity>().Update(entity);
            if (_applyRecordChanges)
                ApplyRecordChanges();
        }

        public virtual void Delete(TEntity entity)
        {
            _db.Set<TEntity>().Remove(entity);
            if (_applyRecordChanges)
                ApplyRecordChanges();
        }

        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            _db.Set<TEntity>().RemoveRange(_db.Set<TEntity>().Where(predicate));
            if (_applyRecordChanges)
                ApplyRecordChanges();
        }

        public virtual void Delete()
        {
            _db.Set<TEntity>().RemoveRange(_db.Set<TEntity>());
            if (_applyRecordChanges)
                ApplyRecordChanges();
        }

        protected virtual void ApplyRecordChanges()
        {
            if (ReflectionRecordModel is not null)
            {
                foreach (var entry in _db.ChangeTracker.Entries<TEntity>())
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            if (ReflectionRecordModel.HasGuid)
                            {
                                entry.CurrentValues[ReflectionRecordModel.Guid] = Guid.NewGuid().ToString();
                            }
                            if (ReflectionRecordModel.HasModifiedBy)
                            {
                                entry.CurrentValues[ReflectionRecordModel.CreateDate] = DateTime.Now;
                                entry.CurrentValues[ReflectionRecordModel.CreatedBy] = _modifiedBy;
                            }
                            break;
                        case EntityState.Modified:
                            if (ReflectionRecordModel.HasGuid)
                            {
                                entry.Property(ReflectionRecordModel.Guid).IsModified = false;
                            }
                            if (UpdateChangesDetected(entry))
                            {
                                if (ReflectionRecordModel.HasModifiedBy)
                                {
                                    entry.CurrentValues[ReflectionRecordModel.UpdateDate] = DateTime.Now;
                                    entry.CurrentValues[ReflectionRecordModel.UpdatedBy] = _modifiedBy;
                                }
                            }
                            else
                            {
                                entry.State = EntityState.Unchanged;
                            }
                            break;
                        case EntityState.Deleted:
                            if (ReflectionRecordModel.HasIsDeleted)
                            {
                                entry.CurrentValues[ReflectionRecordModel.IsDeleted] = true;
                                entry.State = EntityState.Modified;
                            }
                            break;
                    }
                }
            }
        }

        protected virtual bool UpdateChangesDetected(EntityEntry<TEntity> entry)
        {
            return entry.Properties.Any(p => p.IsModified &&
                ((p.CurrentValue is null && p.OriginalValue is not null) ||
                (p.CurrentValue is not null && p.OriginalValue is null) ||
                (p.CurrentValue is not null && p.OriginalValue is not null && !p.CurrentValue.Equals(p.OriginalValue))));
        }

        public void Dispose()
        {
            _db?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
