namespace Mps.Domain.Entities
{
    public class Order
    {
        public Order()
        {
            OrderDetails = [];
            Progresses = [];
        }

        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ShopId { get; set; }
        public string? CustomerName { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public int OrderStatusId { get; set; }
        public int PaymentStatusId { get; set; }
        public int PaymentMethodId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? Discount { get; set; }
        public string? Note { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual Shop? Shop { get; set; }
        public virtual OrderStatus? OrderStatus { get; set; }
        public virtual PaymentStatus? PaymentStatus { get; set; }
        public virtual PaymentMethod? PaymentMethod { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<OrderProgress> Progresses { get; set; }
    }
}
