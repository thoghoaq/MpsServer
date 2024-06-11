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

        public async Task DeleteAccountAsync(string uid, CancellationToken cancellationToken)
        {
            await FirebaseAuth.DefaultInstance.DeleteUserAsync(uid, cancellationToken);
        }

        public Task<string> LoginAsync(string email, string password, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GeneratePasswordResetLinkAsync(string email)
        {
            return await FirebaseAuth.DefaultInstance.GeneratePasswordResetLinkAsync(email);
        }
    }
}
