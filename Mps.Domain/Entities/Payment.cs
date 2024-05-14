namespace Mps.Domain.Entities
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public string? PaymentContent { get; set; }
        public string? PaymentCurrency { get; set; }
        public int? PaymentRefId { get; set; }
        public decimal RequiredAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string? PaymentLanguage { get; set; }
        public int? MerchantId { get; set; }
        public string? PaymentDestinationId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
