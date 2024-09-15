using N4Core.Culture;

namespace N4Core.ArtificialIntelligence.Messages
{
    public class AiMessagesModel
    {
        public string EmptyRequestText { get; set; }
        public string NoResponse { get; set; }
        public string EmptyResponse { get; set; }

        public AiMessagesModel(Languages language = Languages.English)
        {
            EmptyRequestText = language == Languages.English ? "Message not entered!" : "Mesaj girilmedi!";
            NoResponse = language == Languages.English ? "No response!" : "Yanıt alınamadı!";
            EmptyResponse = language == Languages.English ? "Empty response!" : "Boş yanıt!";
        }
    }
}
