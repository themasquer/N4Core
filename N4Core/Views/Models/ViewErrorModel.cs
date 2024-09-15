using N4Core.Culture;

namespace N4Core.Views.Models
{
    public class ViewErrorModel
    {
        public string Title { get; set; }
        public string Message { get; set; }

        public ViewErrorModel(Languages language = Languages.English)
        {
            Title = language == Languages.English ? "Error!" : "Hata!";
            Message = language == Languages.English ? "An error occurred while processing your request!" : "İşlem sırasında hata meydana geldi!";
        }

        public ViewErrorModel(string message, Languages language = Languages.English)
        {
            Title = language == Languages.English ? "Error!" : "Hata!";
            Message = message;
        }

        public ViewErrorModel(string message, string title)
        {
            Title = title;
            Message = message;
        }
    }
}
