namespace Mps.Domain.Entities
{
    public class PaymentMethod
    {
        public int PaymentMethodId { get; set; }
        public required string PaymentMethodName { get; set; }
    }
}
