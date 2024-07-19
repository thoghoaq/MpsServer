namespace Mps.Domain.Entities
{
    public class UserNotification
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Body { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public int UserId { get; set; }
        public string? Data { get; set; }

        public virtual User? User { get; set; }
    }
}
