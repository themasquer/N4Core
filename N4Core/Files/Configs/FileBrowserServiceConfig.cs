using N4Core.Culture;
using N4Core.Files.Managers;

namespace N4Core.Files.Configs
{
    public class FileBrowserServiceConfig : DirectoryManager
    {
        public Languages? Language { get; set; }
        public string StartLink { get; set; } = "Home";
        public string Area { get; set; } = "";
        public string Controller { get; set; } = "Home";
        public string Action { get; set; } = "Index";
        public bool Database { get; set; } = true;
        public bool UsePageSession { get; set; } = true;
        public string[]? RecordsPerPageCounts { get; set; }
        public byte? HideHierarchicalDirectoryAfterLevel { get; set; }

		public Dictionary<string, string> TextFiles { get; set; } = new Dictionary<string, string>
		{
			{ ".txt", "plaintext" },
			{ ".json", "json" },
			{ ".xml", "xml" },
			{ ".htm", "html" },
			{ ".html", "html" },
			{ ".css", "css" },
			{ ".js", "javascript" },
			{ ".cs", "csharp" },
			{ ".java", "java" },
			{ ".sql", "sql" },
			{ ".cshtml", "html" }
		};
		public Dictionary<string, string> ImageFiles { get; set; } = new Dictionary<string, string>()
		{
			{ ".png", "image/png" },
			{ ".jpg", "image/jpeg" },
			{ ".jpeg", "image/jpeg" },
			{ ".gif", "image/gif" }
		};
		public Dictionary<string, string> OtherFiles { get; set; } = new Dictionary<string, string>()
		{
			{ ".zip", "application/zip" },
			{ ".7z", "application/zip" },
			{ ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
			{ ".mp4", "video/mp4" }
		};

        public string AtagStyleNone { get; set; } = "style=\"color: black;text-decoration: none;font-family: Menlo,Monaco,Consolas,'Courier New',monospace !important;font-size: 16px !important;\"";
        public string AtagStyleUnderline { get; set; } = "style=\"color: black;text-decoration: underline;font-family: Menlo,Monaco,Consolas,'Courier New',monospace !important;font-size: 16px !important;\"";
		public string AtagHref => "href=\"" + (string.IsNullOrWhiteSpace(Area) ? "/" : "/" + Area + "/") + Controller + "/" + Action + "?path=";
		public string LiClassCurrent { get; set; } = "class=\"currentdirectories\"";
    }
}
