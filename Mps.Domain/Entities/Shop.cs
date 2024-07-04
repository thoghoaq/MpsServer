namespace Mps.Domain.Entities
{
    public class Shop
    {
        public Shop()
        {
            Payouts = [];
        }

        public int Id { get; set; }
        public int ShopOwnerId { get; set; }
        public required string ShopName { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public required string District { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? Description { get; set; }
        public string? Avatar { get; set; }
        public string? Cover { get; set; }
        public bool IsActive { get; set; }

        public string? PayPalAccount { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Payout> Payouts { get; set; }
    }
}
