using N4Core.Culture;
using N4Core.Records.Messages;

namespace N4Core.Services.Models
{
    public class ViewModel : PageOrderFilterModel
    {
        public RecordMessagesModel Messages { get; }
        public ViewTextsModel ViewTexts { get; }
        public bool PageOrderFilter { get; set; }
        public string? Message { get; set; }
        public bool Modal { get; set; }
        public bool FileOperations { get; set; }
        public bool ExportOperation { get; set; }
        public bool TimePicker { get; set; }

        public ViewModel(Languages language = Languages.English)
        {
            Language = language;
            Messages = new RecordMessagesModel(Language);
            ViewTexts = new ViewTextsModel(Language);
        }
    }
}
