namespace Mps.Domain.Entities
{
    public class ShopOwner
    {
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual User? User { get; set; }
        public virtual ICollection<Shop>? Shops { get; set; }
    }
}
