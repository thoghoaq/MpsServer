using Mps.Application.Abstractions.Authentication;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Mps.Infrastructure.Dependencies.Firebase.Authentication
{
    public class JwtProvider(HttpClient httpClient) : IJwtProvider
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<string> GenerateTokenAsync(string email, string password, CancellationToken cancellationToken)
        {
            var request = new { email, password, returnSecureToken = true };
            var response = await _httpClient.PostAsJsonAsync("", request, cancellationToken);
            var authToken = await response.Content.ReadFromJsonAsync<AuthToken>(cancellationToken: cancellationToken);
            return authToken?.IdToken ?? "";
        }

        public class AuthToken
        {
            [JsonPropertyName("kind")]
            public string? Kind { get; set; }
            [JsonPropertyName("localId")]
            public string? LocalId { get; set; }
            [JsonPropertyName("email")]
            public string? Email { get; set; }
            [JsonPropertyName("displayName")]
            public string? DisplayName { get; set; }
            [JsonPropertyName("idToken")]
            public string? IdToken { get; set; }
            [JsonPropertyName("registered")]
            public bool? Registered { get; set; }
            [JsonPropertyName("refreshToken")]
            public string? RefreshToken { get; set; }
            [JsonPropertyName("expiresIn")]
            public string? ExpiresIn { get; set; }
            [JsonPropertyName("federatedId")]
            public string? FederatedId { get; set; }

        }
    }
}
