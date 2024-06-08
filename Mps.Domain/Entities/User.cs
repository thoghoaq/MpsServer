namespace Mps.Domain.Entities
{
    public class User
    {
        public User()
        {
            UserDevices = [];
        }

        public int Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; }
        public required string IdentityId { get; set; }
        public string? AvatarPath { get; set; }
        public string? PhoneNumber { get; set; }
        public required bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<UserDevice> UserDevices { get; set; }
        public virtual Customer? Customer { get; set; }
        public virtual ShopOwner? ShopOwner { get; set; }
        public virtual Staff? Staff { get; set; }
    }
}
