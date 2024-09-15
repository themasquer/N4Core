using N4Core.Culture;
using N4Core.Responses.Messages;

namespace N4Core.Files.Messages
{
    public class FileBrowserMessagesModel : CrudMessagesModel
    {
        public string SyncedSuccessfully { get; set; }
        public string ResetSuccessfully { get; set; }

        public FileBrowserMessagesModel(Languages language = Languages.English) : base(language) 
        {
            SyncedSuccessfully = language == Languages.Türkçe ? "kayıt veritabanıyla eşleştirildi." : "records synced with database.";
            ResetSuccessfully = language == Languages.Türkçe ? "Kayıtlar sıfırlandı." : "Records reset.";
        }
    }
}
