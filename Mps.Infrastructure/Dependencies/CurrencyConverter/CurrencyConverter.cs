using Microsoft.Extensions.Configuration;
using Mps.Application.Abstractions.Payment;
using Mps.Domain.Dtos;
using System.Net.Http.Json;

namespace Mps.Infrastructure.Dependencies.CurrencyConverter
{
    public class CurrencyConverter(IConfiguration configuration, HttpClient httpClient) : ICurrencyConverter
    {
        public async Task<decimal> ConvertAsync(decimal amount, string fromCurrency, string toCurrency, CancellationToken cancellationToken)
        {
            string baseUrl = configuration.GetSection("CurrencyConverter:BaseUrl").Value!;
            string request = $"{baseUrl}&from={fromCurrency}&to={toCurrency}&amount={amount}";
            var response = await httpClient.GetAsync(new Uri(request), cancellationToken);
            var currencyResponse = await response.Content.ReadFromJsonAsync<CurrencyResponse>(cancellationToken: cancellationToken);
            if (currencyResponse == null || currencyResponse.Rates == null || currencyResponse.Rates.Count == 0)
            {
                return 0;
            }
            return decimal.Parse(currencyResponse!.Rates.First().Value.RateForAmount!);
        }

        public async Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency)
        {
            var clientHttp = new HttpClient();
            string baseUrl = configuration.GetSection("CurrencyConverter:BaseUrl").Value!;
            string request = $"{baseUrl}&from={fromCurrency}&to={toCurrency}";
            var response = await clientHttp.GetAsync(new Uri(request));
            var currencyResponse = await response.Content.ReadFromJsonAsync<CurrencyResponse>();
            if (currencyResponse == null || currencyResponse.Rates == null || currencyResponse.Rates.Count == 0)
            {
                return 0;
            }
            return decimal.Parse(currencyResponse!.Rates.First().Value.Rate!);
        }
    }
}
