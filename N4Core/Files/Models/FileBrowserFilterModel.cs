using N4Core.Services.Models;

namespace N4Core.Files.Models
{
    public class FileBrowserFilterModel : PageOrderModel
    {
        public string? PlaceHolder { get; set; }
        public string Expression { get; set; } = string.Empty;
        public bool MatchCase { get; set; }
        public bool MatchWord { get; set; }
        public bool Find { get; set; }
        public bool FilterSession { get; set; }
        public bool Paging { get; set; }
        public bool FindInCode { get; set; }
        public string? Path { get; set; }
        public bool HasExpression => !string.IsNullOrWhiteSpace(Expression);
    }
}
