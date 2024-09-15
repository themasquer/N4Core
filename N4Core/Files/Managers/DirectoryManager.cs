#nullable disable

namespace N4Core.Files.Managers
{
    public class DirectoryManager
    {
        public string[] Directories { get; private set; }
        public bool HasDirectories => Directories is not null && Directories.Any();

        public string DirectoryPath
        {
            get
            {
                if (HasDirectories)
                {
                    List<string> path = Directories.ToList();
                    path.Insert(0, "wwwroot");
                    return Path.Combine(path.ToArray());
                }
                return string.Empty;
            }
        }

        public void SetDirectories(params string[] directories) => Directories = directories?.ToArray();

        public List<string> GetDirectoriesAndFiles() => Directory.GetFileSystemEntries(DirectoryPath, "*", SearchOption.AllDirectories).OrderBy(e => e).ToList();
    }
}
