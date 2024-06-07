namespace Mps.Domain.Entities
{
    public class Staff
    {
        public int UserId { get; set; }
        public string? IdentityCard { get; set; }
        public string? IdentityCardFrontPath { get; set; }
        public string? IdentityCardBackPath { get; set; }
        public string? Address { get; set; }
        public string? CertificatePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
