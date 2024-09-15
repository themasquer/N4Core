using DotnetGeminiSDK;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using N4Core.Accounts.Configs;
using N4Core.Accounts.Services;
using N4Core.Accounts.Services.Bases;
using N4Core.Accounts.Utils;
using N4Core.Accounts.Utils.Bases;
using N4Core.ArtificialIntelligence.Gemini.Settings;
using N4Core.ArtificialIntelligence.Gemini.Utils;
using N4Core.ArtificialIntelligence.Utils.Bases;
using N4Core.Cookie.Utils;
using N4Core.Cookie.Utils.Bases;
using N4Core.Culture.Utils;
using N4Core.Culture.Utils.Bases;
using N4Core.Expiration.Models;
using N4Core.Files.Services;
using N4Core.Files.Services.Bases;
using N4Core.Files.Utils;
using N4Core.Files.Utils.Bases;
using N4Core.JsonWebToken.Settings;
using N4Core.JsonWebToken.Utils;
using N4Core.JsonWebToken.Utils.Bases;
using N4Core.Mappers.Utils.Bases;
using N4Core.Mappers.Utils;
using N4Core.Reflection.Utils;
using N4Core.Reflection.Utils.Bases;
using N4Core.Reports.Utils;
using N4Core.Reports.Utils.Bases;
using N4Core.Repositories;
using N4Core.Repositories.Bases;
using N4Core.Session.Utils;
using N4Core.Session.Utils.Bases;
using N4Core.Settings.Bases;
using System.Text;

namespace N4Core
{
    public static class Core
    {
        public static WebApplicationBuilder Configure(this WebApplicationBuilder builder, bool useIdentity = false)
        {
            #region App Settings
            var appSettingsBase = new AppSettingsBase(builder.Configuration, builder.Environment);
            appSettingsBase.Bind(useIdentity);
            #endregion

            #region API JWT Settings
            var jwtSettings = new JwtSettings(builder.Configuration, builder.Environment);
            jwtSettings.Bind();
            #endregion

            #region Authentication
            if (!appSettingsBase.UseIdentity)
            {
                var accountServiceConfig = new AccountServiceConfig();
                builder.Services.AddAuthentication(accountServiceConfig.AuthenticationScheme)
                    .AddCookie(accountServiceConfig.AuthenticationScheme, config =>
                    {
                        config.LoginPath = "/Account/AccountLogin";
                        config.LogoutPath = "/Account/AccountLogout";
                        config.AccessDeniedPath = "/Account/AccountAccessDenied";
                        config.SlidingExpiration = true;
                        config.ExpireTimeSpan = TimeSpan.FromMinutes(appSettingsBase.AuthenticationCookieExpirationInMinutes);
                    })
                #region API JWT
                .AddJwtBearer(config =>
                {
                    config.TokenValidationParameters = new TokenValidationParameters()
                    {
                        IssuerSigningKey = jwtSettings.SigningKey,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true
                    };
                });
            }
			    #endregion
			#endregion

			#region API CORS
			builder.Services.AddCors(options =>
			{
				options.AddDefaultPolicy(builder => builder
					.AllowAnyOrigin()
					.AllowAnyHeader()
					.AllowAnyMethod());
			});
			#endregion

			#region Session
			builder.Services.AddSession(config =>
			{
				config.IdleTimeout = new ExpireModel(0, appSettingsBase.SessionExpirationInMinutes).TimeSpan;
			});
			#endregion

			// Inversion of Control for HttpContext:
			builder.Services.AddHttpContextAccessor();

            // Inversion of Control for utilities:
            builder.Services.AddSingleton<SessionUtilBase, SessionUtil>();
            builder.Services.AddSingleton<CookieUtilBase, CookieUtil>();
            builder.Services.AddSingleton<AccountUtilBase, AccountUtil>();
            builder.Services.AddSingleton<CultureUtilBase, CultureUtil>();
            builder.Services.AddSingleton<ReflectionUtilBase, ReflectionUtil>();
            builder.Services.AddSingleton(typeof(MapperUtilBase<,,>), typeof(MapperUtil<,,>));
            builder.Services.AddSingleton<FileUtilBase, FileUtil>();
            builder.Services.AddSingleton<ReportUtilBase, ReportUtil>();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            #region API JWT
            builder.Services.AddSingleton<JwtUtilBase, JwtUtil>();
            #endregion

            // Inversion of Control for repositories:
            builder.Services.AddScoped(typeof(RepoBase<>), typeof(Repo<>));
            builder.Services.AddScoped<UnitOfWorkBase, UnitOfWork>();

            // Inversion of Control for Account Service:
            builder.Services.AddScoped<AccountServiceBase, AccountService>();
            // Inversion of Control for File Browser Service:
            builder.Services.AddScoped<FileBrowserServiceBase, FileBrowserService>();

            #region API ModelState
            builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
            #endregion

            #region AI
            builder.Services.AddSingleton<IAiUtil, GeminiUtil>();
            #region Google Gemini
            var geminiSettings = new GeminiSettings(builder.Configuration, builder.Environment);
            geminiSettings.Bind();
            builder.Services.AddGeminiClient(config =>
            {
                config.ApiKey = geminiSettings.ApiKey;
                config.TextBaseUrl = geminiSettings.ApiTextUrl;
                config.ImageBaseUrl = geminiSettings.ApiImageUrl;
            });
            #endregion
            #endregion

            return builder;
        }

        public static void Configure(this WebApplication application, bool useIdentity = false)
        {
            #region App Settings
            var appSettingsBase = application.Services.GetRequiredService<AppSettingsBase>();
            appSettingsBase.Bind(useIdentity, application.Environment.IsDevelopment());
            #endregion

            #region Authentication
            application.UseAuthentication();
            if (appSettingsBase.UseIdentity)
                application.MapRazorPages();
            #endregion

            #region API CORS
            application.UseCors();
            #endregion

            #region Session
            application.UseSession();
            #endregion
        }
    }
}
