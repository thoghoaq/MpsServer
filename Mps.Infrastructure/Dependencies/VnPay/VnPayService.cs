using Mps.Application.Abstractions.Payment;
using Mps.Infrastructure.Dependencies.VnPay.Helpers;
using System.Net;
using System.Text;

namespace Mps.Infrastructure.Dependencies.VnPay
{
    public class VnPayService : IVnPayService
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

        public SortedList<string, string> requestData = new(new VnPayCompare());

        public void MakeRequestData()
        {
            if (Vnp_Amount != null) requestData.Add("vnp_Amount", Vnp_Amount.ToString() ?? string.Empty);
            if (Vnp_Command != null) requestData.Add("vnp_Command", Vnp_Command);
            if (Vnp_CreateDate != null) requestData.Add("vnp_CreateDate", Vnp_CreateDate);
            if (Vnp_CurrCode != null) requestData.Add("vnp_CurrCode", Vnp_CurrCode);
            if (Vnp_IpAddr != null) requestData.Add("vnp_IpAddr", Vnp_IpAddr);
            if (Vnp_Locale != null) requestData.Add("vnp_Locale", Vnp_Locale);
            if (Vnp_OrderInfo != null) requestData.Add("vnp_OrderInfo", Vnp_OrderInfo);
            if (Vnp_OrderType != null) requestData.Add("vnp_OrderType", Vnp_OrderType);
            if (Vnp_ReturnUrl != null) requestData.Add("vnp_ReturnUrl", Vnp_ReturnUrl);
            if (Vnp_ExpireDate != null) requestData.Add("vnp_ExpireDate", Vnp_ExpireDate);
            if (Vnp_TxnRef != null) requestData.Add("vnp_TxnRef", Vnp_TxnRef);
            if (Vnp_Version != null) requestData.Add("vnp_Version", Vnp_Version);
            if (Vnp_TmnCode != null) requestData.Add("vnp_TmnCode", Vnp_TmnCode);
        }

        public string GetLink(string baseUrl, string secretKey)
        {
            MakeRequestData();
            StringBuilder data = new();
            foreach (var item in requestData)
            {
                if (!string.IsNullOrEmpty(item.Value))
                {
                    data.Append(WebUtility.UrlEncode(item.Key) + "=" + WebUtility.UrlEncode(item.Value) + "&");
                }
            }

            string result = baseUrl + "?" + data.ToString();
            var secureHash = HashHelper.HmacSHA512(secretKey, data.ToString().Remove(data.Length - 1, 1));
            return result += "vnp_SecureHash=" + secureHash;

        }

        public void CreateVnPayRequest(string version, string tmnCode, DateTime createDate, string ipAddress, decimal amount, string currCode, string orderType, string orderInfo, string returnUrl, string txnRef)
        {
            Vnp_Locale = "vn";
            Vnp_IpAddr = ipAddress;
            Vnp_Version = version;
            Vnp_Command = "pay";
            Vnp_TxnRef = txnRef;
            Vnp_OrderType = orderType;
            Vnp_OrderInfo = orderInfo;
            Vnp_ReturnUrl = returnUrl;
            Vnp_CreateDate = createDate.ToString("yyyyMMddHHmmss");
            Vnp_Amount = (int)amount * 100;
            Vnp_CurrCode = currCode;
            Vnp_TmnCode = tmnCode;
        }
    }
}
