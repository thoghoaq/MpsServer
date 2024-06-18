namespace Mps.Domain.Entities
{
    public class PaymentRef
    {
        public int Id { get; set; }
        public int PaymentId { get; set; }
        public int? RefId { get; set; }
        public int? MerchantId { get; set; }
    }
}
