namespace Mps.Domain.Entities
{
    public class ProductImage
    {
        public int Id { get; set; }
        public required string ImagePath { get; set; }
        public int ProductId { get; set; }

        public virtual Product? Product { get; set; }
    }
}
