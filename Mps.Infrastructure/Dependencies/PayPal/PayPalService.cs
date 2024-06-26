using Microsoft.Extensions.Configuration;
using Mps.Application.Abstractions.Payment;
using PayoutsSdk.Payouts;
using PayPalHttp;

namespace Mps.Infrastructure.Dependencies.PayPal
{
    public class PayPalService(IConfiguration configuration) : IPayPalService
    {
        // create payout request
        public async Task<HttpResponse> CreatePayoutRequest(CreatePayoutRequest body)
        {
            var client = new PayPalClient(configuration);
            PayoutsPostRequest request = new();
            request.RequestBody(body);
            var response = await client.Client().Execute(request);
            var result = response.Result<CreatePayoutResponse>();
            Console.WriteLine("Status: {0}", result.BatchHeader.BatchStatus);
            Console.WriteLine("Batch Id: {0}", result.BatchHeader.PayoutBatchId);
            Console.WriteLine("Links:");
            foreach (LinkDescription link in result.Links)
            {
                Console.WriteLine("\t{0}: {1}\tCall Type: {2}", link.Rel, link.Href, link.Method);
            }
            return response;

        }

        // Retrieve a Payouts Batch
        public async Task<HttpResponse> RetrievePayoutsBatch(string batchId)
        {
            var client = new PayPalClient(configuration);
            PayoutsGetRequest request = new(batchId);
            var getResponse = await client.Client().Execute(request);
            var result = getResponse.Result<PayoutBatch>();
            Console.WriteLine("Status: {0}", result.BatchHeader.BatchStatus);
            Console.WriteLine("Item: {0}", result.Items[0].PayoutItemId);
            Console.WriteLine("Batch Id: {0}", result.BatchHeader.PayoutBatchId);
            Console.WriteLine("Links:");
            foreach (LinkDescription link in result.Links)
            {
                Console.WriteLine("\t{0}: {1}\tCall Type: {2}", link.Rel, link.Href, link.Method);
            }
            return getResponse;
        }
    }
}
