using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace N4Core.Accounts.Models
{
    public class AccountRegisterModel
    {
        [Required(ErrorMessage = "{0} is required!;{0} zorunludur!")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "{0} must have minimum {2} maximum {1} characters!;{0} en az {2} en çok {1} karakter olmalıdır!")]
        [DisplayName("{* User Name;* Kullanıcı Adı}")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "{0} is required!;{0} zorunludur!")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "{0} must have minimum {2} maximum {1} characters!;{0} en az {2} en çok {1} karakter olmalıdır!")]
        [DisplayName("{* Password;* Şifre}")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "{0} is required!;{0} zorunludur!")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "{0} must have minimum {2} maximum {1} characters!;{0} en az {2} en çok {1} karakter olmalıdır!")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password must be the same!;Şifre ile Şifre Onay aynı olmalıdır!")]
        [DisplayName("{* Confirm Password;* Şifre Onay}")]
        public string? ConfirmPassword { get; set; }

		[StringLength(200, MinimumLength = 7, ErrorMessage = "{0} must have minimum {2} maximum {1} characters!;{0} en az {2} en çok {1} karakter olmalıdır!")]
		[DisplayName("{E-Mail;E-Posta}")]
        [EmailAddress(ErrorMessage = "{0} is not in correct e-mail format!;{0} uygun e-posta formatında değildir!")]
        public string? EMail { get; set; }

		public string? ReturnUrl { get; set; }
        public bool ShowEmail { get; set; }

		[DisplayName("{Groups;Gruplar}")]
		public List<int>? GroupIds { get; set; }
    }
}
