namespace Mps.Domain.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; }
        public required string IdentityId { get; set; }
        public string? AvatarPath { get; set; }
        public string? PhoneNumber { get; set; }
        public required bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
