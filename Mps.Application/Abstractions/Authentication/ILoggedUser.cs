namespace Mps.Application.Abstractions.Authentication
{
    public interface ILoggedUser
    {
        public int UserId { get; }
        public string FullName { get; }
        public string Email { get; }
        public IEnumerable<string> Roles { get; }
        public string IdentityId { get; }
    }
}
