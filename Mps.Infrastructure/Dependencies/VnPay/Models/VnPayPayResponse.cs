using Mps.Infrastructure.Dependencies.VnPay.Helpers;

namespace Mps.Infrastructure.Dependencies.VnPay.Models
{
    public class VnPayPayResponse
    {
        public string? Vnp_TmnCode { get; set; }
        public string? Vnp_BankCode { get; set; }
        public string? Vnp_BankTranNo { get; set; }
        public string? Vnp_CardType { get; set; }
        public string? Vnp_OrderInfo { get; set; }
        public string? Vnp_TransactionNo { get; set; }
        public string? Vnp_TransactionStatus { get; set; }
        public string? Vnp_TxnRef { get; set; }
        public string? Vnp_SecureHash { get; set; }
        public int? Vnp_Amount { get; set; }
        public string? Vnp_PayDate { get; set; }
        public string? Vnp_ResponseCode { get; set; }

        public SortedList<string, string> ResponseData = new(new VnPayCompare());
    }
}
