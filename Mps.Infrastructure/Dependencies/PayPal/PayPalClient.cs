using Microsoft.Extensions.Configuration;
using PayoutsSdk.Core;
using HttpClient = PayPalHttp.HttpClient;

namespace Mps.Infrastructure.Dependencies.PayPal
{
    public class PayPalClient(IConfiguration configuration)
    {
        private readonly string ClientId = configuration.GetSection("PayPal:ClientId").Value!;
        private readonly string Secret = configuration.GetSection("PayPal:Secret").Value!;

        public PayPalEnvironment Environment()
        {
            return new SandboxEnvironment(ClientId, Secret);
        }

        public HttpClient Client()
        {
            return new PayPalHttpClient(Environment());
        }

        public HttpClient Client(string refreshToken)
        {
            return new PayPalHttpClient(Environment(), refreshToken);
        }
    }
}
