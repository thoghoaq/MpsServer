namespace Mps.Application.Abstractions.Authentication
{
    public interface IJwtProvider
    {
        Task<string> GenerateTokenAsync(string email, string password, CancellationToken cancellationToken);
    }
}
