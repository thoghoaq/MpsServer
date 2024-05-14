using Mps.Infrastructure.Dependencies.VnPay.Helpers;

namespace Mps.Infrastructure.Dependencies.VnPay.Models
{
    public class VnPayPayRequest
    {
        public decimal? Vnp_Amount { get; set; }
        public string? Vnp_Command { get; set; }
        public string? Vnp_CreateDate { get; set; }
        public string? Vnp_CurrCode { get; set; }
        public string? Vnp_IpAddr { get; set; }
        public string? Vnp_Locale { get; set; }
        public string? Vnp_OrderInfo { get; set; }
        public string? Vnp_OrderType { get; set; }
        public string? Vnp_ReturnUrl { get; set; }
        public string? Vnp_ExpireDate { get; set; }
        public string? Vnp_TxnRef { get; set; }
        public string? Vnp_Version { get; set; }
        public string? Vnp_TmnCode { get; set; }

        public SortedList<string, string> RequestData = new(new VnPayCompare());
    }
}
