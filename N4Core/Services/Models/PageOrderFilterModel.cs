namespace N4Core.Services.Models
{
    public class PageOrderFilterModel : PageOrderModel
    {
        public string? Filter { get; set; }
        public bool? ListCards { get; set; }

        public PageOrderFilterModel() : base()
        {
            Filter = string.Empty;
        }
    }
}
