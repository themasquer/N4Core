using N4Core.Culture;

namespace N4Core.Accounts.Messages
{
    public class AccountMessagesModel
    {
        public string UserNotFound { get; set; }
        public string UserFound { get; set; }
        public string UserRegistered { get; set; }
        public string RoleNotFound { get; set; }
        public string UserFoundWithSameUserName { get; set; }
        public string UserAccessDenied { get; set; }

        public AccountMessagesModel(Languages language = Languages.English)
        {
            UserNotFound = language == Languages.Türkçe ? "Kullanıcı bulunamadı!" : "User not found!";
            UserFound = language == Languages.Türkçe ? "Kullanıcı bulundu." : "User found.";
            UserRegistered = language == Languages.Türkçe ? "Kullanıcı kaydedildi." : "User registered.";
            RoleNotFound = language == Languages.Türkçe ? "Rol bulunamadı!" : "Role not found!";
            UserFoundWithSameUserName = language == Languages.Türkçe ? "Aynı kullanıcı adına sahip kullanıcı bulunmaktadır!" : "User with the same user name exists!";
            UserAccessDenied = language == Languages.Türkçe ? "Bu kaynağa erişiminiz bulunmamaktadır!" : "You do not have access to this resource!";
        }
    }
}
