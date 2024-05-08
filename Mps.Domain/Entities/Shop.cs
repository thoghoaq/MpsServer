namespace Mps.Domain.Entities
{
    public class Shop
    {
        public int ShopId { get; set; }
        public int SupplierId { get; set; }
        public required string ShopName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Supplier? Supplier { get; set; }
    }
}
