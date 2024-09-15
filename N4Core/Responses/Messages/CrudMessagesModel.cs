using N4Core.Culture;
using N4Core.Records.Messages;

namespace N4Core.Responses.Messages
{
    public class CrudMessagesModel : RecordMessagesModel
    {
        public string CreatedSuccessfully { get; set; }
        public string UpdatedSuccessfully { get; set; }
        public string DeletedSuccessfully { get; set; }
        public string OperationFailed { get; set; }
        public string RecordWithSameNameExists { get; set; }
        public string RelatedRecordsFound { get; set; }
        public string RelatedRecordsDeletedSuccessfully { get; set; }
        public string RequestMethodNotConfigured { get; set; }

        public CrudMessagesModel(Languages language = Languages.English) : base(language)
        {
            CreatedSuccessfully = language == Languages.Türkçe ? "Kayıt başarıyla eklendi." : "Record added successfully.";
            UpdatedSuccessfully = language == Languages.Türkçe ? "Kayıt başarıyla güncellendi." : "Record updated successfully.";
            DeletedSuccessfully = language == Languages.Türkçe ? "Kayıt başarıyla silindi." : "Record deleted successfully.";
            OperationFailed = language == Languages.Türkçe ? "İşlem gerçekleştirilemedi!" : "Operation failed!";
            RecordWithSameNameExists = OperationFailed + " " + (language == Languages.Türkçe ? "Aynı ada sahip kayıt bulundu!" : "Record found with the same name!");
            RelatedRecordsFound = OperationFailed + " " + (language == Languages.Türkçe ? "İlişkili kayıtlar bulundu!" : "Related records found!");
            RelatedRecordsDeletedSuccessfully = language == Languages.Türkçe ? "İlişkili kayıtlar başarıyla silindi." : "Related records deleted successfully.";
            RequestMethodNotConfigured = OperationFailed + " " + (language == Languages.Türkçe ? "İstek methodu konfigüre edilmemiştir!" : "Request method is not configured!");
        }
    }
}
