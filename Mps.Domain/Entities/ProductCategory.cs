namespace Mps.Domain.Entities
{
    public class ProductCategory
    {
        public ProductCategory()
        {
            Children = [];
        }

        public int Id { get; set; }
        public required string Name { get; set; }
        public int? ParentId { get; set; }

        public virtual ICollection<ProductCategory> Children { get; set; }
    }
}
