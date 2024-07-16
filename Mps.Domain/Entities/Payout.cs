namespace Mps.Domain.Entities
{
    public class Payout
    {
        public int Id { get; set; }
        public int ShopId { get; set; }
        public decimal? ExpectAmount { get; set; }
        public decimal? Amount { get; set; }
        public string? Currency { get; set; }
        public DateTime MonthToDate { get; set; }
        public int PayoutDate { get; set; }
        public bool IsConfirmed { get; set; }
        public string? BatchId { get; set; }
        public int PayoutStatusId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual PayoutStatus? PayoutStatus { get; set; }
    }
}
