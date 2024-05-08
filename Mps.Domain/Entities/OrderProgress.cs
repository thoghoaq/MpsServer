namespace Mps.Domain.Entities
{
    public class OrderProgress
    {
        public int OrderProgressId { get; set; }
        public required string OrderProgressName { get; set; }
        public int OrderId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
