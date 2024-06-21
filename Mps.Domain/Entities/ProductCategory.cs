namespace Mps.Domain.Entities
{
    public class ProductCategory
    {
        public ProductCategory()
        {
            Children = [];
            IsDeleted = false;
        }

        public int Id { get; set; }
        public required string Name { get; set; }
        public int? ParentId { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<ProductCategory> Children { get; set; }
    }
}
