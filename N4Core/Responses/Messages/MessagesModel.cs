using N4Core.Culture;

namespace N4Core.Responses.Messages
{
    public class MessagesModel : CrudMessagesModel
    {
        public string InvalidFileExtensionOrFileLength { get; set; }
        public string FileDeletedSuccessfully { get; set; }
        public string FileOperationsNotConfigured { get; set; }
        public string Report { get; set; }

        public MessagesModel(Languages language = Languages.English) : base(language)
        {
            InvalidFileExtensionOrFileLength = OperationFailed + " " + (language == Languages.Türkçe ? "Geçersiz dosya uzantısı veya boyutu!" : "Invalid file extension or length!");
            FileDeletedSuccessfully = language == Languages.Türkçe ? "Dosya başarıyla silindi." : "File deleted successfully.";
            FileOperationsNotConfigured = OperationFailed + " " + (language == Languages.Türkçe ? "Dosya işlemleri konfigüre edilmemiştir!" : "File operations is not configured!");
            Report = language == Languages.Türkçe ? "Rapor" : "Report";
        }
    }
}
