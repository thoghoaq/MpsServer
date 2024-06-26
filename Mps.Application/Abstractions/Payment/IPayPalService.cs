using PayoutsSdk.Payouts;
using PayPalHttp;

namespace Mps.Application.Abstractions.Payment
{
    public interface IPayPalService
    {
        Task<HttpResponse> CreatePayoutRequest(CreatePayoutRequest body);
        Task<HttpResponse> RetrievePayoutsBatch(string batchId);
    }
}
