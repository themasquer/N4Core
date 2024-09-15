using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace N4Core.Web.Contexts
{
    public class DbFactory : IDesignTimeDbContextFactory<Db>
    {
        public Db CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<Db>();
            optionsBuilder.UseSqlServer("server=SERVER;database=DATABASE;trusted_connection=true;multipleactiveresultsets=true;trustservercertificate=true;");
            return new Db(optionsBuilder.Options);
        }
    }
}
