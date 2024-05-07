using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mps.Application.Abstractions.Authentication;
using Mps.Infrastructure.Dependencies.Firebase.Authentication;
using Mps.Infrastructure.Dependencies.LoggedUser;

namespace Mps.Infrastructure
{
    public static class DependencyInjection
    {
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
        }
    }
}
