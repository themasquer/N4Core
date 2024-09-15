using Microsoft.EntityFrameworkCore;
using N4Core.Files.Entities;

namespace N4Core.Files.Contexts.Bases
{
    public interface IFileBrowserDb
    {
        public DbSet<FileBrowserItem> FileBrowserItems { get; set; }
    }
}
