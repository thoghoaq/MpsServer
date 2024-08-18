namespace Mps.Application.Abstractions.Payment
{
    public interface IVnPayService
    {
        public void CreateVnPayRequest(string version, string tmnCode, DateTime createDate, string ipAddress, decimal amount, string currCode, string orderType, string orderInfo, string returnUrl, string txnRef);
        public string GetLink(string baseUrl, string secretKey);
        public bool IsValidSignature(string secret);
        public string GetResponseMessage(string responseCode);
        public bool IsSuccessResponse(string responseCode);
        public void BindingResponse(int? amount, string? bankCode, string? bankTranNo, string? cardType, string? orderInfo, string? transactionNo, string? transactionStatus, string? txnRef, string? secureHash, string? payDate, string? responseCode, string? tmnCode);
        Task<string> RefundPaymentAsync(string secretKey, string tmnCode, string vnpUrl, string txnRef, long amount, string orderInfo, string transactionNo, string transactionDate, string createBy, string ipAddress);
    }
}
