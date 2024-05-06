namespace Mps.Application.Abstractions.Authentication
{
    public interface IAuthenticationService
    {
        Task<string> RegisterAsync(string email, string password, CancellationToken cancellationToken);
        Task DeleteAccountAsync(string email, CancellationToken cancellationToken);
        Task<string> LoginAsync(string email, string password, CancellationToken cancellationToken);
    }
}
