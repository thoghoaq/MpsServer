namespace Mps.Domain.Entities
{
    public class Product
    {
        public Product()
        {
            Images = [];
        }

        public int Id { get; set; }
        public required string Name { get; set; }
        public required decimal Price { get; set; }
        public required int Stock { get; set; }
        public required string ProductCode { get; set; }
        public required string ProductSKU { get; set; }
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public int? BrandId { get; set; }
        public required int ShopId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ProductCategory? Category { get; set; }
        public virtual ProductBrand? Brand { get; set; }
        public virtual Shop? Shop { get; set; }
        public virtual ICollection<ProductImage> Images { get; set; }
    }
}
