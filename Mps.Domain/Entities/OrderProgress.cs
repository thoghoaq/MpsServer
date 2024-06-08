namespace Mps.Domain.Entities
{
    public class OrderProgress
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int OrderId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
