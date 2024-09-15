using N4Core.Culture;

namespace N4Core.Records.Messages
{
    public class RecordMessagesModel
    {
        public string RecordFound { get; set; }
        public string RecordsFound { get; set; }
        public string RecordNotFound { get; set; }

        public RecordMessagesModel(Languages language = Languages.English)
        {
            RecordFound = language == Languages.Türkçe ? "kayıt bulundu." : "record found.";
            RecordsFound = language == Languages.Türkçe ? "kayıt bulundu." : "records found.";
            RecordNotFound = language == Languages.Türkçe ? "Kayıt bulunamadı!" : "Record not found!";
        }
    }
}
