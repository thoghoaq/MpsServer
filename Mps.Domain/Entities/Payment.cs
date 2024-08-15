namespace Mps.Domain.Entities
{
    public class Payment
    {
        public Payment()
        {
            PaymentRefs = [];
        }

        public int Id { get; set; }
        public string? Content { get; set; }
        public string? Currency { get; set; }
        public decimal RequiredAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string? Language { get; set; }
        public string? PaymentDestinationId { get; set; }
        public int PaymentStatusId { get; set; }
        public int? PaymentSignatureId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? TransactionNo { get; set; }
        public string? OrderInfo { get; set; }

        public virtual ICollection<PaymentRef> PaymentRefs { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public virtual PaymentSignature? PaymentSignature { get; set; }
    }
}
