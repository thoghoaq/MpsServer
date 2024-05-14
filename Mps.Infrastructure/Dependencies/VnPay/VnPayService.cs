using Mps.Application.Abstractions.Payment;
using Mps.Infrastructure.Dependencies.VnPay.Helpers;
using Mps.Infrastructure.Dependencies.VnPay.Models;
using System.Net;
using System.Text;

namespace Mps.Infrastructure.Dependencies.VnPay
{
    public class VnPayService : IVnPayService
    {
        public required VnPayPayRequest Request { get; set; } = new();
        public required VnPayPayResponse Response { get; set; } = new();

        public void MakeRequestData()
        {
            if (Request.Vnp_Amount != null) Request.RequestData.Add("vnp_Amount", Request.Vnp_Amount.ToString() ?? string.Empty);
            if (Request.Vnp_Command != null) Request.RequestData.Add("vnp_Command", Request.Vnp_Command);
            if (Request.Vnp_CreateDate != null) Request.RequestData.Add("vnp_CreateDate", Request.Vnp_CreateDate);
            if (Request.Vnp_CurrCode != null) Request.RequestData.Add("vnp_CurrCode", Request.Vnp_CurrCode);
            if (Request.Vnp_IpAddr != null) Request.RequestData.Add("vnp_IpAddr", Request.Vnp_IpAddr);
            if (Request.Vnp_Locale != null) Request.RequestData.Add("vnp_Locale", Request.Vnp_Locale);
            if (Request.Vnp_OrderInfo != null) Request.RequestData.Add("vnp_OrderInfo", Request.Vnp_OrderInfo);
            if (Request.Vnp_OrderType != null) Request.RequestData.Add("vnp_OrderType", Request.Vnp_OrderType);
            if (Request.Vnp_ReturnUrl != null) Request.RequestData.Add("vnp_ReturnUrl", Request.Vnp_ReturnUrl);
            if (Request.Vnp_ExpireDate != null) Request.RequestData.Add("vnp_ExpireDate", Request.Vnp_ExpireDate);
            if (Request.Vnp_TxnRef != null) Request.RequestData.Add("vnp_TxnRef", Request.Vnp_TxnRef);
            if (Request.Vnp_Version != null) Request.RequestData.Add("vnp_Version", Request.Vnp_Version);
            if (Request.Vnp_TmnCode != null) Request.RequestData.Add("vnp_TmnCode", Request.Vnp_TmnCode);
        }

        public string GetLink(string baseUrl, string secretKey)
        {
            MakeRequestData();
            StringBuilder data = new();
            foreach (var item in Request.RequestData)
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
            Request.Vnp_Locale = "vn";
            Request.Vnp_IpAddr = ipAddress;
            Request.Vnp_Version = version;
            Request.Vnp_Command = "pay";
            Request.Vnp_TxnRef = txnRef;
            Request.Vnp_OrderType = orderType;
            Request.Vnp_OrderInfo = orderInfo;
            Request.Vnp_ReturnUrl = returnUrl;
            Request.Vnp_CreateDate = createDate.ToString("yyyyMMddHHmmss");
            Request.Vnp_Amount = (int)amount * 100;
            Request.Vnp_CurrCode = currCode;
            Request.Vnp_TmnCode = tmnCode;
        }

        public void BindingResponse(int? amount, string? bankCode, string? bankTranNo, string? cardType, string? orderInfo, string? transactionNo, string? transactionStatus, int? txnRef, string? secureHash, string? payDate, string? responseCode, string? tmnCode)
        {
            Response.Vnp_Amount = amount;
            Response.Vnp_BankCode = bankCode;
            Response.Vnp_BankTranNo = bankTranNo;
            Response.Vnp_CardType = cardType;
            Response.Vnp_OrderInfo = orderInfo;
            Response.Vnp_TransactionNo = transactionNo;
            Response.Vnp_TransactionStatus = transactionStatus;
            Response.Vnp_TxnRef = txnRef;
            Response.Vnp_SecureHash = secureHash;
            Response.Vnp_PayDate = payDate;
            Response.Vnp_ResponseCode = responseCode;
            Response.Vnp_TmnCode = tmnCode;
        }

        public void MakeResponseData()
        {
            if (Response.Vnp_Amount != null) Response.ResponseData.Add("vnp_Amount", Response.Vnp_Amount.ToString() ?? string.Empty);
            if (!string.IsNullOrEmpty(Response.Vnp_TmnCode)) Response.ResponseData.Add("vnp_TmnCode", Response.Vnp_TmnCode);
            if (!string.IsNullOrEmpty(Response.Vnp_BankCode)) Response.ResponseData.Add("vnp_BankCode", Response.Vnp_BankCode);
            if (!string.IsNullOrEmpty(Response.Vnp_BankTranNo)) Response.ResponseData.Add("vnp_BankTranNo", Response.Vnp_BankTranNo);
            if (!string.IsNullOrEmpty(Response.Vnp_CardType)) Response.ResponseData.Add("vnp_CardType", Response.Vnp_CardType);
            if (!string.IsNullOrEmpty(Response.Vnp_OrderInfo)) Response.ResponseData.Add("vnp_OrderInfo", Response.Vnp_OrderInfo);
            if (!string.IsNullOrEmpty(Response.Vnp_TransactionNo)) Response.ResponseData.Add("vnp_TransactionNo", Response.Vnp_TransactionNo);
            if (Response.Vnp_TxnRef != null) Response.ResponseData.Add("vnp_TxnRef", Response.Vnp_TxnRef.ToString() ?? string.Empty);
            if (!string.IsNullOrEmpty(Response.Vnp_PayDate)) Response.ResponseData.Add("vnp_PayDate", Response.Vnp_PayDate);
            if (!string.IsNullOrEmpty(Response.Vnp_ResponseCode)) Response.ResponseData.Add("vnp_ResponseCode", Response.Vnp_ResponseCode);
        }

        public bool IsValidSignature(string secret)
        {
            MakeResponseData();
            StringBuilder data = new();
            foreach (var item in Response.ResponseData)
            {
                if (!string.IsNullOrEmpty(item.Value))
                {
                    data.Append(WebUtility.UrlEncode(item.Key) + "=" + WebUtility.UrlEncode(item.Value) + "&");
                }
            }
            string checkSum = HashHelper.HmacSHA512(secret, data.ToString().Remove(data.Length - 1, 1));
            return checkSum.Equals(Response.Vnp_SecureHash, StringComparison.InvariantCultureIgnoreCase);
        }

        public string GetResponseMessage(string responseCode)
        {
            return responseCode switch
            {
                "00" => "Payment success",
                "10" => "Payment process failed",
                "11" => "Can't find payment information",
                "99" => "Unknown error",
                _ => "Unknown error"
            };
        }

        public bool IsSuccessResponse(string responseCode)
        {
            return responseCode.Equals("00", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
