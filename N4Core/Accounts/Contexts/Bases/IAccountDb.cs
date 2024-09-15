using Microsoft.EntityFrameworkCore;
using N4Core.Accounts.Entities;
using N4Core.Accounts.Entities.Bases;

namespace N4Core.Accounts.Contexts.Bases
{
    public interface IAccountDb
    {
        public DbSet<AccountUser> AccountUsers { get; set; }
        public DbSet<AccountRole> AccountRoles { get; set; }
        public DbSet<AccountGroupUser> AccountGroupUsers { get; set; }
        public DbSet<AccountGroupBase> AccountGroups { get; set; }
    }
}
