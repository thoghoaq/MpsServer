namespace Mps.Domain.Entities
{
    public class ShopOwner
    {
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? IdentityFrontImage { get; set; }
        public string? IdentityBackImage { get; set; }
        public string? TaxNumber { get; set; }

        public virtual ICollection<Shop>? Shops { get; set; }
    }
}
