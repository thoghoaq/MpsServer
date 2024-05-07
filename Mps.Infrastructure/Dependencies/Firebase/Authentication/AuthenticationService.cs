using FirebaseAdmin.Auth;
using Mps.Application.Abstractions.Authentication;

namespace Mps.Infrastructure.Dependencies.Firebase.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        public async Task<string> RegisterAsync(string email, string password, CancellationToken cancellationToken)
        {
            var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs
            {
                Email = email,
                Password = password
            }, cancellationToken);
            return userRecord.Uid;
        }

        public async Task DeleteAccountAsync(string email, CancellationToken cancellationToken)
        {
            await FirebaseAuth.DefaultInstance.DeleteUserAsync(email, cancellationToken);
        }

        public Task<string> LoginAsync(string email, string password, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
