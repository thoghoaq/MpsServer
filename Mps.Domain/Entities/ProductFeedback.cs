namespace Mps.Domain.Entities
{
    public class ProductFeedback
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? Feedback { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
