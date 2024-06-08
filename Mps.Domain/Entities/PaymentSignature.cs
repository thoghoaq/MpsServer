﻿namespace Mps.Domain.Entities
{
    public class PaymentSignature
    {
        public int Id { get; set; }
        public string? SignValue { get; set; }
        public DateTime SignDate { get; set; }
        public int? SignOwn { get; set; }
        public bool IsValid { get; set; }
        public int? PaymentId { get; set; }

        public virtual Payment? Payment { get; set; }
    }
}
