namespace Mps.Application.Abstractions.Payment
{
    public interface IVnPayService
    {
        public void CreateVnPayRequest(string version, string tmnCode, DateTime createDate, string ipAddress, decimal amount, string currCode, string orderType, string orderInfo, string returnUrl, string txnRef);
        public string GetLink(string baseUrl, string secretKey);
    }
}
