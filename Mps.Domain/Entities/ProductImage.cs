namespace Mps.Domain.Entities
{
    public class ProductImage
    {
        public int ProductImageId { get; set; }
        public required string ImagePath { get; set; }
        public int ProductId { get; set; }

        public Product? Product { get; set; }
    }
}
