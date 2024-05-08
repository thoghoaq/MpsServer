namespace Mps.Domain.Entities
{
    public class Product
    {
        public int ProductId { get; set; }
        public required string ProductName { get; set; }
        public required decimal Price { get; set; }
        public required int Stock { get; set; }
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public required int ShopId { get; set; }

        public virtual ProductCategory? Category { get; set; }
        public virtual ProductBrand? Brand { get; set; }
        public virtual Shop? Shop { get; set; }
        public virtual ICollection<ProductImage>? Images { get; set; }
    }
}
