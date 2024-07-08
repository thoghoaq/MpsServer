namespace Mps.Domain.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
