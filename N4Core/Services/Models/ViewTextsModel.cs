using N4Core.Culture;

namespace N4Core.Services.Models
{
    public class ViewTextsModel
    {
        public Languages Language { get; }
        public string List { get; set; }
        public string Create { get; set; }
        public string Save { get; set; }
        public string Clear { get; set; }
        public string BackToList { get; set; }
        public string Details { get; set; }
        public string Edit { get; set; }
        public string Delete { get; set; }
        public string DeleteYes { get; set; }
        public string DeleteNo { get; set; }
        public string DeleteFile { get; set; }
        public string DeleteQuestion { get; set; }
        public string Export { get; set; }
        public string Select { get; set; }
        public string DownloadFile { get; set; }
        public string Warning { get; set; }
        public string Error { get; set; }
        public string Filter { get; set; }
        public string Search { get; set; }
        public string PageNumber { get; set; }
        public string RecordsPerPageCount { get; set; }
        public string OrderExpression { get; set; }
        public string OrderDirectionDescending { get; set; }
        public string Login { get; set; }
        public string Logout { get; set; }
        public string Register { get; set; }
        public string ShowHidePassword { get; set; }
        public string ListView { get; set; }
        public string ViewList { get; set; }
        public string ViewCards { get; set; }

        public ViewTextsModel(Languages language = Languages.English)
        {
            Language = language;
            List = Language == Languages.English ? "List" : "Liste";
            Create = Language == Languages.English ? "New" : "Yeni";
            Save = Language == Languages.English ? "Save" : "Kaydet";
            Clear = Language == Languages.English ? "Clear" : "Temizle";
            BackToList = Language == Languages.English ? "Back to List" : "Listeye Dön";
            Details = Language == Languages.English ? "Details" : "Detay";
            Edit = Language == Languages.English ? "Edit" : "Düzenle";
            Delete = Language == Languages.English ? "Delete" : "Sil";
            DeleteYes = Language == Languages.English ? "Yes" : "Evet";
            DeleteNo = Language == Languages.English ? "No" : "Hayır";
            DeleteFile = Language == Languages.English ? "Delete File" : "Dosyayı Sil";
            DeleteQuestion = Language == Languages.English ? "Are you sure you want to delete this record?" : "Bu kaydı silmek istediğinize emin misiniz?";
            Export = Language == Languages.English ? "Export to Excel" : "Excel'e Aktar";
            Select = Language == Languages.English ? "Select" : "Seçiniz";
            DownloadFile = Language == Languages.English ? "Download File" : "Dosyayı İndir";
            Warning = Language == Languages.English ? "Warning!" : "Uyarı!";
            Error = Language == Languages.English ? "Error!" : "Hata!";
            Filter = Language == Languages.English ? "Filter" : "Filtre";
            Search = Language == Languages.English ? "Search" : "Ara";
            PageNumber = Language == Languages.English ? "Page" : "Sayfa";
            RecordsPerPageCount = Language == Languages.English ? "Count" : "Sayı";
            OrderExpression = Language == Languages.English ? "Order" : "Sıra";
            OrderDirectionDescending = Language == Languages.English ? "Descending" : "Azalan";
            Login = Language == Languages.English ? "Login" : "Giriş";
            Logout = Language == Languages.English ? "Logout" : "Çıkış";
            Register = Language == Languages.English ? "Register" : "Kayıt";
            ShowHidePassword = Language == Languages.English ? "Show / Hide Password" : "Şifre Göster / Gizle";
            ListView = Language == Languages.English ? "View" : "Görünüm";
            ViewList = Language == Languages.English ? "List" : "Liste";
            ViewCards = Language == Languages.English ? "Cards" : "Kartlar";
        }
    }
}
