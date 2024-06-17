using Microsoft.Extensions.Configuration;
using Mps.Application.Abstractions.Authentication;
using Mps.Domain.Dtos;
using System.Net.Http.Json;

namespace Mps.Infrastructure.Dependencies.Firebase.Authentication
{
    public class JwtProvider(HttpClient httpClient, IConfiguration configuration) : IJwtProvider
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly string _tokenUrl = configuration.GetSection("Authentication:TokenUri").Value!;
        private readonly string _refreshUrl = configuration.GetSection("Authentication:RefreshUri").Value!;

        public async Task<AuthToken?> GenerateTokenAsync(string email, string password, CancellationToken cancellationToken)
        {
            var request = new { email, password, returnSecureToken = true };
            var response = await _httpClient.PostAsJsonAsync(new Uri(_tokenUrl), request, cancellationToken);
            var authToken = await response.Content.ReadFromJsonAsync<AuthToken>(cancellationToken: cancellationToken);
            return authToken;
        }

        public async Task<AuthRefreshToken?> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            var request = new { grant_type = "refresh_token", refresh_token = refreshToken };
            var response = await _httpClient.PostAsJsonAsync(new Uri(_refreshUrl), request, cancellationToken);
            var authRefreshToken = await response.Content.ReadFromJsonAsync<AuthRefreshToken>(cancellationToken: cancellationToken);
            return authRefreshToken;
        }
    }
}
