namespace Mps.Domain.Entities
{
    public class Shop
    {
        public int ShopId { get; set; }
        public int ShopOwnerId { get; set; }
        public required string ShopName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ShopOwner? ShopOwner { get; set; }
    }
}
