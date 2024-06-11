using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mps.Application.Abstractions.Authentication;
using Mps.Application.Abstractions.Email;
using Mps.Application.Abstractions.Excel;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Abstractions.Messaging;
using Mps.Application.Abstractions.Payment;
using Mps.Application.Abstractions.Storage;
using Mps.Infrastructure.Dependencies.Email;
using Mps.Infrastructure.Dependencies.Excel;
using Mps.Infrastructure.Dependencies.Firebase.Authentication;
using Mps.Infrastructure.Dependencies.Firebase.Messaging;
using Mps.Infrastructure.Dependencies.Firebase.Storage;
using Mps.Infrastructure.Dependencies.Localization;
using Mps.Infrastructure.Dependencies.LoggedUser;
using Mps.Infrastructure.Dependencies.VnPay;

namespace Mps.Infrastructure
{
    public static class DependencyInjection
    {
        private static readonly string[] ConfigureOptions = ["en-US", "vi-VN"];

        [Obsolete("Some services will be obsolete")]
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromJson(configuration.GetSection("FirebaseAdminSdk").Value)
            });

            services.AddSingleton<IAuthenticationService, AuthenticationService>();

            services.AddTransient<ILoggedUser, LoggedUser>();

            services.AddHttpContextAccessor();

            services.AddHttpClient<IJwtProvider, JwtProvider>(client =>
            {
                client.BaseAddress = new Uri(configuration.GetSection("Authentication:TokenUri").Value!);
            });

            services.AddAuthentication()
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = configuration.GetSection("Authentication:ValidIssuer").Value;
                    options.Audience = configuration.GetSection("Authentication:Audience").Value;
                    options.TokenValidationParameters.ValidIssuer = configuration.GetSection("Authentication:ValidIssuer").Value;
                });

            services.AddHangfireServer();
            services.AddHangfire(config =>
            {
                config.UsePostgreSqlStorage(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddLocalization(options => options.ResourcesPath = "");

            services.AddTransient<IAppLocalizer, AppLocalizer>();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = ConfigureOptions;
                options.SetDefaultCulture(supportedCultures[0]);
                options.AddSupportedCultures(supportedCultures);
                options.AddSupportedUICultures(supportedCultures);
                options.ApplyCurrentCultureToResponseHeaders = true;
            });

            services.AddTransient<IVnPayService, VnPayService>();

            services.AddTransient<INotificationService, FirebaseNotificationService>();

            services.AddTransient<IStorageService, FirebaseStorageService>();

            services.AddTransient<IExcelService, ExcelService>();

            services.AddTransient<IEmailService, EmailService>();
        }
    }
}
