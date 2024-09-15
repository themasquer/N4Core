// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using N4Core.Accounts.Entities;
using N4Core.Accounts.Enums;
using N4Core.Accounts.Models;
using N4Core.Accounts.Services.Bases;
using N4Core.Cookie.Utils.Bases;
using N4Core.Culture;
using N4Core.Culture.Utils.Bases;
using N4Core.Settings.Bases;
using N4Core.Views.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace N4Web.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        private readonly AccountServiceBase _accountService;
        private readonly AppSettingsBase _appSettings;
        private readonly CultureUtilBase _cultureUtil;
        private readonly CookieUtilBase _cookieUtil;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,

            AccountServiceBase accountService,
            AppSettingsBase appSettings,
            CultureUtilBase cultureUtil,
            CookieUtilBase cookieUtil)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;

            _accountService = accountService;
            _appSettings = appSettings;
            _cultureUtil = cultureUtil;
            _cookieUtil = cookieUtil;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [EmailAddress(ErrorMessage = "{0} is not in correct e-mail format!;{0} uygun e-posta formatında değildir!")]
            [StringLength(200, MinimumLength = 7, ErrorMessage = "{0} must have minimum {2} maximum {1} characters!;{0} en az {2} en çok {1} karakter olmalıdır!")]
            [Display(Name = "{E-Mail;E-Posta}")]
            public string Email { get; set; }

            [Required(ErrorMessage = "{0} is required!;{0} zorunludur!")]
            [StringLength(100, MinimumLength = 4, ErrorMessage = "{0} must have minimum {2} maximum {1} characters!;{0} en az {2} en çok {1} karakter olmalıdır!")]
            [Display(Name = "{* User Name;* Kullanıcı Adı}")]
            public string UserName { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Required(ErrorMessage = "{0} is required!;{0} zorunludur!")]
            [StringLength(20, MinimumLength = 4, ErrorMessage = "{0} must have minimum {2} maximum {1} characters!;{0} en az {2} en çok {1} karakter olmalıdır!")]
            [Display(Name = "{* Password;* Şifre}")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Required(ErrorMessage = "{0} is required!;{0} zorunludur!")]
            [StringLength(20, MinimumLength = 4, ErrorMessage = "{0} must have minimum {2} maximum {1} characters!;{0} en az {2} en çok {1} karakter olmalıdır!")]
            [Compare("Password", ErrorMessage = "Password and Confirm Password must be the same!;Şifre ile Şifre Onay aynı olmalıdır!")]
            [Display(Name = "{* Confirm Password;* Şifre Onay}")]
            public string ConfirmPassword { get; set; }

            public List<string> SectionNames { get; set; }
            public List<AccountSectionModel> Sections { get; set; }
            public bool ShowEmail { get; set; }
            public Languages Language { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = Url.GetReturnRoute(returnUrl);
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            Input = new InputModel()
            {
                Sections = await _accountService.GetSections(),
                ShowEmail = _appSettings.ShowEmailOnRegister,
                Language = _cultureUtil.GetLanguage(_cookieUtil.Get("lang"))
            };
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.UserName, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Roles.User.ToString().ToLower());

                    if (Input.SectionNames is not null && Input.SectionNames.Any())
                    {
                        foreach (var sectionName in Input.SectionNames)
                        {
                            await _userManager.AddClaimAsync(user, new Claim(nameof(AccountSection), sectionName));
                        }
                    }

                    _logger.LogInformation("User created a new account with password.");

                    if (!string.IsNullOrWhiteSpace(Input.Email))
                    {
                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code, returnUrl = Url.GetReturnRoute(returnUrl) },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, Input.Language == Languages.English ? "Confirm your e-mail" : "E-postanızı onaylayın",
                            Input.Language == Languages.English ? 
                                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>." :
                                $"Lütfen hesabınızı <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>buraya tıklayarak</a> onaylayın.");

                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = Url.GetReturnRoute(returnUrl) });
                        }
                        else
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(Url.GetReturnRoute(returnUrl));
                        }
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(Url.GetReturnRoute(returnUrl));
                    }

                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            Input = new InputModel()
            {
                Sections = await _accountService.GetSections(),
                ShowEmail = _appSettings.ShowEmailOnRegister,
                Language = _cultureUtil.GetLanguage(_cookieUtil.Get("lang"))
            };
            return Page();
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}
