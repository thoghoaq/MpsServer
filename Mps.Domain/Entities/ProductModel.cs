namespace Mps.Domain.Entities
{
    public class ProductModel
    {
        public int Id { get; set; }
        public int BrandId { get; set; }
        public required string Name { get; set; }
        public string? Cc { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ProductBrand? Brand { get; set; }
    }
}
