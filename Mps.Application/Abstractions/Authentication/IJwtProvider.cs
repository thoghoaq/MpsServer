using Mps.Domain.Dtos;

namespace Mps.Application.Abstractions.Authentication
{
    public interface IJwtProvider
    {
        Task<AuthToken?> GenerateTokenAsync(string email, string password, CancellationToken cancellationToken);
        Task<AuthRefreshToken?> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
    }
}
